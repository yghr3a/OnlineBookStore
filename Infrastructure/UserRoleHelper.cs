using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Infrastructure
{
    public static class UserRoleHelper
    {
        public static string UserRole2String(UserRole userRole )
        {
            if (userRole == UserRole.Customer)
                return "Customer";

            if (userRole == UserRole.Manager)
                return "Manager";

            return userRole.ToString();
        }

        public static UserRole String2UserRole(string roleName)
        {
            if(roleName == "Customer")
                return UserRole.Customer;

            if(roleName == "Manager")
                return UserRole.Manager;

            return UserRole.None;
        }
    }
}
