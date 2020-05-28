using DecoStreetIntegracja.Integrations.Base;

namespace DecoStreetIntegracja.Integrations
{
    public class Adansonia_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "adansonia_result.xml";

        internal override string SourcePath => "http://admin.adansonia.pl/service/cennik.xml?name=ghost&producer=&category=";

        internal override string IdPrefix => "Adansonia";
    }
}
