using System;
using System.Collections.Generic;

namespace Ex03.GarageLogic
{
    // Vehicle energy source types
    public enum eFuelType
    {
        Octan96,
        Octan95,
        Octan98,
        Soler
    }

    // Motorcycle license categories
    public enum eLicenseType
    {
        A,
        A2,
        AB,
        B2
    }

    // Car body colors
    public enum eCarColor
    {
        Yellow,
        Black,
        White,
        Silver
    }

    // Wheel definition
    public class Wheel
    {
        public Wheel(string i_Manufacturer, float i_CurrentPressure, float i_MaxPressure)
        {
            Manufacturer = i_Manufacturer;
            if (i_CurrentPressure <= i_MaxPressure)
                CurrentPressure = i_CurrentPressure;
            else
                throw new ArgumentException("Current pressure cannot exceed max pressure.");

            MaxPressure = i_MaxPressure;
        }

        public string Manufacturer { get; set; }

        public float CurrentPressure { get; set; }

        public float MaxPressure { get; }

        public void Inflate(float i_AirToAdd)
        {
            if (CurrentPressure + i_AirToAdd <= MaxPressure)
                CurrentPressure += i_AirToAdd;
            else
                throw new ArgumentException("Cannot inflate beyond max pressure.");
        }
    }

    // Base vehicle
    public abstract class Vehicle
    {
        private float m_EnergyPercentage;

        protected Vehicle(string i_ModelName, string i_LicenseNumber)
        {
            // only set model & license—leave energy at 0, wheels empty
            ModelName = i_ModelName;
            LicenseNumber = i_LicenseNumber;
            m_EnergyPercentage = 0f;
            Wheels = new List<Wheel>();
        }

        protected Vehicle(string i_ModelName,
            string i_LicenseNumber,
            float i_EnergyPercentage,
            int i_AmountOfWheels,
            string i_WheelManufacturer,
            float i_WheelcurrentPressure,
            float i_WheelmaxPressure)
        {
            ModelName = i_ModelName;
            LicenseNumber = i_LicenseNumber;
            m_EnergyPercentage = i_EnergyPercentage;
            Wheels = new List<Wheel>(i_AmountOfWheels);
            for (var i = 0; i < i_AmountOfWheels; i++)
                Wheels.Add(new Wheel(i_WheelManufacturer, i_WheelcurrentPressure, i_WheelmaxPressure));
        }


        public string ModelName { get; }

        public string LicenseNumber { get; }

        public float EnergyPercentage
        {
            get => m_EnergyPercentage;
            set
            {
                if (value >= 0f && value <= 100f)
                    m_EnergyPercentage = value;
                else
                    throw new ArgumentException("Energy percentage must be between 0 and 100.");
            }
        }

        public List<Wheel> Wheels { get; }

        public void SetTiresInfo(float i_airPressure, string r_Manufacturer)
        {
            foreach (var wheel in Wheels)
            {
                wheel.CurrentPressure = i_airPressure;
                wheel.Manufacturer = r_Manufacturer;
            }
        }

        public void fillTiresToMax()
        {
            foreach (var wheel in Wheels) wheel.CurrentPressure = VARIABLE.MaxPressure;
        }
    }

    // Fuel-based vehicle
    public abstract class FuelVehicle : Vehicle
    {
        protected FuelVehicle(string i_ModelName, string i_LicenseNumber)
            : base(i_ModelName, i_LicenseNumber)
        {
        }

        protected FuelVehicle(
            string i_ModelName,
            string i_LicenseNumber,
            float i_EnergyPercentage,
            string i_WheelManufacturer,
            int i_AmountOfWheels,
            float i_WheelcurrentPressure,
            float i_WheelmaxPressure,
            eFuelType i_FuelType,
            float i_CurrentFuelAmount,
            float i_MaxFuelAmount)
            : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_AmountOfWheels, i_WheelManufacturer,
                i_WheelcurrentPressure, i_WheelmaxPressure)
        {
            FuelType = i_FuelType;
            CurrentFuelAmount = i_CurrentFuelAmount;
            MaxFuelAmount = i_MaxFuelAmount;
        }

        public eFuelType FuelType { get; }

        public float CurrentFuelAmount { get; private set; }

        public float MaxFuelAmount { get; }

        public void Refuel(eFuelType i_FuelType, float i_LitersToAdd)
        {
            if (i_FuelType != FuelType) throw new ArgumentException("Fuel type mismatch.");
            if (CurrentFuelAmount + i_LitersToAdd <= MaxFuelAmount)
            {
                CurrentFuelAmount += i_LitersToAdd;
                EnergyPercentage = CurrentFuelAmount / MaxFuelAmount * 100f;
            }
            else
            {
                throw new ArgumentException("Cannot refuel beyond tank capacity.");
            }
        }

        public void Refuel(eFuelType i_FuelType)
        {
            if (i_FuelType != FuelType) throw new ArgumentException("Fuel type mismatch.");

            CurrentFuelAmount = MaxFuelAmount;
        }
    }

    // Electric vehicle
    public abstract class ElectricVehicle : Vehicle
    {
        protected ElectricVehicle(string i_ModelName, string i_LicenseNumber)
            : base(i_ModelName, i_LicenseNumber)
        {
        }

        protected ElectricVehicle(
            string i_ModelName,
            string i_LicenseNumber,
            float i_EnergyPercentage,
            string i_WheelManufacturer,
            int i_AmountOfWheels,
            float i_WheelcurrentPressure,
            float i_WheelmaxPressure,
            float i_RemainingBatteryHours,
            float i_MaxBatteryHours)
            : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_AmountOfWheels, i_WheelManufacturer,
                i_WheelcurrentPressure, i_WheelmaxPressure)

        {
            RemainingBatteryHours = i_RemainingBatteryHours;
            MaxBatteryHours = i_MaxBatteryHours;
        }

        public float RemainingBatteryHours { get; private set; }

        public float MaxBatteryHours { get; }

        public void ChargeBattery(float i_HoursToAdd)
        {
            if (RemainingBatteryHours + i_HoursToAdd <= MaxBatteryHours)
            {
                RemainingBatteryHours += i_HoursToAdd;
                EnergyPercentage = RemainingBatteryHours / MaxBatteryHours * 100f;
            }
            else
            {
                throw new ArgumentException("Cannot charge beyond battery capacity.");
            }
        }

        public void ChargeBattery()
        {
            RemainingBatteryHours = MaxBatteryHours;
        }
    }
}

