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
                    string currAirPressure = parts[5];
                    string ownerName = parts[6];
                    string ownerPhone = parts[7];

                    Vehicle vehicle = VehicleCreator.CreateVehicle(vehicleType, licenseNumber, modelName);
                    vehicle.EnergyPercentage = energyPercentage;
                    vehicle.SetTiresManuFacturer(tireModel);
                    vehicle.SetTiresPressure(currAirPressure);

                    object[] extraFunc = vehicle.getExtraDetailsFunctions();

                    for (int i = 0; i < extraFunc.Length; i++)
                    {
                            string returnValue = parts[i+8];
                            try
                            {
                                ((Action<string>)extraFunc[i]).Invoke(returnValue);
                                break;
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

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