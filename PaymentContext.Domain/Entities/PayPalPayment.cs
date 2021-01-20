using System;

namespace PaymentContext.Domain.Entities
{
    public class PayPalPayment : Payment
    {
        public PayPalPayment(DateTime paidDate,
                             DateTime expireDate,
                             decimal total,
                             decimal totalPaid,
                             string payer,
                             string document,
                             string address,
                             string email,
                             string transactionCode) : base(
                                    paidDate,
                                    expireDate,
                                    total,
                                    totalPaid,
                                    payer,
                                    document,
                                    address,
                                    email)
        {
            TransactionCode = transactionCode;
        }

        public string TransactionCode { get; private set; }
    }
}