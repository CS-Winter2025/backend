using CourseProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CourseProject.Common
{
    public static class Util
    {
        public static bool HasAccess(HttpContext context, params UserRole[] approvedRoles)
        {            
            if (context.Session.GetString("Role") == null) return false;
            UserRole userRole = (UserRole) Enum.Parse(typeof(UserRole), context.Session.GetString("Role")!);
            foreach(UserRole role in approvedRoles)
            {
                if (userRole == role) return true;
            }
            return false;
        }
    }
}
