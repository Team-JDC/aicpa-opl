using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace C2BImposter
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://www.cpa2biz.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string GetOnlinePlatformAuthInfo(string EncryptedUserGUID)
        {
            if (EncryptedUserGUID == null || EncryptedUserGUID == string.Empty)
            {
                return "<userinfo><credentials userguid=\"[none]\"/><error><description>UserC2BGUID was null or empty</description></error></userinfo>";
                //return string.Empty;
            }

            // leave the following code.  This is for a specific test case that
            // expects a failure from the c2b web service.  Since this is just an 
            // imitation of the actual web service, we can do this hokey stuff.
            if (EncryptedUserGUID.ToUpper() == "77EBEF60-5767-4E4E-A669-318C7741C08F")
            {
                throw new Exception("Internal Web Server Error");
            }
            /// Initalize the Database Objects
            SqlConnection C2BDB = new System.Data.SqlClient.SqlConnection();
            SqlDataAdapter DA_Subscription = new System.Data.SqlClient.SqlDataAdapter();
            SqlCommand sqlSelectSubscriptionInfo = new System.Data.SqlClient.SqlCommand();

            /// Set the connection string
            string conString = ConfigurationSettings.AppSettings["Connection String"];
            C2BDB.ConnectionString = conString;

            //Set the command Text and connection object for the Subscription Information
            string subscriptionCommand = ConfigurationSettings.AppSettings["Subscription Command Text"];
            subscriptionCommand = subscriptionCommand + "'" + EncryptedUserGUID + "'";
            sqlSelectSubscriptionInfo.CommandText = subscriptionCommand;
            sqlSelectSubscriptionInfo.Connection = C2BDB;

            //Set the command for the subscription Data adapter
            DA_Subscription.SelectCommand = sqlSelectSubscriptionInfo;

            //Fill the DataTable from the adapter
            DataTable subscripDT = new DataTable();
            try
            {
                DA_Subscription.Fill(subscripDT);

                C2BDB.Close();
                string credentials, permissions, firms, resultingxml;

                //Create an XML String to Return
                //string  userCredentials = ""; 
                //StreamWriter userCredentials = new StreamWriter();
                credentials = "<credentials userguid=\"" + EncryptedUserGUID + "\" />";
                //If the user has any subscription
                if (subscripDT.Rows.Count > 0)
                {
                    permissions = "<permissions domains=\"";
                    IEnumerator rowEnum = subscripDT.Rows.GetEnumerator();
                    rowEnum.Reset();
                    rowEnum.MoveNext();
                    DataRow firstRow = (DataRow)rowEnum.Current;
                    rowEnum.Reset();
                    string domain = firstRow[0].ToString();
                    permissions = permissions + domain + "\" actions=\"\"/>";
                    string aca = firstRow[1].ToString();
                    //If the users has atleast one firm subscription
                    if (aca.Length > 1)
                    {
                        //Start the firms element
                        firms = "<firms>";
                        foreach (DataRow subRow in subscripDT.Rows)
                        {
                            string saca = subRow[1].ToString();
                            string scode = subRow[2].ToString();
                            string sconcurrentusers = subRow[3].ToString();
                            firms = firms + "<firm aca=\"" + saca + "\" code=\"" + scode + "\" concurrentusers=\"" + sconcurrentusers + "\"/>";
                        }
                        //end the firms Element
                        firms = firms + "</firms>";
                    }
                    else
                    {	//start and end the firms Element
                        firms = "<firms/>";
                    }

                    //finish the xml string 
                    resultingxml = "<userinfo>" + credentials + permissions + firms + "</userinfo>";
                }
                else
                {
                    resultingxml = "<userinfo>" + credentials + "<error><description>This user does not have an active resource subscription</description></error></userinfo>";
                }

                return resultingxml;
            }
            catch (Exception e)
            {
                return "<userinfo><credentials userguid=\"" + EncryptedUserGUID + "\"/><error><description>" + e.Message + "</description></error></userinfo>";
            }
        }
    }
}
