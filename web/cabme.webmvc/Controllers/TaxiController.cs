using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using cabme.webmvc.Models;
using Data = cabme.data;

namespace cabme.webmvc.Controllers
{
    public class TaxiController : ApiController
    {
        private Data.Interfaces.IRepository<Data.Taxi> repository;
        private Data.Interfaces.IRepository<Data.User> userRepository;
        private Data.Interfaces.IRepository<Data.UserTaxi> userTaxiRepository;

        //Default constructor initialises with a database context
        public TaxiController()
            : this(new Data.Repositories.Repository<Data.Taxi>(), new Data.Repositories.Repository<Data.User>(), new Data.Repositories.Repository<Data.UserTaxi>()) { }

        public TaxiController(Data.Interfaces.IRepository<Data.Taxi> repository, Data.Interfaces.IRepository<Data.User> userRepository, Data.Interfaces.IRepository<Data.UserTaxi> userTaxiRepository)
        {
            this.repository = repository;
            this.userRepository = userRepository;
            this.userTaxiRepository = userTaxiRepository;
        }

        // GET api/taxi
        public IEnumerable<Taxi> Get()
        {
            return GetAllTaxis(0);
        }

        // GET api/taxi/5
        public Taxi Get(int id)
        {
            return repository.FindAll(p => p.Id == id).Select(taxi => new Taxi
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
            var taxis = repository.All().Select(taxi => new Taxi
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
                PriceEstimate = Taxi.GetPriceEstimate(distance, taxi.RatePerKm, taxi.MinRate, taxi.Units)
            });
            return taxis;
        }

        // GET api/taxi/?user=
        public int GetTaxiIdForUser(string user)
        {
            return (from users in userRepository.All()
                    join userTaxi in userTaxiRepository.All() on users.Id equals userTaxi.UserId
                    where users.Name == user
                    select userTaxi.TaxiId).SingleOrDefault();
        }
    }
}
