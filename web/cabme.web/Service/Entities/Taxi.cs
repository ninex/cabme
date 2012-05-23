using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/taxi")]
	public class Taxi
	{
		[DataMember]
		public int Id { get; set; }

        [DataMember]
		public string Name { get; set; }

        [DataMember]
		public string PhoneNumber { get; set; }

        [DataMember]
		public int RatePerKm { get; set; }

        [DataMember]
		public int MinRate { get; set; }

        [DataMember]
		public string StartOfService { get; set; }

        [DataMember]
		public string EndOfService { get; set; }

        [DataMember]
		public bool Is24HService { get; set; }

        [DataMember]
		public byte FleetSize { get; set; }

		public static Taxis GetAllTaxis ()
		{
			List<Taxi > taxis = new List<Taxi> ();
			taxis.Add (new Taxi () { Id = 1, Name = "taxi 1", PhoneNumber = "02188", RatePerKm = 900, MinRate=2000, StartOfService = "00:00:00", EndOfService="00:00:00", Is24HService = true, FleetSize=12 });
			taxis.Add (new Taxi () { Id = 2, Name = "taxi 2",PhoneNumber = "0218845", RatePerKm = 920, MinRate=1800, StartOfService = "08:00:00", EndOfService="22:00:00", Is24HService = false, FleetSize=10 });
			return new Taxis (taxis);
		}
	}

    [CollectionDataContract(Namespace = "http://cabme.co.za/taxis")]
	public class Taxis : List<Taxi>
	{
		public Taxis ()
		{
		}

		public Taxis (List<Taxi> taxis) : base(taxis)
		{
		}
	}
}