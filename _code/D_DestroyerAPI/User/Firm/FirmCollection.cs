#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    /// <summary>
    ///   Summary description for FirmCollection.
    /// </summary>
    public class FirmCollection : IFirmCollection
    {
        private readonly UserDs.CurrentFirmUsersDataTable currentFirmUsersDataTable;
        private UserDs userDs = new UserDs();

        #region Constructors

        #region Internal Constructors

        ///<summary>
        ///  Create a new firm collection
        ///</summary>
        ///<param name = "currentFirmUsersDataTable">A data table representing the the firms in the collection</param>
        internal FirmCollection(UserDs.CurrentFirmUsersDataTable currentFirmUsersDataTable)
        {
            this.currentFirmUsersDataTable = currentFirmUsersDataTable;
        }

        ///<summary>
        ///  Create a new firm collection
        ///</summary>
        internal FirmCollection()
        {
        }

        #endregion Internal Constructors

        #endregion Constructors

        #region Properties

        #region Private Properties

        ///<summary>
        ///  The firm of the index passed in.
        ///</summary>
        public IFirm this[int index]
        {
            get { return new Firm((UserDs.CurrentFirmUsersRow) currentFirmUsersDataTable.Rows[index]); }
        }

        ///<summary>
        ///  Read-only property returning the number of Firms in this collection.
        ///</summary>
        public int Count
        {
            get
            {
                int retVal = 0;
                if (currentFirmUsersDataTable != null)
                {
                    retVal = currentFirmUsersDataTable.Rows.Count;
                }
                return retVal;
            }
        }

        #endregion Private Properties

        #endregion Properties

        #region Methods

        #region IEnumerable Methods

        ///<summary>
        ///  Gets the enumerator for the collection
        ///</summary>
        ///<returns>An enumerator for firm collecitons</returns>
        public IEnumerator GetEnumerator()
        {
            return new FirmEnumerator(this);
        }

        #endregion IEnumerable Methods

        #endregion Methods

        #region Classes

        #region Private Classes

        private class FirmEnumerator : IEnumerator
        {
            private readonly FirmCollection firmCollection;
            private int index;

            #region Constructors

            #region Public Constructors

            ///<summary>
            ///  Create a new firm enumerator
            ///</summary>
            ///<param name = "firmCollection"></param>
            public FirmEnumerator(FirmCollection firmCollection)
            {
                this.firmCollection = firmCollection;
                Reset();
            }

            #endregion Public Constructors

            #endregion Constructors

            #region Properties

            #region IEnumerator Properties

            ///<summary>
            ///  The current firm.
            ///</summary>
            public object Current
            {
                get { return new Firm((UserDs.CurrentFirmUsersRow) firmCollection.currentFirmUsersDataTable.Rows[index]); }
            }

            #endregion IEnumerator Properties

            #endregion Properties

            #region Methods

            #region IEnumerator Methods

            ///<summary>
            ///  Reset the counter
            ///</summary>
            public void Reset()
            {
                index = -1;
            }

            ///<summary>
            ///  increment to the next item.
            ///</summary>
            ///<returns>A bool indicating if the end of the collection is reached.</returns>
            public bool MoveNext()
            {
                bool retVal = false;
                index++;
                if (firmCollection.currentFirmUsersDataTable != null)
                {
                    retVal = index < firmCollection.currentFirmUsersDataTable.Rows.Count;
                }
                return retVal;
            }

            #endregion IEnumerator Methods

            #endregion Methods
        }

        #endregion Private Classes

        #endregion Classes
    }
}