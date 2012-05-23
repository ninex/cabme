using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/booking")]
	public class Booking
	{		
		[DataMember]
		public int Id { get; set; }
		
        [DataMember]
		public string Name { get; set; }

        [DataMember]
		public string PhoneNumber { get; set; }

        [DataMember]
		public byte NumberOfPeople { get; set; }

        [DataMember]
		public string PickupTime { get; set; }

        [DataMember]
		public string AddrFrom { get; set; }

        [DataMember]
		public int latitudeFrom { get; set; }

        [DataMember]
		public int longitudeFrom { get; set; }

        [DataMember]
		public string AddrTo { get; set; }

        [DataMember]
		public int latitudeTo { get; set; }

        [DataMember]
		public int longitudeTo { get; set; }

        [DataMember]
		public int ComputedDistance { get; set; }
		
		[DataMember]
		public int TaxiId {get;set;}

		public static Booking MakeBooking (Booking booking)
		{
			booking.Id = 999;
			return booking;
		}
		
		public static Bookings GetAllBookings ()
		{
			List<Booking > bookings = new List<Booking> ();
			bookings.Add (new Booking (){ Id=2, Name="Tester", PhoneNumber="0825098244", NumberOfPeople=2, PickupTime="2012-05-21 14:21:00",
				AddrFrom = "12 Carstens street, Cape Town, 8001", AddrTo = "11 Firdale Avenue, Cape Town, 8001", latitudeFrom=0,longitudeFrom=0,
				latitudeTo=0,longitudeTo=0, ComputedDistance=1600, TaxiId=1});
					
			return new Bookings (bookings);
		}

		public static Bookings GetAllBookingsByNumber (string number)
		{
			List<Booking > bookings = new List<Booking> ();
			//return new Bookings (bookings);
			return GetAllBookings();
		}
	}

    [CollectionDataContract(Namespace = "http://cabme.co.za/bookings")]
	public class Bookings : List<Booking>
	{
		public Bookings ()
		{
		}

		public Bookings (List<Booking> bookings) : base(bookings)
		{
		}
	}
}

