using Monadicsh;
using System;
using System.Globalization;

namespace Haengma.GIB
{
    public class GibPropertyValue
    {
        private readonly string _value;

        public GibPropertyValue(string value)
        {
            _value = value;
        }

        public string? Name
        {
            get
            {
                var values = _value.Split(":");
                if (values.Length > 2)
                {
                    return null;
                }

                return values[0];
            }
        }

        public string Value
        {
            get
            {
                var values = _value.Split(":");
                if (values.Length > 2 || values.Length <= 1)
                {
                    return _value;
                }

                return values[1];
            }
        }

        public int? ValueAsNumber() => int.TryParse(Value, out var number)
            ? number
            : default;

        public DateTime? ValueAsDate(string format) => DateTime.TryParseExact(Value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : new DateTime?();

        public override string ToString() => _value;
    }
}
