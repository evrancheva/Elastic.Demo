using Elasticsearch.Net;

namespace Elastic.Demo.Models
{
    public class Listing 
    {
        public long GlobalId { get; set; }
        public string Adres { get; set; }
        public string Woonplaats { get; set; }
        public string KantoorNaam { get; set; }
        public string Omschrijving { get; set; }
        public int OppervlakteVan { get; set; }
        public int OppervlakteTot { get; set; }

    }
}
