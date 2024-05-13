using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrator.Models
{
    public class ProductImageForInsert
    {
        public int product_id { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public string content { get; set; }

        public ProductTranslations translations { get; set; }
    }
}
