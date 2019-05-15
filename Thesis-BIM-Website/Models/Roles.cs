using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class Roles : AuthorizationHandler<Roles>, IAuthorizationRequirement
    {
        public string Role { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Roles requirement)
        {
            var role = context.User.FindFirst(c => c.Type == "Role").Value;

            if (role == requirement.Role)
            {
                context.Succeed(requirement);
            }

            //TODO: Use the following if targeting a version of
            //.NET Framework older than 4.6:
            //      return Task.FromResult(0);
            return Task.CompletedTask;
        }

        public Roles(string role)
        {
            Role = role;
        }
    }
}
