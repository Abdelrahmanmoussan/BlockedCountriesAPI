namespace BlockedCountriesAPI.Models
{
     public class RestCountry
    {
        public Name Name { get; set; }
        public string Cca2 { get; set; }
    }

    public class Name
    {
        public string Common { get; set; }
    }
}
