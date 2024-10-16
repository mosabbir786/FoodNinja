using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja
{
    public class TabBarEventArgs : EventArgs
    {
        public AppShell.PageType CurrentPage { get; private set; }
        public TabBarEventArgs(AppShell.PageType currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
