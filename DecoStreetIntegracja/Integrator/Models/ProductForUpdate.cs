﻿using System;
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

        public SpecialOffer special_offer { get; set; }

        public ProductTranslations translations { get; set; }

        public bool RemovePromotion { get; set; }
    }
}
