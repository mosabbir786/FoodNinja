using FoodNinja.ViewModel;

namespace FoodNinja.Pages.Signup_Screen;

public partial class FourthSignUp : ContentPage
{
	public FourthSignUp(string email,string firstName,string lastName,string mobileNumber)
	{
		InitializeComponent();
        this.BindingContext = new FourthSignupViewModel(Navigation, email, firstName, lastName, mobileNumber/*,paymentMethod*/);

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FourthSignupViewModel viewModel)
        {
            viewModel.IsImageSelected = false;
            viewModel.IsImageCaptured = false;
            viewModel.UploadImage = null;
            viewModel.SelectedImageLbl = string.Empty;
            viewModel.CapturedImageLbl = string.Empty;
        }
    }
    private async void CrossIcon_Tapped(object sender, TappedEventArgs e)
    {
        await crossIcon.ScaleTo(1.1, 100);
        await crossIcon.ScaleTo(1, 100);
    }

    private async void Gallery_Tapped(object sender, TappedEventArgs e)
    {
        await galBorder.ScaleTo(1.1, 100);
        await galBorder.ScaleTo(1, 100);
    }

    private async void Camera_Tapped(object sender, TappedEventArgs e)
    {
        await camBorder.ScaleTo(1.1, 100);
        await camBorder.ScaleTo(1, 100);
    }
}