using CommunityToolkit.Maui.Views;
namespace FoodNinja.Pages.Popups;

public partial class NotificationPermissionPopup : Popup
{
	public NotificationPermissionPopup()
	{
		InitializeComponent();
	}

    private async void YesButton_Clicked(object sender, EventArgs e)
    {
#if ANDROID
            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionApplicationDetailsSettings); 
            intent.AddFlags(Android.Content.ActivityFlags.NewTask); 
            intent.SetData(Android.Net.Uri.FromParts("package", Android.App.Application.Context.PackageName, null));
            Android.App.Application.Context.StartActivity(intent);
            await CloseAsync();
#endif
    }

    private async void NoButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}