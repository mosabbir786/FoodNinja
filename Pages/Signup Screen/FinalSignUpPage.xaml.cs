namespace FoodNinja.Pages.Signup_Screen;

public partial class FinalSignUpPage : ContentPage
{
	public FinalSignUpPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {

        Console.WriteLine("***************" + Navigation.NavigationStack.Count());
        await Navigation.PopToRootAsync();

        /* int pageIndex = Navigation.NavigationStack.ToList().FindIndex(p => p is LoginPage);

         if (pageIndex >= 0)
         {
             while (Navigation.NavigationStack.Count > pageIndex + 1)
             {
                 Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
             }
         }
         else
         {
             Console.WriteLine("Third Page not found in the navigation stack!");
         }*/
    }
}