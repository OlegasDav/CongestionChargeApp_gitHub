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
            var jsonVehicles = File.ReadAllText(@"Data\vehicles.json");
            var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(jsonVehicles);

            PrintReceipts(vehicles);

            List<Vehicle> _data = new List<Vehicle>();

            _data.Add(new Vehicle()
            {
                Name = "Test car",
                EntryDate = DateTime.Now,
                ExitDate = DateTime.Now.AddDays(1),
                Type = VehicleType.Motorbike
            });

            string json = JsonConvert.SerializeObject(_data);

            //File.WriteAllText(@"..\..\..\Data\vehicles1.json", json);
        }

        static void PrintReceipts(List<Vehicle> vehicles)
        {
            int i = 0;

            foreach (var vehicle in vehicles)
            {
                var calculatedCharge = CalculateCharge(vehicle);

                Console.WriteLine($"Receipt {++i}");
                Console.WriteLine(vehicle);
                Console.WriteLine($"Charge for {calculatedCharge.AmHours.Hours}h {calculatedCharge.AmHours.Minutes}m (AM rate): {calculatedCharge.ChargeForAm.ToString("C", new CultureInfo("en-GB"))}");
                Console.WriteLine($"Charge for {calculatedCharge.PmHours.Hours}h {calculatedCharge.PmHours.Minutes}m (PM rate): {calculatedCharge.ChargeForPm.ToString("C", new CultureInfo("en-GB"))}");
                Console.WriteLine($"Total Charge: {calculatedCharge.TotalCharge.ToString("C", new CultureInfo("en-GB"))}");
                Console.WriteLine("------------------------------");
            }
        }

        static CalculatedCharge CalculateCharge(Vehicle vehicle)
        {
            var charge = GetCharge(vehicle);

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

            return new CalculatedCharge()
            {
                ChargeForAm = chargeForAm,
                ChargeForPm = chargeForPm,
                TotalCharge = totalCharge,
                AmHours = amHours,
                PmHours = pmHours
            };
        }

        static Charge GetCharge(Vehicle vehicle)
        {
            if (vehicle.Type == VehicleType.Motorbike)
            {
                return new Charge()
                {
                    AmRate = 1.0M,
                    PmRate = 1.0M
                };
            }

            return new Charge()
            {
                AmRate = 2.0M,
                PmRate = 2.5M
            };
        }
    }
}
