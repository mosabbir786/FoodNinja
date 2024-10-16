using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodNinja.Model;
using FoodNinja.Services;
using System.ComponentModel;

namespace FoodNinja.Pages.Popups;

public partial class ReplaceCartItemPopup : Popup, INotifyPropertyChanged
{
    private FirebaseManager firebaseManager;
    private Func<Task> updateCartCallback;

    private NearestRestaurantModel selectedRestaurant;
    public NearestRestaurantModel SelectedRestaurant
    {
        get { return selectedRestaurant; }
        set
        {
            if (selectedRestaurant != value)
            {
                selectedRestaurant = value;
                OnPropertyChanged(nameof(SelectedRestaurant));
            }
        }
    }

    private FoodItemModel selectedFood;
    public FoodItemModel SelectedFood
    {
        get { return selectedFood; }
        set
        {
            if (selectedFood != value)
            {
                selectedFood = value;
                OnPropertyChanged(nameof(SelectedFood));
            }
        }
    }

    private PopularMenuModel selectedMenuFood;
    public PopularMenuModel SelectedMenuFood
    {
        get { return selectedMenuFood; }
        set
        {
            if(selectedMenuFood != value)
            {
                selectedMenuFood = value;
                OnPropertyChanged(nameof(SelectedMenuFood));
            }
        }
    }


    private string currentPage;
    public string CurrentPage
    {
        get { return currentPage; }
        set
        {
            if(currentPage != value)
            {
                currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
    }

    private string UserId;
    private bool isItemReplaced = false;
    public ReplaceCartItemPopup(FirebaseManager _firebaseManager, Func<Task> _updateCartCallback)
    {
        InitializeComponent();
        firebaseManager = _firebaseManager;
        updateCartCallback = _updateCartCallback;
        UserId = Preferences.Get("LocalId", string.Empty);
    }
    private async void NoButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
    private async void ReplaceButton_Clicked(object sender, EventArgs e)
    {
        switch(CurrentPage)
        {
            case "RestaurantDetailPage":
                if (SelectedFood == null)
                {
                    await Toast.Make("No food selected").Show();
                    return;
                }
                loader.IsVisible = true;
                loader.IsAnimationEnabled = true;
                await firebaseManager.DeleteRestaurantToReplaceAync(UserId, SelectedRestaurant.Id);
                var random = new Random();
                int id = random.Next(1000, 9999);
                var addFoodToCart = new AddFoodToCart
                {
                    CartId = id,
                    RestaurantId = SelectedRestaurant.Id,
                    RestaurantName = SelectedRestaurant.RestaurantName,
                    RestaurantDescription = SelectedRestaurant.Description,
                    RestaurantAddress = SelectedRestaurant.RestroAddress,
                    RestaurantRating = SelectedRestaurant.RestaurantRating,
                    RestaurantDistance = SelectedRestaurant.RestrauntDistance,
                    FoodId = selectedFood.FoodId,
                    FoodImage = selectedFood.FoodImage,
                    FoodDescription = selectedFood.FoodDescription,
                    FoodPrice = selectedFood.FoodPrice,
                    FoodRating = selectedFood.FoodRating,
                    FoodName = selectedFood.FoodName,
                };
                bool response = false;
                try
                {
                    response = await firebaseManager.AddFoodToCartAsync(addFoodToCart);
                    if (response)
                    {
                        loader.IsAnimationEnabled = false;
                        loader.IsVisible = false;
                        await updateCartCallback();
                        isItemReplaced = true;
                        await CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while replacing the " + ex.Message);
                }
                finally
                {
                    loader.IsAnimationEnabled = false;
                    loader.IsVisible = false;
                }
                break;

            case "HomePage":
                if (SelectedMenuFood == null)
                {
                    await Toast.Make("No food selected").Show();
                    return;
                }
                else
                {
                    loader.IsVisible = true;
                    loader.IsAnimationEnabled = true;
                    await firebaseManager.DeleteRestaurantToReplaceAync(UserId, SelectedMenuFood.RestaurantId);
                    var random1 = new Random();
                    int id1 = random1.Next(1000, 9999);
                    var addFoodToCartFromPopularMenue = new AddFoodToCart
                    {
                        CartId = id1,
                        RestaurantId = SelectedMenuFood.RestaurantId,
                        RestaurantName = SelectedMenuFood.RestaurantName,
                        RestaurantDescription = SelectedMenuFood.RestaurantDescription,
                        RestaurantAddress = SelectedMenuFood.RestaurantAddress,
                        FoodId = SelectedMenuFood.Id,
                        FoodImage = SelectedMenuFood.FoodImage,
                        FoodDescription = SelectedMenuFood.FoodDescription,
                        FoodPrice = SelectedMenuFood.Price,
                        FoodRating = SelectedMenuFood.FoodRating,
                        FoodName = SelectedMenuFood.DishName,
                    };
                    bool response1 = false;
                    try
                    {
                        response1 = await firebaseManager.AddFoodToCartAsync(addFoodToCartFromPopularMenue);
                        if (response1)
                        {
                            loader.IsAnimationEnabled = false;
                            loader.IsVisible = false;
                            await updateCartCallback();
                            isItemReplaced = true;
                            await CloseAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while replacing the " + ex.Message);
                    }
                    finally
                    {
                        loader.IsAnimationEnabled = false;
                        loader.IsVisible = false;
                    }
                }
                break;

            case "PopularMenuPage":
                if (SelectedMenuFood == null)
                {
                    await Toast.Make("No food selected").Show();
                    return;
                }
                else
                {
                    loader.IsVisible = true;
                    loader.IsAnimationEnabled = true;
                    await firebaseManager.DeleteRestaurantToReplaceAync(UserId, SelectedMenuFood.RestaurantId);
                    var random1 = new Random();
                    int id1 = random1.Next(1000, 9999);
                    var addFoodToCartFromPopularMenue = new AddFoodToCart
                    {
                        CartId = id1,
                        RestaurantId = SelectedMenuFood.RestaurantId,
                        RestaurantName = SelectedMenuFood.RestaurantName,
                        RestaurantDescription = SelectedMenuFood.RestaurantDescription,
                        RestaurantAddress = SelectedMenuFood.RestaurantAddress,
                        FoodId = SelectedMenuFood.Id,
                        FoodImage = SelectedMenuFood.FoodImage,
                        FoodDescription = SelectedMenuFood.FoodDescription,
                        FoodPrice = SelectedMenuFood.Price,
                        FoodRating = SelectedMenuFood.FoodRating,
                        FoodName = SelectedMenuFood.DishName,
                    };
                    bool response1 = false;
                    try
                    {
                        response1 = await firebaseManager.AddFoodToCartAsync(addFoodToCartFromPopularMenue);
                        if (response1)
                        {
                            loader.IsAnimationEnabled = false;
                            loader.IsVisible = false;
                            await updateCartCallback();
                            isItemReplaced = true;
                            await CloseAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while replacing the " + ex.Message);
                    }
                    finally
                    {
                        loader.IsAnimationEnabled = false;
                        loader.IsVisible = false;
                    }
                }
                break;
        }
       
    }
    private async void CrossButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    public bool IsItemReplaced()
    {
        return isItemReplaced;
    }

    #region INOtifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

