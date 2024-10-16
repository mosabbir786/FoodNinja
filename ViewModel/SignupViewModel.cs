using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Pages.Signup_Screen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class SignupViewModel:ObservableObject
    {
        #region Fields
        private readonly FirebaseManager _firebaseManager = new FirebaseManager();
        private readonly INavigation Navigation;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string userNameError = string.Empty;

        [ObservableProperty]
        private string emailError = string.Empty;

        [ObservableProperty]
        private string passwordError = string.Empty;

        [ObservableProperty]
        private bool isUserNameErrorVisible;

        [ObservableProperty]
        private bool isEmailErrorVisible;

        [ObservableProperty]
        private bool isPasswordErrorVisible;
        #endregion

        #region Constructor
        public SignupViewModel(INavigation navigation, FirebaseManager firebaseManager)
        {
            Navigation = navigation;
            _firebaseManager = firebaseManager;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task AlreadyAccount()
        {
            await Navigation.PopAsync();
        }
        [RelayCommand]
        private async Task Signup()
        {
            ValidateUserName(UserName);
            ValidateEmail(Email);
            ValidatePassword(Password);
            if (!IsUserNameErrorVisible && !IsEmailErrorVisible && !IsPasswordErrorVisible)
            {
                IsLoading = true;
                var response = await _firebaseManager.CreateAccount(UserName, Email, Password);
                if (response != null)
                {
                    await Navigation.PushAsync(new SecondSignUp(Email));
                }
                IsLoading = false;
            }
        }
        #endregion

        #region AllMethods

        #region Property Setters with Validation
        partial void OnUserNameChanged(string value)
        {
            ValidateUserName(value);
        }
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
        private void ValidateUserName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                UserNameError = "Username is required.";
                IsUserNameErrorVisible = true;
            }
            else if (value.Length < 4)
            {
                UserNameError = "User name must be at least 4 characters long.";
                IsUserNameErrorVisible = true;
            }
            else
            {
                UserNameError = string.Empty;
                IsUserNameErrorVisible = false;
            }
        }
        private void ValidateEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                EmailError = "Email is required.";
                IsEmailErrorVisible = true;
            }
            else if (value.Length < 5)
            {
                EmailError = "Email must be at least 5 characters long.";
                IsEmailErrorVisible = true;
            }
            else
            {
                EmailError = string.Empty;
                IsEmailErrorVisible = false;
            }
        }
        private void ValidatePassword(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PasswordError = "Password is required.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsLower))
            {
                PasswordError = "Password must contain at least one lowercase letter.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsUpper))
            {
                PasswordError = "Password must contain at least one uppercase letter.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(char.IsDigit))
            {
                PasswordError = "Password must contain at least one number.";
                IsPasswordErrorVisible = true;
            }
            else if (!value.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                PasswordError = "Password must contain at least one special character.";
                IsPasswordErrorVisible = true;
            }
            else if (value.Length < 6)
            {
                PasswordError = "Password must be at least 6 characters long.";
                IsPasswordErrorVisible = true;
            }
            else
            {
                PasswordError = string.Empty;
                IsPasswordErrorVisible = false;
            }
        }
        #endregion

        #endregion
    }
}
