using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GpsApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginForm : ContentPage
	{
        public static int UserId = 0;
        public static string Name = string.Empty;

        public LoginForm ()
		{
			InitializeComponent ();
		}
        
        async protected void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;            
            try
            {
                LoginModel model = new LoginModel()
                {
                    UserName = txtUserName.Text.Trim(),
                    Password = txtPassword.Text,
                    DeviceId = App._DeviceId
                };
                model = await DoLogin(model);
                if (model != null && model.ResponseCode == 200) //200 for success
                {
                    App._Name = model.Name;
                    App._UserId = model.UserId;
                    lblMessage.Text = model.Message;
                    lblMessage.TextColor = Color.Green;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Application.Current.MainPage = new AttendanceForm();
                }
                else if (model.ResponseCode == 99)
                {
                    lblMessage.Text = model.Message;
                    lblMessage.TextColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.TextColor = Color.Red;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static async Task<LoginModel> DoLogin(LoginModel model)
        {
            string serializedModel = JsonConvert.SerializeObject(model);
            string response = await ApiAsyncPost(serializedModel, "DoLogin");
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