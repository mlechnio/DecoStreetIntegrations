using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrator.Models
{
    public class ProductForInsert
    {
        public int category_id { get; set; } = 1362;

        public string code { get; set; }

        public string pkwiu { get; set; }

        public ProductStock stock { get; set; } = new ProductStock();

        public ProductTranslations translations { get; set; } = new ProductTranslations();

        public Dictionary<string, string> attributes { get; set; } = new Dictionary<string, string>(); // maybe later
    }

    public class ProductTranslations
    {
        public Translation pl_PL { get; set; } = new Translation();
    }

    public class Translation
    {
        public string name { get; set; }

        public string description { get; set; }

        public string short_description { get; set; }

        public string active { get; set; }

        public string seo_url { get; set; } // maybe later
    }
}
