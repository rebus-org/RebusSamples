using System;
using System.Net;
using Owin;

namespace IntegrationSample.ExternalWebService
{
    class OwinApi
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/api", Api);
        }

        public static void Api(IAppBuilder app)
        {
            var random = new Random(DateTime.Now.GetHashCode());
            var randomLock = new object();

            Func<int, int> getRandomNumber = limit =>
            {
                lock (randomLock) return random.Next(limit);
            };

            var classicDanishGreetings = new[]
            {
                "Hej",
                "Daw do",
                "Goddag",
                "Hejsa",
                "Hallå",
                "Zup?",
                "Hey",
                "Ey"
            };

            app.Use(async (context, next) =>
            {
                var owinResponse = context.Response;

                var succeed = getRandomNumber(3) == 0;

                if (succeed)
                {
                    Console.WriteLine("ok");
                    var greetingIndex = getRandomNumber(classicDanishGreetings.Length);
                    var greeting = classicDanishGreetings[greetingIndex];

                    owinResponse.StatusCode = (int)HttpStatusCode.OK;
                    owinResponse.ContentType = "text/plain; charset=utf-8";
                    await owinResponse.WriteAsync(greeting);

                    return;
                }

                Console.WriteLine("internal server error");
                owinResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                owinResponse.ReasonPhrase = "Bummer dude";

                await next();
            });
        }
    }
}