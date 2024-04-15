using System;
using System.Threading.Tasks;
using BankWeb.Pages.CustomersFolder;
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

        // [Test]
        // public async Task OnPostAsync_WithValidModel_ReturnsRedirectToPageResult()
        // {
        //     Assert.IsTrue(_createModel.ModelState.IsValid, "ModelState must be valid for redirection to occur.");
        //
        //     // Arrange
        //     _createModel.Gender = "male";
        //     _createModel.Givenname = "Mille Marcus";
        //     _createModel.Surname = "Elfver";
        //     _createModel.Streetaddress = "Nibblevägen 6";
        //     _createModel.City = "Botkyrka";
        //     _createModel.Zipcode = "146 30";
        //     _createModel.Country = "Sweden";
        //     _createModel.CountryCode = "SE";
        //     _createModel.Emailaddress = "mille.elfver98@gmail.net";
        //     _createModel.Telephonecountrycode = "46";
        //     _createModel.Telephonenumber = "0704601679";
        //     _createModel.NationalId = null; // Consider providing a valid NationalId
        //     _createModel.BirthdayDay = 17;
        //     _createModel.BirthdayMonth = 7;
        //     _createModel.BirthdayYear = 1996;
        //     _createModel.DispositionType = "OWNER";
        //     _createModel.Frequency = "Weekly";
        //     _createModel.InitialDeposit = 1000;
        //
        //     var customer = new Customer { CustomerId = 1 };
        //     var account = new Account { AccountId = 1 };
        //     var disposition = new Disposition { DispositionId = 1 };
        //
        //     _personServiceMock.Setup(x => x.CreateCustomerAsync(
        //         _createModel.Givenname, _createModel.Surname, _createModel.Gender,
        //         _createModel.Streetaddress, _createModel.City, _createModel.Zipcode,
        //         _createModel.Country, _createModel.CountryCode, _createModel.Emailaddress,
        //         _createModel.Telephonecountrycode, _createModel.Telephonenumber, _createModel.NationalId,
        //         _createModel.BirthdayYear, _createModel.BirthdayMonth, _createModel.BirthdayDay,
        //         _createModel.DispositionType, _createModel.InitialDeposit, _createModel.Frequency))
        //         .ReturnsAsync((customer, account, disposition));
        //
        //     // Act
        //     var result = await _createModel.OnPostAsync();
        //
        //     // Assert
        //     Assert.IsInstanceOf<RedirectToPageResult>(result);
        //     var redirectResult = result as RedirectToPageResult;
        //     Assert.IsNotNull(redirectResult);
        //     Assert.AreEqual("/CustomersFolder/CustomerDetails", redirectResult.PageName);
        //     Assert.IsTrue(redirectResult.RouteValues.ContainsKey("id"));
        //     Assert.AreEqual(customer.CustomerId, redirectResult.RouteValues["id"]);
        //
        //     _personServiceMock.Verify(x => x.CreateCustomerAsync(
        //     _createModel.Givenname, _createModel.Surname, _createModel.Gender,
        //     _createModel.Streetaddress, _createModel.City, _createModel.Zipcode,
        //     _createModel.Country, _createModel.CountryCode, _createModel.Emailaddress,
        //     _createModel.Telephonecountrycode, _createModel.Telephonenumber, _createModel.NationalId,
        //     _createModel.BirthdayYear, _createModel.BirthdayMonth, _createModel.BirthdayDay,
        //     _createModel.DispositionType, _createModel.InitialDeposit, _createModel.Frequency), Times.Once());
        // }

        [Test]
        public async Task OnPostAsync_WithInvalidModel_ReturnsPageResult()
        {
            // Arrange
            _createModel.ModelState.AddModelError("Error", "Some error");

            // Act
            var result = await _createModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public async Task OnPostAsync_WithException_ReturnsPageResult()
        {
            // Arrange
            _createModel.ModelState.Clear();
            _createModel.Givenname = "John";
            _createModel.Surname = "Doe";
            // Additional properties should be set similarly to the valid model test.

            _personServiceMock.Setup(x => x.CreateCustomerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                              .ThrowsAsync(new Exception("Simulated service failure"));

            // Act
            var result = await _createModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }
    }
}
