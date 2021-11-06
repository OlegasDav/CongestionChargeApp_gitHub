using Contracts.Enums;
using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CongestionChargeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var vehicles = new List<Vehicle>()
            {
                new Vehicle()
                {
                    Name = "Car",
                    EntryDate = new DateTime(2008, 4, 24, 11, 32, 0),
                    ExitDate = new DateTime(2008, 4, 24, 14, 42, 0),
                    Type = VehicleType.Car
                },
                new Vehicle()
                {
                    Name = "Motorbike",
                    EntryDate = new DateTime(2008, 4, 24, 17, 0, 0),
                    ExitDate = new DateTime(2008, 4, 24, 22, 11, 0),
                    Type = VehicleType.Motorbike
                },
                new Vehicle()
                {
                    Name = "Van",
                    EntryDate = new DateTime(2008, 4, 25, 10, 23, 0),
                    ExitDate = new DateTime(2008, 4, 28, 9, 2, 0),
                    Type = VehicleType.Car
                }
            };

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

            if (vehicle.EntryDate > vehicle.ExitDate)
            {
                amHours = TimeSpan.Zero;
                pmHours = TimeSpan.Zero;
            }
            else
            {
                for (DateTime date = vehicle.EntryDate.Date; date <= vehicle.ExitDate.Date; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    if (vehicle.EntryDate.Date == vehicle.ExitDate.Date)
                    {
                        if (vehicle.EntryDate.TimeOfDay < TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point12Pm)
                        {
                            amHours = vehicle.ExitDate.TimeOfDay - TimeRange.point7Am;
                        }
                        else if (vehicle.EntryDate.TimeOfDay < TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay > TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point7Pm)
                        {
                            amHours = TimeRange.point12Pm - TimeRange.point7Am;
                            pmHours = vehicle.ExitDate.TimeOfDay - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay < TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay > TimeRange.point7Pm)
                        {
                            amHours = TimeRange.point12Pm - TimeRange.point7Am;
                            pmHours = TimeRange.point7Pm - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point12Pm)
                        {
                            amHours = vehicle.ExitDate.TimeOfDay - vehicle.EntryDate.TimeOfDay;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.EntryDate.TimeOfDay < TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point7Pm)
                        {
                            amHours = TimeRange.point12Pm - vehicle.EntryDate.TimeOfDay;
                            pmHours = vehicle.ExitDate.TimeOfDay - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.EntryDate.TimeOfDay < TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay > TimeRange.point7Pm)
                        {
                            amHours = TimeRange.point12Pm - vehicle.EntryDate.TimeOfDay;
                            pmHours = TimeRange.point7Pm - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point7Pm)
                        {
                            pmHours = vehicle.ExitDate.TimeOfDay - vehicle.EntryDate.TimeOfDay;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay > TimeRange.point7Pm)
                        {
                            pmHours = TimeRange.point7Pm - vehicle.EntryDate.TimeOfDay;
                        }
                    }
                    else if (vehicle.EntryDate.Date == date)
                    {
                        if (vehicle.EntryDate.TimeOfDay < TimeRange.point7Am)
                        {
                            amHours += TimeRange.point12Pm - TimeRange.point7Am;
                            pmHours += TimeRange.point7Pm - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.EntryDate.TimeOfDay < TimeRange.point12Pm)
                        {
                            amHours += TimeRange.point12Pm - vehicle.EntryDate.TimeOfDay;
                            pmHours += TimeRange.point7Pm - TimeRange.point12Pm;
                        }
                        else if (vehicle.EntryDate.TimeOfDay > TimeRange.point12Pm
                            && vehicle.EntryDate.TimeOfDay < TimeRange.point7Pm)
                        {
                            pmHours += TimeRange.point7Pm - vehicle.EntryDate.TimeOfDay;
                        }
                    }
                    else if (vehicle.ExitDate.Date == date)
                    {
                        if (vehicle.ExitDate.TimeOfDay > TimeRange.point7Am
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point12Pm)
                        {
                            amHours += vehicle.ExitDate.TimeOfDay - TimeRange.point7Am;
                        }
                        else if (vehicle.ExitDate.TimeOfDay > TimeRange.point12Pm
                            && vehicle.ExitDate.TimeOfDay < TimeRange.point7Pm)
                        {
                            amHours += TimeRange.point12Pm - TimeRange.point7Am;
                            pmHours += vehicle.ExitDate.TimeOfDay - TimeRange.point12Pm;
                        }
                        else if (vehicle.ExitDate.TimeOfDay > TimeRange.point7Pm)
                        {
                            amHours += TimeRange.point12Pm - TimeRange.point7Am;
                            pmHours += TimeRange.point7Pm - TimeRange.point12Pm;
                        }
                    }
                    else
                    {
                        amHours += TimeRange.point12Pm - TimeRange.point7Am;
                        pmHours += TimeRange.point7Pm - TimeRange.point12Pm;
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
