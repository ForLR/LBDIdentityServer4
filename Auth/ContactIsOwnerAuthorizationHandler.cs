
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
    public class ContactIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
           
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
              
                if (!mvcContext.ModelState.IsValid)
                {
                     context.Fail();
                    return Task.CompletedTask;
                }
                var controllerName = mvcContext.ActionDescriptor.RouteValues.FirstOrDefault(x=>x.Key=="controller").Value;
                var actionName = mvcContext.ActionDescriptor.RouteValues.FirstOrDefault(x => x.Key == "action").Value;



            }
           
            if (context.User==null)
            {
                return Task.CompletedTask;
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
