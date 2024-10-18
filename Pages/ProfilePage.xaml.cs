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
    private async void AddPaymentButton_Clicked(object sender, EventArgs e)
    {
		await addMoreBtn.ScaleTo(1.1, 100);
		await addMoreBtn.ScaleTo(1, 100);
    }
}