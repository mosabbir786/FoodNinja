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
    public partial class SecondSignupViewModel:ObservableObject
    {
        #region Fields
        private readonly INavigation Navigation;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string mobileNumber;

        [ObservableProperty]
        private string firstNameErrMsg;

        [ObservableProperty]
        private string lastNameErrMsg;

        [ObservableProperty]
        private string mobileNumberErrMsg;


        [ObservableProperty]
        private bool isFirstNameErrVisible;

        [ObservableProperty]
        private bool isLastNameErrVisible;

        [ObservableProperty]
        private bool isMobileNoErrVisible;

        [ObservableProperty]
        private string email;
        #endregion

        #region Constructor
        public SecondSignupViewModel(INavigation navigation, string email)
        {
            Navigation = navigation;
            Email = email;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Next()
        {
            ValidateFirstName(FirstName);
            ValidateLastName(LastName);
            ValidateMobileNumber(MobileNumber);
            if (!IsFirstNameErrVisible && !IsLastNameErrVisible && !IsMobileNoErrVisible)
            {
                await Navigation.PushAsync(new FourthSignUp(Email, FirstName, LastName, MobileNumber));
            }
        }
        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region AllMethods
        #region  Property Setters with validation
        partial void OnFirstNameChanged(string value)
        {
            ValidateFirstName(value);
        }

        partial void OnLastNameChanged(string value)
        {
            ValidateLastName(value);
        }

        partial void OnMobileNumberChanged(string value)
        {
            ValidateMobileNumber(value);
        }
        #endregion
        #region Validation Method
        private void ValidateFirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                FirstNameErrMsg = "Please enter first name.";
                IsFirstNameErrVisible = true;
            }
            else
            {
                IsFirstNameErrVisible = false;
            }
        }

        private void ValidateLastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LastNameErrMsg = "Please enter last name.";
                IsLastNameErrVisible = true;
            }
            else
            {
                IsLastNameErrVisible = false;
            }
        }

        private void ValidateMobileNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                MobileNumberErrMsg = "Please enter mobile number.";
                IsMobileNoErrVisible = true;
            }
            else if (value.Length < 10 || value.Length > 10)
            {
                MobileNumberErrMsg = "Number should be 10 digit.";
                IsMobileNoErrVisible = true;
            }
            else
            {
                IsMobileNoErrVisible = false;
            }
        }

        #endregion      
        #endregion
    }
}
