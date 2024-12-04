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
			viewModel.IsLoading = true;
			await Task.Delay(500);
			await viewModel.LoadNotification();
			viewModel.IsLoading = false;
		}
    }
}