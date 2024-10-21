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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is ProfilePageViewModel viewModel)
        {
           await viewModel.FetchUserDataAsync();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    private async void AddPaymentButton_Clicked(object sender, EventArgs e)
    {
		await addMoreBtn.ScaleTo(1.1, 100);
		await addMoreBtn.ScaleTo(1, 100);
    }
}