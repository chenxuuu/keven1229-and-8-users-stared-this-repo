using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading;

namespace github_control
{
    class Program
    {
        private static RestClient Client;
        private static int RepoId = 340286268;
        private static int LastCount = 0;
        private static string LastUser = "";

        private static JArray Check(int id)
        {
            var request = new RestRequest($"repositories/{id}/stargazers", DataFormat.Json);
            var response = Client.Get(request);

            return JsonConvert.DeserializeObject<JArray>(response.Content);
        }

        private static void SetName(int id, string name)
        {
            var request = new RestRequest($"repositories/{id}", DataFormat.Json);
            request.AddJsonBody(new
            {
                name,
            });
            var _ = Client.Patch(request);
        }

        static void Main(string[] args)
        {
            Client = new RestClient("https://api.github.com");
            Client.Authenticator = new HttpBasicAuthenticator("token", "replace with your token");

            while(true)
            {
                try
                {
                    var now = Check(RepoId);
                    if (now.Count != LastCount || (string)now.Last["login"] != LastUser)
                    {
                        LastUser = (string)now.Last["login"];
                        LastCount = now.Count;
                        SetName(RepoId, $"{LastUser}-and-{LastCount - 1}-users-stared-this-repo");
                        Console.WriteLine($"changed:{LastUser}-and-{LastCount - 1}-users-stared-this-repo");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(5000);
            }
        }
    }
}
