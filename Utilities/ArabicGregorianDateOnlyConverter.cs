using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp2.Utilities
{
    public class ArabicGregorianDateOnlyConverter : IValueConverter
    {
        private static readonly CultureInfo ArabicCulture = new CultureInfo("ar-SA")
        {
            DateTimeFormat = { Calendar = new GregorianCalendar() }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                string format = parameter as string ?? "dddd، dd MMMM yyyy";
                return date.ToString(format, ArabicCulture);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
