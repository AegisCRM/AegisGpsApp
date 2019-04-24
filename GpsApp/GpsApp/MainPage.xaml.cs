using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GpsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            // animation code here
            LoginModel model = new LoginModel()
            {
                DeviceId = App._DeviceId
            };
            model = await DoAutoLogin(model);

            if (model != null && model.ResponseCode == 200)
            {
                App._Name = model.Name;
                App._UserId = model.UserId;
                Application.Current.MainPage = new AttendanceForm();
            }
            else
            {
                lblMessage.Text = model.Message;
                lblMessage.TextColor = Color.Red;
                await Task.Delay(TimeSpan.FromSeconds(1));
                Application.Current.MainPage = new LoginForm();
            }
        }

        private static async Task<LoginModel> DoAutoLogin(LoginModel model)
        {
            string serializedModel = JsonConvert.SerializeObject(model);
            string response = await ApiAsyncPost(serializedModel, "DoAutoLogin");
            model = JsonConvert.DeserializeObject<LoginModel>(response);
            return model;
        }

        private static async Task<string> ApiAsyncPost(string serializedModel, string action)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://api.aegiscrm.in/api/Login/" + action);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new StringContent(serializedModel, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://api.aegiscrm.in/api/Login/" + action, content);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

    }
}
