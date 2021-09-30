using DecoStreetIntegracja.Integrator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrator.ApiModels
{
    public class ProductListResponse
    {
        public int count { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
        public List<Product> list { get; set; }
    }
}
