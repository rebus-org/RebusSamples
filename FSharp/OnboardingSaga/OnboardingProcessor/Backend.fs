module rec OnboardingProcessor

open System
open Microsoft.Extensions.DependencyInjection
open OnboardingMessages
open Rebus.Auditing.Messages
open Rebus.Bus
open Rebus.Config
open Rebus.Persistence.FileSystem
open Rebus.Retry.Simple
open Rebus.Routing.TypeBased
open Rebus.Transport.FileSystem

let configureRebus (rebus: RebusConfigurer) =
    rebus.Logging       (fun l -> l.Serilog())                                                            |> ignore
    rebus.Routing       (fun r -> r.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue") |> ignore) |> ignore
    rebus.Transport     (fun t -> t.UseFileSystem("c:/rebus", "MainQueue") |> ignore)                     |> ignore
    rebus.Options       (fun t -> t.RetryStrategy(errorQueueName = "ErrorQueue"))                         |> ignore
    rebus.Sagas         (fun s -> s.UseFilesystem("c:/rebus/sagas"))                                      |> ignore
    rebus.Timeouts      (fun s -> s.UseFileSystem("c:/rebus/timeouts"))                                   |> ignore
    rebus.Options       (fun x -> x.EnableMessageAuditing("Audit"))                                       |> ignore

    rebus

type Backend() =
    let mutable provider: ServiceProvider  = null
    let mutable bus: IBus  = null
    do
        let services = ServiceCollection()
        services.AddRebus (configure=configureRebus) |> ignore
        services.AutoRegisterHandlersFromAssemblyOf<Backend>() |> ignore

        provider <- services.BuildServiceProvider()
        provider.StartRebus() |> ignore
        bus <- provider.GetRequiredService<IBus>();

    interface IDisposable with
        member this.Dispose() =
            printfn "Disposing - tchau!"
            if bus <> null then bus.Dispose()
            if provider <> null then provider.Dispose()

    member this.Bus with get (): IBus = bus
