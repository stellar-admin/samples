using System;
using System.Threading.Tasks;
using BlogEfCore.Data;
using StellarAdmin.Actions;
using StellarAdmin.Context;

namespace BlogEfCore.StellarAdmin.Actions
{
    public class PublishBlogPostAction : FormResourceAction<PublishBlogPostModel>
    {
        private readonly BlogDbContext _dbContext;

        public PublishBlogPostAction(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        protected override async Task<ActionResult> Execute(object[] keys, PublishBlogPostModel model, FormActionRequestContext context)
        {
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    var blogPost = await _dbContext.BlogPosts.FindAsync(Convert.ToInt32(key));
                    if (blogPost != null)
                    {
                        blogPost.PublishDate = model.PublishDate;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            return ActionResult.Success()
                .WithNotification("The blog post was published successfully")
                .WithRefresh();
        }
    }
}