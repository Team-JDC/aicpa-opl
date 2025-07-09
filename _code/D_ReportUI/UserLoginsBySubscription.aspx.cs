using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Telerik.Web.UI;

namespace D_ReportUI
{
    public partial class UserLoginsBySubscription : System.Web.UI.Page
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
            SqlDataAdapter adapter = new SqlDataAdapter("D_Report_GetUserLoginsBySubscription", conn);
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

            ReportRadGrid.DataSource = RegroupResults(myDataTable);
        }

        private List<SubscriptionReport> RegroupResults(DataTable myDataTable)
        {
            List<SubscriptionReport> newResults = new List<SubscriptionReport>();

            foreach (DataRow row in myDataTable.Rows)
            {
                // Get fields from DataRow and populate SubscriptionReport object
                SubscriptionReport subFromData = new SubscriptionReport
                {
                    Year = row.Field<int>("Year"),
                    Month = row.Field<int>("Month"),
                    MonthStr = row.Field<string>("MonthStr"),
                    Day = row.Field<int>("Day"),
                    SubscriptionName = row.Field<string>("SubscriptionName"),
                    Total = row.Field<int>("Total")
                };

                // String split on semi-colons to get each individual subscription
                foreach (string subscriptionToken in subFromData.SubscriptionName.Split(';'))
                {
                    // Create a new SubscriptionReport object based on the original, but replace with new broken out
                    //  subscription string; also replace '~' characters with more readable string " & "
                    SubscriptionReport brokenOutSub = new SubscriptionReport(subFromData);
                    brokenOutSub.SubscriptionName = subscriptionToken.Replace("~", " & ");

                    // See if we already have subscription total for this date and subscription name
                    //  (via Equals method I overrode); "default for reference and nullable types is null" -MSDN docs
                    SubscriptionReport testFind = newResults.FirstOrDefault<SubscriptionReport>(x => x.Equals(brokenOutSub));

                    if (testFind == null)
                    {
                        // Add to our list
                        newResults.Add(brokenOutSub);
                    }
                    else
                    {
                        // Increment existing total
                        testFind.AddToTotal(brokenOutSub.Total);
                    }
                }
            }

            newResults.Sort(new Comparison<SubscriptionReport>(SubscriptionReport.Compare));

            return newResults;
        }
    }

    class SubscriptionReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthStr { get; set; }
        public int Day { get; set; }
        public string SubscriptionName { get; set; }
        public int Total { get; set; }

        public SubscriptionReport() { }
        public SubscriptionReport(SubscriptionReport other)
        {
            Year = other.Year;
            Month = other.Month;
            MonthStr = other.MonthStr;
            Day = other.Day;
            SubscriptionName = other.SubscriptionName;
            Total = other.Total;
        }

        public void AddToTotal(int amount)
        {
            Total += amount;
        }

        public override bool Equals(object obj)
        {
            SubscriptionReport other = (SubscriptionReport)obj;

            return Year == other.Year &&
                Month == other.Month &&
                Day == other.Day &&
                SubscriptionName == other.SubscriptionName;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public static int Compare(SubscriptionReport a, SubscriptionReport b)
        {
            if (a.Year == b.Year)
            {
                if (a.Month == b.Month)
                {
                    if (a.Day == b.Day)
                    {
                        return a.SubscriptionName.CompareTo(b.SubscriptionName);
                    }
                    else
                    {
                        return a.Day.CompareTo(b.Day);
                    }
                }
                else
                {
                    return a.Month.CompareTo(b.Month);
                }
            }
            else
            {
                return a.Year.CompareTo(b.Year);
            }
        }
    }
}
