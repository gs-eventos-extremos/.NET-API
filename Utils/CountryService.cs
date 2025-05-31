namespace WeatherEmergencyAPI.Utils
{
    public static class CountryService
    {
        private static readonly Dictionary<string, string> CountryCodes = new()
        {
            { "brasil", "+55" },
            { "brazil", "+55" },
            { "estados unidos", "+1" },
            { "united states", "+1" },
            { "usa", "+1" },
            { "argentina", "+54" },
            { "portugal", "+351" },
            { "espanha", "+34" },
            { "spain", "+34" },
            { "frança", "+33" },
            { "france", "+33" },
            { "alemanha", "+49" },
            { "germany", "+49" },
            { "italia", "+39" },
            { "italy", "+39" },
            { "reino unido", "+44" },
            { "united kingdom", "+44" },
            { "uk", "+44" },
            { "japão", "+81" },
            { "japan", "+81" },
            { "china", "+86" },
            { "canada", "+1" },
            { "méxico", "+52" },
            { "mexico", "+52" },
            { "australia", "+61" },
            { "india", "+91" },
            { "russia", "+7" },
            { "south africa", "+27" },
            { "áfrica do sul", "+27" }
        };

        public static string GetCountryCode(string country)
        {
            var normalizedCountry = country.ToLower().Trim();
            return CountryCodes.TryGetValue(normalizedCountry, out var code) ? code : "+1";
        }

        public static Dictionary<string, string> GetAllCountries()
        {
            return new Dictionary<string, string>(CountryCodes);
        }
    }
}