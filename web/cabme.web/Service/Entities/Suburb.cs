using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Data = cabme.data;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/suburb")]
    public class Suburb
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        public static Suburbs GetAllSuburbs()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return new Suburbs(AllQueryableSuburbs(context).ToList());
            }
        }
        public static Suburbs GetAllByCity(string city)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return new Suburbs(AllQueryableSuburbs(context).Where(p => p.City == city).ToList());
            }
        }

        private static IQueryable<Suburb> AllQueryableSuburbs(Data.contentDataContext context)
        {
            return context.Suburbs.Select(suburb => new Suburb
            {
                Id = suburb.Id,
                Name = suburb.Name,
                City = suburb.City,
                PostalCode = suburb.PostalCode
            }).OrderBy(p => p.Name);
        }
    }
    [CollectionDataContract(Namespace = "http://cabme.co.za/suburbs")]
    public class Suburbs : List<Suburb>
    {
        public Suburbs()
        {
        }

        public Suburbs(List<Suburb> suburbs)
            : base(suburbs)
        {
        }
    }
}