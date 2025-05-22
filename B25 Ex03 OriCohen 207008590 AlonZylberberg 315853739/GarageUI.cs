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
            bool fileLoadSuccess = false;
            while (!fileLoadSuccess)
            {
                try
                {
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
                                        fuelCar.SetColor = color;
                                        fuelCar.SetNumberOfDoors = doors;
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
                                    if (bool.TryParse(parts[8], out bool isHazardous)&& 
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
                            Console.WriteLine($"Error parsing line: {line}");
                            Console.WriteLine($"Details: {ex.Message}");
                        }
                    }
                    
                    fileLoadSuccess = true;
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.Write("Enter a valid file path or press Enter to cancel: ");
                    string newPath = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(newPath))
                    {
                        Console.WriteLine("Database loading canceled.");
                        return;
                    }
                    
                    filePath = newPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading database: {ex.Message}");
                    Console.Write("Do you want to retry? (Y/N): ");
                    string retry = Console.ReadLine();
                    
                    if (retry.ToUpper() != "Y")
                    {
                        Console.WriteLine("Database loading canceled.");
                        return;
                    }
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
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
                {
                    Console.Write("Enter tire model: ");
                    string tireModel = Console.ReadLine();

                    float airPressure = 0;
                    bool validAirPressure = false;
                    while (!validAirPressure)
                    {
                        try
                        {
                            Console.Write("Enter current air pressure: ");
                            if(!float.TryParse(Console.ReadLine(), out airPressure))
                            {
                                throw new FormatException("Invalid air pressure format.");
                            }
                            validAirPressure = true;
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message} Please try again.");
                        }
                    }
                    
                    i_Vehicle.SetTiresInfo(airPressure, tireModel);

                    float energyPercentage = 0;
                    bool validEnergyPercentage = false;
                    while (!validEnergyPercentage)
                    {
                        try
                        {
                            Console.Write("Enter current energy percentage (0-100): ");
                            if (!float.TryParse(Console.ReadLine(), out energyPercentage) || energyPercentage < 0 || energyPercentage > 100)
                            {
                                throw new ValueRangeException("Energy percentage must be between 0 and 100.");
                            }
                            validEnergyPercentage = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message} Please try again.");
                        }
                    }
                    
                    i_Vehicle.EnergyPercentage = energyPercentage;

                    if (i_Vehicle is FuelMotorcycle fuelMotorcycle)
                    {
                        fillFuelMotorcycleDetails(fuelMotorcycle);
                    }
                    else if (i_Vehicle is ElectricMotorcycle elMotorcycle)
                    {
                        fillElectricMotorcycleDetails(elMotorcycle);
                    }
                    else if (i_Vehicle is FuelCar fuelCar)
                    {
                        fillFuelCarDetails(fuelCar);
                    }
                    else if (i_Vehicle is ElectricCar elCar)
                    {
                        fillElectricCarDetails(elCar);
                    }
                    else if (i_Vehicle is Truck truck)
                    {
                        fillTruckDetails(truck);
                    }
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.Write("Do you want to retry filling vehicle details? (Y/N): ");
                    string retry = Console.ReadLine();
                    
                    if (retry.ToUpper() != "Y")
                    {
                        throw; // Re-throw if the user doesn't want to retry
                    }
                }
            }
        }
        
        private void fillFuelMotorcycleDetails(FuelMotorcycle fuelMotorcycle)
        {
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
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
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Please try again.");
                }
            }
        }
        
        private void fillElectricMotorcycleDetails(ElectricMotorcycle elMotorcycle)
        {
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
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
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Please try again.");
                }
            }
        }
        
        private void fillFuelCarDetails(FuelCar fuelCar)
        {
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
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
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Please try again.");
                }
            }
        }
        
        private void fillElectricCarDetails(ElectricCar elCar)
        {
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
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
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Please try again.");
                }
            }
        }
        
        private void fillTruckDetails(Truck truck)
        {
            bool detailsFilledSuccessfully = false;
            
            while (!detailsFilledSuccessfully)
            {
                try
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
                    
                    detailsFilledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Please try again.");
                }
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

            bool chargeSuccess = false;
            while (!chargeSuccess)
            {
                try
                {
                    carToCharge.ChargeBattery(i_AmountToCharge);
                    Console.WriteLine($"Electric car charged by {i_AmountToCharge} hours.");
                    chargeSuccess = true;
                }
                catch (ValueRangeException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine(
                        "Current battery charge is: {0} hours, The maximum hours to charge are {1} hours.",
                        carToCharge.RemainingBatteryHours,
                        carToCharge.MaxBatteryHours);
                    
                    float maxAllowedCharge = carToCharge.MaxBatteryHours - carToCharge.RemainingBatteryHours;
                    Console.WriteLine($"You can charge up to {maxAllowedCharge} hours.");
                    
                    Console.Write("Enter amount to charge (or press Enter to cancel): ");
                    string input = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Charging canceled.");
                        return;
                    }
                    
                    if (float.TryParse(input, out i_AmountToCharge))
                    {
                        Console.WriteLine($"Amount set to: {i_AmountToCharge} hours");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Charging canceled.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.Write("Do you want to retry? (Y/N): ");
                    if (Console.ReadLine().ToUpper() != "Y")
                    {
                        Console.WriteLine("Charging canceled.");
                        return;
                    }
                }
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

            bool refuelSuccess = false;
            while (!refuelSuccess)
            {
                try
                {
                    vehicleToRefuel.Refuel(i_FuelType, i_AmountToRefuel);
                    Console.WriteLine($"Fuel Vehicle refueled by {i_AmountToRefuel} liters.");
                    refuelSuccess = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    
                    Console.WriteLine("Available fuel types:");
                    foreach (eFuelType fuelType in Enum.GetValues(typeof(eFuelType)))
                    {
                        Console.WriteLine($"- {fuelType}");
                    }
                    
                    Console.Write("Enter fuel type: ");
                    if (Enum.TryParse(Console.ReadLine(), true, out i_FuelType))
                    {
                        Console.WriteLine($"Fuel type set to: {i_FuelType}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid fuel type. Operation canceled.");
                        return;
                    }
                }
                catch (ValueRangeException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.Write("Enter amount to refuel: ");
                    if (float.TryParse(Console.ReadLine(), out i_AmountToRefuel))
                    {
                        Console.WriteLine($"Amount set to: {i_AmountToRefuel}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Operation canceled.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.Write("Do you want to retry? (Y/N): ");
                    if (Console.ReadLine().ToUpper() != "Y")
                    {
                        Console.WriteLine("Refueling canceled.");
                        return;
                    }
                }
            }
        }
        
        public void showVehiclesDetails(string i_licenseNumber)
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
}

