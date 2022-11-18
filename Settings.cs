namespace JwtAuthServer;

public static class Settings
{

    public static string Secret;
    public static int ExpirationHours;
    public static string Issuer;
    public static string ApiKey;
    public static WhatsappConfiguration WhatsappConfig = new();
    
    public class WhatsappConfiguration
    {
        public static string Endpoint;
        public static string InstanceId;
        public static string Token;
    }
}