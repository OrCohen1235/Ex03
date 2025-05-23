using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class userInput
    {
        public string getTireModel()
        {
            Console.Write("Enter tire model: ");
            string tireModel = Console.ReadLine();
            return tireModel;
        }

        public float getTireAirPressure()
        {
            Console.Write("Enter current air pressure: ");
            if (!float.TryParse(Console.ReadLine(), out float airPressure))
            {
                throw new FormatException("Invalid air pressure format.");
            }

            return airPressure;
        }

        public float getCurrentEnergyPercentage()
        {
            Console.Write("Enter current energy percentage (0-100): ");
            if (!float.TryParse(Console.ReadLine(), out float energyPercentage) || energyPercentage < 0 ||
                energyPercentage > 100)
            {
                throw new ValueRangeException("Energy percentage must be between 0 and 100.");
            }

            return energyPercentage;
        }

        public eLicenseType getLicenseType()
        {
            Console.Write("Enter license type (A, A2, AB, B2): ");
            if (!Enum.TryParse(Console.ReadLine(), ignoreCase: true, out eLicenseType licenseType))
            {
                throw new ArgumentException("Invalid license type.");
            }

            return licenseType;
        }

        public int getEngineCapacity()
        {
            Console.Write("Enter engine capacity (int): ");
            if (!int.TryParse(Console.ReadLine(), out int engineCapacity))
            {
                throw new FormatException("Invalid engine capacity format.");
            }

            return engineCapacity;
        }

        public eCarColor getCarColor()
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

        public int getNumberOfDoors()
        {
            Console.Write("Enter number of doors (2-5): ");
            if (!int.TryParse(Console.ReadLine(), out int numDoors) || numDoors < 2 || numDoors > 5)
            {
                throw new ValueRangeException("Number of doors must be between 2 and 5.");
            }

            return numDoors;
        }

        public bool getIsHazardous()
        {
            Console.Write("Does the truck carry hazardous materials? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool hasHazard))
            {
                throw new FormatException("Invalid boolean value.");
            }

            return hasHazard;
        }

        public float getCargoVolume()
        {
            Console.Write("Enter cargo volume (float): ");
            if (!float.TryParse(Console.ReadLine(), out float cargoVolume))
            {
                throw new FormatException("Invalid cargo volume format.");
            }

            return cargoVolume;
        }

        public string getOwnerName()
        {
            Console.Write("Enter owner name: ");
            string ownerName = Console.ReadLine();
            return ownerName;
        }

        public string getOwnerPhone()
        {
            Console.Write("Please enter your phone number: ");
            string ownerPhone = Console.ReadLine();
            while (!long.TryParse(ownerPhone, out _))
            {
                Console.Write("Invalid phone number. Please enter a valid phone number: ");
                ownerPhone = Console.ReadLine();
            }

            return ownerPhone;
        }

        public string getvehicleType()
        {
            string selectedType;
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

            return selectedType;
        }

        public string getVeicleModelName()
        {
            Console.Write("Please enter the vehicle model name: ");
            string modelName = Console.ReadLine();
            return modelName;
        }

        public bool getIfUserWatchOptions()
        {
            Console.WriteLine("Would  you like to choose what status vehicles do you want to see? (Y/N)");
            string watchOptionsOrNot = Console.ReadLine();

            return watchOptionsOrNot.ToUpper() == "Y";
        }
    }
}