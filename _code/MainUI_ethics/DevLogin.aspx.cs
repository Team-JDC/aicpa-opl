using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AICPA.Destroyer.User;
using MainUI.Shared;
using System.Configuration;

namespace MainUI
{
    public partial class DevLogin : System.Web.UI.Page
    {
        private string WEBCONFIG_ALLOW_DEV_LOGIN = "Security_AllowDevLogin";
        private string ALL_BOOKS_SUBSCRIPTION_STRING = "DevLogin_Master_Subscription";

        protected void Page_Load(object sender, EventArgs e)
        {
            string allowDevLoginFlag = ConfigurationManager.AppSettings[WEBCONFIG_ALLOW_DEV_LOGIN];

            if (string.IsNullOrEmpty(allowDevLoginFlag) || allowDevLoginFlag.ToLower() != "true")
            {
                // Development Login is not allowed (as per the web config)
                Response.Redirect("~/SessionExpired.aspx");
            }

            if (!IsPostBack)
            {
                InitUserList();
            }
        }
        
        private const string AICPA_EXAMS_USER = "28BBC090-47CA-4D9D-82C8-7D79FE368FFF";
        private const string AICPA_CEB_USER = "28BBC090-47CA-4D9D-82C8-7D79FE36EEEE";
        private const string AICPA_MGD_USER = "28BBC090-47CA-4D9D-82C8-7D79FE36DDDD";
        private const string AICPA_C2B_USERID1 = "61455B8A-F153-4FD7-991E-4AF6B3C7AAAA";
        private const string AICPA_C2B_USERID3 = "44455B8A-F153-4FD7-991E-4AF6B3C7ABAB";
        private const string AICPA_C2B_USERID4 = "46455B8A-F153-4FD7-991E-4AF6B3C7CDCD";
        private const string AICPA_C2B_USERID2 = "6A03900A-6EE6-45A0-B6C4-09B04747BBBB";
        private const string AICPA_TEST_USERID1 = "61455B8A-F153-4FD7-991E-4AF6B3C781EE";
        private const string AICPA_TEST_USERID2 = "6A03900A-6EE6-45A0-B6C4-09B04747F82F";
        private const string AICPA_TEST_USERID3 = "28BBC090-47CA-4D9D-82C8-7D79FE368538";
        private const string KNOW_EXAMS_USER = "32BBC090-47CA-4D9D-82C8-7D79FE368FFF";
        private const string KNOW_CEB_USER = "32BBC090-47CA-4D9D-82C8-7D79FE36EEEE";
        private const string KNOW_MGD_USER = "32BBC090-47CA-4D9D-82C8-7D79FE36DDDD";
        private const string KNOW_C2B_USERID1 = "32455B8A-F153-4FD7-991E-4AF6B3C7AAAA";
        private const string KNOW_C2B_USERID2 = "2303900A-6EE6-45A0-B6C4-09B04747BBBB";
        private const string KNOW_TEST_USERID1 = "32455B8A-F153-4FD7-991E-4AF6B3C781EE";
        private const string KNOW_TEST_USERID2 = "3203900A-6EE6-45A0-B6C4-09B04747F82F";
        private const string KNOW_TEST_USERID4 = "7806AE4E-A45A-4D1D-AB67-DC5E0CDCD340";
        private const string KNOW_TEST_USERID3 = "NEW_GUID";

        //UAT Testing Accounts All content users 
        private const string AICPA_LW_USER = "7806AE4E-A45A-4D1D-AB67-DC5E0CDCDDDD";
        private const string AICPA_SR_USER = "7806AE4E-A45A-4D1D-AB67-DC5E0CDCAAAA";
        private const string AICPA_CP_USER = "F5A2BEC3-0A67-482E-BB92-724B9948D424";
        private const string AICPA_NP_USER = "7806AE4E-A45A-4D1D-AB67-DC5E0CDCCCCC";
        private const string AICPA_JB_USER = "7806AE4E-A45A-4D1D-AB67-DC5E0CDCFFFF";
        private const string AICPA_PG_USER = "7806AE4E-A45A-4D1D-AB67-DC5E0CDC1111";
        //UAT Testing Accounts Limited Content Access
        private const string AICPA_T1_USER = "071958e6-b683-42e1-bdd0-d5bc5e010000";
        private const string AICPA_T2_USER = "071958e6-b683-42e1-bdd0-d5bc5e011111";
        private const string AICPA_T3_USER = "071958e6-b683-42e1-bdd0-d5bc5e022222";
        private const string AICPA_T4_USER = "071958e6-b683-42e1-bdd0-d5bc5e033333";
        private const string AICPA_T5_USER = "071958e6-b683-42e1-bdd0-d5bc5e044444";
        private const string AICPA_T6_USER = "071958e6-b683-42e1-bdd0-d5bc5e055555";
        private const string AICPA_T7_USER = "071958e6-b683-42e1-bdd0-d5bc5e066666";
        private const string AICPA_T8_USER = "071958e6-b683-42e1-bdd0-d5bc5e077777";
        private const string AICPA_T9_USER = "071958e6-b683-42e1-bdd0-d5bc5e088888";
        private const string AICPA_T10_USER = "071958e6-b683-42e1-bdd0-d5bc5e099999";
        private const string AICPA_T11_USER = "071958e6-b683-42e1-bdd0-d5bc5e0AAAAA";
        private const string AICPA_T12_USER = "071958e6-b683-42e1-bdd0-d5bc5e0BBBBB";


       
        private void InitUserList()
        
