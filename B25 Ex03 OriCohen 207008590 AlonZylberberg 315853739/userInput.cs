using System;
using System.Collections.Generic;
using System.IO;
using Ex03.GarageLogic;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class UserInput
    {
        public string GetOwnerName()
        {
            Console.Write("Enter owner name: ");
            string ownerName = Console.ReadLine();
            
            return ownerName;
        }

        public string GetOwnerPhone()
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

        public string GetVehicleType()
        {
            string selectedType;
            while (true)
            {
                Console.Write("Please enter the type of vehicle: ");
                selectedType = Console.ReadLine();

                if (VehicleFactory.SupportedTypes.Contains(selectedType))
                {
                    break;
                }
                Console.WriteLine("Invalid vehicle type. Try again.");
            }

            return selectedType;
        }

        public string GetVehicleModelName()
        {
            Console.Write("Please enter the vehicle model name: ");
            string modelName = Console.ReadLine();
            
            return modelName;
        }

        public bool GetIfUserWatchOptions()
        {
            Console.WriteLine("Would  you like to choose what status vehicles do you want to see? (Y/N)");
            string watchOptionsOrNot = Console.ReadLine();

            return watchOptionsOrNot.ToUpper() == "Y";
        }
    }
}