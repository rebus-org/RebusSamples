using System;
using FullBlownConsoleApplication.Models;

namespace FullBlownConsoleApplication.Config
{
    public class AmbientUserContext : IDisposable
    {
        [ThreadStatic]
        static UserContext _current;
        
        public static UserContext Current
        {
            get { return _current; }
        }

        public AmbientUserContext(UserContext context)
        {
            _current = context;
        }

        public void Dispose()
        {
            _current = null;
        }

    }
}