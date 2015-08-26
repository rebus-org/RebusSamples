using System;

namespace FullBlownConsoleApplication.Helpers
{
    public static class PreFabGreetings
    {
        static readonly Random Random = new Random();

        static readonly string[] Repertoire =
        {
            "Hello there!",
            "Bonjour!",
            "Hej med dig min ven!",
            "Buongiorno",
            "Guten Tag"
        };

        public static string GetOne()
        {
            return Repertoire[Random.Next(Repertoire.Length)];
        }
    }
}