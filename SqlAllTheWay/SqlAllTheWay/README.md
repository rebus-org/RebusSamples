## SQL ALL THE WAY

The purpose of this sample is to demonstrate how Rebus with the SQL transport can be customized in such 
a way that a user-provided SQL connection (and accompanying transaction) is used by the transport, allowing
for

1. Sending multiple messages atomically as part of own work transaction (e.g. as part of a web request)
1. Sending messages and performing own work transaction inside a Rebus handler

The code might seem slightly convoluted - sorry about that :smile: - but it is actually pretty
straightforward, and it should be clear how it works after this explanation! Here goes:

### Scenarios

The following scenarios need to be covered:

* When calling `await bus.Send(...)` inside a custom DB context
* When calling `await bus.Send(...)` inside a Rebus handler context
* When calling `await bus.Send(...)` outside of any kind of context

This means that the `connectionFactory` passed to the SQL transport configuration spell in this line:

	.Transport(t => t.UseSqlServer(DbConfig.GetDbConnection, "Messages", "all-the-way-baby"))

(which is the `DbConfig.GetDbConnection` method in this case) needs to account for all three scenarios in
the way is chooses to provide the connection.

This method implements the prioritization of how to get the connection in the different scenarios:

    public static async Task<IDbConnection> GetDbConnection()
    {
        return
            // if CustomDbContext started the connection, use it:
            GetCustomDbConnectionOrNull()

            // if we are in a Rebus message handler and we already provided a connection, use it:
            ?? await GetCurrentlyOngoingRebusDbConnectionOrNull()

            // otherwise create a new connection, letting Rebus manage it from now on:
            ?? await OpenNewDbConnection();
    }

In this case, `GetCustomDbConnectionOrNull` tries to fetch an "ambient" user-created SQL connection. How
such connection can be implemented can be seen in `CustomDbContext`.

The `GetCurrentlyOngoingRebusDbConnectionOrNull` method will try to retrieve the currently ongoing Rebus
message context's transaction context if one is present.

Lastly, `OpenNewDbConnection` will simply open a new connection and let Rebus manage it.

Here are the scenarios in which the methods will yield a result:

* `GetCustomDbConnectionOrNull`: Will ONLY return a connection when a user-created ambient connection could be found.
* `GetCurrentlyOngoingRebusDbConnectionOrNull`: Will ONLY return a connection when `OpenNewDbConnection` has already created one, e.g. when called inside a Rebus message handler
* `OpenNewDbConnection`: Will return a new connection every time, marking the connection as one for Rebus to manage

