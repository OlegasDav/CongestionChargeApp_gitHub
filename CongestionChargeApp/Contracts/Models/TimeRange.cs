using System;

namespace Contracts.Models
{
    public class TimeRange
    {
        public static readonly TimeSpan Point7Am = TimeSpan.FromHours(7);

        public static readonly TimeSpan Point12Pm = TimeSpan.FromHours(12);

        public static readonly TimeSpan Point7Pm = TimeSpan.FromHours(19);
    }
}
