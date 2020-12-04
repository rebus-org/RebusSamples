module rec Handlers

open System
open System.Threading.Tasks
open OnboardingMessages
open Rebus.Bus
open Rebus.Handlers
open FSharp.Control.Tasks.V2.ContextInsensitive
open Serilog

type CreateCustomerAccountHandler(b: IBus) =
    let bus = b

    interface IHandleMessages<CreateCustomerAccount> with
        member x.Handle(m: CreateCustomerAccount) =
            task
                {
                    Log.Information($"Creating customer account for {m.Name}, {m.Email}.")
                    do! Task.Delay(500) // Pretend we're doing something!
                    do! bus.Reply(CustomerAccountCreated.For m.Email (Random().Next()))
                } :> Task

type SendWelcomeEmailHandler(b: IBus) =
    let bus = b

    interface IHandleMessages<SendWelcomeEmail> with
        member x.Handle(m: SendWelcomeEmail) =
            task
                {
                    Log.Information($"Sending welcome email for account {m.AccountId}.")
                    do! Task.Delay(10000); // This delay will breach our OLA rules!
                    do! bus.Reply(WelcomeEmailSent.For m.AccountId)
                } :> Task

type ScheduleSalesCallHandler(b: IBus) =
    let bus = b

    interface IHandleMessages<ScheduleSalesCall> with
        member x.Handle(m: ScheduleSalesCall) =
            task
                {
                    Log.Information($"Scheduling sales call for account {m.AccountId}.")
                    do! Task.Delay(500) // Pretend we're doing something!
                    do! bus.Reply(SalesCallScheduled.For m.AccountId)
                } :> Task

type CancelSalesCallHandler() =
    interface IHandleMessages<CancelSalesCall> with
        member x.Handle(m: CancelSalesCall) =
            task
                {
                    Log.Information($"Cancelling sales call for account {m.AccountId}.")
                } :> Task

type NotifyServiceDeskHandler() =
    interface IHandleMessages<NotifyServiceDesk> with
        member x.Handle(m: NotifyServiceDesk) =
            task
                {
                    Log.Information($"Notifying the service desk that: {m.Message}.")
                } :> Task