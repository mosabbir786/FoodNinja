using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using CommunityToolkit.Mvvm.Messaging;
using FoodNinja.Model;

namespace FoodNinja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        internal static readonly string Channel_ID = "TestChannel";
        internal static readonly int NotificationID = 101;
        public static TaskCompletionSource<bool> PermissionTcs { get; set; }
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Title = "";
            SupportActionBar.Hide();
            Platform.Init(this, savedInstanceState);
            await CreateNotificationChannel();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                {
                    if (key == "NavigationID")
                    {
                        string idValue = intent.Extras.GetString(key);
                        if (Preferences.ContainsKey("NavigationID"))
                            Preferences.Remove("NavigationID");

                        Preferences.Set("NavigationID", idValue);

                        WeakReferenceMessenger.Default.Send(new PushNotificationReceived("test"));
                    }
                }
            }
        }
        private async Task CreateNotificationChannel()
        {
            if (OperatingSystem.IsOSPlatformVersionAtLeast("android", 26))
            {
                var channel = new NotificationChannel(Channel_ID, "Test Notfication Channel", NotificationImportance.Default);

                var notificaitonManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
                notificaitonManager.CreateNotificationChannel(channel);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if(requestCode == 100)
            {
                if(grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    PermissionTcs?.TrySetResult(true);
                }
                else
                {
                    PermissionTcs?.TrySetResult(false);
                }
            }
        }
    }
}
