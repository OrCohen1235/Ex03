using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class GarageUI
    {
        private Dictionary<string, VehicleRecords> m_Vehicles;
        private LoadDataFromDb m_LoadDataFromDb;
        private UserInput m_UserInput;

        public GarageUI()
        {
            m_Vehicles = new Dictionary<string, VehicleRecords>();
            m_LoadDataFromDb = new LoadDataFromDb();
            m_UserInput = new UserInput();
        }

        public void LoadVehiclesFromDatabase(string filePath)
        {
            try
            {
                m_Vehicles = m_LoadDataFromDb.LoadVehiclesFromDatabase(filePath);
                Console.WriteLine("Vehicles loaded successfully from the database");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void AddOrUpdateVehicle(string i_LicenseNumber)
        {
            if (m_Vehicles.ContainsKey(i_LicenseNumber))
            {
                VehicleRecords record = m_Vehicles[i_LicenseNumber];
                Console.WriteLine("The vehicle is already in the garage. Status set to 'In Repair'.");
                record.Status = VehicleRecords.eVehicleStatus.InRepair;
            }
            else
            {
                Vehicle newVehicle = addVehicle(i_LicenseNumber);

                string ownerName = m_UserInput.GetOwnerName();
                string ownerPhone = m_UserInput.GetOwnerPhone();

                VehicleRecords newRecord = new VehicleRecords
                {
                    m_Vehicle = newVehicle,
                    m_NameOfOwner = ownerName,
                    m_PhoneNumber = ownerPhone,
                    Status = VehicleRecords.eVehicleStatus.InRepair
                };

                m_Vehicles.Add(i_LicenseNumber, newRecord);
                Console.WriteLine("Vehicle successfully added to the garage.");
            }
        }

        public void InflateTiresToMax(string i_LicenseNumber)
        {
            if (!m_Vehicles.ContainsKey(i_LicenseNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_LicenseNumber}");
            }
            else
            {
                m_Vehicles[i_LicenseNumber].m_Vehicle.FillTiresToMax();
            }
        }

        private Vehicle addVehicle(string i_LicenseNumber)
        {
            Console.WriteLine("Supported vehicle types:");
            foreach (string type in VehicleFactory.SupportedTypes)
            {
                Console.WriteLine($"- {type}");
            }

            string selectedType = m_UserInput.GetVehicleType();
            string modelName = m_UserInput.GetVehicleModelName();

            Vehicle newVehicle = VehicleFactory.CreateVehicle(selectedType, i_LicenseNumber, modelName);
            fillVehicleDetails(newVehicle);

            return newVehicle;
        }


        private void fillVehicleDetails(Vehicle i_Vehicle)
        {
            string[] getInfoFuncNames = i_Vehicle.GetAllSetInfoStrings();
            object[] getInfoFunctions = i_Vehicle.GetAllSetInfoFunctions();

            for(int i = 0; i < getInfoFunctions.Length; i++)
            {
                while(true)
                {
                    Console.Write(getInfoFuncNames[i]);
                    string inputFromUser = Console.ReadLine();
                    try
                    {
                        ((Action<string>)getInfoFunctions[i]).Invoke(inputFromUser);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }


            object[] extraFunc = i_Vehicle.GetExtraDetailsFunctions();
            string[] extraFuncNames = i_Vehicle.GetExtraDetailsText();
            
            for (int i = 0; i < extraFunc.Length; i++)
            {
                while (true)
                {
                    Console.Write(extraFuncNames[i]);
                    string returnValue = Console.ReadLine();
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

            }
        }

        public void DisplayAllVehicles()
        {
            string userInput;
            bool watchOptionsOrNot = m_UserInput.GetIfUserWatchOptions();

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
                foreach (KeyValuePair<string, VehicleRecords> vehicle in m_Vehicles)
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
                foreach (KeyValuePair<string, VehicleRecords> vehicle in m_Vehicles)
                {
                    Console.WriteLine($"- {vehicle.Key}");
                }
            }
        }

        public void ChangeVehicleStatus(string i_ModelNumber, string i_NewStatus)
        {
            if (!m_Vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                
                return;
            }

            bool isParsed = Enum.TryParse(i_NewStatus, out VehicleRecords.eVehicleStatus selectedStatus);

            if (!isParsed)
            {
                Console.WriteLine($"Invalid status: {i_NewStatus}. Valid statuses are:");
                foreach (string status in Enum.GetNames(typeof(VehicleRecords.eVehicleStatus)))
                {
                    Console.WriteLine($"- {status}");
                }

                return;
            }

            m_Vehicles[i_ModelNumber].Status = selectedStatus;
            Console.WriteLine($"Vehicle status updated to: {selectedStatus}");
        }


        public void ChargeElectricVehicle(string i_ModelNumber, float i_AmountToCharge)
        {
            if (!m_Vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                
                return;
            }

            ElectricVehicle carToCharge = m_Vehicles[i_ModelNumber].m_Vehicle as ElectricVehicle;
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
            if (!m_Vehicles.ContainsKey(i_ModelNumber))
            {
                Console.WriteLine($"No vehicle found with model number: {i_ModelNumber}");
                
                return;
            }

            FuelVehicle vehicleToRefuel = m_Vehicles[i_ModelNumber].m_Vehicle as FuelVehicle;
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
            if (!m_Vehicles.ContainsKey(i_licenseNumber))
            {
                Console.WriteLine($"No vehicle found with license number: {i_licenseNumber}");
                
                return;
            }

            VehicleRecords record = m_Vehicles[i_licenseNumber];
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
