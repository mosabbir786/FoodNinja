using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Custom
{
    public class BoxSkiletonView:BoxView
    {
        public BoxSkiletonView()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1.5), () =>
            {
                this.FadeTo(0.5, 750, Easing.CubicInOut).ContinueWith((x) =>
                {
                    this.FadeTo(1, 750, Easing.CubicInOut);
                });
                return true;
            });
        }
    }
}
