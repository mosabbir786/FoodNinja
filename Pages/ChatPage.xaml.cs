using CommunityToolkit.Maui.Views;
using FoodNinja.Pages.Popups;

namespace FoodNinja.Pages;

public partial class ChatPage : ContentPage
{
	public ChatPage()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await this.ShowPopupAsync(new ShowExitConfirmationPopup());
        });
        return true;
    }
}