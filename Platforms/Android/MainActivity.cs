using Android.App;
using Android.Content.PM;
using Android.OS;

namespace FoodNinja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Title = "";
            SupportActionBar.Hide();
            Platform.Init(this, savedInstanceState);
        }
    }
}
