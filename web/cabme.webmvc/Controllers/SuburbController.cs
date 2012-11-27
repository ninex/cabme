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
        private Data.Interfaces.IRepository<Data.Suburb> repository;

        //Default constructor initialises with a database context
        public SuburbController() : this(new Data.Repositories.Repository<Data.Suburb>()) { }

        public SuburbController(Data.Interfaces.IRepository<Data.Suburb> repository)
        {
            this.repository = repository;
        }
        // GET api/suburb
        public IEnumerable<Suburb> Get()
        {
            return repository.All().Select(suburb => new Suburb
            {
                Id = suburb.Id,
                Name = suburb.Name,
                City = suburb.City,
                PostalCode = suburb.PostalCode
            }).OrderBy(p => p.Name).ToList();
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
