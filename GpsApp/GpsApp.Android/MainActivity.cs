using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Android.Content;

namespace GpsApp.Droid
{
    [Activity(Label = "Aegis", Icon = "@drawable/logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                App._IsMockLocation = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AllowMockLocation).Equals("0");
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                App._IsMockLocation = false;
            }

            //Getting device unique id
#pragma warning disable CS0618 // Type or member is obsolete
            App._DeviceId = Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
#pragma warning restore CS0618 // Type or member is obsolete

        }

        public override void OnRequestPermissionsResult(int requestCode,
            string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode,
                permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions,
                grantResults);

            App._DeviceId = Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        }
    }
}