using Microsoft.AspNetCore.Authorization;

namespace AuthECAPI.Controllers
{
    public static class AuthorizationDemoEndpoints
    {
        public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/AdminOnly", AdminOnly);
            
            app.MapGet("/AdminOrTeacher", [Authorize(Roles = "Admin, Teacher")] () =>
            {
                return "Admin or teacher only";
            });

            app.MapGet("/LibraryMembersOnly", [Authorize(Policy = "HasLibraryId")] () =>
            {
                return "Library Members only";
            });

            app.MapGet("/MaternityLeave", [Authorize(Roles = "Teacher", Policy = "FemalesOnly")] () =>
            {
                return "Applied for maternity leave.";
            });

            return app;
        }
        [Authorize(Roles = "Admin")]
        private static string AdminOnly()
        {
            return "Admin Only";
        }
    }
}
