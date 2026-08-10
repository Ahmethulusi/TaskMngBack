namespace TaskMngBack.Configuration
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }

        // User Secrets'tan "JwtSettings:SecretKey" olarak gelir, appsettings*.json'da tanımlı değildir.
        public string SecretKey { get; set; } = string.Empty;
    }
}
