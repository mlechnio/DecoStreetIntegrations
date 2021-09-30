using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrator.Models
{
    public class ProductForUpdate
    {
        public int product_id { get; set; }

        public ProductStock stock { get; set; }
    }
}
