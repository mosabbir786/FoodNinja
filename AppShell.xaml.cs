using FoodNinja.Pages;
using Microsoft.Maui.Controls;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;

namespace FoodNinja
{
    public partial class AppShell : SimpleShell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("homepage", typeof(HomePage));
            Routing.RegisterRoute("profilepage", typeof(ProfilePage));
            Routing.RegisterRoute("cartpage", typeof(CartPage));
            Routing.RegisterRoute("chatpage", typeof(ChatPage));
            AddTab(typeof(HomePage), PageType.HomePage);
            AddTab(typeof(ProfilePage), PageType.ProfilePage);
            AddTab(typeof(CartPage), PageType.CartPage);
            AddTab(typeof(ChatPage), PageType.ChatPage);
            Loaded += AppShellLoacded;
        }

        private void AppShellLoacded(object? sender, EventArgs e)
        {
            var shell = sender as AppShell;
            shell.Window.SubscribeToSafeAreaChanges(safeArea =>
            {
                shell.pageContainer.Margin = safeArea;
                shell.tabBarView.TabsPadding = new Thickness(safeArea.Left, 0, safeArea.Right, safeArea.Bottom);
            });
        }
        private void AddTab(Type page, PageType pageEnum)
        {
            var tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
            tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });
            tabBar.Items.Add(tab);
        }
        private async void TabBarViewCurrentPageChanged(object sender, TabBarEventArgs e)
        {
            await Shell.Current.GoToAsync("///" + e.CurrentPage.ToString());
            /* var navigationPage = (NavigationPage)Application.Current.MainPage;
             if (navigationPage != null)
             {
                 Page page = null;
                 switch (e.CurrentPage.ToString())
                 {
                     case "Page 1":
                         page = new HomePage();
                         break;
                     case "Page 2":
                         page = new ProfilePage();
                         break;
                     case "Page 3":
                         page = new CartPage();
                         break;
                     case "Page 4":
                         page = new ChatPage();
                         break;
                 }

                 if (page != null)
                 {
                     await Navigation.PushAsync(page);
                 }
             }*/
        }
        public enum PageType
        {
            HomePage, ProfilePage, CartPage, ChatPage
        }
    }
}
