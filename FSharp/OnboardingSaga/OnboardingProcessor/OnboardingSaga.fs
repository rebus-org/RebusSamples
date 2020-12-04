module rec OnboardingSaga

open System
open System.Threading.Tasks
open OnboardingMessages
open Rebus.Bus
open Rebus.Sagas
open FSharp.Control.Tasks.V2.ContextInsensitive
open Serilog

type OnboardingSagaData() =
    interface ISagaData with
        member this.Id
            with get () = this.Id
            and set (value) = this.Id <- value
        member this.Revision
            with get () = this.Revision
            and set (value) = this.Revision <- value

    member val Id       = Guid.Empty with get, set
    member val Revision = 0          with get, set

    member val CustomerName  = "" with get, set
    member val CustomerEmail = "" with get, set
    member val AccountId     = 0  with get, set

    member val AccountCreated     = false with get, set
    member val WelcomeEmailSent   = false with get, set
    member val SalesCallScheduled = false with get, set

    member this.Completed() = this.AccountCreated && this.WelcomeEmailSent && this.SalesCallScheduled

let CustomerEmail =
    // https://github.com/fsharp/fslang-design/blob/master/preview/FS-1003-nameof-operator.md#names-of-instance-membersabove
    let x = OnboardingSagaData()
    nameof x.CustomerEmail

let SagaId =
    let x = OnboardingSagaData()
    nameof x.Id

let AccountId =
    let x = OnboardingSagaData()
    nameof x.AccountId

type OnboardingSaga(b: IBus) =
    inherit Saga<OnboardingSagaData>()
    let bus = b

    override this.CorrelateMessages(config: ICorrelationConfig<OnboardingSagaData>) =
        config.Correlate<OnboardNewCustomer>     (Func<OnboardNewCustomer,     obj>(fun m -> m.Email     :> obj), CustomerEmail)
        config.Correlate<CustomerAccountCreated> (Func<CustomerAccountCreated, obj>(fun m -> m.Email     :> obj), CustomerEmail)
        config.Correlate<WelcomeEmailSent>       (Func<WelcomeEmailSent,       obj>(fun m -> m.AccountId :> obj), AccountId)
        config.Correlate<SalesCallScheduled>     (Func<SalesCallScheduled,     obj>(fun m -> m.AccountId :> obj), AccountId)
        config.Correlate<OnboardingOlaBreached>  (Func<OnboardingOlaBreached,  obj>(fun m -> m.SagaId    :> obj), SagaId)

    member this.TryComplete() =
        if this.Data.Completed() then
            Log.Information($"Onboarding completed for {this.Data.CustomerName}, {this.Data.CustomerEmail}, {this.Data.AccountId}.")
            this.MarkAsComplete()

    // This is to allows access to IsNew and MarkAsComplete from inside the interface sections below.
    member this.IsNew = base.IsNew
    member this.MarkComplete() = base.MarkAsComplete()

    interface IAmInitiatedBy<OnboardNewCustomer> with
        member this.Handle(m: OnboardNewCustomer) =
            task {
                if not this.IsNew then return ()
                Log.Information($"Beginning onboarding process for {m.Name}, {m.Email}.")

                this.Data.CustomerName  <- m.Name
                this.Data.CustomerEmail <- m.Email

                do! bus.Send(CreateCustomerAccount.For m.Name m.Email)
                do! bus.Defer(TimeSpan.FromSeconds 5., OnboardingOlaBreached.For this.Data.Id)

                this.TryComplete()
            } :> Task

    interface IAmInitiatedBy<CustomerAccountCreated> with
        member this.Handle(m: CustomerAccountCreated) =
            task {
                Log.Information($"Customer account created for {m.Email} with ID {m.AccountId}.")

                this.Data.AccountId      <- m.AccountId
                this.Data.AccountCreated <- true

                do! bus.Send( SendWelcomeEmail.For  m.AccountId)
                do! bus.Send( ScheduleSalesCall.For m.AccountId)

                this.TryComplete()
            } :> Task

    interface IAmInitiatedBy<WelcomeEmailSent> with
        member this.Handle(m: WelcomeEmailSent) =
            task {
                Log.Information($"Welcome email sent for {m.AccountId}.")
                this.Data.WelcomeEmailSent <- true
                this.TryComplete()
            } :> Task

    interface IAmInitiatedBy<SalesCallScheduled> with
        member this.Handle(m: SalesCallScheduled) =
            task {
                Log.Information($"Sales call scheduled for {m.AccountId}.")
                this.Data.SalesCallScheduled <- true
                this.TryComplete()
            } :> Task

    interface IAmInitiatedBy<OnboardingOlaBreached> with
        member this.Handle(m: OnboardingOlaBreached) =
            task {
                Log.Information($"ONBOARDING OLA BREACH PENDING FOR for saga {m.SagaId}.")
                if this.Data.SalesCallScheduled then
                    do! bus.Send(CancelSalesCall.For this.Data.AccountId)

                do! bus.Send(NotifyServiceDesk.With $"Customer onboarding OLA breach pending for new customer {this.Data.CustomerName} with email {this.Data.CustomerEmail}.")

                Log.Information($"Abandoning saga {this.Data.Id}.");
                this.MarkComplete()
            } :> Task


