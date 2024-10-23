using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using FoodNinja.Model;
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
        private bool borderContentVisible = true;

        [ObservableProperty]
        private bool cameraBorderContentVisible = true;

        [ObservableProperty]
        private string methodType;
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
            var options = new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images
            };

            int deniedCount =  permissionService.GetPermissionDeniedCount();
            if(DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (deniedCount == 1)
                {
                    await Application.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                    return;
                }
            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                if (deniedCount >= 2)
                {
                    await Application.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                    return;
                }
            }     
            
            var permissionGranted = await permissionService.CheckPhotoPermission();
            if(permissionGranted)
            {
                MethodType = "Gallery";
                await PickPhoto(options);
            }
            else  
            {
                bool isPermissionGrantedAfterRequest = await permissionService.RequestPhotoPermission();
                if(isPermissionGrantedAfterRequest)
                {
                    MethodType = "Gallery";
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

            var deniedCount =  permissionService.GetCameraPermissionDeniedCount();
            if(DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if(deniedCount == 1)
                {
                    await App.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                }
            }
            else if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                if(deniedCount >= 2)
                {
                    await App.Current.MainPage.ShowPopupAsync(new PermissionPopup());
                }
            }

            /* var permissionGranted = await permissionService.CheckAndRequestCameraPermission();
             if (permissionGranted)
             {
                 MethodType = "Camera";
                 await PickPhoto(options);
             }
             else
             {
                 bool isPermissionGrantedAfterRequest = await permissionService.CheckAndRequestCameraPermission();
                 if (isPermissionGrantedAfterRequest)
                 {
                     MethodType = "Camera";
                     await PickPhoto(options);
                 }
             }*/

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
#if ANDROID
               var permissionGranted = await permissionService.CheckCameraPermission();
               if (permissionGranted)
               {
                    MethodType = "Camera";
                    await PickPhoto(options);
               }
               else
               {
                 bool isPermissionGrantedAfterRequest = await permissionService.RequestCameraPermission();
                 if (isPermissionGrantedAfterRequest)
                 {
                     MethodType = "Camera";
                     await PickPhoto(options);
                 }
              }
#endif
            }
        }

        [RelayCommand]
        private async Task RemoveGalleryImage()
        {
            await Task.Delay(1);
            if (MethodType == "Gallery")
            {
                ImageFileName = string.Empty;
                UpdatedImage = string.Empty;
                SelectedImage = null;
                BorderContentVisible = true;
            }
            else if (MethodType == "Camera")
            {
                ImageFileName = string.Empty;
                UpdatedImage = string.Empty;
                SelectedImage = null;
                CameraBorderContentVisible = true;
            }
          
        }

        [RelayCommand]
        private async Task Edit()
        {
            bool isEmpty = string.IsNullOrWhiteSpace(FirstName) && 
                           string.IsNullOrWhiteSpace(LastName) && 
                           string.IsNullOrWhiteSpace(PhoneNumber) 
                           && string.IsNullOrWhiteSpace(UpdatedImage);
            if(isEmpty)
            {
                await Toast.Make("Please fill in at least one field to edit your profile.").Show();
            }
            else
            {
                await EditProfileAsync();
            }
        }
#endregion

        #region Methods
        private async Task EditProfileAsync()
        {
            var updatedData = new UserDataModel
            {
                FirstName = FirstName,
                LastName = LastName,
                MobileNumber = PhoneNumber,
                Image = UpdatedImage,
            };

            IsLoading = true;
            var success = await firebaseManager.EditUserProfile(UserId,updatedData);
            if(success)
            {
                IsLoading = false;
                FirstName = string.Empty;
                LastName = string.Empty;
                PhoneNumber = string.Empty;
                UpdatedImage = string.Empty;
                await Toast.Make("Profile edited successfully!").Show();
                await Navigation.PopAsync();
            }
            else
            {
                await Toast.Make("Something went wrong.Please try after some time").Show();
            }
        }
        private async Task PickPhoto(PickOptions options)
        {
            try
            {
                if (MethodType == "Gallery")
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
                else if (MethodType == "Camera")
                {
                    if(MediaPicker.Default.IsCaptureSupported)
                    {
                        var photo = await MediaPicker.Default.CapturePhotoAsync();
                        if(photo != null)
                        {
                            SelectedImage = new MemoryStream();
                            using (var stream = await photo.OpenReadAsync())
                            {
                                await stream.CopyToAsync(SelectedImage);
                            }
                            SelectedImage.Seek(0, SeekOrigin.Begin);
                            ImageFileName = TurncatedFileName(photo.FileName, 30);
                            UpdatedImage = await ConvertMemoryStreamToBase64(SelectedImage);
                            CameraBorderContentVisible = false;
                        }
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
            await Task.Delay(1);
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
