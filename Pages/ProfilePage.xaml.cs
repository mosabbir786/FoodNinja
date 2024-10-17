using CommunityToolkit.Maui.Views;
using FoodNinja.Services;
using FoodNinja.ViewModel;

namespace FoodNinja.Pages;

public partial class ProfilePage : ContentPage
{
	private FirebaseManager firebaseManager;
	public ProfilePage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new ProfilePageViewModel(Navigation, firebaseManager);
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        //await this.ShowPopupAsync(new LogoutPopup());
    }
    private async void AddPaymentButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddPaymentMethodPage());
    }

 
  

}