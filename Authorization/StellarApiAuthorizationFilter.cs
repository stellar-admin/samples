using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization
{
    public class StellarApiAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments(new PathString("/stellarapi")))
            {
                var policyProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationPolicyProvider >();
                var policy = await policyProvider.GetPolicyAsync("IsAdministrator");
                if (policy != null)
                {
                    var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
                    var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context.HttpContext);

                    var authorizeResult = await policyEvaluator.AuthorizeAsync(
                        policy, authenticateResult, context.HttpContext, context);
                    
                    if (authorizeResult.Challenged)
                    {
                        context.Result = new ChallengeResult(policy.AuthenticationSchemes.ToArray());
                    }
                    else if (authorizeResult.Forbidden)
                    {
                        context.Result = new ForbidResult(policy.AuthenticationSchemes.ToArray());
                    }
                }
            }
        }
    }
}