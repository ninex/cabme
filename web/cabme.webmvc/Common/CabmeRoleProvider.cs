using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Data = cabme.data;

namespace cabme.webmvc.Common
{
    public class CabmeRoleProvider : RoleProvider
    {
        private string mApplicationName;
        private Data.Interfaces.IRepository<Data.User> userRepository;
        private Data.Interfaces.IRepository<Data.Role> roleRepository;
        private Data.Interfaces.IRepository<Data.UserRole> userRoleRepository;

        public CabmeRoleProvider()
        {
            this.userRepository = new Data.Repositories.Repository<Data.User>();
            this.roleRepository = new Data.Repositories.Repository<Data.Role>();
            this.userRoleRepository = new Data.Repositories.Repository<Data.UserRole>();
        }

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
            throw new NotImplementedException();
            /*var list = from user in userRepository.All()
                       join name in usernames on user.Name equals name
                       from role in roleRepository.All()
                       join roleName in roleNames on role.Name equals roleName
                       select new Data.UserRole
                       {
                           UserId = user.Id,
                           RoleId = role.Id
                       };
            userRoleRepository.InsertAllOnSubmit(list);
            userRoleRepository.SaveAll();*/
        }

        public override string ApplicationName
        {
            get { return mApplicationName; }
            set { mApplicationName = value; }
        }

        public override void CreateRole(string roleName)
        {
            if (roleRepository.FindAll(p => p.Name == roleName).Count() <= 0)
            {
                Data.Role role = roleRepository.CreateInstance();
                role.Name = roleName;
                roleRepository.SaveAll();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = roleRepository.FindAll(p => p.Name == roleName).SingleOrDefault();
            if (role != null)
            {
                roleRepository.MarkForDeletion(role);
                roleRepository.SaveAll();
                return true;
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return roleRepository.All().Select(p => p.Name).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return (from user in userRepository.All()
                    join userRole in userRoleRepository.All() on user.Id equals userRole.UserId
                    join role in roleRepository.All() on userRole.RoleId equals role.Id
                    where user.Name == username
                    select role.Name).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return (from role in roleRepository.All()
                    join userRole in userRoleRepository.All() on role.Id equals userRole.RoleId
                    join user in userRepository.All() on userRole.UserId equals user.Id
                    where role.Name == roleName
                    select user.Name).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return (from user in userRepository.All()
                    join userRole in userRoleRepository.All() on user.Id equals userRole.UserId
                    join role in roleRepository.All() on userRole.RoleId equals role.Id
                    where user.Name == username && role.Name == roleName
                    select role
                    ).Count() > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            /*var list = from user in userRepository.All()
                       join name in usernames on user.Name equals name
                       join userRole in userRoleRepository.All() on user.Id equals userRole.UserId
                       join role in roleRepository.All() on userRole.RoleId equals role.Id
                       join roleName in roleNames on role.Name equals roleName
                       select userRole;
            userRoleRepository.DeleteAllOnSubmit(list);
            userRoleRepository.SaveAll();*/
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return roleRepository.FindAll(p => p.Name == roleName).Count() > 0;
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
}