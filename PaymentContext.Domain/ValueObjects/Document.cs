using Flunt.Validations;
using PaymentContext.Domain.Enums;
using PaymentContext.Shared.ValueObjects;

namespace PaymentContext.Domain.ValueObjects
{
    public class Document : ValueObject
    {
        public string Number { get; private set; }
        public EDocumentType Type { get; private set; }

        public Document(string number, EDocumentType type)
        {
            Number = number;
            Type = type;

            AddNotifications(new Contract()
                .Requires()
                .IsTrue(Validate(), "Document.Number", "Document invalid")
            );
        }

        private bool Validate()
        {
            switch (Type)
            {
                // include the rules for validation
                case EDocumentType.CNPJ when Number.Length == 14:
                case EDocumentType.CPF when Number.Length == 11:
                    return true;

                default:
                    return false;
            }
        }
    }
}