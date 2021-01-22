using Flunt.Validations;
using PaymentContext.Shared.ValueObjects;

namespace PaymentContext.Domain.ValueObjects
{
    public class Name : ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            AddNotifications(new Contract()
                .Requires()
                .HasMinLen(FirstName, 3, "Name.FirstName", "Name must have characters minimum")
                .HasMaxLen(FirstName, 40, "Name.FirstName", "Name must have 40 characters maximum ")

                .HasMinLen(LastName, 3, "Name.LastName", "Last Name must have 3 characters minimum")
                .HasMaxLen(LastName, 40, "Name.LastName", "Last Name must have 40 characters maximum")
            );
        }
    }
}