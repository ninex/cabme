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
        public string Name { get; set; }

        public static Taxis GetAllTaxis()
        {
            List<Taxi> taxis = new List<Taxi>();
            taxis.Add(new Taxi() { Name = "test 1" });
            taxis.Add(new Taxi() { Name = "test 2" });
            return new Taxis(taxis);
        }
    }

    [CollectionDataContract(Namespace = "http://cabme.co.za/taxis")]
    public class Taxis : List<Taxi>
    {
        public Taxis() { }
        public Taxis(List<Taxi> taxis) : base(taxis) { }
    }
}