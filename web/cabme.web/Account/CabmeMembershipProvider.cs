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
        private string mProviderName;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CabmeMembershipProvider";
            mProviderName = name;

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
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var user = context.Users.Where(p => p.Name == username).SingleOrDefault();
                if (user == null)
                {
                    return false;
                }
                else
                {
                    bool valid = Hash.ValidatePassword(oldPassword, user.Password);
                    if (valid)
                    {
                        user.Password = Hash.HashPassword(newPassword);
                        user.LastModified = DateTime.Now;
                        context.SubmitChanges();
                    }
                    return valid;
                }
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                try
                {
                    if (context.Users.Where(p => p.Name == username).Count() > 0)
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                        return null;
                    }
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
                    var cabUser = GetUser(user);
                    HttpContext.Current.Cache.Add(cabUser.UserName, cabUser, null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration, FormsAuthentication.Timeout,
                        System.Web.Caching.CacheItemPriority.Default, null);
                    return cabUser;
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
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var user = context.Users.Where(p => p.Name == username).SingleOrDefault();
                if (user != null)
                {
                    //Deletes the record. We may consider in the future to disable as per deleteAllRelatedData
                    context.Users.DeleteOnSubmit(user);
                    context.SubmitChanges();
                    return true;
                }
            }
            return false;
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
            return HttpContext.Current.Cache.Get(username) as CabMeUser;
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
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var user = context.Users.Where(p => p.Name == username).SingleOrDefault();
                if (user == null)
                {
                    return false;
                }
                else
                {
                    bool valid = Hash.ValidatePassword(password, user.Password);
                    if (valid)
                    {
                        user.LastAccess = DateTime.Now;
                        context.SubmitChanges();
                    }
                    var cabUser = GetUser(user);
                    HttpContext.Current.Cache.Add(cabUser.UserName, cabUser, null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration, FormsAuthentication.Timeout,
                        System.Web.Caching.CacheItemPriority.Default, null);
                    return valid;
                }
            }
        }

        public MembershipUser GetUser(Data.User user)
        {
            return new CabMeUser(mProviderName,
                user.Name,
                null,
                user.Email,
                null,
                null,
                true,
                false,
                user.Created,
                user.LastAccess.HasValue ? user.LastAccess.Value : user.Created,
                user.LastAccess.HasValue ? user.LastAccess.Value : user.Created,
                user.LastModified,
                DateTime.MinValue,
                user.PhoneNumber);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
    public class CabMeUser : MembershipUser
    {
        public string PhoneNumber { get; set; }

        public CabMeUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  string PhoneNumber) :
            base(providername, username, providerUserKey, email, passwordQuestion, comment,
                                       isApproved, isLockedOut, creationDate, lastLoginDate,
                                       lastActivityDate, lastPasswordChangedDate, lastLockedOutDate)
        {
            this.PhoneNumber = PhoneNumber;
        }
    }
}