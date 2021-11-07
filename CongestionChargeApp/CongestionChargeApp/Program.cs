using Contracts.Enums;
using Contracts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CongestionChargeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonVehicles = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\vehicles.json"));
            var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(jsonVehicles);

            PrintReceipts(vehicles);
        }

        static void PrintReceipts(List<Vehicle> vehicles)
        {
            int i = 0;

            foreach (var vehicle in vehicles)
            {
                Console.WriteLine($"Receipt {++i}");
                CalculateCharge(vehicle);
                Console.WriteLine("------------------------------");
            }
        }

        static void CalculateCharge(Vehicle vehicle)
        {
            var charge = new Charge
            {
                AmRate = 2.0M,
                PmRate = 2.5M
            };

            if (vehicle.Type == VehicleType.Motorbike)
            {
                charge.AmRate = 1.0M;
                charge.PmRate = 1.0M;
            }

            TimeSpan amHours = TimeSpan.Zero;
            TimeSpan pmHours = TimeSpan.Zero;

            for (DateTime date = vehicle.EntryDate.Date; date <= vehicle.ExitDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                TimeSpan startChargeTime = TimeRange.Point7Am;
                TimeSpan endChargeTime = TimeRange.Point7Pm;

                if (vehicle.EntryDate.Date == date && vehicle.EntryDate.TimeOfDay > TimeRange.Point7Am)
                {
                    startChargeTime = vehicle.EntryDate.TimeOfDay;
                }

                if (vehicle.ExitDate.Date == date && vehicle.ExitDate.TimeOfDay < TimeRange.Point7Pm)
                {
                    endChargeTime = vehicle.ExitDate.TimeOfDay;
                }

                if (endChargeTime > startChargeTime)
                {
                    if (startChargeTime < TimeRange.Point12Pm && endChargeTime < TimeRange.Point12Pm)
                    {
                        amHours += endChargeTime - startChargeTime;
                    }
                    else if (startChargeTime < TimeRange.Point12Pm && endChargeTime > TimeRange.Point12Pm)
                    {
                        amHours += TimeRange.Point12Pm - startChargeTime;
                        pmHours += endChargeTime - TimeRange.Point12Pm;
                    }
                    else if (startChargeTime > TimeRange.Point12Pm && endChargeTime > TimeRange.Point12Pm)
                    {
                        pmHours += endChargeTime - startChargeTime;
                    }
                }
            }

            var chargeForAm = Math.Floor((decimal)amHours.TotalHours * charge.AmRate * 10) / 10;
            var chargeForPm = Math.Floor((decimal)pmHours.TotalHours * charge.PmRate * 10) / 10;
            var totalCharge = chargeForAm + chargeForPm;

            Console.WriteLine(vehicle);
            Console.WriteLine($"Charge for {amHours.Hours}h {amHours.Minutes}m (AM rate): {chargeForAm.ToString("C", new CultureInfo("en-GB"))}");
            Console.WriteLine($"Charge for {pmHours.Hours}h {pmHours.Minutes}m (PM rate): {chargeForPm.ToString("C", new CultureInfo("en-GB"))}");
            Console.WriteLine($"Total Charge: {totalCharge.ToString("C", new CultureInfo("en-GB"))}");
        }
    }
}
