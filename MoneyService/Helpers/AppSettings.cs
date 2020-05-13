namespace MoneyService.Helpers
{
    public class AppSettings
    {
        public string SecureKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresMinutes { get; set; }
    }
}