using ContactList.Data;
using FluentValidation;

namespace ContactList.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(contact => contact.FirstName).NotEmpty();
            RuleFor(contact => contact.LastName).NotEmpty();
            RuleFor(contact => contact.EmailAddress).NotEmpty().EmailAddress();
        }
    }
}