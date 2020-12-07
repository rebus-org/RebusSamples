open System
open OnboardingProcessor
open Serilog
open Topper

[<EntryPoint>]
let main _ =
    Log.Logger <- LoggerConfiguration()
       .WriteTo.Console()
       .CreateLogger()

    let configuration = ServiceConfiguration().Add("OurBackendBus", fun () -> (new Backend() :> IDisposable))
    ServiceHost.Run(configuration);

    0