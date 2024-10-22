using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages.ProfileTabScreen;

public partial class EditProfilePage : ContentPage
{
    private readonly FirebaseManager firebaseManager;
    private readonly PermissionService permissionService;
	public EditProfilePage()
	{
		InitializeComponent();
        firebaseManager = new FirebaseManager();
        permissionService = new PermissionService();
        this.BindingContext = new EditProfileViewModel(Navigation,firebaseManager,permissionService);
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
    }
    private async void firstNameEntry_Completed(object sender, EventArgs e)
    {
        await firstNameBorder.ScaleTo(1.1, 100);
        await firstNameBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        lastNameEntry.Focus();
    }
    private async void lastNameEntry_Completed(object sender, EventArgs e)
    {
        await lastNameBorder.ScaleTo(1.1, 100);
        await lastNameBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        phoneEntry.Focus();
    }
    private async void phoneEntry_Completed(object sender, EventArgs e)
    {
        await phoneNumberBorder.ScaleTo(1.1, 100);
        await phoneNumberBorder.ScaleTo(1, 100);
        phoneEntry.Unfocus();
    }

    private async void GalleryBorder_Tapped(object sender, TappedEventArgs e)
    {
        await galleryBorder.ScaleTo(1.1, 100);
        await galleryBorder.ScaleTo(1, 100);
        galleryBorder.BackgroundColor = Colors.Gray;
        for(double i = 1; i >= 0; i -= 0.1)
        {
            galleryBorder.Opacity = 1;
            await Task.Delay(20);
        }
        await Task.Delay(50);
        galleryBorder.BackgroundColor = Color.FromArgb("#80252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            galleryBorder.Opacity = i;
            await Task.Delay(20);
        }
    }

    private async void CameraBorder_Tapped(object sender, TappedEventArgs e)
    {
        await cameraBorder.ScaleTo(1.1, 100);
        await cameraBorder.ScaleTo(1, 100);
        cameraBorder.BackgroundColor = Color.FromArgb("#333333");
        for (double i = 1; i >= 0; i -= 0.1)
        {
            cameraBorder.Opacity = 1;
            await Task.Delay(20);
        }
        await Task.Delay(50);
        cameraBorder.BackgroundColor = Color.FromArgb("#80252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            cameraBorder.Opacity = i;
            await Task.Delay(20);
        }
    }
}