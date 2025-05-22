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
		private string r_Manufacturer;
		private float m_CurrentPressure;
		private readonly float r_MaxPressure;

		public Wheel(string i_Manufacturer, float i_CurrentPressure, float i_MaxPressure)
		{
			r_Manufacturer = i_Manufacturer;
			if (i_CurrentPressure <= i_MaxPressure)
			{
				m_CurrentPressure = i_CurrentPressure;
			}
			else
			{
				throw new ArgumentException("Current pressure cannot exceed max pressure.");
			}

			r_MaxPressure = i_MaxPressure;
		}

		public string Manufacturer
		{
			get { return r_Manufacturer; }
			set { r_Manufacturer = value; }
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
		private List<Wheel> r_Wheels;
		
		protected Vehicle(string i_ModelName, string i_LicenseNumber)
		{
			// only set model & license—leave energy at 0, wheels empty
			r_ModelName     = i_ModelName;
			r_LicenseNumber = i_LicenseNumber;
			m_EnergyPercentage = 0f;
			r_Wheels = new List<Wheel>();
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
			CreateWheels(i_WheelManufacturer,i_AmountOfWheels,i_WheelcurrentPressure,i_WheelmaxPressure);
		}

		public void CreateWheels(string i_WheelManufacturer, int i_AmountOfWheels, float i_WheelcurrentPressure,
			float i_WheelmaxPressure)
		{
			r_Wheels = new List<Wheel>(i_AmountOfWheels);
			for (int i = 0; i < i_AmountOfWheels; i++)
			{
				r_Wheels.Add(new Wheel(i_WheelManufacturer, i_WheelcurrentPressure, i_WheelmaxPressure));
			}
		}
        public void SetTiresInfo(float i_airPressure,string r_Manufacturer)
        {
            foreach(Wheel wheel in r_Wheels )
            {
                wheel.CurrentPressure = i_airPressure;
                wheel.Manufacturer = r_Manufacturer;
            }
        }

        public void fillTiresToMax()
        {
            foreach(Wheel wheel in Wheels)
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
					throw new ArgumentException("Energy percentage must be between 0 and 100.");
				}
			}
		}

		public List<Wheel> Wheels
		{
			get { return r_Wheels; }
		}
		public abstract string PrintVehicleDetails();
	}

	// Fuel-based vehicle
	public abstract class FuelVehicle : Vehicle
	{
		public eFuelType r_FuelType { get; set; }
		private float m_CurrentFuelAmount;
		public float r_MaxFuelAmount { get; set; }

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
			r_FuelType = i_FuelType;
			m_CurrentFuelAmount = i_CurrentFuelAmount;
			r_MaxFuelAmount = i_MaxFuelAmount;
		}

		public eFuelType FuelType
		{
			get { return r_FuelType; }
		}

		public float CurrentFuelAmount
		{
			get { return m_CurrentFuelAmount; }
		}
		
		public void Refuel(eFuelType i_FuelType, float i_LitersToAdd)
		{
			if (i_FuelType != r_FuelType)
			{
				throw new ArgumentException("Fuel type mismatch.");
			}

			if (m_CurrentFuelAmount + i_LitersToAdd <= r_MaxFuelAmount)
			{
				m_CurrentFuelAmount += i_LitersToAdd;
				EnergyPercentage = (m_CurrentFuelAmount / r_MaxFuelAmount) * 100f;
			}
			else
			{
				throw new ArgumentException("Cannot refuel beyond tank capacity.");
			}
		}
	}

	// Electric vehicle
	public abstract class ElectricVehicle : Vehicle
	{
		private float m_RemainingBatteryHours;	
		public float r_MaxBatteryHours { get; set; }

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
			r_MaxBatteryHours = i_MaxBatteryHours;
		}

		public float RemainingBatteryHours
		{
			get { return m_RemainingBatteryHours; }
		}

		public float MaxBatteryHours
		{
			get { return r_MaxBatteryHours; }
		}

		public void ChargeBattery(float i_HoursToAdd)
		{
			if (m_RemainingBatteryHours + i_HoursToAdd <= r_MaxBatteryHours)
			{
				m_RemainingBatteryHours += i_HoursToAdd;
				EnergyPercentage = (m_RemainingBatteryHours / r_MaxBatteryHours) * 100f;
			}
			else
			{
				throw new ArgumentException("Cannot charge beyond battery capacity.");
			}
		}
	}

	// Fuel motorcycle
	public class FuelMotorcycle : FuelVehicle
	{
		private eLicenseType r_LicenseType;
		private int r_EngineCapacity;

		public FuelMotorcycle(string i_LicenseNumber, string i_ModelName)
			: base(i_ModelName, i_LicenseNumber)
		{
			this.r_FuelType = eFuelType.Octan98;
			this.r_MaxFuelAmount = 5.8f;
			this.CreateWheels("",2,0,30);
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
			: base(i_ModelName, i_LicenseNumber, i_EnergyPercentage, i_WheelManufacturer,2,
				i_WheelcurrentPressure,30, eFuelType.Octan98, i_CurrentFuelAmount,5.8f)
		{
			r_LicenseType = i_LicenseType;
			r_EngineCapacity = i_EngineCapacity;
		}

		public eLicenseType LicenseType
		{
			get { return r_LicenseType; }
			set { r_LicenseType = value; }
		}

		public int EngineCapacity
		{
			get { return r_EngineCapacity; }
			set { r_EngineCapacity = value; }
		}
		
		public override string PrintVehicleDetails()
		{
			return $"Vehicle Details for {LicenseNumber}:\n" +
			       $"Vehicle Type: Fuel Motorcycle\n" +
			       $"Model Name: {ModelName}\n" +
			       $"License Type: {LicenseType}\n" +
			       $"Engine Capacity: {EngineCapacity}cc\n" +
			       $"Current Fuel Amount: {CurrentFuelAmount}\n" +
			       $"Max Fuel Amount: {r_MaxFuelAmount}\n" +
			       $"Fuel Type: {FuelType}\n" +
			       $"Energy Percentage: {EnergyPercentage}%";
		}
	}

	// Electric motorcycle
	public class ElectricMotorcycle : ElectricVehicle
	{
		private  eLicenseType r_LicenseType;
		private  int r_EngineCapacity;

		public ElectricMotorcycle(string i_LicenseNumber, string i_ModelName)
			: base(i_ModelName, i_LicenseNumber)
		{
			this.CreateWheels("", 2, 0, 30);
			this.r_MaxBatteryHours = 3.2f;
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
			r_LicenseType = i_LicenseType;
			r_EngineCapacity = i_EngineCapacity;
		}

		public eLicenseType LicenseType
		{
			get { return r_LicenseType; }
			set { r_LicenseType = value; }
		}

		public int EngineCapacity
		{
			get { return r_EngineCapacity; }
			set { r_EngineCapacity = value; }
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
			       $"Energy Percentage: {EnergyPercentage}%";
		}
	}

	// Fuel car
	public class FuelCar : FuelVehicle
	{
		public eCarColor r_Color;
		public int r_NumberOfDoors;
        public eLicenseType r_LicenseType;
		
		public FuelCar(string i_LicenseNumber, string i_ModelName)
			: base(i_ModelName, i_LicenseNumber)
        {
            this.r_FuelType = eFuelType.Octan95;
            this.r_MaxFuelAmount = 48f;
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
			r_Color = i_Color;
			if (i_NumberOfDoors < 2 || i_NumberOfDoors > 5)
			{
				throw new ArgumentException("Number of doors must be between 2 and 5.");
			}
			r_NumberOfDoors = i_NumberOfDoors;
		}

        public eCarColor SetColor
        {
			get { return r_Color; }
			set { r_Color = value; }
        }

        public int SetNumberOfDoors
        {
            get
            {
                return r_NumberOfDoors;
            }
            set
            {
                r_NumberOfDoors = value;
            }
        }
        
        public override string PrintVehicleDetails()
        {
	        return $"Vehicle Details for {LicenseNumber}:\n" +
	               $"Vehicle Type: Fuel Car\n" +
	               $"Model Name: {ModelName}\n" +
	               $"Color: {r_Color}\n" +
	               $"Number of Doors: {r_NumberOfDoors}\n" +
	               $"Current Fuel Amount: {CurrentFuelAmount}\n" +
	               $"Max Fuel Amount: {r_MaxFuelAmount}\n" +
	               $"Fuel Type: {FuelType}\n" +
	               $"Energy Percentage: {EnergyPercentage}%";
        }
	}
	
	public class ElectricCar : ElectricVehicle
	{
		private eCarColor r_Color;
		private int r_NumberOfDoors;

		public ElectricCar(string i_LicenseNumber, string i_ModelName)
			: base(i_ModelName, i_LicenseNumber)
		{
			this.CreateWheels("", 5, 0, 32);
			this.r_MaxBatteryHours = 4.8f;
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
			r_Color = i_Color;
			if (i_NumberOfDoors < 2 || i_NumberOfDoors > 5)
			{
				throw new ArgumentException("Number of doors must be between 2 and 5.");
			}
			r_NumberOfDoors = i_NumberOfDoors;
		}

		public eCarColor Color
		{
			get { return r_Color; }
            set
            {
                r_Color = value;
            }
        }

		public int NumberOfDoors
		{
			get { return r_NumberOfDoors; }
            set
            {
                r_NumberOfDoors = value;
            }
        }
		
		public override string PrintVehicleDetails()
		{
			return $"Vehicle Details for {LicenseNumber}:\n" +
			       $"Vehicle Type: Electric Car\n" +
			       $"Model Name: {ModelName}\n" +
			       $"Color: {Color}\n" +
			       $"Number of Doors: {NumberOfDoors}\n" +
			       $"Remaining Battery: {RemainingBatteryHours} hours\n" +
			       $"Max Battery: {MaxBatteryHours} hours\n" +
			       $"Energy Percentage: {EnergyPercentage}%";
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
            this.r_FuelType = eFuelType.Soler;
            this.r_MaxFuelAmount = 135f;
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
	               $"Max Fuel Amount: {r_MaxFuelAmount}\n" +
	               $"Fuel Type: {FuelType}\n" +
	               $"Energy Percentage: {EnergyPercentage}%";
        }
    }
}
