using CommunityToolkit.Maui.Views;
#if IOS
using UIKit;
using Foundation;
#elif ANDROID
using Android.Content;
using Android.App;
#endif
using Microsoft.Maui.ApplicationModel;


namespace FoodNinja.Pages.Popups;

public partial class PermissionPopup : Popup
{
	public PermissionPopup()
	{
		InitializeComponent();
	}

    
    private async void YesButton_Clicked(object sender, EventArgs e)
    {
#if ANDROID
        var context = Platform.CurrentActivity;
        var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
        intent.AddFlags(ActivityFlags.NewTask);
        intent.SetData(Android.Net.Uri.FromParts("package", Android.App.Application.Context.PackageName, null));
        Android.App.Application.Context.StartActivity(intent);
        await CloseAsync();
#elif IOS
        var url = new NSUrl(UIApplication.OpenSettingsUrlString);
        if (UIApplication.SharedApplication.CanOpenUrl(url))
        {
            UIApplication.SharedApplication.OpenUrl(url);
        }
#endif
    }

    private async void NoButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}