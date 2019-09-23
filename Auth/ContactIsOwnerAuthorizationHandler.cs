
using LBDIdentityServer4.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User.IsInRole("Admin".ToLower()))
            {
                context.Succeed(requirement);
            }

            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
              
                if (!mvcContext.ModelState.IsValid)
                {
                     context.Fail();
                    return Task.CompletedTask;
                }
                var controllerName = mvcContext.ActionDescriptor.RouteValues.FirstOrDefault(x=>x.Key=="controller").Value;
                var actionName = mvcContext.ActionDescriptor.RouteValues.FirstOrDefault(x => x.Key == "action").Value;
                if (!context.User.HasClaim(x=>x.Value==actionName))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                if (!context.User.HasClaim(x=>x.Type!= Constants.ReadOperationName||x.Value!=actionName))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (!actionName.StartsWith(Constants.CreateOperationName)||!context.User.HasClaim(x=>x.Type== Constants.CreateOperationName&&x.Value==actionName))
                {

                }
                if (!actionName.StartsWith(Constants.UpdateOperationName) || !context.User.HasClaim(x => x.Type == Constants.UpdateOperationName && x.Value == actionName))
                {

                }
                if (!actionName.StartsWith(Constants.DeleteOperationName) || !context.User.HasClaim(x => x.Type == Constants.DeleteOperationName && x.Value == actionName))
                {

                }

            }

           


            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
              
                return Task.CompletedTask;
                
            }
           
            return Task.CompletedTask;
        }
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
    }
}
