using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrator.Models
{
    public class SpecialOffer
    {
        public int promo_id { get; set; }

        public decimal discount { get; set; }

        public string date_from { get; set; }

        public string date_to { get; set; }
    }
}
