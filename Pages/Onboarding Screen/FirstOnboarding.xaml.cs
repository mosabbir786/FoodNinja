namespace FoodNinja.Pages.Onboarding_Screen;

public partial class FirstOnboarding : ContentPage
{
	public FirstOnboarding()
	{
		InitializeComponent();
	}

    private async void nextBtn_Clicked(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new SecondOnboarding());
    }
}