using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Data = cabme.data;
using Web = cabme.webmvc;

namespace cabme.tests
{
    [TestClass]
    public class TaxiTest
    {
        Web.Controllers.TaxiController controller;

        List<Data.Taxi> taxis;

        [TestInitialize]
        public void TestStartup()
        {
            var taxiMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.Taxi>>();
            var userMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.User>>();
            var userTaxiMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.UserTaxi>>();

            taxis = new List<Data.Taxi>();

            taxis.Add(new Data.Taxi()
            {
                Id = 1,
                Name = "Taxi 1"
            });
            taxis.Add(new Data.Taxi()
            {
                Id = 2,
                Name = "Taxi 2"
            });

            taxiMock.Stub(x => x.All())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Data.Taxi>(taxis));

            controller = new Web.Controllers.TaxiController(taxiMock, userMock, userTaxiMock);
        }

        [TestMethod]
        public void TestTaxiGetAll()
        {
            var actual = controller.Get();
            Assert.IsNotNull(actual);
            Assert.AreEqual(taxis.Count(), actual.Count());
        }
    }
}
