#region Directives

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using MainUI.Shared;
using System.Xml;
using ASPENCRYPTLib;
using AICPA.Destroyer.User;

#endregion

namespace MainUI.WS
{
    /// <summary>
    /// Summary description for HomePage
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class HomePage : AicpaService
    {
        /// <summary>
        /// GetHomePageData  
        /// </summary>
        /// <returns>HomePageNode</returns>
        [WebMethod(true, Description = "GetHomePageData")]
        
        public HomePageNode GetHomePageData()
        {
            string homePageXmlFile = ContextManager.XmlFile_HomePage;

            XmlDocument doc = new XmlDocument();
            doc.Load(homePageXmlFile);

            HomePageNode root = new HomePageNode(doc.DocumentElement, true);

            root.PruneChildrenBySubscription(CurrentSite);
            root.FillInTargetPointers(CurrentSite);
            root.SetIsFafInMySubscription(CurrentUser);
            root.SetIsWhatsnewInMySubscription(CurrentUser);
            root.SetIsNGToolInMySubscription(CurrentUser);
            

            //var objNew = new object();
            //ICryptoManager objCM = new CryptoManager();
            //ICryptoContext objContext = objCM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 0, objNew);
            //ICryptoKey objKey = objContext.GenerateKeyFromPassword("$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk", CryptoAlgorithms.calgSHA, CryptoAlgorithms.calgRC4, 128);
            //ICryptoBlob objBlob = objKey.EncryptText(CurrentUser.UserId.ToString());
            //string encryptedGuid = objBlob.Hex;
            string encryptedGuid = CurrentUser.UserId.ToString();

            root.UserGuid = encryptedGuid;
            root.Domain = CurrentUser.UserSecurity.Domain;
            

            switch (CurrentUser.ReferringSiteValue)
            {
                case ReferringSite.C2b:
                    root.ReferringSite = "C2B";
                    break;

                case ReferringSite.Ceb:
                    root.ReferringSite = "C2B";
                    break;

                case ReferringSite.Csc:
                    root.ReferringSite = "CSC";
                    break;

                case ReferringSite.Exams:
                    root.ReferringSite = "Exams";
                    break;
            }

            return root;
        }

        /// <summary>
        /// GetHomePageFirstLevelData  
        /// </summary>
        /// <returns>HomePageNode</returns>
        [WebMethod(true, Description = "GetHomePageFirstLevelData")]
        public HomePageNode GetHomePageFirstLevelData()
        {
            string homePageXmlFile = ContextManager.XmlFile_HomePage;

            XmlDocument doc = new XmlDocument();
            doc.Load(homePageXmlFile);

            // we could optimize this, so we don't have to build the whole home page structure out, we just need the first level
            HomePageNode root = new HomePageNode(doc.DocumentElement, true);

            root.PruneChildrenBySubscription(CurrentSite);

            // we only want the first level children
            foreach (HomePageNode child in root.Children)
            {
                child.Children = null;
            }

            root.FillInTargetPointers(CurrentSite);
            root.SetIsFafInMySubscription(CurrentUser);
            root.SetIsWhatsnewInMySubscription(CurrentUser);
            root.SetIsNGToolInMySubscription(CurrentUser);
            


            return root;
        }

        /// <summary>
        /// GetHelpVisibility  
        /// </summary>
        /// <returns>HelpVisibility</returns>
        [WebMethod(true, Description = "GetHelpVisibility")]
        public HelpVisibility GetHelpVisibility()
        {
            HelpVisibility hv = new HelpVisibility();

            if (CurrentUser.ReferringSiteValue == AICPA.Destroyer.User.ReferringSite.Exams)
            {
                hv.ShowContactInfo = false;
            }
            else if (CurrentUser.ReferringSiteValue == AICPA.Destroyer.User.ReferringSite.Ceb)
            {
                hv.ShowContactInfo = false;
            }
            else
            {
                // everyone else should see the contact information
                hv.ShowContactInfo = true;
            }

            return hv;
        }

        [WebMethod(true)]
        public HomePageNode GetRecentDocuments()
        {
            string homePageXmlFile = ContextManager.XmlFile_HomePage;

            XmlDocument doc = new XmlDocument();
            doc.Load(homePageXmlFile);

            HomePageNode root = new HomePageNode(doc.DocumentElement, true);

            root.PruneChildrenBySubscription(CurrentSite);
            root.FillInTargetPointers(CurrentSite);
            root.SetIsFafInMySubscription(CurrentUser);
            //root.getBroadcastPosts(CurrentUser);
            //root.getBookmarks(CurrentUser);
            root.getRecentDocuments(CurrentUser, CurrentSite, 5, false);
            //root.setPreferences(CurrentUser);
            root.DisplayName = CurrentUser.FirstName;

            return root;
        }


    }
}

