using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Durbas_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "durbas_result.xml";

        internal override string SourcePath => "http://umeblujmieszkanie.pl/products/xml/ceneo";

        internal override string IdPrefix => "Durbas";
    }
}
