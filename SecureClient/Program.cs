using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SecureClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Making the call.....");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync(){
            AuthConfig config = AuthConfig.ReadJsonFromFile("appsettings.json");
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();
            
            //Here we only have one resourceId, but there can be more
            string[] ResourceIds = new string[] {config.ResourceId};

            AuthenticationResult result = null;
            try{
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token Aquired \n");
                Console.WriteLine(result.AccessToken);
                Console.WriteLine();
                Console.ResetColor();
            }
            catch (MsalClientException ex){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor(); 
            }

            if(!string.IsNullOrEmpty(result.AccessToken)){
                //The remote certificate is invalid according to the validation procedure
                //Fix: bypassing certificate error
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                var httpClient = new HttpClient(clientHandler);
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                //Checked to make sure we pass the right media type (application/json)
                if(defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m=>m.MediaType == "application/json")){
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/josn"));
                }

                //Setting Authorization to bearer and passing access token
                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
                HttpResponseMessage response = await httpClient.GetAsync(config.BaseAddress);
 
                if(response.IsSuccessStatusCode){
                    Console.ForegroundColor = ConsoleColor.Green;
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }
                else{
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API: {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                }
                Console.ResetColor();
            }
        }
    }
}