        {

            ddlUser.Items.Clear();
            /*ddlUser.Items.Add(new ListItem("AICPA UAT Exams User", AICPA_EXAMS_USER));
            ddlUser.Items.Add(new ListItem("AICPA UAT CEB User", AICPA_CEB_USER));
            ddlUser.Items.Add(new ListItem("AICPA UAT MCGladry User", AICPA_MGD_USER));
            ddlUser.Items.Add(new ListItem("AICPA UAT C2B User (ABM,EMAP,ProflitPlus,Fasb,Gasb)", AICPA_C2B_USERID1));
            ddlUser.Items.Add(new ListItem("AICPA UAT C2B User 2 (ABM,EMAP,ProflitPlus,Fasb,Gasb)", AICPA_C2B_USERID3));
            ddlUser.Items.Add(new ListItem("AICPA UAT C2B User 3 (ABM,EMAP,ProflitPlus,Fasb,Gasb)", AICPA_C2B_USERID4));
            ddlUser.Items.Add(new ListItem("AICPA UAT C2B User (ebp-att,npo-att,pra-rps,pra-ctb,pra-sqcs,pra-aso,pra-ocb)", AICPA_C2B_USERID2));
            ddlUser.Items.Add(new ListItem("AICPA UAT CSC User (ifrs-afr,coso-int,ras-isa,pra-egl,pra-arm,frf-sme,fvs-sew,non-erm,pra-ssae)", AICPA_TEST_USERID1));
            ddlUser.Items.Add(new ListItem("AICPA UAT CSC User (cosocollection,fvscollection,pfp)", AICPA_TEST_USERID2));

            ddlUser.Items.Add(new ListItem("AICPA UAT CSC User (All content)", AICPA_TEST_USERID3));
            ddlUser.Items.Add(new ListItem("KNOW DEV Exams User", KNOW_EXAMS_USER));
            ddlUser.Items.Add(new ListItem("KNOW DEV CEB User", KNOW_CEB_USER));
            ddlUser.Items.Add(new ListItem("KNOW DEV MCGladry User", KNOW_MGD_USER));
            ddlUser.Items.Add(new ListItem("KNOW DEV C2B User (ABM,EMAP,ProflitPlus,Fasb,Gasb)", KNOW_C2B_USERID1));
            ddlUser.Items.Add(new ListItem("KNOW DEV C2B User (ebp-att,npo-att,pra-rps,pra-ctb,pra-sqcs,pra-aso,pra-ocb)", KNOW_C2B_USERID2));
            ddlUser.Items.Add(new ListItem("KNOW DEV CSC User (ifrs-afr,coso-int,ras-isa,pra-egl,pra-arm,frf-sme,fvs-sew,non-erm,pra-ssae)", KNOW_TEST_USERID1));
            ddlUser.Items.Add(new ListItem("KNOW DEV CSC User (cosocollection,fvscollection,pfp)", KNOW_TEST_USERID2));
            ddlUser.Items.Add(new ListItem("KNOW DEV CSC User (All content, persistent)", KNOW_TEST_USERID4));*/
            

            //UAT Specific Accounts
            ddlUser.Items.Add(new ListItem("KNOW DEV CSC User (All content)", KNOW_TEST_USERID3));
            ddlUser.Items.Add(new ListItem("Lois Wolfteich", AICPA_LW_USER));
            ddlUser.Items.Add(new ListItem("Som Roy", AICPA_SR_USER));
            ddlUser.Items.Add(new ListItem("Courtney Paschal", AICPA_CP_USER));
            ddlUser.Items.Add(new ListItem("Nancy Potts", AICPA_NP_USER));
            ddlUser.Items.Add(new ListItem("Jane Booth", AICPA_JB_USER));
            ddlUser.Items.Add(new ListItem("Prati Gupta", AICPA_PG_USER));

            ddlUser.Items.Add(new ListItem("UAT Test User 1", AICPA_T1_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 2", AICPA_T2_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 3", AICPA_T3_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 4", AICPA_T4_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 5", AICPA_T5_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 6", AICPA_T6_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 7", AICPA_T7_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 8", AICPA_T8_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 9", AICPA_T9_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 10", AICPA_T10_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 11", AICPA_T11_USER));
            ddlUser.Items.Add(new ListItem("UAT Test User 12", AICPA_T12_USER));


            
            
       
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string selectedUser = ddlUser.SelectedValue;

            ReferringSite referringSite;
            string domainList;
            
            string Url = "~/default";
            
            switch (selectedUser)
            {
                
                case AICPA_EXAMS_USER:
                case KNOW_EXAMS_USER:
                   referringSite = ReferringSite.Exams;
                   domainList = "exam";
                   break;
                case AICPA_CEB_USER:
                case KNOW_CEB_USER:
                   referringSite = ReferringSite.Ceb;
                   domainList = "proflitplus;fasb";
                   break;
                case AICPA_MGD_USER:
                case KNOW_MGD_USER:
                   referringSite = ReferringSite.Mcgdy;
                   domainList = "mcgladrey";
                   break;
                case AICPA_C2B_USERID1:
                case AICPA_C2B_USERID3:
                case AICPA_C2B_USERID4:
                case KNOW_C2B_USERID1:
                   referringSite = ReferringSite.Csc;
                   domainList = "abm;abm-tk~emap~proflitplus;fasb;gasb";
                   break; 
                case AICPA_C2B_USERID2:
                case KNOW_C2B_USERID2:
                   referringSite = ReferringSite.Csc;
                   domainList = "ebp-att~npo-att~pra-rps~pra-ctb~pra-sqcs~pra-aso~pra-ocb";
                   break;     
                case AICPA_TEST_USERID1:
                case KNOW_TEST_USERID1:
                   referringSite = ReferringSite.Csc;
                   domainList = "ifrs-afr~coso-int~ras-isa~pra-egl~pra-arm~frf-sme~fvs-sew~non-erm~pra-ssae";
                   break;  
                case AICPA_TEST_USERID2:
                case KNOW_TEST_USERID2:
                   referringSite = ReferringSite.Csc;
                   domainList = "cosocollection~fvscollection~pfpcollection";
                   break;
                case AICPA_T1_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "pfpcollection";
                   break;
                case AICPA_T2_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "proflit;fasb;pcaob;gasb";
                   break;
                case AICPA_T3_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "aag;aag-ara;fasb";
                   break;
                case AICPA_T4_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "cosocollection~aag-npo;ara-npo;chk-npo;att-npo";
                   break;
                case AICPA_T5_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "fvscollection";
                   break;
                case AICPA_T6_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "ps;tpa";
                   break;
               case AICPA_T7_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "proflitplus;fasb;gasb";
                   break;
                case AICPA_T8_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "abm~emap";
                   break;
                case AICPA_T9_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "wng~chk-dcp";
                   break;
                case AICPA_T10_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "aag-sla;ara-sga";
                   break;
                case AICPA_T11_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "pcaob~pra-arm";
                   break;
                case AICPA_T12_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "pra-aso~pra-ssae~aam";
                   break;
                case KNOW_TEST_USERID4:
                case AICPA_TEST_USERID3:
                case AICPA_LW_USER:
                case AICPA_SR_USER:
                case AICPA_NP_USER:
                case AICPA_JB_USER:
                case AICPA_PG_USER:
                case KNOW_TEST_USERID3:
                   referringSite = ReferringSite.Csc;
                   domainList = ConfigurationManager.AppSettings[ALL_BOOKS_SUBSCRIPTION_STRING];
                   break;       
                case AICPA_CP_USER:
                   referringSite = ReferringSite.Csc;
                   domainList = "cosocollection~proflitplus;fasb;gasb";
                   break;       

                default:
                    throw new Exception("Unexpected User");
            }

            

            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;


            IUser user;

            if (ddlUser.SelectedValue==KNOW_TEST_USERID3)
            {
                //Create a throw away user with an email we can search for and delete later.
                user = new User(Guid.NewGuid(), referringSite,"deleteme@knowlysis.com");
            }
            else
            {
                user = new User(new Guid(ddlUser.SelectedValue), referringSite);
            }

            
            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;

            Response.Redirect(Url);
        }
    }
}