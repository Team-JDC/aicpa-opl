using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Telerik.Web.UI;

namespace D_ReportUI
{
    public partial class FAFLicenseAgreement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ReportRadGrid_NeedDataSource(null, null);
                ReportRadGrid.DataBind();
            }
        }

        protected void ReportRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            string ConnString = ConfigurationManager.ConnectionStrings["sdox_reports_stagingConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnString);
            SqlCommand command = new SqlCommand("SET ARITHABORT ON", conn);
            SqlDataAdapter adapter = new SqlDataAdapter("D_Report_GetLicenseAgreementCountsForFAFUsers", conn);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable myDataTable = new DataTable();
            conn.Open();

            try
            {
                command.ExecuteNonQuery();
                adapter.Fill(myDataTable);
            }
            finally
            {
                conn.Close();
            }

            ReportRadGrid.DataSource = myDataTable;
        }
    }
}