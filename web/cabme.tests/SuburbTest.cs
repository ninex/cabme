using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using cabme.data;

namespace cabme.tests
{
    [TestClass]
    public class SuburbTest
    {
        [TestMethod]
        public void TestGetAllByCity()
        {
            /*var dbMock = MockRepository.GenerateMock<IDataContextWrapper>();*/
            
            List<Suburb> suburbs = new List<Suburb>();

            var suburb = new Suburb()
            {
                City = "Cape Town",
                Id = 1,
                Name = "Sea Point",
                PostalCode = "8001"
            };

            
            suburbs.Add(suburb);

            /*dbMock.Stub(x => x.Table<Suburb>())
                .IgnoreArguments()
                .Return(new EnumerableQuery<Suburb>(suburbs));*/

            webmvc.Controllers.SuburbController controller = new webmvc.Controllers.SuburbController();
            var actual = controller.GetAllByCity("Cape Town");

            Assert.AreEqual(suburb, actual);
        }
    }
}
