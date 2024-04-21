using System;
using System.Threading.Tasks;
using BankWeb.Pages.Customers;
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace TestProject
{
    public class CreateCustomerTest
    {
#nullable disable
        private Mock<IPersonService> _personServiceMock;
        private CreateModel _createModel;
#nullable restore

        [SetUp]
        public void Setup()
        {
            _personServiceMock = new Mock<IPersonService>();
            _createModel = new CreateModel(_personServiceMock.Object);
        }

        [Test]
        public async Task OnPostAsync_WithInvalidModel_ReturnsPageResult()
        {
            _createModel.ModelState.AddModelError("Error", "Some error");

            var result = await _createModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public async Task OnPostAsync_WithException_ReturnsPageResult()
        {
            _createModel.ModelState.Clear();
            _createModel.Givenname = "John";
            _createModel.Surname = "Doe";

            _personServiceMock.Setup(x => x.CreateCustomerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                              .ThrowsAsync(new Exception("Simulated service failure"));

            var result = await _createModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
        }
    }
}
