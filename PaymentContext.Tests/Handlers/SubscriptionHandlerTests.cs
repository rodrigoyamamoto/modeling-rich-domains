using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests.Handlers
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                Document = "99999999999",
                Email = "email@gmail.com",

                BarCode = "123456789",
                BoletoNumber = "123456789",

                PaymentNumber = "123121",
                PaidDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddMonths(1),
                Total = 60,
                TotalPaid = 60,
                Payer = "WAYNE CORP",
                PayerDocument = "12345678911",
                PayerDocumentType = EDocumentType.CPF,
                PayerEmail = "batman@dc.com",

                Street = "asdasd",
                Number = "sdaasda",
                Neighborhood = "dasdas",
                City = "sdasd",
                State = "dasdas",
                Country = "sadasd",
                ZipCode = "123456789"
            };

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);
        }
    }
}