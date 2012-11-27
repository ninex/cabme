using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Data = cabme.data;
using Web = cabme.webmvc;

namespace cabme.tests
{
    [TestClass]
    public class BookingTest
    {
        Web.Controllers.BookingController controller;

        List<Data.Booking> bookings;

        [TestInitialize]
        public void TestStartup()
        {
            var bookingMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.Booking>>();
            var taxiMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.Taxi>>();
            var userMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.User>>();
            var userTaxiMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.UserTaxi>>();
            var suburbMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.Suburb>>();
            var contactDetailMock = MockRepository.GenerateMock<Data.Interfaces.IRepository<Data.ContactDetail>>();

            bookings = new List<Data.Booking>();

            bookings.Add(new Data.Booking()
            {
                Id = 1,
                Name = "Booking 1"
            });
            bookings.Add(new Data.Booking()
            {
                Id = 2,
                Name = "Booking 2"
            });

            var taxis = new List<Data.Taxi>();
            taxis.Add(new Data.Taxi()
            {
                Id = 1,
                Name = "Taxi 1"
            });

            var suburbs = new List<Data.Suburb>();
            suburbs.Add(new Data.Suburb()
            {
                City = "Cape Town",
                Id = 1,
                Name = "Sea Point",
                PostalCode = "8001"
            });

            bookingMock.Stub(x => x.All())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Data.Booking>(bookings));

            taxiMock.Stub(x => x.All())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Data.Taxi>(taxis));

            suburbMock.Stub(x => x.All())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Data.Suburb>(suburbs));


            controller = new Web.Controllers.BookingController(bookingMock, suburbMock, taxiMock, userMock, userTaxiMock, contactDetailMock);
        }

        [TestMethod]
        public void TestBookingGetAll()
        {
            var actual = controller.Get();
            Assert.IsNotNull(actual);
            Assert.AreEqual(bookings.Count(), actual.Count());
        }
    }
}
