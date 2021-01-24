using System;
using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repository;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Handlers;

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
            // fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Subscription register failed");
            }

            // verificar se documento ja esta cadastrado
            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            // verificar se email ja esta cadastrado
            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            // gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            // gerar as entidades
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

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // agrupar as validacoes
            AddNotifications(name, document, email, address, student, subscription, payment);

            // salvar as informacoes
            _studentRepository.CreateSubscription(student);

            // enviar email de boas vindas
            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            // retornar informacoes
            return new CommandResult(true, "Subscription successful added");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            // verificar se documento ja esta cadastrado
            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            // verificar se email ja esta cadastrado
            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            // gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            // gerar as entidades
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

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // agrupar as validacoes
            AddNotifications(name, document, email, address, student, subscription, payment);

            // salvar as informacoes
            _studentRepository.CreateSubscription(student);

            // enviar email de boas vindas
            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            // retornar informacoes
            return new CommandResult(true, "Subscription successful added");
        }

        public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
        {
            // verificar se documento ja esta cadastrado
            if (_studentRepository.DocumentExists(command.Document))
                AddNotification("Document", "Document already in use");

            // verificar se email ja esta cadastrado
            if (_studentRepository.EmailExists(command.Email))
                AddNotification("Email", "E-mail already in use");

            // gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            var address = new Address(command.Street, command.Number, command.Neighborhood,
                                      command.City, command.State, command.Country, command.ZipCode);

            // gerar as entidades
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

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // agrupar as validacoes
            AddNotifications(name, document, email, address, student, subscription, payment);

            // salvar as informacoes
            _studentRepository.CreateSubscription(student);

            // enviar email de boas vindas
            _emailService.Send(student.Name.ToString(),
                               student.Email.Address,
                               $"Welcome {student.Name.ToString()}!",
                               "Your subscription has been approved. Go ahead and create your study plan.");

            // retornar informacoes
            return new CommandResult(true, "Subscription successful added");
        }
    }
}