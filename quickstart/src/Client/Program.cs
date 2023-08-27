// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using IdentityModel.Client;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

var client =new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
if (disco.IsError)
{
    Console.Write(disco.Error);
    return;
}
Console.WriteLine(JsonConvert.SerializeObject(disco));

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
{
   Address = disco.TokenEndpoint, // http://localhost:5001/connect/token
   ClientId = "client",
   ClientSecret = "secret",
   Scope = "myapi"
});

var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("http://localhost:6001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    Console.WriteLine("Success!");

    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
}