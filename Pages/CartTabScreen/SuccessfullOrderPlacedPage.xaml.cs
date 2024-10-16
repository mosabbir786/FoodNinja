using FoodNinja.Model;
using System.ComponentModel;

namespace FoodNinja.Pages.CartTabScreen;

public partial class SuccessfullOrderPlacedPage : ContentPage,INotifyPropertyChanged
{
    private OrderPlacedModel placedOrderItem;
    public OrderPlacedModel PlacedOrderItem
    {
        get { return placedOrderItem; }
        set
        {
            if (placedOrderItem != value)
            {
                placedOrderItem = value;
                OnPropertyChanged(nameof(PlacedOrderItem));
            }
        }
    }
    public SuccessfullOrderPlacedPage(OrderPlacedModel placedOrderItem)
	{
		InitializeComponent();
        PlacedOrderItem = placedOrderItem;
	}

    private async void TrackOrder_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrackOrderPage(PlacedOrderItem));
    }


    protected override bool OnBackButtonPressed()
    {
        Preferences.Set("ReturnFromPage", "SuccessfullOrderPlacedPage");
        Navigation.PopAsync();
        return base.OnBackButtonPressed();

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Preferences.Set("ReturnFromPage", "SuccessfullOrderPlacedPage");
    }
    private async void backBtn_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("ReturnFromPage", "SuccessfullOrderPlacedPage");
        await Navigation.PopAsync();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}