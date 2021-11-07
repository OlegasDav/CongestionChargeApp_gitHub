using System;

namespace Contracts.Models
{
    public class CalculatedCharge
    {
        public decimal ChargeForAm { get; set; }

        public decimal ChargeForPm { get; set; }

        public decimal TotalCharge { get; set; }

        public TimeSpan AmHours { get; set; }

        public TimeSpan PmHours { get; set; }
    }
}
