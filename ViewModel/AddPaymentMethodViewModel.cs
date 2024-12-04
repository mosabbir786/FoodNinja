using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class AddPaymentMethodViewModel : ObservableObject
    {
        #region Fields
        private FirebaseManager firebaseManager;
        private FoodNinja.Views.BottomSheet currentBottomSheet;

        [ObservableProperty]
        private Color paypalBorderColor;

        [ObservableProperty]
        private Color visaBorderColor;

        [ObservableProperty]
        private Color payoneerBorderColor;

        [ObservableProperty]
        private string paypalEmail;

        [ObservableProperty]
        private string visaCardNumber;

        [ObservableProperty]
        private string visaCVV;

        [ObservableProperty]
        private DateTime expiryDate = DateTime.Now;

        [ObservableProperty]
        private string visaExpiryDate;

        [ObservableProperty]
        private string payoneerEmail;

        [ObservableProperty]
        private bool isLoading;
        string UserId;
        public INavigation Navigation { get; }

        #endregion

        #region Constructor
        public AddPaymentMethodViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            UserId = Preferences.Get("LocalId", string.Empty);
        }

        #endregion

        #region Commands
        [RelayCommand]
        private async Task AddPaypal()
        {
            if (currentBottomSheet != null)
            {
                Random random = new Random();
                int id = random.Next(1000, 9999);
                var paymentMethod = new PaymentModel
                {
                    Id = id,
                    PaymentEmail = PaypalEmail,
                    CardNumber = string.Empty,
                    CVV = string.Empty,
                    ExpiryDate = string.Empty,
                    PaymentMethodName = "Paypal",

                };
                IsLoading = true;
                await firebaseManager.AddPaymentMethodAsync(UserId, paymentMethod);
                IsLoading = false;
                await currentBottomSheet.CloseBottomSheet();
                PaypalBorderColor = Colors.Transparent;
                PaypalEmail = string.Empty;
            }
        }

        [RelayCommand]
        private async Task AddVisaCard()
        {
            VisaExpiryDate = ExpiryDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);
            if (currentBottomSheet != null)
            {
                Random random = new Random();
                int id = random.Next(1000, 9999);
                var paymentMethod = new PaymentModel
                {
                    Id = id,
                    PaymentEmail = string.Empty,
                    CardNumber = VisaCardNumber,
                    CVV = VisaCVV,
                    ExpiryDate = VisaExpiryDate,
                    PaymentMethodName = "Visa",
                };
                IsLoading = true;
                await firebaseManager.AddPaymentMethodAsync(UserId, paymentMethod);
                IsLoading = false;
                await currentBottomSheet.CloseBottomSheet();
                VisaBorderColor = Colors.Transparent;
                VisaCardNumber = string.Empty;
                VisaCVV = string.Empty;
                VisaExpiryDate = string.Empty;
            }
        }

        [RelayCommand]
        private async Task AddPayoneer()
        {
            if (currentBottomSheet != null)
            {
                Random random = new Random();
                int id = random.Next(1000, 9999);
                var paymentMethod = new PaymentModel
                {
                    Id = id,
                    PaymentEmail = PayoneerEmail,
                    CardNumber = string.Empty,
                    CVV = string.Empty,
                    ExpiryDate = string.Empty,
                    PaymentMethodName = "Payoneer"
                };
                IsLoading = true;
                await firebaseManager.AddPaymentMethodAsync(UserId, paymentMethod);
                IsLoading = false;
                await currentBottomSheet.CloseBottomSheet();
                PayoneerBorderColor = Colors.Transparent;
                PayoneerEmail = string.Empty;
            }
        }
        #endregion

        #region Methods
        public void SetCurrentBottomSheet(FoodNinja.Views.BottomSheet bottomSheet)
        {
            currentBottomSheet = bottomSheet;
        }
        #endregion
    }
}
