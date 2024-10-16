namespace FoodNinja.Pages.Onboarding_Screen;

public partial class SecondOnboarding : ContentPage
{
	public SecondOnboarding()
	{
		InitializeComponent();
	}

    private async void nextBtn_Clicked(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new LoginPage());
    }
}