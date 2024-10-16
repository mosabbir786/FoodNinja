using FoodNinja.Model;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.HomeTabScreen;

public partial class PopularMenuPage : ContentPage
{
	public PopularMenuPage(ObservableCollection<PopularMenuModel> popularMenueList)
	{
		InitializeComponent();
	}
}