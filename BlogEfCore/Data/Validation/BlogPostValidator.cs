using FluentValidation;

namespace BlogEfCore.Data.Validation
{
    public class BlogPostValidator : AbstractValidator<BlogPost>
    {
        public BlogPostValidator()
        {
            RuleFor(b => b.Title).NotEmpty();
            RuleFor(b => b.Author).NotEmpty();
        }
    }
}