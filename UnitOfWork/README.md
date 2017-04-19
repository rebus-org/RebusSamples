## Unit of work

This sample shows how a SQL Server ADO.NET-based unit of work can be set up with Rebus and Castle Windsor.

The approach should be easy to transfer to other databases and containers.

Every second a timer will send a string to us, which we will handle with a two-handler pipeline:

* First handler (`InsertRowsIntoDatabase`) inserts the received text into the database
* Second handler (`FailSometimes`) throws an exception when the hash code of the string modulo 10 == 0

The `InsertRowsIntoDatabase` handler gets a `SqlConnection` and a `SqlTransaction` injected, which
it will use to perform its work. 

Injection of those two is set up in `SqlUnitOfWorkInstaller`, demonstrating how the two can be retrieved 
from the current `UnitOfWork` instance that we stash in the transaction context's `Items` collection after
it is created.

The creation and management of the unit of work is set up by calling the `EnableUnitOfWork` method
when configuring Rebus.
