using FluentValidation;

namespace BlogEfCore.Data.Validation
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(a => a.Name).NotEmpty();
        }
    }
}