using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Model
{
    public class HomePageModel
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public string Distance { get; set; }
        public string RestaurantImage { get; set; }
    }

    public class PopularMenuModel
    {
        public int Id { get; set; }
        public string DishName { get; set; }
        public string RestaurantName { get; set; }
        public double Price { get; set; }
        public string FoodImage { get; set; }
        public int RestaurantId { get; set; }
        public double FoodRating { get; set; }
        public string RestaurantAddress { get; set; }
        public string FoodDescription { get; set; }
        public string RestaurantDescription { get; set; }
        public double RestaurantLong { get; set; }
        public double RestaurantLat { get; set; }

        public ImageSource FoodImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(FoodImage))
                {
                    byte[] imageBytes = Convert.FromBase64String(FoodImage);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                return null;
            }
        }
    }

    public class NearestRestaurantModel
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public double RestrauntDistance { get; set; }
        public string RestaurantImage { get; set; }
        public string Description { get; set; }
        public ImageSource RestrauntImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(RestaurantImage))
                {
                    byte[] imageBytes = Convert.FromBase64String(RestaurantImage);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                return null;
            }
        }
        public List<FoodItemModel> FoodItems { get; set; } = new List<FoodItemModel>();
        public double RestaurantRating { get; set; }
        public string RestroAddress { get; set; }
        public double RestaurantLat { get; set; }
        public double RestaurantLong { get; set; }
    }

    public partial class FoodItemModel : ObservableObject
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public string FoodImage { get; set; }
        public double FoodPrice { get; set; }
        public string FoodDescription { get; set; }
        public double FoodRating { get; set; }
        public double RestaurantLat { get; set; }
        public double RestaurantLong { get; set; }

        public ImageSource FoodImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(FoodImage))
                {
                    byte[] imageBytes = Convert.FromBase64String(FoodImage);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                return null;
            }
        }

        [ObservableProperty]
        private bool isFullFoodDescriptionVisible;

        public string ShortFoodDescription => GetShortFoodDescription(FoodDescription);

        public string DisplayFoodDescription => IsFullFoodDescriptionVisible ? FoodDescription : ShortFoodDescription;

        public string FoodMoreText => IsFullFoodDescriptionVisible ? "Show Less" : "Show More";

        private string GetShortFoodDescription(string foodDescription)
        {
            return foodDescription.Length > 50 ? foodDescription.Substring(0, 110) + "..." : foodDescription;
        }

        [RelayCommand]
        public void ToggleFoodDescription()
        {
            IsFullFoodDescriptionVisible = !IsFullFoodDescriptionVisible;
            OnPropertyChanged(nameof(DisplayFoodDescription));
            OnPropertyChanged(nameof(FoodMoreText));
        }
    }
}
