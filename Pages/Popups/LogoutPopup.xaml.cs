using CommunityToolkit.Maui.Views;
using FoodNinja.Services;

namespace FoodNinja.Pages.Popups;

public partial class LogoutPopup : Popup
{
	private readonly FirebaseManager firebaseManager;
	public LogoutPopup()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
	}

    private async void YesButton_Clicked(object sender, EventArgs e)
    {
        await firebaseManager.LogoutAsync();
        await CloseAsync();
    }

    private async void NoButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}