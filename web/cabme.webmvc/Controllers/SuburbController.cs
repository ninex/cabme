using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cabme.webmvc.Models;
using Data = cabme.data;

namespace cabme.webmvc.Controllers
{
    public class SuburbController : ApiController
    {
        // GET api/suburb
        public IEnumerable<Suburb> Get()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return context.Suburbs.Select(suburb => new Suburb
                {
                    Id = suburb.Id,
                    Name = suburb.Name,
                    City = suburb.City,
                    PostalCode = suburb.PostalCode
                }).OrderBy(p => p.Name).ToList();
            }
        }

        // GET api/suburb/?city=
        public IEnumerable<Suburb> GetAllByCity(string city)
        {
            if (!string.IsNullOrEmpty(city))
            {
                return Get().Where(p => p.City.ToUpper() == city.ToUpper());
            }
            else
            {
                return null;
            }
        }
    }
}
