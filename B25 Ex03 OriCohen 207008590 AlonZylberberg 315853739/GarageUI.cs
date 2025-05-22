
using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class GarageUI
    {
        private Dictionary<string, VehicleRecords> vehicles;

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

        public GarageUI()
        {
            vehicles = new Dictionary<string, VehicleRecords>();
        }

        public void LoadVehiclesFromDatabase(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.");
            }

            foreach (string line in File.ReadLines(filePath))
            {
                if (line.Trim() == "*****")
                {
                    break;
                }

                try
                {
                    string[] parts = line.Split(',');
                    string vehicleType = parts[0].Trim();

                    Vehicle vehicle = null;
                    string licenseNumber = parts[1].Trim();
                    string modelName = parts[2].Trim();

                    if (!float.TryParse(parts[3].Trim(), out float energyPercentage))
                    {
                        throw new FormatException("Invalid energy percentage format.");
                    }

                    string tireModel = parts[4].Trim();

                    if (!float.TryParse(parts[5].Trim(), out float currAirPressure))
                    {
                        throw new FormatException("Invalid air pressure format.");
                    }

                    string ownerName = parts[6].Trim();
                    string ownerPhone = parts[7].Trim();

                    switch (vehicleType)
                    {
                        case "FuelMotorcycle":
                            if (!Enum.TryParse(parts[8].Trim(), true, out eLicenseType licenseTypeFM) ||
                                !int.TryParse(parts[9].Trim(), out int engineCapacityFM))
                            {
                                throw new FormatException("Invalid FuelMotorcycle data.");
                            }

                            vehicle = new FuelMotorcycle(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 5.8f, licenseTypeFM, engineCapacityFM);
                            break;

                        case "ElectricMotorcycle":
                            if (!Enum.TryParse(parts[8].Trim(), true, out eLicenseType licenseTypeEM) ||
                                !int.TryParse(parts[9].Trim(), out int engineCapacityEM))
                            {
                                throw new FormatException("Invalid ElectricMotorcycle data.");
                            }

                            vehicle = new ElectricMotorcycle(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 3.2f, licenseTypeEM, engineCapacityEM);
                            break;

                        case "FuelCar":
                            if (!Enum.TryParse(parts[8].Trim(), true, out eCarColor colorFC) ||
                                !int.TryParse(parts[9].Trim(), out int numDoorsFC))
                            {
                                throw new FormatException("Invalid FuelCar data.");
                            }

                            vehicle = new FuelCar(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 48f, colorFC, numDoorsFC);
                            break;

                        case "ElectricCar":
                            if (!Enum.TryParse(parts[8].Trim(), true, out eCarColor colorEC) ||
                                !int.TryParse(parts[9].Trim(), out int numDoorsEC))
                            {
                                throw new FormatException("Invalid ElectricCar data.");
                            }

                            vehicle = new ElectricCar(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 4.8f, colorEC, numDoorsEC);
                            break;

                        case "Truck":
                            if (!bool.TryParse(parts[8].Trim(), out bool hasHazard) ||
                                !float.TryParse(parts[9].Trim(), out float cargoVolume))
                            {
                                throw new FormatException("Invalid Truck data.");
                            }

                            vehicle = new Truck(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 135f, hasHazard, cargoVolume);
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
                    Console.WriteLine($"Error parsing line: {line}");
                    Console.WriteLine($"Details: {ex.Message}");
                }
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

                Console.Write("Please enter your name: ");
                string ownerName = Console.ReadLine();

                Console.Write("Please enter your phone number: ");
                string ownerPhone = Console.ReadLine();

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
                return;
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

            string selectedType;
            VehicleRecords recordNewVehicle;
            while (true)
            {
                Console.Write("Please enter the type of vehicle: ");
                selectedType = Console.ReadLine();

                if (VehicleCreator.SupportedTypes.Contains(selectedType))
                {
                    break;
                }
                Console.WriteLine("Invalid vehicle type. Try again.");
            }

            Console.Write("Please enter the vehicle model name: ");
            string modelName = Console.ReadLine();

            Vehicle newVehicle = VehicleCreator.CreateVehicle(selectedType, i_LicenseNumber, modelName);
            fillVehicleDetails(newVehicle);


            return newVehicle;
        }


        private void fillVehicleDetails(Vehicle i_Vehicle)
        {
            Console.Write("Enter tire model: ");
            string tireModel = Console.ReadLine();

            Console.Write("Enter current air pressure: ");
            if(!float.TryParse(Console.ReadLine(), out float airPressure))
            {
                throw new FormatException("Invalid air pressure format.");
            }
            
            i_Vehicle.SetTiresInfo(airPressure,tireModel);

            Console.Write("Enter current energy percentage (0-100): ");
            if (!float.TryParse(Console.ReadLine(), out float energyPercentage) || energyPercentage < 0 || energyPercentage > 100)
            {
                throw new ValueRangeException("Energy percentage must be between 0 and 100.");
            }
            i_Vehicle.EnergyPercentage = energyPercentage;

            if (i_Vehicle is FuelMotorcycle fuelMotorcycle)
            {
                Console.Write("Enter license type (A, A2, AB, B2): ");
                if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eLicenseType licenseType))
                {
                    throw new ArgumentException("Invalid license type.");
                }

                Console.Write("Enter engine capacity (int): ");
                if (!int.TryParse(Console.ReadLine(), out int engineCapacity))
                {
                    throw new FormatException("Invalid engine capacity format.");
                }

                fuelMotorcycle.LicenseType = licenseType;
                fuelMotorcycle.EngineCapacity = engineCapacity;
            }
            else if (i_Vehicle is ElectricMotorcycle elMotorcycle)
            {
                Console.Write("Enter license type (A, A2, AB, B2): ");
                if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eLicenseType licenseType))
                {
                    throw new ArgumentException("Invalid license type.");
                }

                Console.Write("Enter engine capacity (int): ");
                if (!int.TryParse(Console.ReadLine(), out int engineCapacity))
                {
                    throw new FormatException("Invalid engine capacity format.");
                }

                elMotorcycle.LicenseType = licenseType;
                elMotorcycle.EngineCapacity = engineCapacity;
            }
            else if (i_Vehicle is FuelCar fuelCar)
            {
                Console.Write("Enter car color (Yellow, Black, White, Silver): ");
                if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eCarColor color))
                {
                    throw new ArgumentException("Invalid car color.");
                }

                Console.Write("Enter number of doors (2-5): ");
                if (!int.TryParse(Console.ReadLine(), out int numDoors) || numDoors < 2 || numDoors > 5)
                {
                    throw new ValueRangeException("Number of doors must be between 2 and 5.");
                }

                fuelCar.SetColor = color;
                fuelCar.SetNumberOfDoors = numDoors;
            }
            else if (i_Vehicle is ElectricCar elCar)
            {
                Console.Write("Enter car color (Yellow, Black, White, Silver): ");
                if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eCarColor color))
                {
                    throw new ArgumentException("Invalid car color.");
                }

                Console.Write("Enter number of doors (2-5): ");
                if (!int.TryParse(Console.ReadLine(), out int numDoors) || numDoors < 2 || numDoors > 5)
                {
                    throw new ValueRangeException("Number of doors must be between 2 and 5.");
                }

                elCar.Color = color;
                elCar.NumberOfDoors = numDoors;
            }
            else if (i_Vehicle is Truck truck)
            {
                Console.Write("Does the truck carry hazardous materials? (true/false): ");
                if (!bool.TryParse(Console.ReadLine(), out bool hasHazard))
                {
                    throw new FormatException("Invalid boolean value.");
                }

                Console.Write("Enter cargo volume (float): ");
                if (!float.TryParse(Console.ReadLine(), out float cargoVolume))
                {
                    throw new FormatException("Invalid cargo volume format.");
                }

                truck.CarriesHazardousMaterials = hasHazard;
                truck.CargoVolume = cargoVolume;
            }
        }

        public void DisplayAllVehicles()
        {
            string userInput;
            string watchOptionsOrNot;
            Console.WriteLine("Would  you like to choose what status vehicles do you want to see? (Y/N)");
            watchOptionsOrNot = Console.ReadLine();
            if(watchOptionsOrNot == "Y")
            {
                Console.WriteLine("Type which vehicles would you like to see by their status.");
                Console.WriteLine("The options are:");

                
                foreach(string status in Enum.GetNames(typeof(VehicleRecords.eVehicleStatus)))
                {
                    Console.WriteLine($"- {status}");
                }

                Console.Write("Enter your choice: ");
                userInput = Console.ReadLine();

                bool isParsed = Enum.TryParse(userInput, out VehicleRecords.eVehicleStatus selectedStatus);

                if(!isParsed)
                {
                    Console.WriteLine("Invalid status entered. Please try again.");
                    return;
                }

                Console.WriteLine($"Vehicles with status '{selectedStatus}':");

                bool found = false;
                foreach(var vehicle in vehicles)
                {
                    if(vehicle.Value.Status == selectedStatus)
                    {
                        Console.WriteLine($"- {vehicle.Key}");
                        found = true;
                    }
                }

                if(!found)
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


        public void chargeElectricCar(string i_ModelNumber, float i_AmountToCharge)
        {
            if (!vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                return;
            }

            ElectricCar carToCharge = vehicles[i_ModelNumber].m_Vehicle as ElectricCar;
            if (carToCharge == null)
            {
                Console.WriteLine("The vehicle is not an electric car.");
                return;
            }

            try
            {
                carToCharge.ChargeBattery(i_AmountToCharge);
                Console.WriteLine($"Electric car charged by {i_AmountToCharge} hours.");
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
        
        public void RefuelFuelCar(string i_ModelNumber, float i_AmountToRefuel, eFuelType i_FuelType)
        {
            if (!vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                return;
            }

            FuelCar carToRefuel = vehicles[i_ModelNumber].m_Vehicle as FuelCar;
            if (carToRefuel == null)
            {
                Console.WriteLine("The vehicle is not a fuel car.");
                return;
            }

            carToRefuel.Refuel(i_FuelType,i_AmountToRefuel);
            Console.WriteLine($"Fuel car refueled by {i_AmountToRefuel} liters.");
        }
        
        public void showVehiclesDetails(string i_licenseNumber)
        {
            if (!vehicles.ContainsKey(i_licenseNumber))
            {
                Console.WriteLine($"No vehicle found with license number: {i_licenseNumber}");
                return;
            }

            VehicleRecords record = vehicles[i_licenseNumber];
            Console.WriteLine($"Vehicle Details for {i_licenseNumber}:");
            Console.WriteLine($"Owner Name: {record.m_NameOfOwner}");
            Console.WriteLine($"Phone Number: {record.m_PhoneNumber}");
            Console.WriteLine($"Status: {record.Status}");
            Console.WriteLine($"Vehicle Type: {record.m_Vehicle.GetType().Name}");
            Console.WriteLine($"Model Name: {record.m_Vehicle.ModelName}");
        }
        
    }
}



