using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Utils
{
    public static class Logger
    {
        public static List<Event> Events = new List<Event>();

        public static List<string> NewProducts = new List<string>();

        public static List<Exception> Exceptions = new List<Exception>();

        public static void Clear()
        {
            Events = new List<Event>();
            NewProducts = new List<string>();
            Exceptions = new List<Exception>();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            Events.Add(new Event(message));
        }

        internal static void LogException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Exceptions.Add(ex);
        }
    }
}
