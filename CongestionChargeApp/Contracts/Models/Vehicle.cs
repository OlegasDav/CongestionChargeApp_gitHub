using Contracts.Enums;
using System;

namespace Contracts.Models
{
    public class Vehicle
    {
        public string Name { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime ExitDate { get; set; }

        public VehicleType Type { get; set; }

        public override string ToString()
        {
            return $"{Name}: {EntryDate:dd\\/MM\\/yyyy HH:mm} - {ExitDate:dd\\/MM\\/yyyy HH:mm}";
        }
    }
}
