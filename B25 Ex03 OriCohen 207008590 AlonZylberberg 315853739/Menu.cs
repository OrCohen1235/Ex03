using System;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class Menu
    {
        private readonly GarageUI r_GarageManager;
        private readonly string r_DatabaseName = "Vehicles.db";

        public Menu()
        {
            r_GarageManager = new GarageUI();
        }

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                printMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        r_GarageManager.LoadVehiclesFromDatabase(r_DatabaseName);
                        break;
                    case "2":
                        insertNewVehicle();
                        break;
                    case "3":
                        r_GarageManager.DisplayAllVehicles();
                        break;
                    case "4":
                        changeVehicleStatus();
                        break;
                    case "5":
                        inflateVehicleTires();
                        break;
                    case "6":
                        refuelFuelVehicle();
                        break;
                    case "7":
                        chargeElectricVehicle();
                        break;
                    case "8":
                        displayVehicleDetails();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Exiting the system. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to return to the main menu...");
                    Console.ReadKey();
                }
            }
        }

        private void printMenu()
        {
            Console.WriteLine("Garage Management System - Main Menu");
            Console.WriteLine("1. Load vehicle data from 'Vehicles.db'");
            Console.WriteLine("2. Insert a new vehicle into the garage");
            Console.WriteLine("3. Display all vehicles by license number");
            Console.WriteLine("4. Change vehicle status");
            Console.WriteLine("5. Inflate vehicle tires to maximum");
            Console.WriteLine("6. Refuel a fuel-based vehicle");
            Console.WriteLine("7. Charge an electric vehicle");
            Console.WriteLine("8. Display full vehicle details by license number");
            Console.WriteLine("0. Exit");
            Console.Write("Your choice: ");
        }

        private void insertNewVehicle()
        {
            string license = getLicenseFromUser();
            r_GarageManager.AddOrUpdateVehicle(license);
        }

        private void changeVehicleStatus()
        {
            string license = getLicenseFromUser();
            Console.Write("Enter new status: ");
            string status = Console.ReadLine();
            r_GarageManager.ChangeVehicleStatus(license, status);
        }

        private void inflateVehicleTires()
        {
            string license = getLicenseFromUser();
            r_GarageManager.InflateTiresToMax(license);
        }

        private void refuelFuelVehicle()
        {
            string license = getLicenseFromUser();

            while (true)
            {
                Console.Write("Enter fuel type (e.g., Soler, Octan95,Octan96,Octan98): ");
                string fuelInput = Console.ReadLine();

                if(!Enum.TryParse(fuelInput, true, out eFuelType fuelType))
                {
                    Console.WriteLine("Invalid fuel type.");
                    return;
                }

                float amount = getFloatFromUser("Enter amount to refuel: ");
                try
                {
                    r_GarageManager.RefuelFuelVehicle(license, amount, fuelType);
                    break;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void chargeElectricVehicle()
        {
            string license = getLicenseFromUser();
            float hours = getFloatFromUser("Enter amount of hours to charge: ");
            r_GarageManager.ChargeElectricVehicle(license, hours);
        }

        private void displayVehicleDetails()
        {
            string license = getLicenseFromUser();
            r_GarageManager.ShowVehiclesDetails(license);
        }

        private string getLicenseFromUser()
        {
            Console.Write("Enter vehicle license number: ");
            return Console.ReadLine();
        }

        private float getFloatFromUser(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (float.TryParse(input, out float result))
                {
                    return result;
                }

                Console.WriteLine("Invalid number, please try again.");
            }
        }
    }
}
