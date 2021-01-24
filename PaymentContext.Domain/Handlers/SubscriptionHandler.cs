using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repository;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Handlers;
using System;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable,
        IHandler<CreateBoletoSubscriptionCommand>,
        IHandler<CreatePayPalSubscriptionCommand>,
        IHandler<CreateCreditCardSubscriptionCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository studentRepository, IEmailService emailService)
        {
            _studentRepository = studentRepository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Subscription register failed");
            }

            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.PaidDate,
                                            command.ExpireDate,
                                            command.Total,
                                            command.TotalPaid,
                                            command.Payer,
                                            new Document(command.PayerDocument, command.PayerDocumentType = EDocumentType.CPF),
                                            address,
                                            email,
                                            command.BarCode,
                                            command.BoletoNumber);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            return new CommandResult(true, "Subscription successful added");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.PaidDate,
                                            command.ExpireDate,
                                            command.Total,
                                            command.TotalPaid,
                                            command.Payer,
                                            new Document(command.PayerDocument, command.PayerDocumentType = EDocumentType.CPF),
                                            address,
                                            email,
                                            command.TransactionCode);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            if (Invalid)
                return new CommandResult(false, "An error occurred creating your subscription");

            _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            return new CommandResult(true, "Subscription successful added");
        }

        public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
        {
            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new CreditCardPayment(command.PaidDate,
                                            command.ExpireDate,
                                            command.Total,
                                            command.TotalPaid,
                                            command.Payer,
                                            new Document(command.PayerDocument, command.PayerDocumentType = EDocumentType.CPF),
                                            address,
                                            email,
                                            command.CardHolderName,
                                            command.CardNumber,
                                            command.LastTransactionNumber);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            return new CommandResult(true, "Subscription successful added");
        }
    }
}