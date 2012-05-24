package za.co.cabme.android;

public class Entities {
	public class Taxi {
		public int Id;
		public String Name;
		public String PhoneNumber;
		public int RatePerKm;
		public int MinRate;
		public String StartOfService;
		public String EndOfService;
		public boolean Is24HService;
		public byte FleetSize;

		public Taxi() {

		}
	}
	public class Booking {
		public int Id;
		public String Name;
		public String PhoneNumber;
		public byte NumberOfPeople;
		public String PickupTime;
		public String AddrFrom;
		public int latitudeFrom;
		public int longitudeFrom;
		public String AddrTo;
		public int latitudeTo;
		public int longitudeTo;
		public int ComputedDistance;
		public int TaxiId;
		public Taxi SelectedTaxi;
		
		public Booking(){			
		}
	}
}
