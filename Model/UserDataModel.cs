using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Model
{
    public class UserDataModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public Dictionary<int, PaymentModel> PaymentMethod { get; set; } = new Dictionary<int, PaymentModel>();
      //  public ObservableCollection<PaymentModel> PaymentMethod { get; set; } = new ObservableCollection<PaymentModel>();
        public string Image { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public partial class PaymentModel : ObservableObject
    {
        public int Id { get; set; }
        public string PaymentEmail { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }
        public string PaymentMethodName { get; set; }

        public static PaymentModel currentlySelectedPayment;

        private bool isSelectedPayment;

        public bool IsSelectedPayment
        {
            get => isSelectedPayment;
            //  set => SetProperty(ref isSelectedPayment, value);

            set
            {
                if (SetProperty(ref isSelectedPayment, value) && value)
                {
                    if (currentlySelectedPayment != null && currentlySelectedPayment != this)
                    {
                        currentlySelectedPayment.IsSelectedPayment = false;
                    }
                    currentlySelectedPayment = this;
                }
            }
        }

        public static void DeselectCurrentPayment()
        {
            if (currentlySelectedPayment != null)
            {
                currentlySelectedPayment.IsSelectedPayment = false;
                currentlySelectedPayment = null;
            }
        }
    }
}
