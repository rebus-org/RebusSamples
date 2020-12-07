namespace OnboardingMessages

open System

type OnboardNewCustomer =
    {
        Name: string
        Email: string
    }
    static member For name email = { Name = name; Email = email }

type CreateCustomerAccount =
    {
        Name: string
        Email: string
    }
    static member For name email = { Name = name; Email = email }

type CustomerAccountCreated =
    {
        Email: string
        AccountId: int
    }
    static member For email accountId = { Email = email; AccountId = accountId }

type SendWelcomeEmail =
    {
        AccountId: int
    }
    static member For accountId = { AccountId = accountId }

type WelcomeEmailSent =
    {
        AccountId: int
    }
    static member For accountId = { AccountId = accountId }

type ScheduleSalesCall =
    {
        AccountId: int
    }
    static member For accountId = { AccountId = accountId }

type SalesCallScheduled =
    {
        AccountId: int
    }
    static member For accountId = { AccountId = accountId }

type OnboardingOlaBreached =
    {
        SagaId: Guid
    }
    static member For sagaId = { SagaId = sagaId }

type CancelSalesCall =
    {
        AccountId: int
    }
    static member For accountId = { AccountId = accountId }

type NotifyServiceDesk =
    {
        Message: string
    }
    static member With message = { Message = message }
