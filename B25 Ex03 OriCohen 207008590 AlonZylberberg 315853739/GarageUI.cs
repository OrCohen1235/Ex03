using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public void LoadVehiclesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.");
            }

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Trim() == "*****")
                {
                    break;
                }
                try
                {
                    var parts = line.Split(',');

                    // הסוג תמיד ב- parts[0]
                    string vehicleType = parts[0].Trim();

                    Vehicle vehicle = null;
                    string licenseNumber = parts[1].Trim();
                    string modelName = parts[2].Trim();
                    float energyPercentage = float.Parse(parts[3].Trim());
                    string tireModel = parts[4].Trim();
                    float currAirPressure = float.Parse(parts[5].Trim());
                    string ownerName = parts[6].Trim();
                    string ownerPhone = parts[7].Trim();

                    switch (vehicleType)
                    {
                        case "FuelMotorcycle":
                            

                            if (!Enum.TryParse<Ex03.GarageLogic.eLicenseType>(parts[9].Trim(), ignoreCase: true, out var licenseType))
                            {
                                // todo : understand why the continue 
                                throw new ArgumentException($"Invalid license type for FuelMotorcycle: {parts[8]}");
                                Console.WriteLine($"Invalid license type for FuelMotorcycle: {parts[8]}");
                                continue; // דילוג על השורה כי הערך לא תקין
                            }
                            int engineCapacity = int.Parse(parts[9].Trim());

                            vehicle = new FuelMotorcycle(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure,
                                (energyPercentage / 100f) * 5.8f, licenseType, engineCapacity);
                            break;

                        case "ElectricMotorcycle":
                            // 0=type,1=licenseNumber,2=modelName,3=energy%,4=tireModel,5=currAirPressure,6=maxAirPressure,
                            // 7=ownerName,8=ownerPhone,9=licenseType,10=engineCapacity

                            var elLicenseType = (Ex03.GarageLogic.eLicenseType)Enum.Parse(typeof(Ex03.GarageLogic.eLicenseType), parts[8].Trim());
                            engineCapacity = int.Parse(parts[9].Trim());

                            vehicle = new ElectricMotorcycle(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure, (energyPercentage / 100f) * 3.2f, elLicenseType, engineCapacity);
                            break;

                        case "FuelCar":
                            // 0=type,1=licenseNumber,2=modelName,3=energy%,4=tireModel,5=currAirPressure,6=maxAirPressure,
                            // 7=ownerName,8=ownerPhone,9=carColor,10=numOfDoors

                            var carColor = (Ex03.GarageLogic.eCarColor)Enum.Parse(typeof(Ex03.GarageLogic.eCarColor), parts[8].Trim());
                            int numOfDoors = int.Parse(parts[9].Trim());

                            vehicle = new FuelCar(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure, (energyPercentage / 100f) * 48f, carColor, numOfDoors);
                            break;

                        case "ElectricCar":
                            // 0=type,1=licenseNumber,2=modelName,3=energy%,4=tireModel,5=currAirPressure,6=maxAirPressure,
                            // 7=ownerName,8=ownerPhone,9=carColor,10=numOfDoors

                            var elCarColor = (Ex03.GarageLogic.eCarColor)Enum.Parse(typeof(Ex03.GarageLogic.eCarColor), parts[8].Trim());
                            numOfDoors = int.Parse(parts[9].Trim());

                            vehicle = new ElectricCar(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure, (energyPercentage / 100f) * 4.8f, elCarColor, numOfDoors);
                            break;

                        case "Truck":
                            // 0=type,1=licenseNumber,2=modelName,3=energy%,4=tireModel,5=currAirPressure,6=maxAirPressure,
                            // 7=ownerName,8=ownerPhone,9=hazardousMaterial,10=cargoVolume

                            bool hazardousMaterial = bool.Parse(parts[8].Trim());
                            float cargoVolume = float.Parse(parts[9].Trim());

                            vehicle = new Truck(modelName, licenseNumber, energyPercentage, tireModel, currAirPressure, (energyPercentage / 100f) * 135f, hazardousMaterial, cargoVolume);
                            break;

                        default:
                            Console.WriteLine($"Unknown vehicle type: {vehicleType}");
                            continue; // דילוג על השורה
                    }

                    VehicleRecords record = new VehicleRecords()
                    {
                        m_Vehicle = vehicle,
                        m_NameOfOwner = ownerName,
                        m_PhoneNumber = ownerPhone,
                        Status = VehicleRecords.eVehicleStatus.InRepair
                    };

                    vehicles[licenseNumber] = record;
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



        public Vehicle AddVehicle(string i_LicenseNumber)
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
            float airPressure = float.Parse(Console.ReadLine());

            i_Vehicle.SetTiresInfo(airPressure, tireModel);

            Console.Write("Enter current energy percentage (0-100): ");
            float energyPercentage = float.Parse(Console.ReadLine());
            i_Vehicle.EnergyPercentage= energyPercentage; 

            if (i_Vehicle is FuelMotorcycle fuelMotorcycle)
            {
                Console.Write("Enter license type (A, A2, AB, B2): ");
                var licenseType = (eLicenseType)Enum.Parse(typeof(eLicenseType), Console.ReadLine(), ignoreCase: true);

                Console.Write("Enter engine capacity (int): ");
                int engineCapacity = int.Parse(Console.ReadLine());

                fuelMotorcycle.LicenseType = licenseType;
                fuelMotorcycle.EngineCapacity = engineCapacity;
            }
            else if (i_Vehicle is ElectricMotorcycle elMotorcycle)
            {
                Console.Write("Enter license type (A, A2, AB, B2): ");
                var licenseType = (eLicenseType)Enum.Parse(typeof(eLicenseType), Console.ReadLine(), ignoreCase: true);

                Console.Write("Enter engine capacity (int): ");
                int engineCapacity = int.Parse(Console.ReadLine());

                elMotorcycle.LicenseType = licenseType;
                elMotorcycle.EngineCapacity = engineCapacity;
            }
            else if (i_Vehicle is FuelCar fuelCar)
            {
                Console.Write("Enter car color (Yellow,Black,White,Silver): ");
                var color = (eCarColor)Enum.Parse(typeof(eCarColor), Console.ReadLine(), ignoreCase: true);

                Console.Write("Enter number of doors (2-5): ");
                int numDoors = int.Parse(Console.ReadLine());

                fuelCar.SetColor = color;
                fuelCar.SetNumberOfDoors= numDoors;
            }
            else if (i_Vehicle is ElectricCar elCar)
            {
                Console.Write("Enter car color (Yellow,Black,White,Silver): ");
                var color = (eCarColor)Enum.Parse(typeof(eCarColor), Console.ReadLine(), ignoreCase: true);

                Console.Write("Enter number of doors (2-5): ");
                int numDoors = int.Parse(Console.ReadLine());

                elCar.Color = color;
                elCar.NumberOfDoors = numDoors;
            }
            else if (i_Vehicle is Truck truck)
            {
                Console.Write("Does the truck carry hazardous materials? (true/false): ");
                bool hasHazard = bool.Parse(Console.ReadLine());

                Console.Write("Enter cargo volume (float): ");
                float cargoVolume = float.Parse(Console.ReadLine());
               
                truck.CarriesHazardousMaterials = hasHazard;
                truck.CargoVolume = cargoVolume;
            }
        }

        public void GetAllNumberLicenseInGarage()
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
                throw new ArgumentException("Invalid status entered. Please try again.");
                // todo : understand what is the reset of consols 
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


        public void showVeichleDetailsByLicense(string i_LicenseNumber)
        {
            if (vehicles.ContainsKey(i_LicenseNumber))
            {
                var vehicleRecord = vehicles[i_LicenseNumber];
                Console.WriteLine($"License Number: {vehicleRecord.m_Vehicle.LicenseNumber}");
                Console.WriteLine($"Model Name: {vehicleRecord.m_Vehicle.ModelName}");
                Console.WriteLine($"Owner Name: {vehicleRecord.m_NameOfOwner}");
                Console.WriteLine($"Phone Number: {vehicleRecord.m_PhoneNumber}");
                Console.WriteLine($"Status: {vehicleRecord.Status}");
            }
            else
            {
                throw new ArgumentException($"No vehicle found with license number: {i_LicenseNumber}");
            }
        }

        public static void Main()
        {
            GarageUI garage = new GarageUI();
            string fileName =
                "C:\\Users\\orcoh\\source\\repos\\B25 Ex03 OriCohen 207008590 AlonZylberberg 315853739\\B25 Ex03 OriCohen 207008590 AlonZylberberg 315853739\\Vehicles.db";
            garage.LoadVehiclesFromFile(fileName);  

            Console.Write("Should load data from db?(y/n): ");
            string loadFromDb = Console.ReadLine();
            if (loadFromDb == "y")
            {
                garage.LoadVehiclesFromFile(fileName);
            }

            Console.Write("Enter license plate to add or update: ");
            string licensePlate = Console.ReadLine();

            garage.AddOrUpdateVehicle(licensePlate);

            garage.GetAllNumberLicenseInGarage();

        }
    }
}



