using System;

namespace Contracts.Models
{
    public class TimeRange
    {
        public static TimeSpan point7Am { get; set; } = TimeSpan.FromHours(7);

        public static TimeSpan point12Pm { get; set; } = TimeSpan.FromHours(12);

        public static TimeSpan point7Pm { get; set; } = TimeSpan.FromHours(19);
    }
}
