using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class GarageUI
    {
        private Dictionary<string, VehicleRecords> vehicles;
        private loadDataFromDb loadDataFromDb;
        private userInput getUserInput;

        public GarageUI()
        {
            vehicles = new Dictionary<string, VehicleRecords>();
            loadDataFromDb = new loadDataFromDb();
            getUserInput = new userInput();
        }

        public void LoadVehiclesFromDatabase(string filePath)
        {
            try
            {
                vehicles = loadDataFromDb.LoadVehiclesFromDatabase(filePath);
                Console.WriteLine("Vehicles loaded successfully from the database");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void AddOrUpdateVehicle(string i_LicenseNumber)
        {
            if (vehicles.ContainsKey(i_LicenseNumber))
            {
                VehicleRecords record = vehicles[i_LicenseNumber];
                Console.WriteLine("The vehicle is already in the garage. Status set to 'In Repair'.");
                record.Status = VehicleRecords.eVehicleStatus.InRepair;
            }
            else
            {
                Vehicle newVehicle = AddVehicle(i_LicenseNumber);

                string ownerName = getUserInput.getOwnerName();
                string ownerPhone = getUserInput.getOwnerPhone();

                VehicleRecords newRecord = new VehicleRecords
                {
                    m_Vehicle = newVehicle,
                    m_NameOfOwner = ownerName,
                    m_PhoneNumber = ownerPhone,
                    Status = VehicleRecords.eVehicleStatus.InRepair
                };

                vehicles.Add(i_LicenseNumber, newRecord);
                Console.WriteLine("Vehicle successfully added to the garage.");
            }
        }

        public void InflateTiresToMax(string i_LicenseNumber)
        {
            if (!vehicles.ContainsKey(i_LicenseNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_LicenseNumber}");
            }
            else
            {
                vehicles[i_LicenseNumber].m_Vehicle.fillTiresToMax();
            }
        }

        private Vehicle AddVehicle(string i_LicenseNumber)
        {
            Console.WriteLine("Supported vehicle types:");
            foreach (string type in VehicleCreator.SupportedTypes)
            {
                Console.WriteLine($"- {type}");
            }

            string selectedType = getUserInput.getvehicleType();
            string modelName = getUserInput.getVeicleModelName();

            Vehicle newVehicle = VehicleCreator.CreateVehicle(selectedType, i_LicenseNumber, modelName);
            fillVehicleDetails(newVehicle);

            return newVehicle;
        }


        private void fillVehicleDetails(Vehicle i_Vehicle)
        {
            string tireModel = getUserInput.getTireModel();
            float airPressure;
            float energyPercentage;
            float cargoVolume;
            eLicenseType licenseType;
            eCarColor color;
            int engineCapacity;
            int numDoors;
            bool hasHazard;

            while (true)
            {
                try
                {
                    airPressure = getUserInput.getTireAirPressure();
                    i_Vehicle.SetTiresInfo(airPressure, tireModel);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            while (true)
            {
                try
                {
                    energyPercentage = getUserInput.getCurrentEnergyPercentage();
                    i_Vehicle.EnergyPercentage = energyPercentage;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (i_Vehicle is FuelMotorcycle fuelMotorcycle)
            {
                while (true)
                {
                    try
                    {
                        licenseType = getUserInput.getLicenseType();
                        fuelMotorcycle.LicenseType = licenseType;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (true)
                {
                    try
                    {
                        engineCapacity = getUserInput.getEngineCapacity();
                        fuelMotorcycle.EngineCapacity = engineCapacity;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (i_Vehicle is ElectricMotorcycle elMotorcycle)
            {
                while (true)
                {
                    try
                    {
                        licenseType = getUserInput.getLicenseType();
                        elMotorcycle.LicenseType = licenseType;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (true)
                {
                    try
                    {
                        engineCapacity = getUserInput.getEngineCapacity();
                        elMotorcycle.EngineCapacity = engineCapacity;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (i_Vehicle is FuelCar fuelCar)
            {
                while (true)
                {
                    try
                    {
                        color = getUserInput.getCarColor();
                        fuelCar.SetColor = color;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }


                while (true)
                {
                    try
                    {
                        numDoors = getUserInput.getNumberOfDoors();
                        fuelCar.SetNumberOfDoors = numDoors;
                        break;
                    }
                    catch (ValueRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (i_Vehicle is ElectricCar elCar)
            {
                while (true)
                {
                    try
                    {
                        color = getUserInput.getCarColor();
                        elCar.SetColor = color;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }


                while (true)
                {
                    try
                    {
                        numDoors = getUserInput.getNumberOfDoors();
                        elCar.SetNumberOfDoors = numDoors;
                        break;
                    }
                    catch (ValueRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (i_Vehicle is Truck truck)
            {
                while (true)
                {
                    try
                    {
                        hasHazard = getUserInput.getIsHazardous();
                        truck.CarriesHazardousMaterials = hasHazard;
                        break;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (true)
                {
                    try
                    {
                        cargoVolume = getUserInput.getCargoVolume();
                        truck.CargoVolume = cargoVolume;
                        break;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public void DisplayAllVehicles()
        {
            string userInput;
            bool watchOptionsOrNot = getUserInput.getIfUserWatchOptions();

            if (watchOptionsOrNot == true)
            {
                Console.WriteLine("Type which vehicles would you like to see by their status.");
                Console.WriteLine("The options are:");


                foreach (string status in Enum.GetNames(typeof(VehicleRecords.eVehicleStatus)))
                {
                    Console.WriteLine($"- {status}");
                }

                Console.Write("Enter your choice: ");
                userInput = Console.ReadLine();

                bool isParsed = Enum.TryParse(userInput, out VehicleRecords.eVehicleStatus selectedStatus);

                if (!isParsed)
                {
                    Console.WriteLine("Invalid status entered. Please try again.");
                    return;
                }

                Console.WriteLine($"Vehicles with status '{selectedStatus}':");

                bool found = false;
                foreach (var vehicle in vehicles)
                {
                    if (vehicle.Value.Status == selectedStatus)
                    {
                        Console.WriteLine($"- {vehicle.Key}");
                        found = true;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("No vehicles found with the selected status.");
                }
            }
            else
            {
                Console.WriteLine("Those are all the vehicles in the garage at this moment.");
                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"- {vehicle.Key}");
                }
            }
        }

        public void ChangeVehicleStatus(string i_ModelNumber, string i_newStatus)
        {
            if (!vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                return;
            }

            bool isParsed = Enum.TryParse(i_newStatus, out VehicleRecords.eVehicleStatus selectedStatus);

            if (!isParsed)
            {
                Console.WriteLine($"Invalid status: {i_newStatus}. Valid statuses are:");
                foreach (string status in Enum.GetNames(typeof(VehicleRecords.eVehicleStatus)))
                {
                    Console.WriteLine($"- {status}");
                }

                return;
            }

            vehicles[i_ModelNumber].Status = selectedStatus;
            Console.WriteLine($"Vehicle status updated to: {selectedStatus}");
        }


        public void ChargeElectricVehicle(string i_ModelNumber, float i_AmountToCharge)
        {
            if (!vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                return;
            }

            ElectricVehicle carToCharge = vehicles[i_ModelNumber].m_Vehicle as ElectricVehicle;
            if (carToCharge == null)
            {
                Console.WriteLine("The vehicle is not an electric vehicle.");
                return;
            }

            try
            {
                carToCharge.ChargeBattery(i_AmountToCharge);
                Console.WriteLine($"Electric vehicle charged by {i_AmountToCharge} hours.");
            }
            catch
            {
                Console.WriteLine(
                    "Current battery charge is :{0}, The maximum hours to charge are {1} hours.",
                    carToCharge.RemainingBatteryHours,
                    carToCharge.MaxBatteryHours);
                Console.WriteLine("The car has not been charged.");
            }
        }

        public void RefuelFuelVehicle(string i_ModelNumber, float i_AmountToRefuel, eFuelType i_FuelType)
        {
            if (!vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                return;
            }

            FuelVehicle vehicleToRefuel = vehicles[i_ModelNumber].m_Vehicle as FuelVehicle;
            if (vehicleToRefuel == null)
            {
                Console.WriteLine("The vehicle is not a fuel Vehicle.");
                return;
            }

            vehicleToRefuel.Refuel(i_FuelType, i_AmountToRefuel);
            Console.WriteLine($"Fuel Vehicle refueled by {i_AmountToRefuel} liters.");
        }

        public void ShowVehiclesDetails(string i_licenseNumber)
        {
            if (!vehicles.ContainsKey(i_licenseNumber))
            {
                Console.WriteLine($"No vehicle found with license number: {i_licenseNumber}");
                return;
            }

            VehicleRecords record = vehicles[i_licenseNumber];
            Console.WriteLine(record.m_Vehicle.PrintVehicleDetails());
            Console.WriteLine($"Owner Name: {record.m_NameOfOwner}");
            Console.WriteLine($"Phone Number: {record.m_PhoneNumber}");
            Console.WriteLine($"Status: {record.Status}");
        }
    }

    public class VehicleRecords
    {
        public Vehicle m_Vehicle { get; set; }
        public string m_NameOfOwner { get; set; }
        public string m_PhoneNumber { get; set; }

        public enum eVehicleStatus
        {
            InRepair,
            Repaired,
            Paid
        }

        public eVehicleStatus Status { get; set; }
    }
}