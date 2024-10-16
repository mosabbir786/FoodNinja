using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class LoginViewModel:ObservableObject
    {
        #region Fields
        private readonly FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isEmailErrorVisible;

        [ObservableProperty]
        private bool isPasswordErrorVisible;

        [ObservableProperty]
        private string emailErrorMsg = string.Empty;

        [ObservableProperty]
        private string passwordErrorMsg = string.Empty;

        #endregion


        #region Constructor
        public LoginViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
        }
        #endregion

        #region Commands

        [RelayCommand]
        private async Task Login()
        {

            ValidateEmail(Email);
            ValidatePassword(Password);
            if (!IsEmailErrorVisible && !IsPasswordErrorVisible)
            {
                try
                {
                    IsLoading = true;
                    var response = await firebaseManager.LoginAsync(Email, Password).ConfigureAwait(false);
                    if (response != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = new AppShell();
                        });
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Login failed, please try again.", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                }
                finally
                {
                    IsLoading = false;
                }

            }
        }
        #endregion

        #region Methods

        #region Property Setters with Validation
        partial void OnEmailChanged(string value)
        {
            ValidateEmail(value);
        }

        partial void OnPasswordChanged(string value)
        {
            ValidatePassword(value);
        }
        #endregion

        #region Validation Method
        private void ValidateEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                EmailErrorMsg = "Email is required.";
                IsEmailErrorVisible = true;
            }
            else if (value.Length < 5)
            {
                EmailErrorMsg = "Email must be at least 5 characters long.";
                IsEmailErrorVisible = true;
            }
            else
            {
                EmailErrorMsg = string.Empty;
                IsEmailErrorVisible = false;
            }
        }

        private void ValidatePassword(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PasswordErrorMsg = "Password is required.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsLower))
            {
                PasswordErrorMsg = "Password must contain at least one lowercase letter.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsUpper))
            {
                PasswordErrorMsg = "Password must contain at least one uppercase letter.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsDigit))
            {
                PasswordErrorMsg = "Password must contain at least one number.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                PasswordErrorMsg = "Password must contain at least one special character.";
                IsPasswordErrorVisible = true;
            }
            else if (value.Length < 6)
            {
                PasswordErrorMsg = "Password must be at least 6 characters long.";
                IsPasswordErrorVisible = true;
            }
            else
            {
                PasswordErrorMsg = string.Empty;
                IsPasswordErrorVisible = false;
            }
        }
        #endregion

        #endregion

    }
}
