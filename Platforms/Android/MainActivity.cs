using Android.App;
using Android.Content.PM;
using Android.OS;

namespace FoodNinja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static TaskCompletionSource<bool> PermissionTcs { get; set; }
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Title = "";
            SupportActionBar.Hide();
            Platform.Init(this, savedInstanceState);
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
