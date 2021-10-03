using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Utils
{
    public class Event
    {
        public Event()
        {

        }

        public Event(string message)
        {
            Timestamp = DateTime.Now;
            Message = message;
        }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {Message}";
        }
    }
}
