
using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class GarageUI
    {
        private Dictionary<string, VehicleRecords> vehicles;


        public GarageUI()
        {
            vehicles = new Dictionary<string, VehicleRecords>();
        }

        public void LoadVehiclesFromDatabase(string filePath)
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
                                electricCar.SetColor = elecColor;
                                electricCar.SetNumberOfDoors = elecDoors;
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

        private string getTireModel()
        {
            Console.Write("Enter tire model: ");
            string tireModel = Console.ReadLine();
            return tireModel;
        }

        private float getTireAirPressure()
        {

            Console.Write("Enter current air pressure: ");
            if(!float.TryParse(Console.ReadLine(), out float airPressure))
            {
                throw new FormatException("Invalid air pressure format.");
            }

            return airPressure;
        }

        private float getCurrentEnergyPercentage()
        {
            Console.Write("Enter current energy percentage (0-100): ");
            if (!float.TryParse(Console.ReadLine(), out float energyPercentage) || energyPercentage < 0 || energyPercentage > 100)
            {
                throw new ValueRangeException("Energy percentage must be between 0 and 100.");
            }

            return energyPercentage;
        }

        private eLicenseType getLicenseType()
        {
            Console.Write("Enter license type (A, A2, AB, B2): ");
            if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eLicenseType licenseType))
            {
                throw new ArgumentException("Invalid license type.");
            }
            return licenseType;
        }

        private int getEngineCapacity()
        {
            Console.Write("Enter engine capacity (int): ");
            if (!int.TryParse(Console.ReadLine(), out int engineCapacity))
            {
                throw new FormatException("Invalid engine capacity format.");
            }

            return engineCapacity;
        }

        private eCarColor getCarColor()
        {
            string[] colorOptions = Enum.GetNames(typeof(eCarColor));
            Console.Write($"Enter car color ({string.Join(", ", colorOptions)}): ");

            string input = Console.ReadLine();

            if (!Enum.TryParse(input, ignoreCase: true, out eCarColor color) ||
                !Enum.IsDefined(typeof(eCarColor), color))
            {
                throw new ArgumentException("Invalid car color.");
            }

            return color;
        }

        private int getNumberOfDoors()
        {
            Console.Write("Enter number of doors (2-5): ");
            if (!int.TryParse(Console.ReadLine(), out int numDoors) || numDoors < 2 || numDoors > 5)
            {
                throw new ValueRangeException("Number of doors must be between 2 and 5.");
            }

            return numDoors;
        }

        private bool getIsHazardous()
        {
            Console.Write("Does the truck carry hazardous materials? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool hasHazard))
            {
                throw new FormatException("Invalid boolean value.");
            }
            
            return hasHazard;
        }

        private float getCargoVolume()
        {
            Console.Write("Enter cargo volume (float): ");
            if (!float.TryParse(Console.ReadLine(), out float cargoVolume))
            {
                throw new FormatException("Invalid cargo volume format.");
            }

            return cargoVolume;
        }
        private void fillVehicleDetails(Vehicle i_Vehicle)
        {
            string tireModel = getTireModel();
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
                    airPressure = getTireAirPressure();
                    i_Vehicle.SetTiresInfo(airPressure,tireModel);
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
                    energyPercentage = getCurrentEnergyPercentage();
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
                        licenseType = getLicenseType();
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
                        engineCapacity = getEngineCapacity();
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
                        licenseType = getLicenseType();
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
                        engineCapacity = getEngineCapacity();
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
                        color = getCarColor();
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
                        numDoors = getNumberOfDoors();
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
                        color = getCarColor();
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
                        numDoors = getNumberOfDoors();
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
                        hasHazard = getIsHazardous();
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
                        cargoVolume = getCargoVolume();
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
            vehicleToRefuel.Refuel(i_FuelType,i_AmountToRefuel);
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



