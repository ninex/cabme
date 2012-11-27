using cabme.data.Interfaces;

namespace cabme.data.Factories
{
    public class DataContext : IDataContextFactory
    {
        private System.Data.Linq.DataContext mContext;

        #region IDataContextFactory Members

        public System.Data.Linq.DataContext Context
        {
            get
            {
                if (mContext == null)
                {
                    mContext = new contentDataContext();
                }
                return mContext;
            }
        }

        public void SaveAll()
        {
            if (mContext != null)
            {
                mContext.SubmitChanges();
            }
        }

        #endregion
    }
}
