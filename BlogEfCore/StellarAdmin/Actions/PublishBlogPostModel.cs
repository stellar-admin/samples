using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace BlogEfCore.StellarAdmin.Actions
{
    public class PublishBlogPostModel
    {
        [Display(Name = "Publication Date")]
        public DateTime PublishDate { get; set; }
    }

    public class PublishBlogPostModelValidator : AbstractValidator<PublishBlogPostModel>
    {
        public PublishBlogPostModelValidator()
        {
            RuleFor(m => m.PublishDate).NotEmpty();
        }
    }
}