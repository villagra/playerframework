using System;

namespace Microsoft.VideoAdvertising
{
    public static class EnumEx
    {
        public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
        {
            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
                return true;
            }
            catch (ArgumentException)
            {
                result = default(TEnum);
                return false;
            }
        }
    }
}
