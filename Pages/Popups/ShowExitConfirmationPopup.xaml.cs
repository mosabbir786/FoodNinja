using CommunityToolkit.Maui.Views;

namespace FoodNinja.Pages.Popups;

public partial class ShowExitConfirmationPopup : Popup
{
	public ShowExitConfirmationPopup()
	{
		InitializeComponent();
	}

    private void Exit_Button_Clicked(object sender, EventArgs e)
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    private async void Cancel_Button_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}