using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Telerik.Web.UI;

namespace D_ReportUI
{
    public partial class UserLogins : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ReportRadGrid_NeedDataSource(null, null);
                ReportRadGrid.DataBind();
            }
            else // Not postback
            {
                BeginRadDatePicker.SelectedDate = DateTime.Today;
                EndRadDatePicker.SelectedDate = DateTime.Today;
            }
        }

        protected void ReportRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime begin = BeginRadDatePicker.SelectedDate.Value;
            DateTime end = EndRadDatePicker.SelectedDate.Value;

            // Add one day to end date to make it inclusive
            end = end.AddDays(1);

            string ConnString = ConfigurationManager.ConnectionStrings["sdox_reports_stagingConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnString);
            SqlCommand command = new SqlCommand("SET ARITHABORT ON", conn);
            SqlDataAdapter adapter = new SqlDataAdapter("D_Report_GetUserLogins", conn);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add(new SqlParameter("beginDate", begin));
            adapter.SelectCommand.Parameters.Add(new SqlParameter("endDate", end));

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
