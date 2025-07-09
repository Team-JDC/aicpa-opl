using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AICPA.Destroyer.User.UserPreferences
{
    public interface IUserPreference
    {
        #region Properties

        int PreferenceId { get; }

        Guid UserId { get; }

        string Key { get; }

        string Value { get; set; }

        #endregion Properties

        #region Methods

        void Save();

        #endregion
    }
}
