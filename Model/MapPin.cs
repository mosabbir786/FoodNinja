using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodNinja.Model
{
    public class MapPin
    {
        public string Id { get; set; }

        public string Address { get; set; }
        public string Label { get; set; }
        public Location Position { get; set; }
        public string IconSrc { get; set; }
        public ICommand ClickedCommand { get; set; }

        public MapPin(Action<MapPin> clicked)
        {
            ClickedCommand = new Command(() => clicked(this));
        }
    }
}
