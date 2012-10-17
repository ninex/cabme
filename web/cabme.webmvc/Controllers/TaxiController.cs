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
    public class TaxiController : ApiController
    {
        // GET api/taxi
        public IEnumerable<Taxi> Get()
        {
            return GetAllTaxis(0);
        }

        // GET api/taxi/5
        public Taxi Get(int id)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return context.Taxis.Where(p => p.Id == id).Select(taxi => new Taxi
                {
                    Id = taxi.Id,
                    Name = taxi.Name,
                    PhoneNumber = taxi.PhoneNumber,
                    RatePerKm = taxi.RatePerKm,
                    MinRate = taxi.MinRate,
                    Units = taxi.Units,
                    StartOfService = taxi.StartOfService,
                    EndOfService = taxi.EndOfService,
                    Is24HService = taxi.Is24HService,
                    FleetSize = taxi.FleetSize
                }).SingleOrDefault();
            }
        }

        // POST api/taxi
        public void Post([FromBody]Taxi value)
        {
        }

        // PUT api/taxi/5
        public void Put(int id, [FromBody]Taxi value)
        {
        }

        // DELETE api/taxi/5
        public void Delete(int id)
        {
        }

        // GET api/taxi/?distance=
        public IEnumerable<Taxi> GetAllTaxis(int distance)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var taxis = context.Taxis.AsEnumerable().Select(taxi => new Taxi
                {
                    Id = taxi.Id,
                    Name = taxi.Name,
                    PhoneNumber = taxi.PhoneNumber,
                    RatePerKm = taxi.RatePerKm,
                    MinRate = taxi.MinRate,
                    Units = taxi.Units,
                    StartOfService = taxi.StartOfService,
                    EndOfService = taxi.EndOfService,
                    Is24HService = taxi.Is24HService,
                    FleetSize = taxi.FleetSize,
                    PriceEstimate = GetPriceEstimate(distance, taxi.RatePerKm, taxi.MinRate, taxi.Units)
                });
                return taxis.ToList();
            }
        }

        // GET api/taxi/?user=
        public int GetTaxiIdForUser(string user)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return (from users in context.Users
                        join userTaxi in context.UserTaxis on users.Id equals userTaxi.UserId
                        where users.Name == user
                        select userTaxi.TaxiId).SingleOrDefault();
            }
        }

        private int GetPriceEstimate(int distance, int ratePerKm, int minRate, int units)
        {
            int estimate = 0;
            if (distance > 0)
            {
                var computedPrice = (ratePerKm * distance) / 1000;
                if (computedPrice < minRate)
                {
                    computedPrice = minRate;
                }
                else
                {
                    if (computedPrice % units > 0)
                    {
                        computedPrice = computedPrice - (computedPrice % units) + units;
                    }
                }
                estimate = computedPrice;
            }
            return estimate;
        }

    }
}
