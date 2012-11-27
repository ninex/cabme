using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Data = cabme.data;
using Web = cabme.webmvc;

namespace cabme.tests
{
    [TestClass]
    public class SuburbTest
    {
        Web.Controllers.SuburbController controller;

        List<Data.Suburb> suburbs;

        [TestInitialize]
        public void TestStartup()
        {
            var dbMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.Suburb>>();

            suburbs = new List<Data.Suburb>();

            suburbs.Add(new Data.Suburb()
            {
                City = "Cape Town",
                Id = 1,
                Name = "Sea Point",
                PostalCode = "8001"
            });
            suburbs.Add(new Data.Suburb()
            {
                City = "Johannesburg",
                Id = 2,
                Name = "Mellville",
                PostalCode = "1234"
            });

            dbMock.Stub(x => x.All())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Data.Suburb>(suburbs));

            controller = new Web.Controllers.SuburbController(dbMock);
        }

        [TestMethod]
        public void TestGet()
        {
            var actual = controller.Get();
            Assert.IsNotNull(actual);
            Assert.AreEqual(suburbs.Count(), actual.Count());
        }

        [TestMethod]
        public void TestGetAllByCity()
        {
            var expected = new Web.Models.Suburb()
            {
                City = "Cape Town",
                Id = 1,
                Name = "Sea Point",
                PostalCode = "8001"
            };

            var list = controller.GetAllByCity("Cape Town");
            Assert.IsNotNull(list);
            var actual = list.First();
            AreSame(expected, actual);

            expected = new Web.Models.Suburb()
            {
                City = "Johannesburg",
                Id = 2,
                Name = "Mellville",
                PostalCode = "1234"
            };

            list = controller.GetAllByCity("Johannesburg");
            Assert.IsNotNull(list);
            actual = list.First();
            AreSame(expected, actual);
        }

        [TestMethod]
        public void TestIgnoreCase()
        {
            var list = controller.GetAllByCity("CAPe TOwn");
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count());
        }

        private void AreSame(Web.Models.Suburb expected, Web.Models.Suburb actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.City, actual.City);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PostalCode, actual.PostalCode);
        }
    }
}
