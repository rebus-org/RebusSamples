## Unit of work

This sample shows how a SQL Server ADO.NET-based unit of work can be set up with Rebus and Castle Windsor.

The approach should be easy to transfer to other databases and containers.

Every second a timer will send a string to us, which we will handle with a two-handler pipeline:

* First handler (`InsertRowsIntoDatabase`) inserts the received text into the database
* Second handler (`FailSometimes`) throws an exception when the hash code of the string modulo 10 == 0

The `InsertRowsIntoDatabase` handler gets a `SqlConnection` and a `SqlTransaction` injected, which
it will use to perform its work. Injection of those two is set up in `SqlUnitOfWorkInstaller` which
demostrates how the two are kept in the transaction context's `Items` collection throughout while
handling the message, and the `SqlTransaction`'s `Commit` action is enlisted in the commit phase of
Rebus' transaction context.
