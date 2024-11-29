using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class NotificationViewModel : ObservableObject
    {
        #region Fields
        public INavigation Navigation { get; }
        private readonly FirebaseManager _firebaseManager;

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool noDataFoundVisible;

        [ObservableProperty]
        private bool notificationCollectionVisiblity;

        [ObservableProperty]
        private ObservableCollection<Item> dummyList;

        /*[ObservableProperty]
        private ObservableCollection<NotificationGroup> notificationList;*/

        [ObservableProperty]
        private ObservableCollection<SaveNotificationModel> notificationList;

        public  IAsyncRelayCommand DeleteNotificationCommand { get; }
        #endregion
        #region Constructor
        public NotificationViewModel(INavigation navigation, FirebaseManager firebaseManager)
        {
            Navigation = navigation;
            _firebaseManager = firebaseManager;
            DummyList = new ObservableCollection<Item>
            {
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
            };
            // NotificationList = new ObservableCollection<NotificationGroup>();
            UserId = Preferences.Get("LocalId", string.Empty);
            DeleteNotificationCommand = new AsyncRelayCommand<SaveNotificationModel>(async (selectedNotification) => await OnSelectedNotificationDelete(selectedNotification));

        }
        #endregion

        #region Methods
        public async Task LoadNotification()
        {
            var response =await _firebaseManager.FetchNotificationListAsync(UserId);
            if (response != null && response.Any())
            {
               /* var groupedData = response
                .GroupBy(n => n.ReceivedAt)
                .Select(g => new NotificationGroup(g.Key, new ObservableCollection<SaveNotificationModel>(g))).ToList();
                NotificationList = new ObservableCollection<NotificationGroup>(groupedData);
                foreach (var group in NotificationList)
                {
                    Console.WriteLine($"Group: {group.Date}");
                    foreach (var notification in group)
                    {
                        Console.WriteLine($" - Title: {notification.Title}, Message: {notification.Message}");
                    }
                }*/
                /* var grouped = response
                 .GroupBy(n => DateTime.ParseExact(n.ReceivedAt, "hh:mm tt dd-MM-yyyy", null).ToString("dd-MM-yyyy"))
                 .Select(g => new NotificationGroup(g.Key, g))
                 .ToList();
                 NotificationList = new ObservableCollection<NotificationGroup>(grouped);
                 foreach (var group in NotificationList)
                 {
                     Debug.WriteLine($"Group: {group.Date}");
                     foreach (var notification in group.Notifications)
                     {
                         Debug.WriteLine($"Notification: {notification.Title} - {notification.Message}");
                     }
                 }*/

                NotificationList = new ObservableCollection<SaveNotificationModel>(response);
                NotificationCollectionVisiblity = NotificationList.Any();
            }
            else
            {
                NoDataFoundVisible = true;
                Console.WriteLine("No notifications found.");
            }
        }

        private async Task OnSelectedNotificationDelete(SaveNotificationModel? selectedNotification)
        {
            var isDelete = await _firebaseManager.DeleteNotificationById(UserId, selectedNotification.Id);
            if(isDelete)
            {
                await Toast.Make("Notification Deleted").Show();
                NotificationList.Remove(selectedNotification);
            }
            else
            {
                await Toast.Make("Something went wrong.Please try again later").Show();
            }
        }
        #endregion

        #region Command

        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task ClearNotification()
        {
            var isClear = await _firebaseManager.ClearAllNotificationAsync(userId);
            if(isClear)
            {
                NotificationList.Clear();
                NotificationCollectionVisiblity = NotificationList.Any();
                NoDataFoundVisible = true;
            }
            else
            {
                await Toast.Make("Something went wrong.Please try again later").Show();
            }
        }
        #endregion
    }
    //public class NotificationGroup:ObservableCollection<SaveNotificationModel>
    //{
    //    public string Date { get; set; }
    // //   public List<SaveNotificationModel> Notifications { get; set; }
    //    public NotificationGroup(string date, ObservableCollection<SaveNotificationModel> notifications) : base(notifications) 
    //    {
    //        Date = date;
    //       // Notifications = notifications.ToList();
    //    }
    //}
}
