using Flunt.Validations;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PaymentContext.Domain.Entities
{
    public class Student : Entity
    {
        private IList<Subscription> _subscriptions;

        public Name Name { get; private set; }
        public Document Document { get; private set; }
        public Email Email { get; private set; }

        public IReadOnlyCollection<Subscription> Subscriptions
        {
            get { return _subscriptions.ToArray(); }
        }

        public Address Address { get; private set; }

        public Student(Name name, Document document, Email email)
        {
            Name = name;
            Document = document;
            Email = email;
            _subscriptions = new List<Subscription>();

            AddNotifications(name, document, email);
        }

        public void AddSubscription(Subscription subscription)
        {
            var hasSubscriptionActive = false;

            foreach (var sub in _subscriptions)
            {
                if (sub.Active)
                    hasSubscriptionActive = true;
            }

            AddNotifications(new Contract()
                .Requires()
                .IsFalse(hasSubscriptionActive, "Student.Subscriptions", "You already have a subscription active")
                .AreEquals(0, subscription.Payments.Count, "Student.Subscriptions.Payments", "This subscriptions has no payments yet")
            );
        }
    }
}