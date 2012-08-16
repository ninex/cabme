using System.Runtime.Serialization;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/confirmation")]
    public class Confirmation
    {
        [DataMember]
        public string Arrival { get; set; }

        [DataMember]
        public string Hash { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string RefCode { get; set; }
    }
}