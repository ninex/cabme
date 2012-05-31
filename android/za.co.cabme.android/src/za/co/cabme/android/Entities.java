package za.co.cabme.android;

import java.text.DecimalFormat;

import com.google.android.maps.GeoPoint;

public class Entities {
	public class Taxi {
		public int Id;
		public String Name;
		public String PhoneNumber;
		public int RatePerKm;
		public int MinRate;
		public int Units;
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
		public int EstimatedPrice;
		public boolean Active;
		public boolean Confirmed;
		public int TaxiId;
		public Taxi SelectedTaxi;

		public Booking() {
			NumberOfPeople = 1;
			Active = true;
			Confirmed = false;
		}

		public GeoPoint getFromPoint() {
			if (longitudeFrom != 0 && latitudeFrom != 0) {
				return new GeoPoint(latitudeFrom, longitudeFrom);
			}
			return null;
		}

		public GeoPoint getToPoint() {
			if (longitudeTo != 0 && latitudeTo != 0) {
				return new GeoPoint(latitudeTo, longitudeTo);
			}
			return null;
		}

		public void setFromPoint(GeoPoint p) {
			if (p != null) {
				latitudeFrom = p.getLatitudeE6();
				longitudeFrom = p.getLongitudeE6();
			} else {
				latitudeFrom = 0;
				longitudeFrom = 0;
			}
		}

		public void setToPoint(GeoPoint p) {
			if (p != null) {
				latitudeTo = p.getLatitudeE6();
				longitudeTo = p.getLongitudeE6();
			} else {
				latitudeTo = 0;
				longitudeTo = 0;
			}
		}

		public String getPriceEstimate() {
			if (SelectedTaxi != null) {
				int computedPrice = (int) (SelectedTaxi.RatePerKm * ComputedDistance) / 1000;
				if (computedPrice < SelectedTaxi.MinRate) {
					computedPrice = SelectedTaxi.MinRate;
				} else {
					if (computedPrice % SelectedTaxi.Units > 0) {
						computedPrice = computedPrice
								- (computedPrice % SelectedTaxi.Units)
								+ SelectedTaxi.Units;
					}
				}
				EstimatedPrice = computedPrice;
				DecimalFormat dec = new DecimalFormat("###.##");
				return "R" + dec.format((float) computedPrice / 100);
			}
			EstimatedPrice = 0;
			return "No estimate";
		}
	}
}
