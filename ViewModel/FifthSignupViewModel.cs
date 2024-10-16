using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Pages.Signup_Screen;
using Microsoft.Maui.Graphics.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class FifthSignupViewModel:ObservableObject
    {
        #region Field
        public INavigation Navigation { get; }


        [ObservableProperty]
        private Stream finalImage;

        [ObservableProperty]
        private ImageSource finalImageSource;

        [ObservableProperty]
        private string base64String;

        private string FirstName;
        private string LastName;
        private string MobileNumber;
        private string PaymentMethod;
        private string Email;
        #endregion

        #region Constructor
        public FifthSignupViewModel(INavigation navigation, string email, string firstName, string lastName, string mobileNumber,/* string paymentMethod,*/ Stream uploadImage)
        {
            Navigation = navigation;
            Base64String = ConvertStreamToBase64(uploadImage);
            uploadImage.Seek(0, SeekOrigin.Begin);
            FirstName = firstName;
            LastName = lastName;
            MobileNumber = mobileNumber;
            Email = email;
            //PaymentMethod = paymentMethod;
            this.FinalImage = uploadImage;
            if (uploadImage != null)
            {
                // this code also work great for resizing image 
                /* Microsoft.Maui.Graphics.IImage image = PlatformImage.FromStream(uploadImage);
                 var resizeImage = image.Resize(250, 280);
                 var str = resizeImage.AsStream();
                 var lngh = str.Length;
                 uploadImage.Seek(0, SeekOrigin.Begin);
                 FinalImageSource = ImageSource.FromStream(() => str);*/
                ResizeAndSetImage(uploadImage);
            }
        }
        #endregion

        #region Methods
        private string ConvertStreamToBase64(Stream uploadImage)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                uploadImage.CopyTo(memoryStream); // Copy the image stream to the memory stream
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        private void ResizeAndSetImage(Stream uploadImage)
        {
            if (uploadImage != null)
            {
                Microsoft.Maui.Graphics.IImage image = PlatformImage.FromStream(uploadImage);
                int desiredWidth = 280;
                int desiredHeight = 270;
                var resizedImage = image.Resize(desiredWidth, desiredHeight, ResizeMode.Stretch);
                FinalImage = resizedImage.AsStream();
                FinalImage.Seek(0, SeekOrigin.Begin);
                FinalImageSource = ImageSource.FromStream(() => FinalImage);
            }
        }

        [RelayCommand]
        private async Task NavigateNext()
        {
            await Navigation.PushAsync(new SixthSignUp(Email, FirstName, LastName, MobileNumber, Base64String));
        }

        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }
        #endregion
    }
}
