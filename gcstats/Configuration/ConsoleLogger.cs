using gcstats.Common;
using System;

namespace gcstats.Configuration
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
