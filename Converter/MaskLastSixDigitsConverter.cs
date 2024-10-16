using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Converter
{
    public class MaskLastSixDigitsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (parameter is string paymentMethod && paymentMethod == "Visa")
                {
                    return FormatVisaCardNumber(text);
                }
                else if (text.Length > 6)
                {
                    return text.Substring(0, text.Length - 6) + new string('*', 6);
                }
            }
            return value;
        }

        private object? FormatVisaCardNumber(string cardNumber)
        {
            string maskedCardNumber = cardNumber.Substring(0, cardNumber.Length - 6) + new string('*', 6);
            for (int i = 4; i < maskedCardNumber.Length; i += 5)
            {
                maskedCardNumber = maskedCardNumber.Insert(i, " ");
            }
            return maskedCardNumber;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
