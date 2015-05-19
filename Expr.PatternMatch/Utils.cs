using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprSemantic
{
    public class Utils
    {
        public static bool IsInt(object expression, out int number)
        {
            if (expression == null)
            {
                number = 0;
                return false;
            }

            return Int32.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

        public static bool IsDouble(object expression, out double number)
        {
            if (expression == null)
            {
                number = 0.0;
                return false;
            }

            return Double.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

    }
}
