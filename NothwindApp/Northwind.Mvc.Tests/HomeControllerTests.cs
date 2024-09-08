using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Northwind.Mvc.Controllers;
using Packt.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Mvc.Tests
{

    //Only ProductDetail ction was tested

    public class HomeControllerTests
    {
        [Fact]
        public async void ProductDetailIsReturnBadRequest()
        {
            //Arrange
            var mock = new Mock<ILogger<HomeController>>();
            var mockDb = new Mock<NorthwindContext>();
            var controller = new HomeController(mock.Object,mockDb.Object);

            //Act
            var result = await controller.ProductDetail(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void ProductDetailIsReturnNotFound()
        {
            //Arrange
            var mock = new Mock<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<NorthwindContext>().UseSqlServer(@"Data Source=DESKTOP-A8O78SR;Initial Catalog=Northwind;Integrated Security=True;Encrypt=True").Options;
            int testId = 200;


            //Act
            using (var context = new NorthwindContext(options))
            {
                var controller = new HomeController(mock.Object, context);
                var result = await controller.ProductDetail(testId);

                //Assert
                Assert.IsType<NotFoundObjectResult>(result);
            }
        }

        [Fact]
        public async void ProductDetailIsReturnProduct()
        {
            //Arrange
            var mock = new Mock<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<NorthwindContext>().UseSqlServer(@"Data Source=DESKTOP-A8O78SR;Initial Catalog=Northwind;Integrated Security=True;Encrypt=True").Options;
            int testId = 1;
            IActionResult? result;

            //Act
            using (var context = new NorthwindContext(options))
            {
                var controller = new HomeController(mock.Object, context);
                result = await controller.ProductDetail(testId);
            }

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Chai", model.ProductName);
            Assert.Equal(18, model.UnitPrice);
            Assert.Equal("10 boxes x 20 bags", model.QuantityPerUnit);
        }

    }
}
