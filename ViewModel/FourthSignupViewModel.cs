using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Pages.Signup_Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class FourthSignupViewModel:ObservableObject
    {
        #region Fileds
        public INavigation Navigation { get; }

        [ObservableProperty]
        private string selectedImageLbl;

        [ObservableProperty]
        private bool isImageSelected;

        [ObservableProperty]
        private MemoryStream uploadImage;

        [ObservableProperty]
        private string capturedImageLbl;

        [ObservableProperty]
        private bool isImageCaptured;

        [ObservableProperty]
        private bool isCam;

        [ObservableProperty]
        private bool isGal;

        [ObservableProperty]
        private bool isImageErrorMsgVisible;

        [ObservableProperty]
        private string imageErrorMsg;

        private string FirstName;
        private string LastName;
        private string MobileNumber;
        private string Email;
        // private string PaymentMethod;
        #endregion

        public FourthSignupViewModel(INavigation navigation, string email, string firstName, string lastName, string mobileNumber/*, string paymentMethod*/)
        {
            Navigation = navigation;
            FirstName = firstName;
            LastName = lastName;
            MobileNumber = mobileNumber;
            Email = email;
            // PaymentMethod = paymentMethod;
        }

        #region Commands

        [RelayCommand]
        private async Task PickPhotoFromGallery()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images,
            };

            await PickAndShow(options);
        }

        [RelayCommand]
        private void DeSelectImage()
        {
            if (IsGal == true)
            {
                IsImageSelected = false;
                UploadImage = null;
                SelectedImageLbl = string.Empty;
                IsGal = false;
            }
            else if (IsCam == true)
            {
                IsImageCaptured = false;
                UploadImage = null;
                CapturedImageLbl = string.Empty;
                IsCam = false;
            }

        }

        [RelayCommand]
        private async Task CaptureImage()
        {
            IsCam = true;
            IsImageSelected = false;
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    UploadImage = new MemoryStream();
                    using (var stream = await photo.OpenReadAsync())
                    {
                        await stream.CopyToAsync(UploadImage);
                    }
                    CapturedImageLbl = TurncatedFileName(photo.FileName, 40);
                    IsImageCaptured = true;
                }
            }
        }

        [RelayCommand]
        private async Task NavigateNext()
        {
            if (UploadImage != null)
            {
                UploadImage.Seek(0, SeekOrigin.Begin);
                await Navigation.PushAsync(new FifthSignup(Email, FirstName, LastName, MobileNumber,/*PaymentMethod,*/ UploadImage));
            }
            else
            {
                IsImageErrorMsgVisible = true;
                ImageErrorMsg = "Please pick a image";
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region Methods
        public async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                IsGal = true;
                IsImageCaptured = false;
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        UploadImage = new MemoryStream();
                        using (var stream = await result.OpenReadAsync())
                        {
                            await stream.CopyToAsync(UploadImage);
                        }
                        UploadImage.Seek(0, SeekOrigin.Begin);
                        SelectedImageLbl = TurncatedFileName(result.FileName, 40);
                        IsImageSelected = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error wile picking image from gallery" + ex.Message);
            }

            return null;
        }

        private string TurncatedFileName(string fileName, int maxLength)
        {
            if (fileName.Length <= maxLength)
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
