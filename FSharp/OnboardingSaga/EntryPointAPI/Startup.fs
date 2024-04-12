namespace EntryPointAPI

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Rebus.Config

module Lib =
    open OnboardingMessages
    open Rebus.Retry.Simple
    open Rebus.Routing.TypeBased
    open Rebus.Transport.FileSystem

    let configure (rebus: RebusConfigurer) =
           rebus.Logging   (fun l -> l.Console())                                                  |> ignore
           rebus.Routing   (fun r -> r.TypeBased().Map<OnboardNewCustomer>("MainQueue") |> ignore) |> ignore
           rebus.Transport (fun t -> t.UseFileSystemAsOneWayClient("c:/rebus"))                    |> ignore
           rebus.Options   (fun t -> t.RetryStrategy(errorQueueName = "ErrorQueue"))               |> ignore
           rebus


type Startup(configuration: IConfiguration) =
    member _.Configuration = configuration

    member _.ConfigureServices(services: IServiceCollection) =
        services.AddControllers()        |> ignore
        services.AddRebus(Lib.configure) |> ignore

    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseHttpsRedirection()
           .UseRouting()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllers() |> ignore
            ) |> ignore
