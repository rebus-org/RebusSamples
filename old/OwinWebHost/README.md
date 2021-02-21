## OWIN Web Host

This sample shows how Rebus via the [Rebus.Owin](https://www.nuget.org/packages/Rebus.Owin/) package can host OWIN endpoints.

The sample uses [Microsoft.Owin.StaticFiles](https://www.nuget.org/packages/Microsoft.Owin.StaticFiles/) to serve the contents
of the "site" directory directly from within the source code. The path used when initializing the `PhysicalFileSystem` should
be changed accordingly if the application is built/packaged.

In addition to serving static files, a pretty simple timed greeting API is hosted on the `/api/hello` route, to which the web app
will periodically GET a JSON object and print the response.

Just hit F5 and watch the action :)

Happy hosting!