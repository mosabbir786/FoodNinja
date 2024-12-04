using FoodNinja.Services;
using FoodNinja.ViewModel;

namespace FoodNinja.Pages;

public partial class NotificationPage : ContentPage
{
	private FirebaseManager firebaseManager;
	public NotificationPage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new NotificationViewModel(Navigation, firebaseManager);
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		if(BindingContext is NotificationViewModel viewModel)
		{
			await viewModel.LoadNotification();
		}
    }
}