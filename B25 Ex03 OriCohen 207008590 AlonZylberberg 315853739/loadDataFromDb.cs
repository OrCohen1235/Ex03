using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class LoadDataFromDb
    {
        public Dictionary<string, VehicleRecords> LoadVehiclesFromDatabase(string filePath)
        {
            Dictionary<string, VehicleRecords> vehicles = new Dictionary<string, VehicleRecords>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The database file was not found.", filePath);
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line == "*****")
                {
                    break;
                }

                try
                {
                    string[] parts = line.Split(',');

                    string vehicleType = parts[0];
                    string licenseNumber = parts[1];
                    string modelName = parts[2];

                    if (!float.TryParse(parts[3], out float energyPercentage))
                    {
                        Console.WriteLine($"Invalid energy percentage: {parts[3]}");
                        continue;
                    }

                    string tireModel = parts[4];

                    if (!float.TryParse(parts[5], out float currAirPressure))
                    {
                        Console.WriteLine($"Invalid air pressure: {parts[5]}");
                        continue;
                    }

                    string ownerName = parts[6];
                    string ownerPhone = parts[7];

                    Vehicle vehicle = VehicleCreator.CreateVehicle(vehicleType, licenseNumber, modelName);
                    vehicle.EnergyPercentage = energyPercentage;
                    vehicle.SetTiresInfo(currAirPressure, tireModel);

                    switch (vehicle)
                    {
                        case FuelCar fuelCar:
                            if (Enum.TryParse(parts[8], out eCarColor color) &&
                                int.TryParse(parts[9], out int doors))
                            {
                                fuelCar.Color = color;
                                fuelCar.NumberOfDoors = doors;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid data for FuelCar: {line}");
                                continue;
                            }

                            break;

                        case ElectricCar electricCar:
                            if (Enum.TryParse(parts[8], out eCarColor elecColor) &&
                                int.TryParse(parts[9], out int elecDoors))
                            {
                                electricCar.Color = elecColor;
                                electricCar.NumberOfDoors = elecDoors;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid data for ElectricCar: {line}");
                                continue;
                            }

                            break;

                        case FuelMotorcycle fuelMoto:
                            if (Enum.TryParse(parts[8], out eLicenseType fuelLicType) &&
                                int.TryParse(parts[9], out int engineCap))
                            {
                                fuelMoto.LicenseType = fuelLicType;
                                fuelMoto.EngineCapacity = engineCap;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid data for FuelMotorcycle: {line}");
                                continue;
                            }

                            break;

                        case ElectricMotorcycle elecMoto:
                            if (Enum.TryParse(parts[8], out eLicenseType elecLicType) &&
                                int.TryParse(parts[9], out int elecEngineCap))
                            {
                                elecMoto.LicenseType = elecLicType;
                                elecMoto.EngineCapacity = elecEngineCap;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid data for ElectricMotorcycle: {line}");
                                continue;
                            }

                            break;

                        case Truck truck:
                            if (bool.TryParse(parts[8], out bool isHazardous) &&
                                float.TryParse(parts[9], out float cargoVolume))
                            {
                                truck.CargoVolume = cargoVolume;
                                truck.CarriesHazardousMaterials = isHazardous;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid data for Truck: {line}");
                                continue;
                            }

                            break;

                        default:
                            Console.WriteLine($"Unknown vehicle type: {vehicleType}");
                            continue;
                    }

                    vehicles[licenseNumber] = new VehicleRecords
                    {
                        m_Vehicle = vehicle,
                        m_NameOfOwner = ownerName,
                        m_PhoneNumber = ownerPhone,
                        Status = VehicleRecords.eVehicleStatus.InRepair
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Details: {ex.Message}");
                }
            }

            return vehicles;
        }
    }
}