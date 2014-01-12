## Timeouts

The purpose of this sample is to demonstrate how Rebus can easily _send messages into the future!_

Please note that for the sample to work, you must have a Rebus Timeout Manager running that this program can send deferred messages to.

### How to start the timeout manager?

#### Make sure timeout can be properly persisted

The timeout manager needs some kind of persistent storage for timeouts - this is how your timeouts will never be forgotten, even though your machine might reboot in the middle of doing important work.

Go to the 'Rebus Timeout Manager' folder and open up `Rebus.Timeout.exe.config` in your favorite text editor - it should have a `timeout` element that looks somewhat like this:

	<timeout inputQueue="rebus_timeouts" errorQueue="error" storageType="SQL" 
			 connectionString="server=.;initial catalog=rebus_test;integrated security=sspi" tableName="test_timeouts" />

which configures the timeout manager to use the local `rebus_test` database to store timeouts - the table `test_timeouts` will automatically be created when the timeout manager starts up.


#### Make sure the timeout manager has the right input queue

Take a look at the `timeout` element again - the `inputQueue` property configures the name of the timeout manager's input queue. Pick whatever suits you, just make sure that you remember the name.


#### Make sure your Rebus endpoint is configured to send timeout requests to the timeout manager