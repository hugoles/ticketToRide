using System.ComponentModel;

namespace TicketToRideAPI.Domain
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            DescriptionAttribute[] attributes = value
                .GetType()
                .GetField(value.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[]
                ?? [];

            return attributes.Length != 0 ? attributes[0].Description : value.ToString();
        }
    }
}