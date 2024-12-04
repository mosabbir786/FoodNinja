using FoodNinja.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Services
{
    public class RestaurantService
    {
        public ObservableCollection<NearestRestaurantModel> Restaurant { get; set; }
        public ObservableCollection<PopularMenuModel> PopularMenu { get; set; }
        public NearestRestaurantModel? SelectedRestaurant { get; set; }
    }
}
