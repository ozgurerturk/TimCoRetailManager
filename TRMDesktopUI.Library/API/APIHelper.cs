using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.Library.API
{
    public class APIHelper : IAPIHelper
    {
        //APIHelper'a geldi, sag tikladik extract interface dedik, orada prop'lar metodlar falan seciliyor sozlesmede olacak
        //ve ApiClient'i secmedik cunku 
        private HttpClient apiClient;
        private ILoggedInUserModel _loggedInUserModel;

        public APIHelper(ILoggedInUserModel loggedInUser) //Bootstrapper wpf'i calistirince APIHelper singleton ile basliyor, ctor'i da Initialize client yapiyor
        {
            InitializeClient();//Starts as soon as apihelper initialized
            _loggedInUserModel = loggedInUser;
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"]; //App.config'den geldi, key = "api". System.Configuration ekledik 

            apiClient = new HttpClient(); //Will be used for the lifetime for the WPF instance and only one
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

        public async Task GetLoggedInUserInfo(string token)
        {
            apiClient.DefaultRequestHeaders.Clear();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("Authorization", $"bearer { token }");

            using (HttpResponseMessage response = await apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    _loggedInUserModel.CreatedDate = result.CreatedDate;
                    _loggedInUserModel.EmailAddress = result.EmailAddress;
                    _loggedInUserModel.FirstName = result.FirstName;
                    _loggedInUserModel.Id = result.Id;
                    _loggedInUserModel.LastName = result.LastName;
                    _loggedInUserModel.Token = token;
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
