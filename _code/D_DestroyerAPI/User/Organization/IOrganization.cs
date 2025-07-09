using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AICPA.Destroyer.User.Organization
{
    public interface IOrganization
    {
        int Id { get; set; }

        string Name { get; set; }

        /// <summary>
        /// Lookup OrganizationId using the Subscription
        /// </summary>
        /// <param name="sub">Subscription string</param>
        /// <returns>Organization Id</returns>
        int GetOrganizationIdBySub(string sub);

        /// <summary>
        /// Return the subscription name given the Id
        /// </summary>
        /// <param name="id">Id of subscription</param>
        /// <returns>string value of the subscription</returns>
        string GetSubscriptionById(int id);
    }
}
