using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.Helpers
{
    public class APIHelper : IAPIHelper
    {
        //APIHelper'a geldi, sag tikladik extract interface dedik, orada prop'lar metodlar falan seciliyor sozlesmede olacak
        //ve ApiClient'i secmedik cunku 
        private HttpClient apiClient;

        public APIHelper()
        {
            InitializeClient(); //Starts as soon as apihelper initialized
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"]; //App.config'den geldi, key = "api". System.Configuration ekledik 

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api); //localhost/*number* geldi, asagida /token ekledik ardina
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type", "password"),
                new KeyValuePair<string,string>("username", username),
                new KeyValuePair<string,string>("password", password)
            });

            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode) //http200-299 gelirse yani
                {
                    //Microsoft.AspNet.WebApi.Client yukledik nugetten, post async, asnc cagri yolladi. cagirdik using'den token'i
                    //response'in status kodu basarili ise 200 falan yani, await'e geldi response. content'i okuduk. Token'i result'a atti
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
