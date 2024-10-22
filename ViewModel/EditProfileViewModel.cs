using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class EditProfileViewModel:ObservableObject
    {
        #region Property
        private readonly PermissionService permissionService = new PermissionService();
        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private bool isUpdated = false;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private string updatedImage;

        [ObservableProperty]
        private MemoryStream selectedImage;

        [ObservableProperty]
        private string imageFileName;

        [ObservableProperty]
        private bool isFileNameVisible;

        [ObservableProperty]
        private bool borderContentVisible = true;
        #endregion

        #region Constructor
        public EditProfileViewModel(INavigation navigation, FirebaseManager _firebaseManager, PermissionService _permissionService)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            this.permissionService = _permissionService;
            UserId = Preferences.Get("LocalId", string.Empty);
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task PickPhotoFromGallery()
        {
            IsFileNameVisible = true;
            var options = new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images
            };

            int deniedCount = permissionService.GetPermissionDeniedCount();
            if(deniedCount >= 2)
            {
                await Application.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                return;
            }
            var permissionGranted = await permissionService.CheckPhotoPermission();
            if(permissionGranted)
            {
                await PickPhoto(options);
            }
            else 
            {
                bool isPermissionGrantedAfterRequest = await permissionService.RequestPhotoPermission();
                if(isPermissionGrantedAfterRequest)
                {
                    await PickPhoto(options);
                }
            }

        }

        [RelayCommand]
        private async Task CapturePhotoFromCamera()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images
            };

            int deniedCount = permissionService.GetPermissionDeniedCount();
            if (deniedCount >= 2)
            {
                await Application.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                return;
            }
            var permissionGranted = await permissionService.CheckPhotoPermission();
            if (permissionGranted)
            {
                await PickPhoto(options);
            }
            else
            {
                bool isPermissionGrantedAfterRequest = await permissionService.RequestPhotoPermission();
                if (isPermissionGrantedAfterRequest)
                {
                    await PickPhoto(options);
                }
            }
        }

        [RelayCommand]
        private async Task Edit()
        {
        }
        #endregion

        #region Methods
        private async Task PickPhoto(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                       result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        SelectedImage = new MemoryStream();
                        using (var stream = await result.OpenReadAsync())
                        {
                            await stream.CopyToAsync(SelectedImage);
                        }
                        SelectedImage.Seek(0, SeekOrigin.Begin);
                        ImageFileName = TurncatedFileName(result.FileName, 30);
                        UpdatedImage = await ConvertMemoryStreamToBase64(SelectedImage);
                        BorderContentVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error wile picking image from gallery" + ex.Message);
            } 
        }

        private async Task<string>ConvertMemoryStreamToBase64(MemoryStream selectedImage)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                selectedImage.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        private string TurncatedFileName(string fileName, int maxLength)
        {
            if(fileName.Length <= maxLength)
            {
                return fileName;
            }
            else
            {
                return fileName.Substring(0, maxLength) + "....";
            }
        }
        #endregion
    }
}
