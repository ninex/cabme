using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Data = cabme.data;

namespace cabme.web.Account
{
    public class CabmeRoleProvider : RoleProvider
    {
        private string mApplicationName;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CabmeRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Cabme Role Provider");
            }
            base.Initialize(name, config);

            mApplicationName = GetConfigValue(config["applicationName"],
                          System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var list = from user in context.Users
                           join name in usernames on user.Name equals name
                           from role in context.Roles
                           join roleName in roleNames on role.Name equals roleName
                           select new Data.UserRole
                           {
                               UserId = user.Id,
                               RoleId = role.Id
                           };
                context.UserRoles.InsertAllOnSubmit(list);
                context.SubmitChanges();
            }
        }

        public override string ApplicationName
        {
            get { return mApplicationName; }
            set { mApplicationName = value; }
        }

        public override void CreateRole(string roleName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                if (context.Roles.Where(p => p.Name == roleName).Count() <= 0)
                {
                    Data.Role role = new Data.Role()
                    {
                        Name = roleName
                    };
                    context.Roles.InsertOnSubmit(role);
                    context.SubmitChanges();
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var role = context.Roles.Where(p => p.Name == roleName).SingleOrDefault();
                if (role != null)
                {
                    context.Roles.DeleteOnSubmit(role);
                    context.SubmitChanges();
                    return true;
                }                
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return context.Roles.Select(p => p.Name).ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return (from user in context.Users
                        join userRole in context.UserRoles on user.Id equals userRole.UserId
                        join role in context.Roles on userRole.RoleId equals role.Id
                        where user.Name == username
                        select role.Name).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return (from role in context.Roles
                        join userRole in context.UserRoles on role.Id equals userRole.RoleId
                        join user in context.Users on userRole.UserId equals user.Id
                        where role.Name == roleName
                        select user.Name).ToArray();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return (from user in context.Users
                        join userRole in context.UserRoles on user.Id equals userRole.UserId
                        join role in context.Roles on userRole.RoleId equals role.Id
                        where user.Name == username && role.Name == roleName
                        select role
                        ).Count() > 0;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var list = from user in context.Users
                           join name in usernames on user.Name equals name
                           join userRole in context.UserRoles on user.Id equals userRole.UserId
                           join role in context.Roles on userRole.RoleId equals role.Id
                           join roleName in roleNames on role.Name equals roleName
                           select userRole;
                context.UserRoles.DeleteAllOnSubmit(list);
                context.SubmitChanges();
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return context.Roles.Where(p => p.Name == roleName).Count() > 0;
            }
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
}