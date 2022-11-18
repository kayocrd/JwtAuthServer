using System.Text;
using System.Text.Json;

namespace JwtAuthServer.Services;

public class WhatsappService : IWhatsappService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _endpoint = Settings.WhatsappConfiguration.Endpoint;
    private readonly string _instanceId = Settings.WhatsappConfiguration.InstanceId;
    private readonly string _token = Settings.WhatsappConfiguration.Token;
    
    public WhatsappService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> SendWhatsappMessage(string phone, string message)
    {
        var url = $"{_endpoint}/instances/{_instanceId}/token/{_token}/send-text";
        var client = _clientFactory.CreateClient();
        
        var body = new
        {
            phone,
            message
        };
        var json = JsonSerializer.Serialize(body);
        var data = new StringContent(json, Encoding.Unicode, "application/json");
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        return response;
    }
}