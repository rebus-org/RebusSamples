namespace EntryPointAPI.Controllers

open Microsoft.AspNetCore.Mvc
open Rebus.Bus
open FSharp.Control.Tasks.V2.ContextInsensitive
open OnboardingMessages

[<ApiController>]
type CustomerController (bus : IBus) =
    inherit ControllerBase()
    let _bus = bus

    [<Route("newcustomer")>]
    [<HttpPost>]
    member this.NewCustomer(name: string, email: string) = task {
        do! _bus.Send(OnboardNewCustomer.For name email)
        return this.Ok()
    }
