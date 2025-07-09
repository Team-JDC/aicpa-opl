using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.Content.Search
{
    /// <summary>
    ///   Summary description for SearchCriteria.
    /// </summary>
    public class SearchCriteria : DestroyerBpc, ISearchCriteria
    {
        #region Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAKEYWORDS = "Keywords";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIA = "SearchCriteria";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIASEARCHTYPE = "SearchType";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAMAXHITS = "MaxHits";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAPAGESIZE = "PageSize";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAPAGEOFFSET = "PageOffset";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIASORTORDER = "SortOrder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAEXTRAPARAMS = "ExtraParams";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAEXTRAPARAM = "ExtraParam";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAEXTRAPARAMNAME = "ParamName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAEXTRAPARAMVALUE = "ParamValue";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIADIMENSIONIDS = "DimensionIds";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIADIMENSIONID = "DimensionId";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAEXCERPTS = "Excerpts";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHCRITERIAFILTERUNSUBSCRIBED = "FilterUnsubscribed";

        #endregion Constants

        #region Private Fields

        /// <summary>
        ///   A dalc to use for database management of this dataset
        /// </summary>
        private ISearchDalc _activeSearchDalc;

        /// <summary>
        /// </summary>
        private SearchDs _activeSearchDs;

        private SearchDs.SearchRow _activeSearchRow;

        private SearchDs ActiveSearchDs
        {
            get { return _activeSearchDs ?? (_activeSearchDs = new SearchDs()); }
            set { _activeSearchDs = value; }
        }

        private SearchDs.SearchRow ActiveSearchRow
        {
            get
            {
                return _activeSearchRow ??
                       (_activeSearchRow =
                        ActiveSearchDs.Search.AddSearchRow(Guid.Empty, string.Empty, string.Empty, DateTime.Now));
            }
            set { _activeSearchRow = value; }
        }

        private ISearchDalc ActiveSearchDalc
        {
            get { return _activeSearchDalc ?? (_activeSearchDalc = new SearchDalc()); }
        }

        #endregion Private Fields

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SearchCriteria" /> class.
        /// </summary>
        /// <param name = "dimensionIds">The dimension ids.</param>
        /// <param name = "keywords">The keywords.</param>
        /// <param name = "searchType">Type of the search.</param>
        /// <param name = "maxHits">The max hits.</param>
        /// <param name = "pageSize">Size of the page.</param>
        /// <param name = "pageOffset">The page offset.</param>
        /// <param name = "sortOrder">The sort order.</param>
        /// <param name = "showExcerpts">if set to <c>true</c> [show excerpts].</param>
        /// <param name = "filterUnsubscribed">if set to <c>true</c> [filter unsubscribed].</param>
        /// <param name = "searchParameters">The search parameters.</param>
        public SearchCriteria(string[] dimensionIds, string keywords, SearchType searchType, int maxHits, int pageSize,
                              int pageOffset, string sortOrder, bool showExcerpts, bool filterUnsubscribed,
                              NameValueCollection searchParameters)
        {
            ActiveSearchRow.UserId = Guid.Empty;
            ActiveSearchRow.Name = string.Empty;
            ActiveSearchRow.SearchCriteria = CreateSearchCriteriaXml(dimensionIds, keywords, searchType, maxHits,
                                                                     pageSize, pageOffset, sortOrder, showExcerpts,
                                                                     filterUnsubscribed, searchParameters);
            ActiveSearchRow.LastModifiedDate = DateTime.Now;
        }

        internal SearchCriteria(SearchDs.SearchRow searchRow)
        {
            ActiveSearchRow = searchRow;
            ActiveSearchDs = (SearchDs) ActiveSearchRow.Table.DataSet;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   The search text.
        /// </summary>
        /// <value></value>
        public string Keywords
        {
            get { return GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAKEYWORDS); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAKEYWORDS, value); }
        }

        /// <summary>
        ///   The prefered type of search.
        /// </summary>
        /// <value></value>
        public SearchType SearchType
        {
            get
            {
                return
                    (SearchType)
                    Enum.Parse(typeof (SearchType), GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIASEARCHTYPE), true);
            }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIASEARCHTYPE, value.ToString()); }
        }

        /// <summary>
        ///   The maximum number of hits to return. Send 0 to request the maximum number of hits supported by the search engine.
        /// </summary>
        /// <value></value>
        public int MaxHits
        {
            get { return int.Parse(GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAMAXHITS)); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAMAXHITS, value.ToString()); }
        }

        /// <summary>
        ///   The desired number of hits per page.
        /// </summary>
        /// <value></value>
        public int PageSize
        {
            get { return int.Parse(GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAPAGESIZE)); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAPAGESIZE, value.ToString()); }
        }

        /// <summary>
        ///   The desired sort order.
        /// </summary>
        /// <value></value>
        public string SortOrder
        {
            get { return GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIASORTORDER); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIASORTORDER, value); }
        }

        /// <summary>
        ///   The offset at which to start the hit page, relative to the first overal search hit. A value of zero (0) will start the hit page at the first overall hit.
        /// </summary>
        /// <value></value>
        public int PageOffset
        {
            get { return int.Parse(GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAPAGEOFFSET)); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAPAGEOFFSET, value.ToString()); }
        }

        /// <summary>
        ///   The list of dimension ids to use for narrowing the search results
        /// </summary>
        /// <value></value>
        public string[] DimensionIds
        {
            get { return GetSearchCriteriaDimensionList(); }
            set { SetSearchCriteriaDimensionList(value); }
        }

        /// <summary>
        /// </summary>
        public bool Excerpts
        {
            get { return bool.Parse(GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAEXCERPTS)); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAEXCERPTS, value.ToString()); }
        }

        /// <summary>
        ///   Determines whether or not the search results should include unsubscribed information.
        /// </summary>
        /// <value></value>
        public bool FilterUnsubscribed
        {
            get { return bool.Parse(GetSearchCriteriaXmlValue(XML_ELE_SEARCHCRITERIAFILTERUNSUBSCRIBED)); }
            set { SetSearchCriteriaElementValue(XML_ELE_SEARCHCRITERIAFILTERUNSUBSCRIBED, value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public NameValueCollection SearchParameters
        {
            get { return GetSearchCriteriaExtraXmlParams(); }
            set { SetSearchCriteriaExtraXmlParams(value); }
        }

        /// <summary>
        ///   Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public Guid UserId
        {
            get { return ActiveSearchRow.UserId; }
        }

        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return ActiveSearchRow.Name; }
        }

        /// <summary>
        ///   Gets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified
        {
            get { return ActiveSearchRow.LastModifiedDate; }
        }

        /// <summary>
        ///   Gets the save id.
        /// </summary>
        /// <value>The save id.</value>
        public int SaveId
        {
            get { return ActiveSearchRow.SearchId; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This version of Save is only for first-time save to the database
        /// </summary>
        public void Save(string name, Guid userId)
        {
            if (string.IsNullOrEmpty(name) || userId == Guid.Empty)
            {
                throw new Exception("Cannot pass null nor empty search name and userId in order to persist a search to the database");                
            }

            if (SaveId != 0)
            {
                throw new Exception("This version of the Save method is only for first-time saves to the database");
            }

            ActiveSearchRow.UserId = userId;
            ActiveSearchRow.Name = name;

            ActiveSearchRow.SearchId = ActiveSearchDalc.Save(ActiveSearchRow.SearchId, ActiveSearchRow.Name, ActiveSearchRow.UserId, ActiveSearchRow.SearchCriteria,
                                  ActiveSearchRow.LastModifiedDate);
        }

        /// <summary>
        /// This version of Save is only for renaming an already persisted search; it also saves any other pending changes to the Search Criteria
        /// </summary>
        public void Save(string name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception("Cannot use null nor empty search name");
            }

            if (SaveId == 0)
            {
                throw new Exception("This version of the Save method is only for renaming an already persisted search");
            }

            ActiveSearchRow.Name = name;

            ActiveSearchDalc.Save(ActiveSearchRow.SearchId, ActiveSearchRow.Name, ActiveSearchRow.UserId, ActiveSearchRow.SearchCriteria,
                                  ActiveSearchRow.LastModifiedDate);
        }

        /// <summary>
        /// This version of Save is only for saving pending changes to already persisted searches
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(Name) || UserId == Guid.Empty || SaveId == 0)
            {
                throw new Exception("This version of the Save method is only for saving pending changes to already persisted searches; please use another Save method overload in order to populate the search name, userId, and searchId (saveId)");
            }

            ActiveSearchDalc.Save(ActiveSearchRow.SearchId, ActiveSearchRow.Name, ActiveSearchRow.UserId, ActiveSearchRow.SearchCriteria,
                                  ActiveSearchRow.LastModifiedDate);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return true;
        }

        /// <summary>
        ///   Gets the saved search by id.
        /// </summary>
        /// <param name = "searchId">The search id.</param>
        /// <returns></returns>
        public static ISearchCriteria GetSavedSearchById(int searchId)
        {
            var retrievedSearchRow = new SearchDalc().GetSearch(searchId);
            ISearchCriteria searchCriteria;

            if (retrievedSearchRow != null)
            {
                searchCriteria = new SearchCriteria(retrievedSearchRow);
            }
            else
            {
                searchCriteria = null;
            }

            return searchCriteria;
        }

        /// <summary>
        /// Gets the saved searches for user.	
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IEnumerable<ISearchCriteria> GetSavedSearchesForUser(Guid UserId)
        {
            var retrievedSearchRows = new SearchDalc().GetSearches(UserId);

            return retrievedSearchRows.Select(row => (ISearchCriteria) new SearchCriteria(row));
        }

        public static void DeleteSearchById(int searchId)
        {
            new SearchDalc().DeleteSearch(searchId);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ISearchCriteria)
            {
                ISearchCriteria other = (ISearchCriteria) obj;
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            if (SaveId != 0)
            {
                hashCode = SaveId;
            }

            return hashCode;
        }

        #region IEquatable<ISearchCriteria> Members

        public bool Equals(ISearchCriteria other)
        {
            if (SaveId == 0 && other.SaveId == 0)
            {
                return GetHashCode() == other.GetHashCode();    
            }

            return SaveId == other.SaveId;
        }

        #endregion

        #endregion

        /// <summary>
        ///   Creates an XML representation of our search criteria
        /// </summary>
        /// <param name = "dimensionIds">The dimension ids.</param>
        /// <param name = "keywords">The keywords.</param>
        /// <param name = "searchType">Type of the search.</param>
        /// <param name = "maxHits">The max hits.</param>
        /// <param name = "pageSize">Size of the page.</param>
        /// <param name = "pageOffset">The page offset.</param>
        /// <param name = "sortOrder">The sort order.</param>
        /// <param name = "showExcerpts">if set to <c>true</c> [show excerpts].</param>
        /// <param name = "filterUnsubscribed">if set to <c>true</c> [filter unsubscribed].</param>
        /// <param name = "searchParameters">The search parameters.</param>
        /// <returns></returns>
        private static string CreateSearchCriteriaXml(string[] dimensionIds, string keywords, SearchType searchType,
                                                      int maxHits, int pageSize, int pageOffset, string sortOrder,
                                                      bool showExcerpts, bool filterUnsubscribed,
                                                      NameValueCollection searchParameters)
        {
            string retXml = string.Empty;

            MemoryStream memStream = new MemoryStream();
            XmlTextWriter xmlWriter = new XmlTextWriter(memStream, new UTF8Encoding(false));

            //write the xml declaration
            xmlWriter.WriteStartDocument(true);

            //write the search criteria element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIA);

            //write the dimensionids element
            if (dimensionIds != null)
            {
                xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIADIMENSIONIDS);
                foreach (string dimensionId in dimensionIds)
                {
                    xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIADIMENSIONID);
                    xmlWriter.WriteString(dimensionId);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }

            //write the keywords element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAKEYWORDS);
            xmlWriter.WriteString(keywords);
            xmlWriter.WriteEndElement();

            //write the search type element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIASEARCHTYPE);
            xmlWriter.WriteString(searchType.ToString());
            xmlWriter.WriteEndElement();

            //write the max hits element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAMAXHITS);
            xmlWriter.WriteString(maxHits.ToString());
            xmlWriter.WriteEndElement();

            //write the page size  element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAPAGESIZE);
            xmlWriter.WriteString(pageSize.ToString());
            xmlWriter.WriteEndElement();

            //write the page offset element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAPAGEOFFSET);
            xmlWriter.WriteString(pageOffset.ToString());
            xmlWriter.WriteEndElement();

            //write the sort order element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIASORTORDER);
            xmlWriter.WriteString(sortOrder);
            xmlWriter.WriteEndElement();

            //write the show excerpts element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAEXCERPTS);
            xmlWriter.WriteString(showExcerpts.ToString());
            xmlWriter.WriteEndElement();

            //write the filter unsubscribed element
            xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAFILTERUNSUBSCRIBED);
            xmlWriter.WriteString(filterUnsubscribed.ToString());
            xmlWriter.WriteEndElement();

            //write the extra search parameter element
            if (searchParameters != null)
            {
                xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMS);
                foreach (string key in searchParameters)
                {
                    xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAEXTRAPARAM);
                    xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMNAME);
                    xmlWriter.WriteString(key);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMVALUE);
                    string paramValue = searchParameters[key];
                    xmlWriter.WriteString(paramValue);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }

                //write the end of the extra search parameter element
                xmlWriter.WriteEndElement();
            }

            //write the end of the search criteria element
            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            //get the string and return it
            UTF8Encoding encoding = new UTF8Encoding();
            retXml = encoding.GetString(memStream.ToArray());

            return retXml;
        }

        /// <summary>
        ///   Returns a string value corresponding to the specified a specified name/value pair into the search criteria object's private XML storage
        /// </summary>
        /// <param name = "elementName"></param>
        /// <returns></returns>
        private string GetSearchCriteriaXmlValue(string elementName)
        {
            string retValue = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);
            XmlNode node = xmlDoc.SelectSingleNode("//" + elementName);
            if (node != null)
            {
                retValue = node.InnerText;
            }
            return retValue;
        }

        /// <summary>
        ///   Stores a specified name/value pair into the search criteria object's private XML storage
        /// </summary>
        /// <param name = "elementName"></param>
        /// <param name = "elementValue"></param>
        private void SetSearchCriteriaElementValue(string elementName, string elementValue)
        {
            //load the serch criteria text into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);

            //grab the root node; create it if it doesn't exist
            XmlNode rootNode = xmlDoc.SelectSingleNode("/" + XML_ELE_SEARCHCRITERIA);
            if (rootNode == null)
            {
                XmlElement element = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIA);
                xmlDoc.AppendChild(element);
                rootNode = element;
            }

            //grab the element; create it if it doesn't exist
            XmlNode node = rootNode.SelectSingleNode(elementName);
            if (node == null)
            {
                XmlElement element = xmlDoc.CreateElement(elementName);
                rootNode.AppendChild(element);
                node = element;
            }

            //set the text of the element
            node.InnerText = elementValue;

            //set the xml into the active search row
            ActiveSearchRow.SearchCriteria = xmlDoc.OuterXml;
            ActiveSearchRow.LastModifiedDate = DateTime.Now;
        }

        /// <summary>
        ///   Returns a name value collection from the search criteria object's XML private storage
        /// </summary>
        private NameValueCollection GetSearchCriteriaExtraXmlParams()
        {
            NameValueCollection nvCollection = new NameValueCollection();

            //load the search criteria text into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);

            //locate the extra parameter data and add it into a n/v colleciton
            XmlNodeList nodes = xmlDoc.SelectNodes("//" + XML_ELE_SEARCHCRITERIAEXTRAPARAM);
            foreach (XmlNode node in nodes)
            {
                XmlNode paramNameNode = node.SelectSingleNode(XML_ELE_SEARCHCRITERIAEXTRAPARAMNAME);
                XmlNode paramValueNode = node.SelectSingleNode(XML_ELE_SEARCHCRITERIAEXTRAPARAMVALUE);

                if (paramNameNode != null && paramValueNode != null)
                {
                    nvCollection.Add(paramNameNode.InnerText, paramValueNode.InnerText);
                }
            }

            //return the n/v collection
            return nvCollection;
        }

        private void SetSearchCriteriaDimensionList(string[] idList)
        {
            //load the serch criteria text into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);

            //grab the root node; create it if it doesn't exist
            XmlNode rootNode = xmlDoc.SelectSingleNode("/" + XML_ELE_SEARCHCRITERIA);
            if (rootNode == null)
            {
                XmlElement element = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIA);
                xmlDoc.AppendChild(element);
                rootNode = element;
            }

            //grab the dimensionids element; delete it and then create an empty element
            XmlNode dimensionIdsNode = rootNode.SelectSingleNode(XML_ELE_SEARCHCRITERIADIMENSIONIDS);
            if (dimensionIdsNode != null)
            {
                dimensionIdsNode.ParentNode.RemoveChild(dimensionIdsNode);
            }
            XmlElement dimensionIdsEle = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIADIMENSIONIDS);
            rootNode.AppendChild(dimensionIdsEle);
            dimensionIdsNode = dimensionIdsEle;

            //add the dimension ids to the element
            foreach (string id in idList)
            {
                XmlElement dimensionIdEle = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIADIMENSIONID);
                dimensionIdEle.InnerText = id;
                dimensionIdsNode.AppendChild(dimensionIdEle);
            }

            //set the xml into the active search row
            ActiveSearchRow.SearchCriteria = xmlDoc.OuterXml;
            ActiveSearchRow.LastModifiedDate = DateTime.Now;
        }

        /// <summary>
        ///   Returns an array of strings indicating the dimension ids for the search criteria
        /// </summary>
        private string[] GetSearchCriteriaDimensionList()
        {
            //use an array list for temporarily holding the ids
            ArrayList dimensionList = new ArrayList();

            //load the search criteria text into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);

            //go through the dimension ids and add them to the array list
            XmlNodeList nodes = xmlDoc.SelectNodes("//" + XML_ELE_SEARCHCRITERIADIMENSIONID);
            foreach (XmlNode node in nodes)
            {
                dimensionList.Add(node.InnerText);
            }

            //return as a regular string array
            return (string[]) dimensionList.ToArray(typeof (string));
        }

        /// <summary>
        ///   Stores a name value collection representing extra search criteria parameters into the search criteria object's XML private storage
        /// </summary>
        private void SetSearchCriteriaExtraXmlParams(NameValueCollection nameValueCollection)
        {
            //load the serch criteria text into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ActiveSearchRow.SearchCriteria);

            //grab the root node; create it if it doesn't exist
            XmlNode rootNode = xmlDoc.SelectSingleNode("/" + XML_ELE_SEARCHCRITERIA);
            if (rootNode == null)
            {
                XmlElement element = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIA);
                xmlDoc.AppendChild(element);
                rootNode = element;
            }

            //grab the extra params element; create it if it doesn't exist
            XmlNode extraParamsNode = rootNode.SelectSingleNode(XML_ELE_SEARCHCRITERIAEXTRAPARAMS);
            if (extraParamsNode == null)
            {
                XmlElement element = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMS);
                xmlDoc.AppendChild(element);
                extraParamsNode = element;
            }

            //delete everything under the extra params node
            extraParamsNode.RemoveAll();

            //now add to the node from the provided n/v list
            foreach (string name in nameValueCollection)
            {
                //create the extra parameter element
                XmlElement extraParamElement = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIAEXTRAPARAM);

                //create the extra parameter name element and pop the name in its text
                XmlElement extraParamNameElement = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMNAME);
                extraParamNameElement.InnerText = name;

                //create the extra parameter value element and pop the value in its text
                XmlElement extraParamValueElement = xmlDoc.CreateElement(XML_ELE_SEARCHCRITERIAEXTRAPARAMVALUE);
                extraParamValueElement.InnerText = nameValueCollection[name];

                //add the name and value elements into the extra param element
                extraParamElement.AppendChild(extraParamNameElement);
                extraParamElement.AppendChild(extraParamValueElement);

                //add the extra param element into the extra parameters element
                extraParamsNode.AppendChild(extraParamElement);
            }

            //set the xml into the active search row
            ActiveSearchRow.SearchCriteria = xmlDoc.OuterXml;
            ActiveSearchRow.LastModifiedDate = DateTime.Now;
        }
    }
}