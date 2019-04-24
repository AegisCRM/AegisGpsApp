using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
//using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GpsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttendanceForm : ContentPage
    {
        public AttendanceForm()
        {
            InitializeComponent();

            lblName.Text = string.Concat("Welcome ", App._Name);
            btnIn.IsVisible = false;
            btnOut.IsVisible = false;
            btnExit.IsVisible = false;
        }

        async protected override void OnAppearing()
        {
            try
            {
                AttendanceModel model = new AttendanceModel()
                {
                    UserId = Convert.ToString(App._UserId),
                    AttendanceMode = "GETSTATE"
                };

                AttendanceModel resultobject = await GetAttendanceState(model);
                if (resultobject.ResponseCode == 200)
                {
                    if (resultobject.CurrentState.ToLower().Equals("in"))
                    {
                        btnIn.IsVisible = false;
                        btnOut.IsVisible = true;
                        btnExit.IsVisible = false;
                    }
                    else
                    {
                        btnIn.IsVisible = true;
                        btnOut.IsVisible = false;
                        btnExit.IsVisible = false;
                    }
                }
                else if (resultobject.ResponseCode == 99)
                {
                    lblMessage.Text = resultobject.Message;
                    lblMessage.TextColor = Color.Red;
                }
                else
                {
                    lblMessage.Text = "Something went wrong. Please retry or contact Aegis.";
                    lblMessage.TextColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                lblMessage.Text = ex.Message;
                lblMessage.TextColor = Color.Red;
            }
        }

        async private void Button_Clicked_In(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;
                IGeolocator locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                Position position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                AttendanceModel model = new AttendanceModel()
                {
                    IsMockedLocation = App._IsMockLocation.ToString(),
                    Latitude = Convert.ToString(position.Latitude),
                    Longitude = Convert.ToString(position.Longitude),
                    UserId = Convert.ToString(App._UserId),
                    AttendanceMode = "IN"
                };

                await CallApiForAttendance(model);
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

        async private void Button_Clicked_Out(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;
                IGeolocator locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                Position position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                AttendanceModel model = new AttendanceModel()
                {
                    IsMockedLocation = App._IsMockLocation.ToString(),
                    Latitude = Convert.ToString(position.Latitude),
                    Longitude = Convert.ToString(position.Longitude),
                    UserId = Convert.ToString(App._UserId),
                    AttendanceMode = "OUT"
                };
                await CallApiForAttendance(model);
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

        private void Button_Clicked_Exit(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private async System.Threading.Tasks.Task CallApiForAttendance(AttendanceModel model)
        {
            try
            {
                AttendanceModel resultobject = await PostAttendance(model);
                if (resultobject.ResponseCode == 200)
                {
                    btnIn.IsVisible = false;
                    btnOut.IsVisible = false;
                    btnExit.IsVisible = true;
                    lblMessage.Text = resultobject.Message;
                    lblMessage.TextColor = Color.Green;
                }
                else if (resultobject.ResponseCode == 99)
                {
                    lblMessage.Text = resultobject.Message;
                    lblMessage.TextColor = Color.Red;
                }
                else
                {
                    lblMessage.Text = "Something went wrong. Please retry or contact Aegis.";
                    lblMessage.TextColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                lblMessage.Text = ex.Message;
                lblMessage.TextColor = Color.Red;
            }
        }

        public static async System.Threading.Tasks.Task<AttendanceModel> PostAttendance(AttendanceModel model)
        {
            AttendanceModel resultobject = new AttendanceModel();
            string serializedModel = JsonConvert.SerializeObject(model);
            string response = await ApiAsyncPost(serializedModel, "MakeAttendance");
            resultobject = JsonConvert.DeserializeObject<AttendanceModel>(response);
            return resultobject;
        }

        public static async System.Threading.Tasks.Task<AttendanceModel> GetAttendanceState(AttendanceModel model)
        {
            AttendanceModel resultobject = new AttendanceModel();
            string serializedModel = JsonConvert.SerializeObject(model);
            string response = await ApiAsyncPost(serializedModel, "GetAttendanceState");
            resultobject = JsonConvert.DeserializeObject<AttendanceModel>(response);
            return resultobject;
        }

        private static async System.Threading.Tasks.Task<string> ApiAsyncPost(string serializedModel, string action)
        {
            var client = new System.Net.Http.HttpClient();

            client.BaseAddress = new Uri("http://api.aegiscrm.in/api/Attendance/" + action);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new StringContent(serializedModel, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://api.aegiscrm.in/api/Attendance/" + action, content);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
    }
}