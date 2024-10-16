using FoodNinja.ViewModel;

namespace FoodNinja.Pages.Signup_Screen;

public partial class FifthSignup : ContentPage
{
	public FifthSignup(string email, string firstName, string lastName, string mobileNumber, Stream uploadImage)
	{
		InitializeComponent();
        this.BindingContext = new FifthSignupViewModel(Navigation, email, firstName, lastName, mobileNumber, uploadImage);
    }
}