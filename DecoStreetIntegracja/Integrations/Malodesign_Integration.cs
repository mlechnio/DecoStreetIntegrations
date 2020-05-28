using DecoStreetIntegracja.Integrations.Base;

namespace DecoStreetIntegracja.Integrations
{
    public class Malodesign_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "malodesign_result.xml";

        internal override string SourcePath => "https://malodesign.pl/modules/pricewars2/service.php?id_xml=7";

        internal override string IdPrefix => "Malodesign";
    }
}