// Fuel motorcycle
public class FuelMotorcycle : FuelVehicle
{
    public FuelMotorcycle(string i_LicenseNumber, string i_ModelName)
        : base(i_ModelName, i_LicenseNumber)
    {
    }

    public FuelMotorcycle(
        string i_ModelName,
        string i_LicenseNumber,
        float i_EnergyPercentage,
        string i_WheelManufacturer,
        float i_WheelcurrentPressure,
        float i_CurrentFuelAmount,
        eLicenseType i_LicenseType,
        int i_EngineCapacity)
        : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer, 2,
            i_WheelcurrentPressure, 30, eFuelType.Octan98, i_CurrentFuelAmount, 5.8f)
    {
        LicenseType = i_LicenseType;
        EngineCapacity = i_EngineCapacity;
    }

    public eLicenseType LicenseType { get; set; }

    public int EngineCapacity { get; set; }
}

// Electric motorcycle
public class ElectricMotorcycle : ElectricVehicle
{
    public ElectricMotorcycle(string i_LicenseNumber, string i_ModelName)
        : base(i_ModelName, i_LicenseNumber)
    {
    }

    public ElectricMotorcycle(
        string i_ModelName,
        string i_LicenseNumber,
        float i_EnergyPercentage,
        string i_WheelManufacturer,
        float i_WheelcurrentPressure,
        float i_RemainingBatteryHours,
        eLicenseType i_LicenseType,
        int i_EngineCapacity)
        : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer, 2,
            i_WheelcurrentPressure, 30, i_RemainingBatteryHours, 3.2f)
    {
        LicenseType = i_LicenseType;
        EngineCapacity = i_EngineCapacity;
    }

    public eLicenseType LicenseType { get; set; }

    public int EngineCapacity { get; set; }
}

// Fuel car
public class FuelCar : FuelVehicle
{
    public eCarColor r_Color;
    public eLicenseType r_LicenseType;
    public int r_NumberOfDoors;

    public FuelCar(string i_LicenseNumber, string i_ModelName)
        : base(i_ModelName, i_LicenseNumber)
    {
    }

    public FuelCar(
        string i_ModelName,
        string i_LicenseNumber,
        float i_EnergyPercentage,
        string i_WheelManufacturer,
        float i_WheelcurrentPressure,
        float i_CurrentFuelAmount,
        eCarColor i_Color,
        int i_NumberOfDoors)
        : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer, 5,
            i_WheelcurrentPressure, 32, eFuelType.Octan95, i_CurrentFuelAmount, 48f)
    {
        r_Color = i_Color;
        if (i_NumberOfDoors < 2 || i_NumberOfDoors > 4)
            throw new ArgumentException("Number of doors must be between 2 and 4.");
        r_NumberOfDoors = i_NumberOfDoors;
    }

    public eCarColor SetColor
    {
        get => r_Color;
        set => r_Color = value;
    }

    public int SetNumberOfDoors
    {
        get => r_NumberOfDoors;
        set => r_NumberOfDoors = value;
    }
}

public class ElectricCar : ElectricVehicle
{
    public ElectricCar(string i_LicenseNumber, string i_ModelName)
        : base(i_ModelName, i_LicenseNumber)
    {
    }

    public ElectricCar(
        string i_ModelName,
        string i_LicenseNumber,
        float i_EnergyPercentage,
        string i_WheelManufacturer,
        float i_WheelcurrentPressure,
        float i_RemainingBatteryHours,
        eCarColor i_Color,
        int i_NumberOfDoors)
        : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer, 5,
            i_WheelcurrentPressure, 32, i_RemainingBatteryHours, 4.8f)
    {
        Color = i_Color;
        if (i_NumberOfDoors < 2 || i_NumberOfDoors > 4)
            throw new ArgumentException("Number of doors must be between 2 and 4.");
        NumberOfDoors = i_NumberOfDoors;
    }

    public eCarColor Color { get; set; }

    public int NumberOfDoors { get; set; }
}


// Fuel truck
public class Truck : FuelVehicle
{
    public Truck(string i_LicenseNumber, string i_ModelName)
        : base(i_ModelName, i_LicenseNumber)
    {
    }

    public Truck(
        string i_ModelName,
        string i_LicenseNumber,
        float i_EnergyPercentage,
        string i_WheelManufacturer,
        float i_WheelcurrentPressure,
        float i_CurrentFuelAmount,
        bool i_CarriesHazardousMaterials,
        float i_CargoVolume)
        : base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer, 12,
            i_WheelcurrentPressure, 27, eFuelType.Soler, i_CurrentFuelAmount, 135f)
    {
        CarriesHazardousMaterials = i_CarriesHazardousMaterials;
        CargoVolume = i_CargoVolume;
    }

    public bool CarriesHazardousMaterials { get; set; }
    public float CargoVolume { get; set; }
}


}
