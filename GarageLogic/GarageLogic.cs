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
        private string m_Manufacturer;
        private float m_CurrentPressure;
        private readonly float r_MaxPressure;

        public Wheel(string i_Manufacturer, float i_CurrentPressure, float i_MaxPressure)
        {
            m_Manufacturer = i_Manufacturer;
            if (i_CurrentPressure <= i_MaxPressure)
            {
                m_CurrentPressure = i_CurrentPressure;
            }
            else
            {
                throw new ValueRangeException("Current pressure cannot exceed max pressure.");
            }

            r_MaxPressure = i_MaxPressure;
        }

        public string Manufacturer
        {
            get { return m_Manufacturer; }
            set { m_Manufacturer = value; }
        }

        public float CurrentPressure
        {
            get { return m_CurrentPressure; }
            set { m_CurrentPressure = value; }
        }

        public float MaxPressure
        {
            get { return r_MaxPressure; }
        }

        public void Inflate(float i_AirToAdd)
        {
            if (m_CurrentPressure + i_AirToAdd <= r_MaxPressure)
            {
                m_CurrentPressure += i_AirToAdd;
            }
            else
            {
                throw new ArgumentException("Cannot inflate beyond max pressure.");
            }
        }
    }

    // Base vehicle
    public abstract class Vehicle
    {
        private readonly string r_ModelName;
        private readonly string r_LicenseNumber;
        private float m_EnergyPercentage;
        private List<Wheel> m_Wheels;

        protected Vehicle(string i_ModelName, string i_LicenseNumber)
        {
            // only set model & license—leave energy at 0, wheels empty
            r_ModelName = i_ModelName;
            r_LicenseNumber = i_LicenseNumber;
            m_EnergyPercentage = 0f;
            m_Wheels = new List<Wheel>();
        }

        protected Vehicle(string i_ModelName,
            string i_LicenseNumber,
            float i_EnergyPercentage,
            int i_AmountOfWheels,
            string i_WheelManufacturer,
            float i_WheelcurrentPressure,
            float i_WheelmaxPressure)
        {
            r_ModelName = i_ModelName;
            r_LicenseNumber = i_LicenseNumber;
            m_EnergyPercentage = i_EnergyPercentage;
            CreateWheels(i_WheelManufacturer, i_AmountOfWheels, i_WheelcurrentPressure, i_WheelmaxPressure);
        }

        public void CreateWheels(string i_WheelManufacturer, int i_AmountOfWheels, float i_WheelcurrentPressure,
            float i_WheelmaxPressure)
        {
            m_Wheels = new List<Wheel>(i_AmountOfWheels);
            for (int i = 0; i < i_AmountOfWheels; i++)
            {
                m_Wheels.Add(new Wheel(i_WheelManufacturer, i_WheelcurrentPressure, i_WheelmaxPressure));
            }
        }

        public void SetTiresInfo(float i_airPressure, string i_Manufacturer)
        {
            foreach (Wheel wheel in m_Wheels)
            {
                if (i_airPressure >= wheel.MaxPressure)
                {
                    throw new ValueRangeException(0, wheel.MaxPressure, "Current pressure cannot exceed max pressure.");
                }

                wheel.CurrentPressure = i_airPressure;
                wheel.Manufacturer = i_Manufacturer;
            }
        }

        public void FillTiresToMax()
        {
            foreach (Wheel wheel in Wheels)
            {
                wheel.CurrentPressure = wheel.MaxPressure;
            }
        }


        public string ModelName
        {
            get { return r_ModelName; }
        }

        public string LicenseNumber
        {
            get { return r_LicenseNumber; }
        }

        public float EnergyPercentage
        {
            get { return m_EnergyPercentage; }
            set
            {
                if (value >= 0f && value <= 100f)
                {
                    m_EnergyPercentage = value;
                }
                else
                {
                    throw new ValueRangeException(0, 100);
                }
            }
        }

        public List<Wheel> Wheels
        {
            get { return m_Wheels; }
        }

        public abstract string PrintVehicleDetails();
        public abstract object[] getExtraDetailsFunctions();
        public abstract string[] getExtraDetailsText();
    }

    // Fuel-based vehicle
    public abstract class FuelVehicle : Vehicle
    {
        private eFuelType m_FuelType;
        private float m_CurrentFuelAmount;
        private float m_MaxFuelAmount;
        
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
            m_FuelType = i_FuelType;
            m_CurrentFuelAmount = i_CurrentFuelAmount;
            m_MaxFuelAmount = i_MaxFuelAmount;
        }

        public eFuelType FuelType
        {
            get { return m_FuelType; }
            set { m_FuelType = value;}
        }
    
        public float MaxFuelAmount
        {
            get { return m_MaxFuelAmount; }
            set
            {
                if (value <= 0)
                {
                    throw new ValueRangeException(0, float.MaxValue, "Max fuel amount must be positive.");
                }

                m_MaxFuelAmount = value;
            }
        }
        public float CurrentFuelAmount
        {
            get;
        }

        public void Refuel(eFuelType i_FuelType, float i_LitersToAdd)
        {
            if (i_FuelType != m_FuelType)
            {
                throw new ArgumentException("Fuel type mismatch.");
            }

            if (i_LitersToAdd < 0)
            {
                throw new ArgumentException("Cannot refuel a negative amount.");
            }

            if (m_CurrentFuelAmount + i_LitersToAdd > m_MaxFuelAmount)
            {
                float maxAmountCanAdd = m_MaxFuelAmount - m_CurrentFuelAmount;
                throw new ValueRangeException(0, maxAmountCanAdd, "Cannot refuel beyond tank capacity.");
            }

            m_CurrentFuelAmount += i_LitersToAdd;
            EnergyPercentage = (m_CurrentFuelAmount / m_MaxFuelAmount) * 100f;
        }
    }

    // Electric vehicle
    public abstract class ElectricVehicle : Vehicle
    {
        private float m_RemainingBatteryHours;
        private float m_MaxBatteryHours;

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
            m_RemainingBatteryHours = i_RemainingBatteryHours;
            m_MaxBatteryHours = i_MaxBatteryHours;
        }

        public float RemainingBatteryHours
        {
            get { return m_RemainingBatteryHours; }
        }

        public float MaxBatteryHours
        {
            get { return m_MaxBatteryHours; }
            set { m_MaxBatteryHours = value; }
        }

        public void ChargeBattery(float i_HoursToAdd)
        {
            if (m_RemainingBatteryHours + i_HoursToAdd <= m_MaxBatteryHours)
            {
                m_RemainingBatteryHours += i_HoursToAdd;
                EnergyPercentage = (m_RemainingBatteryHours / m_MaxBatteryHours) * 100f;
            }
            else
            {
                throw new ValueRangeException(0, m_MaxBatteryHours, "Cannot charge beyond battery capacity.");
            }
        }
    }

    // Fuel motorcycle
    public class FuelMotorcycle : FuelVehicle
    {
        private eLicenseType m_LicenseType;
        private int m_EngineCapacity;

        public FuelMotorcycle(string i_LicenseNumber, string i_ModelName)
            : base(i_ModelName, i_LicenseNumber)
        {
            this.FuelType = eFuelType.Octan98;
            this.MaxFuelAmount = 5.8f;
            this.CreateWheels("", 2, 0, 30);
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
            m_LicenseType = i_LicenseType;
            m_EngineCapacity = i_EngineCapacity;
        }

        public eLicenseType LicenseType
        {
            get { return m_LicenseType; }
            set { m_LicenseType = value; }
        }

        public int EngineCapacity
        {
            get { return m_EngineCapacity; }
            set { m_EngineCapacity = value; }
        }

        public override string PrintVehicleDetails()
        {
            return $"Vehicle Details for {LicenseNumber}:\n" +
                   $"Vehicle Type: Fuel Motorcycle\n" +
                   $"Model Name: {ModelName}\n" +
                   $"License Type: {LicenseType}\n" +
                   $"Engine Capacity: {EngineCapacity}cc\n" +
                   $"Current Fuel Amount: {CurrentFuelAmount}\n" +
                   $"Max Fuel Amount: {MaxFuelAmount}\n" +
                   $"Fuel Type: {FuelType}\n" +
                   $"Energy Percentage: {EnergyPercentage}%\n" +
                   $"Wheels Pressure:{Wheels[0].CurrentPressure}\n" +
                   $"Wheels Manufacturer:{Wheels[0].Manufacturer}";
        }

        
        private void SetLicenseType(string i_LicenseType)
        {
            if (Enum.TryParse(i_LicenseType, true, out eLicenseType licenseType) && Enum.IsDefined(typeof(eLicenseType), licenseType))
            {
                LicenseType = licenseType;
            }
            else
            {
                throw new ArgumentException("Invalid license type.");
            }
        }
        
        private void SetEngineCapacity(string i_EngineCapacity)
        {
            if (!int.TryParse(i_EngineCapacity, out int engineCapacity))
            {
                throw new FormatException("Invalid engine capacity format.");
            }
            
            if (engineCapacity <= 0)
            {
                throw new ValueRangeException(1, int.MaxValue, "Engine capacity must be a positive value.");
            }

            EngineCapacity = engineCapacity;
        }

        public override object[] getExtraDetailsFunctions()
        {
            object[] extraDetails = new object[2];
            extraDetails[0] = new Action<string>(SetLicenseType);
            extraDetails[1] = new Action<string>(SetEngineCapacity);
            return extraDetails;
        }
        
        public override string[] getExtraDetailsText()
        {
            string[] extraDetailsText = new string[2];
            extraDetailsText[0] = "Enter License Type (A, A2, AB, B2):";
            extraDetailsText[1] = "Enter Engine Capacity (in cc):";
            return extraDetailsText;
        }
    }

    // Electric motorcycle
    public class ElectricMotorcycle : ElectricVehicle
    {
        private eLicenseType m_LicenseType;
        private int m_EngineCapacity;

        public ElectricMotorcycle(string i_LicenseNumber, string i_ModelName)
            : base(i_ModelName, i_LicenseNumber)
        {
            this.CreateWheels("", 2, 0, 30);
            this.MaxBatteryHours = 3.2f;
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
            m_LicenseType = i_LicenseType;
            m_EngineCapacity = i_EngineCapacity;
        }

        public eLicenseType LicenseType
        {
            get { return m_LicenseType; }
            set { m_LicenseType = value; }
        }

        public int EngineCapacity
        {
            get { return m_EngineCapacity; }
            set { m_EngineCapacity = value; }
        }

        public override string PrintVehicleDetails()
        {
            return $"Vehicle Details for {LicenseNumber}:\n" +
                   $"Vehicle Type: Electric Motorcycle\n" +
                   $"Model Name: {ModelName}\n" +
                   $"License Type: {LicenseType}\n" +
                   $"Engine Capacity: {EngineCapacity}cc\n" +
                   $"Remaining Battery: {RemainingBatteryHours} hours\n" +
                   $"Max Battery: {MaxBatteryHours} hours\n" +
                   $"Energy Percentage: {EnergyPercentage}%\n" +
                   $"Wheels Pressure: {Wheels[0].CurrentPressure}\n" +
                   $"Wheels Manufacturer: {Wheels[0].Manufacturer}";
        }

        private void SetLicenseType(string i_LicenseType)
        {
            if (Enum.TryParse(i_LicenseType, true, out eLicenseType licenseType) && Enum.IsDefined(typeof(eLicenseType), licenseType))
            {
                LicenseType = licenseType;
            }
            else
            {
                throw new ArgumentException("Invalid license type.");
            }
        }
        
        private void SetEngineCapacity(string i_EngineCapacity)
        {
            if (!int.TryParse(i_EngineCapacity, out int engineCapacity))
            {
                throw new FormatException("Invalid engine capacity format.");
            }
            
            if (engineCapacity <= 0)
            {
                throw new ValueRangeException(1, int.MaxValue, "Engine capacity must be a positive value.");
            }

            EngineCapacity = engineCapacity;
        }

        public override object[] getExtraDetailsFunctions()
        {
            object[] extraDetails = new object[2];
            extraDetails[0] = new Action<string>(SetLicenseType);
            extraDetails[1] = new Action<string>(SetEngineCapacity);
            return extraDetails;
        }
        
        public override string[] getExtraDetailsText()
        {
            string[] extraDetailsText = new string[2];
            extraDetailsText[0] = "Enter License Type (A, A2, AB, B2):";
            extraDetailsText[1] = "Enter Engine Capacity (in cc):";
            return extraDetailsText;
        }
    }

    // Fuel car
    public class FuelCar : FuelVehicle
    {
        private eCarColor m_Color;
        private int m_NumberOfDoors;
        public eLicenseType m_LicenseType;

        public FuelCar(string i_LicenseNumber, string i_ModelName)
            : base(i_ModelName, i_LicenseNumber)
        {
            this.FuelType = eFuelType.Octan95;
            this.MaxFuelAmount = 48f;
            this.CreateWheels("", 5, 0, 32);
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
            m_Color = i_Color;
            if (i_NumberOfDoors < 2 || i_NumberOfDoors > 5)
            {
                throw new ValueRangeException(2, 5);
            }

            m_NumberOfDoors = i_NumberOfDoors;
        }

        public eCarColor Color
        {
            get;
            set;
        }

        public int NumberOfDoors
        {
            get;
            set;
        }

        public override string PrintVehicleDetails()
        {
            return $"Vehicle Details for {LicenseNumber}:\n" +
                   $"Vehicle Type: Fuel Car\n" +
                   $"Model Name: {ModelName}\n" +
                   $"Color: {m_Color}\n" +
                   $"Number of Doors: {m_NumberOfDoors}\n" +
                   $"Current Fuel Amount: {CurrentFuelAmount}\n" +
                   $"Max Fuel Amount: {MaxFuelAmount}\n" +
                   $"Fuel Type: {FuelType}\n" +
                   $"Energy Percentage: {EnergyPercentage}%\n" +
                   $"Wheels Pressure: {Wheels[0].CurrentPressure}\n" +
                   $"Wheels Manufacturer: {Wheels[0].Manufacturer}";
        }

        private void SetColor(string i_Color)
        {
            if (Enum.TryParse(i_Color, true, out eCarColor color) && Enum.IsDefined(typeof(eCarColor), color))
            {
                Color = color;
            }
            else
            {
                throw new ArgumentException("Invalid car color.");
            }
        }
        
        private void setNumOfDoors(string i_NumDoors)
        {
            if (!int.TryParse(i_NumDoors, out int numDoors))
            {
                throw new FormatException("Invalid number of doors format.");
            }
            
            if (numDoors < 2 || numDoors > 5)
            {
                throw new ValueRangeException(2, 5, "Number of doors must be between 2 and 5.");
            }

            NumberOfDoors = numDoors;
        }
        
        public override object[] getExtraDetailsFunctions()
        {
            object[] extraDetails = new object[2];
            extraDetails[0] = new Action<string>(SetColor);
            extraDetails[1] = new Action<string>(setNumOfDoors);
            return extraDetails;
        }
        
        public override string[] getExtraDetailsText()
        {
            string[] extraDetailsText = new string[2];
            extraDetailsText[0] = "Enter car color: (Yellow, Black, White, Silver): ";
            extraDetailsText[1] = "Enter number of doors (2-5): ";
            return extraDetailsText;
        }
    }

    public class ElectricCar : ElectricVehicle
    {
        private eCarColor m_Color;
        private int m_NumberOfDoors;

        public ElectricCar(string i_LicenseNumber, string i_ModelName)
            : base(i_ModelName, i_LicenseNumber)
        {
            this.CreateWheels("", 5, 0, 32);
            this.MaxBatteryHours = 4.8f;
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
            m_Color = i_Color;
            if (i_NumberOfDoors < 2 || i_NumberOfDoors > 5)
            {
                throw new ArgumentException("Number of doors must be between 2 and 5.");
            }

            m_NumberOfDoors = i_NumberOfDoors;
        }

        public eCarColor Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        public int NumberOfDoors
        {
            get { return m_NumberOfDoors; }
            set { m_NumberOfDoors = value; }
        }

        public override string PrintVehicleDetails()
        {
            return $"Vehicle Details for {LicenseNumber}:\n" +
                   $"Vehicle Type: Electric Car\n" +
                   $"Model Name: {ModelName}\n" +
                   $"Color: {m_Color}\n" +
                   $"Number of Doors: {m_NumberOfDoors}\n" +
                   $"Remaining Battery: {RemainingBatteryHours} hours\n" +
                   $"Max Battery: {MaxBatteryHours} hours\n" +
                   $"Energy Percentage: {EnergyPercentage}%\n" +
                   $"Wheels Pressure: {Wheels[0].CurrentPressure}\n" +
                   $"Wheels Manufacturer: {Wheels[0].Manufacturer}";
        }

        private void SetColor(string i_Color)
        {
            if (Enum.TryParse(i_Color, true, out eCarColor color) && Enum.IsDefined(typeof(eCarColor), color))
            {
                Color = color;
            }
            else
            {
                throw new ArgumentException("Invalid car color.");
            }
        }
        
        private void setNumOfDoors(string i_NumDoors)
        {
            if (!int.TryParse(i_NumDoors, out int numDoors))
            {
                throw new FormatException("Invalid number of doors format.");
            }
            
            if (numDoors < 2 || numDoors > 5)
            {
                throw new ValueRangeException(2, 5, "Number of doors must be between 2 and 5.");
            }

            NumberOfDoors = numDoors;
        }
        public override object[] getExtraDetailsFunctions()
        {
            object[] extraDetails = new object[2];
            extraDetails[0] = new Action<string>(SetColor);
            extraDetails[1] = new Action<string>(setNumOfDoors);
            return extraDetails;
        }
        
        public override string[] getExtraDetailsText()
        {
            string[] extraDetailsText = new string[2];
            extraDetailsText[0] = "Enter car color: (Yellow, Black, White, Silver): ";
            extraDetailsText[1] = "Enter number of doors (2-5): ";
            return extraDetailsText;
        }
         
    }


    // Fuel truck
    public class Truck : FuelVehicle
    {
        public bool CarriesHazardousMaterials { get; set; }
        public float CargoVolume { get; set; }

        public Truck(string i_LicenseNumber, string i_ModelName)
            : base(i_ModelName, i_LicenseNumber)
        {
            this.FuelType = eFuelType.Soler;
            this.MaxFuelAmount = 135f;
            this.CreateWheels("", 12, 0, 27);
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

        public override string PrintVehicleDetails()
        {
            return $"Vehicle Details for {LicenseNumber}:\n" +
                   $"Vehicle Type: Truck\n" +
                   $"Model Name: {ModelName}\n" +
                   $"Carries Hazardous Materials: {CarriesHazardousMaterials}\n" +
                   $"Cargo Volume: {CargoVolume}\n" +
                   $"Current Fuel Amount: {CurrentFuelAmount}\n" +
                   $"Max Fuel Amount: {MaxFuelAmount}\n" +
                   $"Fuel Type: {FuelType}\n" +
                   $"Energy Percentage: {EnergyPercentage}%\n" +
                   $"Wheels Pressure: {Wheels[0].CurrentPressure}\n" +
                   $"Wheels Manufacturer: {Wheels[0].Manufacturer}";
        }

        private void SetHazardousMaterials(string i_CarriesHazardous)
        {
            if (bool.TryParse(i_CarriesHazardous, out bool carriesHazardous))
            {
                CarriesHazardousMaterials = carriesHazardous;
            }
            else
            {
                throw new FormatException("Invalid format for hazardous materials flag. Please enter 'true' or 'false'.");
            }
        }
        
        private void SetCargoVolume(string i_CargoVolume)
        {
            if (!float.TryParse(i_CargoVolume, out float cargoVolume))
            {
                throw new FormatException("Invalid cargo volume format.");
            }
            
            if (cargoVolume <= 0)
            {
                throw new ValueRangeException(0, float.MaxValue, "Cargo volume must be a positive value.");
            }

            CargoVolume = cargoVolume;
        }

        public override object[] getExtraDetailsFunctions()
        {
            object[] extraDetails = new object[2];
            extraDetails[0] = new Action<string>(SetHazardousMaterials);
            extraDetails[1] = new Action<string>(SetCargoVolume);
            return extraDetails;
        }
        
        public override string[] getExtraDetailsText()
        {
            string[] extraDetailsText = new string[2];
            extraDetailsText[0] = "Does the truck carry hazardous materials? (true/false): ";
            extraDetailsText[1] = "Enter cargo volume: ";
            return extraDetailsText;
        }
    }
}
