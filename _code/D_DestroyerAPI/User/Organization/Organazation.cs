using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AICPA.Destroyer.User.Organization
{
    public class Organazation : IOrganization
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Lookup OrganizationId using the Subscription
        /// </summary>
        /// <param name="sub">Subscription string</param>
        /// <returns>Organization Id</returns>
        public int GetOrganizationIdBySub(string sub)
        {
            return OrganizationDAL.GetOrganizationIdBySub(sub);
        }

        /// <summary>
        /// Return the subscription name given the Id
        /// </summary>
        /// <param name="id">Id of subscription</param>
        /// <returns>string value of the subscription</returns>
        public string GetSubscriptionById(int id)
        {
            return OrganizationDAL.GetSubscriptionById(id);
        }
    }
}
