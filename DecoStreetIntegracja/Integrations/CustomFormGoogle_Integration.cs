using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrations
{
    public class CustomFormGoogle_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "customformgoogle_result.xml";

        internal override string SourcePath => "https://customform.co/wp-content/uploads/googlefeeds/google.xml";

        internal override string IdPrefix => "CustomForm";

        public override void GenerateResult()
        {
            throw new NotImplementedException();
        }
    }
}
