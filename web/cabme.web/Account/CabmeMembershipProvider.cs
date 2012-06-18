using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Data = cabme.data;
using System.Collections.Specialized;

namespace cabme.web.Account
{
    public class CabmeMembershipProvider : MembershipProvider
    {
        private string mApplicationName;
        private bool mEnablePasswordReset;
        private bool mEnablePasswordRetrieval = false;
        private bool mRequiresQuestionAndAnswer = false;
        private bool mRequiresUniqueEmail = true;
        private int mMaxInvalidPasswordAttempts;
        private int mPasswordAttemptWindow;
        private int mMinRequiredPasswordLength;
        private int mMinRequiredNonalphanumericCharacters;
        private string mPasswordStrengthRegularExpression;
        private MembershipPasswordFormat mPasswordFormat = MembershipPasswordFormat.Hashed;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CabmeMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Cabme Membership Provider");
            }

            base.Initialize(name, config);

            mApplicationName = GetConfigValue(config["applicationName"],
                          System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            mMaxInvalidPasswordAttempts = Convert.ToInt32(
                          GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            mPasswordAttemptWindow = Convert.ToInt32(
                          GetConfigValue(config["passwordAttemptWindow"], "10"));
            mMinRequiredNonalphanumericCharacters = Convert.ToInt32(
                          GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
            mMinRequiredPasswordLength = Convert.ToInt32(
                          GetConfigValue(config["minRequiredPasswordLength"], "6"));
            mEnablePasswordReset = Convert.ToBoolean(
                          GetConfigValue(config["enablePasswordReset"], "true"));
            mPasswordStrengthRegularExpression = Convert.ToString(
                           GetConfigValue(config["passwordStrengthRegularExpression"], ""));

        }

        public override string ApplicationName
        {
            get { return mApplicationName; }
            set { mApplicationName = value; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (Data.securityDataContext context = new Data.securityDataContext())
            {
                try
                {
                    Data.User user = new Data.User()
                    {
                        Name = username,
                        Created = DateTime.Now,
                        LastModified = DateTime.Now,
                        Email = email,
                        Password = Hash.HashPassword(password)
                    };
                    //TODO: Checks before inserting
                    context.Users.InsertOnSubmit(user);
                    context.SubmitChanges();
                    status = MembershipCreateStatus.Success;
                    //TODO: Return membership user
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    status = MembershipCreateStatus.ProviderError;
                }
            }
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { return mEnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return mEnablePasswordRetrieval; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return mMaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return mMinRequiredNonalphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return mMinRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return mPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return mPasswordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return mPasswordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return mRequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return mRequiresUniqueEmail; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            using (Data.securityDataContext context = new Data.securityDataContext())
            {
                var user = context.Users.Where(p => p.Name == username).SingleOrDefault();
                if (user == null)
                {
                    return false;
                }
                else
                {
                    return Hash.ValidatePassword(password, user.Password);
                }
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