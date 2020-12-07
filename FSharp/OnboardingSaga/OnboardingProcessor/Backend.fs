module rec OnboardingProcessor

open System
open Microsoft.Extensions.DependencyInjection
open OnboardingMessages
open Rebus.Bus
open Rebus.Config
open Rebus.Persistence.FileSystem
open Rebus.Retry.Simple
open Rebus.Routing.TypeBased
open Rebus.ServiceProvider
open Rebus.Transport.FileSystem

let configureRebus (rebus: RebusConfigurer) =
    rebus.Logging       (fun l -> l.Serilog())                                                            |> ignore
    rebus.Routing       (fun r -> r.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue") |> ignore) |> ignore
    rebus.Transport     (fun t -> t.UseFileSystem("c:/rebus", "MainQueue") |> ignore)                     |> ignore
    rebus.Options       (fun t -> t.SimpleRetryStrategy(errorQueueAddress = "ErrorQueue"))                |> ignore
    rebus.Sagas         (fun s -> s.UseFilesystem("c:/rebus/sagas"))                                      |> ignore
    rebus.Timeouts      (fun s -> s.UseFileSystem("c:/rebus/timeouts"))                                   |> ignore
    rebus

type Backend() =
    let mutable provider: ServiceProvider  = null
    let mutable bus: IBus  = null
    do
        let services = ServiceCollection()
        services.AddRebus configureRebus |> ignore
        services.AutoRegisterHandlersFromAssemblyOf<Backend>() |> ignore

        provider <- services.BuildServiceProvider()
        provider.UseRebus(Action<IBus>(fun x -> bus <- x)) |> ignore

    interface IDisposable with
        member this.Dispose() =
            printfn "Disposing - tchau!"
            if bus <> null then bus.Dispose()
            if provider <> null then provider.Dispose()

    member this.Bus with get (): IBus = bus
