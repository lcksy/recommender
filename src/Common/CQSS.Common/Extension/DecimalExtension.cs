using CQSS.Common.Extension;

namespace CQSS.Common.Extension
{
    public static class DecimalExtension
    {
        public static decimal RoundFormat(this decimal value, int decimals = 2)
        {
            var format = "f{0}".FormatWith(decimals);
            var tmpValue = value.ToString(format);
            decimal formatValue = default(decimal);
            decimal.TryParse(tmpValue, out formatValue);

            return formatValue;
        }
    }
}