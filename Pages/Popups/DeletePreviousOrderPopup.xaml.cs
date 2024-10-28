using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using FoodNinja.Services;

namespace FoodNinja.Pages.Popups;

public partial class DeletePreviousOrderPopup : Popup
{
	private readonly FirebaseManager firebaseManager;
	private string UserId;
	private int OrderId;
	private Action<int> OnDeleteConfirmd;
	public DeletePreviousOrderPopup(int orderId, Action<int> onDeleteConfirmed)
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.OrderId = orderId;
		UserId = Preferences.Get("LocalId", string.Empty);
		this.OnDeleteConfirmd = onDeleteConfirmed;
	}

    private async void Cancel_Button_Clicked(object sender, EventArgs e)
    {
		await CloseAsync();
    }

    private async void Delete_Button_Clicked(object sender, EventArgs e)
    {
		var isDeleted = await firebaseManager.DeletePlacedOrderByIdAsync(UserId, OrderId);
		if(isDeleted)
		{
			OnDeleteConfirmd?.Invoke(OrderId);
			await Toast.Make("Order deleted successfully.").Show();
			await CloseAsync();
		}
		else
		{
			await Toast.Make("Something went wrong.Please try again later.").Show();
		}
    }
}