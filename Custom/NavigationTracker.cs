using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Custom
{
    public class NavigationTracker
    {
        private static readonly List<string> _navigationHistory = new List<string>();

        /// <summary>
        /// Adds a page name to the navigation tracker.
        /// </summary>
        /// <param name="pageName">The name of the page to add.</param>
        public static void AddPage(string pageName)
        {
            _navigationHistory.Add(pageName);
        }

        /// <summary>
        /// Gets the complete navigation history.
        /// </summary>
        /// <returns>A list of page names.</returns>
        public static int GetNavigationHistory()
        {
            return _navigationHistory.Count();
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        public static void ClearHistory()
        {
            _navigationHistory.Clear();
        }


       public static List<string> GetNavigationHistoryPageName()
       {
           return new List<string>(_navigationHistory);
       }
    }
}
