using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AICPA.Destroyer.User.Organization
{
    public static class OrganizationDAL
    {
        public static IList<Organazation> GetAll()
        {
            OrganizationDS.D_OrganizationDataTable table = new OrganizationDSTableAdapters.D_OrganizationTableAdapter().SelectAll();

            return (from row in table
                    select new Organazation
                    {
                        Id = row.Id,
                        Name = row.Name
                    }
                    ).ToList();
        }

        public static int GetOrganizationIdBySub(string subscription)
        {
            OrganizationDS.D_OrganizationDataTable table = new OrganizationDSTableAdapters.D_OrganizationTableAdapter().GetOrganizationIdBySub(subscription);

            if (table.Count > 0)
            {
                int Id = ((OrganizationDS.D_OrganizationRow)table.Rows[0]).Id;
                return Id;
            }
            else
            {
                return -1;
            }                     
        }


        public static string GetSubscriptionById(int id)
        {
            OrganizationDS.D_OrganizationDataTable table = new OrganizationDSTableAdapters.D_OrganizationTableAdapter().GetSubscriptionById(id);

            if (table.Count > 0)
            {                
                return ((OrganizationDS.D_OrganizationRow)table.Rows[0]).Subscription;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
