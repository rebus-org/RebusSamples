## Logging

This samples shows how to integrate Rebus with your preferred way of logging.

### A small note on the philosophy of logging

It is assumed that you have a preferred way of logging things - you might have picked Serilog already (good choice) to
integrate into your application, which means that you can use all of Serilog's cool structured logging features and stuff.

Now, when you introduce Rebus, you should make Rebus log _your way_ (i.e. via Serilog), and not try to make your application log _the Rebus way_.

### Just remember this

Configure Rebus to log _your way_.