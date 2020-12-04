## F# Onboarding Saga

This sample demonstrates how to work with Rebus using F#.  The project contains a 
Web API front end and a backend saga that has a timeout and compensating actions. 

The project uses the file system as the message transport with `c:/rebus` being 
the default location.  The location can be changed where Rebus is configured in the 
[Web API](/FSharp/OnboardingSaga/EntryPointAPI/Startup.fs) and the 
[backend saga](/FSharp/OnboardingSaga/OnboardingProcessor/Backend.fs).

This sample models a made-up business process for onboarding of new customers.  
As well as functional requirements, the business also have operational requirements 
and the saga has a timeout in place to help them meet that requirement.  When a new
customer is taken on, the business wants the following to happen:

* An account is to be created for the customer.
* A welcome email is to be sent to the customer after the account is created.
* A sales call is scheduled in the CRM after the account has been created.

To support the business' operational requirements that new customers must be 
processed within a given time, our saga must complete within a given time.  If 
it does not do that then the business wants the following to happen:

* Any placed sales call is to be cancelled.
* The service desk takes over the process.