#region

using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using Endeca.Navigation;
using System.Collections.Generic;

#endregion

namespace AICPA.Destroyer.Content.Search
{
    /// <summary>
    ///   Summary description for SearchResults.
    /// </summary>
    public class SearchResults : DestroyerBpc, ISearchResults
    {
        #region Constants

        #endregion Constants

        #region Private Fields

        private readonly ENEQueryResults _endecaQueryResults;
        private readonly ISearchCriteria _searchCriteria;
        private readonly ISite _site;
        private string _activeDimensionsXml;
        private IDocument[] _activeDocuments;
        private string[] _activeWordInterpretations;
        private List<int> _DocumentIDCache;


        /// <summary>
        ///   Gets the active word interpretations.
        /// </summary>
        /// <value>The active word interpretations.</value>
        private string[] ActiveWordInterpretations
        {
            get { return _activeWordInterpretations ?? (_activeWordInterpretations = GetWordInterpretations()); }
        }


        /// <summary>
        ///   Gets the active dimensions XML.
        /// </summary>
        /// <value>The active dimensions XML.</value>
        private string ActiveDimensionsXml
        {
            get { return _activeDimensionsXml ?? (_activeDimensionsXml = GetDimensionsXml(_endecaQueryResults)); }
        }

        /// <summary>
        ///   Gets the active documents.
        /// </summary>
        /// <value>The active documents.</value>
        private IDocument[] ActiveDocuments
        {
            get { return _activeDocuments ?? (_activeDocuments = GetResultDocuments()); }
        }

        #endregion Private Fields

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SearchResults" /> class.
        /// </summary>
        /// <param name = "site">The site.</param>
        /// <param name = "searchCriteria">The search criteria.</param>
        /// <param name = "endecaQueryResults">The endeca query results.</param>
        public SearchResults(ISite site, ISearchCriteria searchCriteria, ENEQueryResults endecaQueryResults)
        {
            //hang on to the site that was searched
            _site = site;

            //hang onto our original search criteria
            _searchCriteria = searchCriteria;

            //hang on to our endeca search results
            _endecaQueryResults = endecaQueryResults;
        }

        #endregion Constructors

        #region ISearchResults Members

        /// <summary>
        ///   The search criteria used to generate the search results.
        /// </summary>
        /// <value></value>
        public ISearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
        }

        /// <summary>
        ///   The total number of hits resulting from the query.
        /// </summary>
        public long TotalHitCount
        {
            get { return _endecaQueryResults.Navigation.TotalNumERecs; }
        }

        /// <summary>
        ///   The total number of hits on the current page (for the last page of search hits this value could be less than ISearchCriteria.PageSize).
        /// </summary>
        public long PageHitCount
        {
            get { return _endecaQueryResults.Navigation.ERecs.Count; }
        }

        /// <summary>
        ///   XML describing the search results returned for the query
        /// </summary>
        public string DimensionsXml
        {
            get { return ActiveDimensionsXml; }
        }

        /// <summary>
        ///   The document objects from the current page of search results
        /// </summary>
        public IDocument[] Documents
        {
            get { return ActiveDocuments; }
        }
		
		/// <summary>
        /// Hold list of Document ID's in the order they were returned
        /// </summary>
        public List<int> DocumentIDCache {
            get
            {
                if (_DocumentIDCache == null)
                    _DocumentIDCache = new List<int>();
                return _DocumentIDCache;
            }
            set { _DocumentIDCache = value; }
        
        }
		
		

        /// <summary>
        ///   The starting hit of the page
        /// </summary>
        public long PageOffset
        {
            get { return _endecaQueryResults.Navigation.ERecsOffset; }
        }

        /// <summary>
        ///   The words that were ultimately used to perform the query (used for hit highlighting).
        /// </summary>
        /// <value></value>
        public string[] WordInterpretations
        {
            get { return ActiveWordInterpretations; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Gets the result documents.
        /// </summary>
        /// <returns></returns>
        private IDocument[] GetResultDocuments()
        {
            //the document array return
            var docs = new ArrayList();            

            //for convenience
            Navigation nav = _endecaQueryResults.Navigation;
            
            //go through each record and populate a document collection
            int recordCount = nav.ERecs.Count;
            for (int i = 0; i < recordCount; i++)
            {
                //our current record
                var eRec = (ERec) nav.ERecs[i];

                //get properties from the record that will help us constuct our document
                var bookName = (string) eRec.Properties[ENDECA_BOOKNAME_PROPERTY];
                var documentName = (string) eRec.Properties[ENDECA_DOCUMENTNAME_PROPERTY];

                //get our document
                IBook book = null;
                IDocument doc = null;
                bool checkUnsubscribed = true;

                //try to get the document out of the site that only returns books to which the user is subscribed
                book = _site.Books[bookName];
                if (book != null)
                {
                    doc = book.Documents[documentName];
                    if (doc == null)
                        doc = book.GetDocumentByAnchorName(documentName);
                    if (doc != null)
                    {
                        checkUnsubscribed = false;
                        doc.InSubscription = true;
                    }
                }

                //if this book was not found in the books to which the user is subscribed, try extending our search for this document to the entire site
                if (checkUnsubscribed)
                {
                    ISite allBookSite = new Site.Site(_site.Id);
                    book = allBookSite.Books[bookName];
                    if (book != null)
                    {
                        doc = book.Documents[documentName];
                        if (doc == null)
                            doc = book.GetDocumentByAnchorName(documentName);
                        if (doc != null)
                        {
                            doc.InSubscription = false;
                        }
                    }
                }

                //if there are document excerpts, add them to the document object, regardless of whether or not the document is part of the user's subscription
                if (SearchCriteria.Excerpts)
                {
                    //get the kwic info from the record
                    var kwic = (string) eRec.Properties[ENDECA_DOCUMENTTEXTSNIPPET_PROPERTY];
                    if (kwic != null)
                    {
                        kwic = PrepareKwic(kwic);
                        if (doc != null)
                        {
                            doc.KeyWordsInContext = kwic;
                        }
                    }
                }

                //add the document to the document arraylist
                if (doc != null)
                    docs.Add(doc);                
            }

            
            

            //return our document array
            return (IDocument[]) docs.ToArray(typeof (IDocument));
        }

        /// <summary>
        ///   Prepares the kwic.
        /// </summary>
        /// <param name = "kwicText">The kwic text.</param>
        /// <returns></returns>
        private string PrepareKwic(string kwicText)
        {
            string fixCharPattern = "([\\(\\)])";
            int count = 0;
            foreach (string word in WordInterpretations)
            {
                string tempWord = Regex.Replace(word, fixCharPattern, "");
                WordInterpretations[count] = tempWord;
                count++;
            }
            string retKwicText = WordInterpretations.Aggregate(kwicText, (current, wordInterp) => Regex.Replace(current, "([^A-Za-z0-9])(" + wordInterp + ")([^A-Za-z0-9])", "$1<b class='endeca_term'>$2</b>$3", RegexOptions.IgnoreCase));

            retKwicText = retKwicText.Replace(ENDECA_KWICTERMMARKUPBEGIN, "");
            retKwicText = retKwicText.Replace(ENDECA_KWICTERMMARKUPEND, "");
            retKwicText = retKwicText.Replace("ï»¿", "");

            return retKwicText;
        }

        /// <summary>
        ///   Gets the dimensions XML.
        /// </summary>
        /// <param name = "endecaQueryResults">The endeca query results.</param>
        /// <returns></returns>
        private string GetDimensionsXml(ENEQueryResults endecaQueryResults)
        {
            //for convenience
            Navigation nav = _endecaQueryResults.Navigation;

            //for our returned search results xml
            var dimensionsXml = new XmlDocument();

            //create a root element
            XmlElement dimensionsEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONS);

            //create an xml element for containing selected dimensions
            XmlElement selectedDimensionsEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSSELECTEDDIMENSIONS);
            foreach (Dimension dimension in nav.DescriptorDimensions)
            {
                string dimensionName = CleanDimensionName(dimension.Descriptor.Name);
                string dimensionId = dimension.Descriptor.Id.ToString();

                XmlElement dimensionEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSION);
                XmlElement dimensionNameEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONNAME);
                dimensionNameEle.InnerText = dimensionName;
                XmlElement dimensionIdEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONID);
                dimensionIdEle.InnerText = dimensionId;
                dimensionEle.AppendChild(dimensionNameEle);
                dimensionEle.AppendChild(dimensionIdEle);

                //add the ancestors to the xml
                XmlElement dimensionAncestorsEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORS);
                foreach (DimVal dimensionAncestor in dimension.Ancestors)
                {
                    string dimensionAncestorName = CleanDimensionName(dimensionAncestor.Name);
                    string dimensionAncestorId = dimensionAncestor.Id.ToString();

                    XmlElement dimensionAncestorEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEM);
                    XmlElement dimensionAncestorNameEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMNAME);
                    dimensionAncestorNameEle.InnerText = dimensionAncestorName;
                    dimensionAncestorEle.AppendChild(dimensionAncestorNameEle);
                    XmlElement dimensionAncestorIdEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMID);
                    dimensionAncestorIdEle.InnerText = dimensionAncestorId;
                    dimensionAncestorEle.AppendChild(dimensionAncestorIdEle);
                    dimensionAncestorsEle.AppendChild(dimensionAncestorEle);
                }

                //add the dimension ancestors to the dimension node
                dimensionEle.AppendChild(dimensionAncestorsEle);

                //add the complete path to the xml
                XmlElement dimensionCompletePathsEle =
                    dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATH);
                foreach (DimVal dimensionCompletePath in dimension.CompletePath)
                {
                    string dimensionCompletePathName = CleanDimensionName(dimensionCompletePath.Name);
                    string dimensionCompletePathId = dimensionCompletePath.Id.ToString();

                    XmlElement dimensionCompletePathEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEM);
                    XmlElement dimensionCompletePathNameEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMNAME);
                    dimensionCompletePathNameEle.InnerText = dimensionCompletePathName;
                    dimensionCompletePathEle.AppendChild(dimensionCompletePathNameEle);
                    XmlElement dimensionCompletePathIdEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMID);
                    dimensionCompletePathIdEle.InnerText = dimensionCompletePathId;
                    dimensionCompletePathEle.AppendChild(dimensionCompletePathIdEle);
                    dimensionCompletePathsEle.AppendChild(dimensionCompletePathEle);
                }

                //add the dimension complete path to the dimension node
                dimensionEle.AppendChild(dimensionCompletePathsEle);

                //add the dimension element to the selected dimensions element
                selectedDimensionsEle.AppendChild(dimensionEle);
            }

            //add the selected dimensions element into the dimensions element
            dimensionsEle.AppendChild(selectedDimensionsEle);

            //create an xml element for containing refinement dimensions
            XmlElement refinementDimensionsEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSREFINEMENTDIMENSIONS);
            foreach (Dimension dimension in nav.RefinementDimensions)
            {
                string dimensionName = CleanDimensionName(dimension.Name);
                string dimensionId = dimension.Id.ToString();

                XmlElement dimensionEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSION);
                XmlElement dimensionNameEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONNAME);
                dimensionNameEle.InnerText = dimensionName;
                XmlElement dimensionIdEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONID);
                dimensionIdEle.InnerText = dimensionId;
                dimensionEle.AppendChild(dimensionNameEle);
                dimensionEle.AppendChild(dimensionIdEle);

                //add in the dimension values (must have ENEQuery.NavAllRefinements set for these to be available)
                foreach (DimVal dimensionValue in dimension.Refinements)
                {
                    string dimensionValName = CleanDimensionName(dimensionValue.Name);
                    string dimensionValId = dimensionValue.Id.ToString();
                    var dimensionValRecCount = (string) dimensionValue.Properties["DGraph.Bins"];

                    //create the main dimension value element
                    XmlElement dimensionValEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONVALUE);

                    //create the name element, insert its value, and append to the main dimension value element
                    XmlElement dimensionValNameEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONVALUENAME);
                    dimensionValNameEle.InnerText = dimensionValName;
                    dimensionValEle.AppendChild(dimensionValNameEle);

                    //create the id element, insert its value, and append to the main dimension value element
                    XmlElement dimensionValIdEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONVALUEID);
                    dimensionValIdEle.InnerText = dimensionValId;
                    dimensionValEle.AppendChild(dimensionValIdEle);

                    //create the record count element, insert its value, and append to the main dimension value element
                    XmlElement dimensionVaRecCountEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONVALUERECORDCOUNT);
                    dimensionVaRecCountEle.InnerText = dimensionValRecCount;
                    dimensionValEle.AppendChild(dimensionVaRecCountEle);

                    //append the main dimension value element to the main dimension element
                    dimensionEle.AppendChild(dimensionValEle);
                }

                //add the ancestors to the xml
                XmlElement dimensionAncestorsEle = dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORS);
                foreach (DimVal dimensionAncestor in dimension.Ancestors)
                {
                    string dimensionAncestorName = CleanDimensionName(dimensionAncestor.Name);
                    string dimensionAncestorId = dimensionAncestor.Id.ToString();

                    XmlElement dimensionAncestorEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEM);
                    XmlElement dimensionAncestorNameEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMNAME);
                    dimensionAncestorNameEle.InnerText = dimensionAncestorName;
                    dimensionAncestorEle.AppendChild(dimensionAncestorNameEle);
                    XmlElement dimensionAncestorIdEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMID);
                    dimensionAncestorIdEle.InnerText = dimensionAncestorId;
                    dimensionAncestorEle.AppendChild(dimensionAncestorIdEle);
                    dimensionAncestorsEle.AppendChild(dimensionAncestorEle);
                }

                //add the dimension ancestors to the dimension node
                dimensionEle.AppendChild(dimensionAncestorsEle);

                //add the complete path to the xml
                XmlElement dimensionCompletePathsEle =
                    dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATH);
                foreach (DimVal dimensionCompletePath in dimension.CompletePath)
                {
                    string dimensionCompletePathName = CleanDimensionName(dimensionCompletePath.Name);
                    string dimensionCompletePathId = dimensionCompletePath.Id.ToString();

                    XmlElement dimensionCompletePathEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEM);
                    XmlElement dimensionCompletePathNameEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMNAME);
                    dimensionCompletePathNameEle.InnerText = dimensionCompletePathName;
                    dimensionCompletePathEle.AppendChild(dimensionCompletePathNameEle);
                    XmlElement dimensionCompletePathIdEle =
                        dimensionsXml.CreateElement(XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMID);
                    dimensionCompletePathIdEle.InnerText = dimensionCompletePathId;
                    dimensionCompletePathEle.AppendChild(dimensionCompletePathIdEle);
                    dimensionCompletePathsEle.AppendChild(dimensionCompletePathEle);
                }

                //add the dimension complete path to the dimension node
                dimensionEle.AppendChild(dimensionCompletePathsEle);

                //add the dimension element to the refinement dimensions element
                refinementDimensionsEle.AppendChild(dimensionEle);
            }

            //add the refinement dimensions element into the dimensions element
            dimensionsEle.AppendChild(refinementDimensionsEle);

            //add the dimensions element to the document
            dimensionsXml.AppendChild(dimensionsEle);

            //set the private string that holds our search results xml
            return dimensionsXml.OuterXml;
        }

        //this helper method is used to clean up dimension names that have a prepended string (the string was needed as a work-around in endeca to force
        // a dimension listing order)
        private string CleanDimensionName(string dimensionName)
        {
            string cleanDimensionName = dimensionName;
            cleanDimensionName = Regex.Replace(cleanDimensionName, @"^[0-9]+" + ENDECA_DESTROYERSITEHIERARCHY_SEPCHAR,
                                               "");
            return cleanDimensionName;
        }


        /// <summary>
        ///   This helper function retrieves the word 
        ///   interpretations from the endeca query results object and returns them as a string array
        /// </summary>
        /// <returns></returns>
        private string[] GetWordInterpretations()
        {
            //the array we will return
            string[] retWordInterps = null;

            //for convenience
            Navigation nav = _endecaQueryResults.Navigation;

            //add the hilite terms to the xml
            var endecaSearchReport = (ESearchReport) nav.ESearchReports[ENDECA_FULLTEXT_FIELD];
            if (endecaSearchReport != null)
            {
                var highlightTermList = new ArrayList();
                if (endecaSearchReport.Terms != null && endecaSearchReport.Terms != EMPTY_STRING)
                {
                    string endecaTerms = endecaSearchReport.Terms;
                    AddEndecaTermsToHighlightList(highlightTermList, endecaTerms);
                }
                PropertyMap wordMap = endecaSearchReport.WordInterps;
                IList props = wordMap.EntrySet;
                foreach (object t in props)
                {
                    var prop = (Property) t;
                    string propKey = prop.Key.ToString();
                    string propVal = prop.Value.ToString();
                    //don't add the property key unless it is not already contained in the array
                    if (!highlightTermList.Contains(propKey))
                    {
                        highlightTermList.Add(propKey);
                    }
                    //don't add the property value unless it is not already contained in the array
                    if (!highlightTermList.Contains(propVal))
                    {
                        highlightTermList.Add(propVal);
                    }
                }

                //remove the boolean operators from the highlight list if search results were from a boolean query
                if (SearchCriteria.SearchType == SearchType.Boolean)
                {
                    string[] removalTerms = {"and", "or", "not"};
                    foreach (string removalTerm in removalTerms)
                    {
                        while (highlightTermList.Contains(removalTerm))
                        {
                            highlightTermList.Remove(removalTerm);
                        }
                    }
                }

                //remove other characters that will throw things off
                string[] removalStrings = {"[", "]", "{", "}"};
                for (int i = 0; i < highlightTermList.Count; i++)
                {
                    var newTerm = (string) highlightTermList[i];
                    newTerm = removalStrings.Aggregate(newTerm, (current, removalString) => current.Replace(removalString, ""));
                    highlightTermList[i] = newTerm;
                }

                //set our arraylist to a regular string array
                retWordInterps = (string[]) highlightTermList.ToArray(typeof (string));
            }
            return retWordInterps;
        }

        /// <summary>
        ///   Helper method to add endeca terms to an arraylist. Care is taken in this method to make sure terms within phrases are not split out but are kept together as a single highlight "term".
        /// </summary>
        /// <param name = "highlightTermList">Ther arraylist to which we should add the terms.</param>
        /// <param name = "endecaTerms">The endeca term string representing terms and phrases we should add to the array.</param>
        private static void AddEndecaTermsToHighlightList(ArrayList highlightTermList, string endecaTerms)
        {
            //special handling if there are quotes (phrases) in the search string
            if (endecaTerms.IndexOf("\"") >= 0)
            {
                string[] phrases = endecaTerms.Split('"');
                foreach (string phrase in phrases)
                {
                    if (phrase.StartsWith(" ") || phrase.EndsWith(" "))
                    {
                        //this case means the element contains regular terms that need to be split up and added to our highlight terms list
                        string[] phraseSplits = phrase.Trim().Split(' ');
                        foreach (string phraseSplit in
                            phraseSplits.Where(phraseSplit => !highlightTermList.Contains(phraseSplit) && phraseSplit != string.Empty))
                        {
                            highlightTermList.Add(phraseSplit);
                        }
                    }
                    else
                    {
                        //this case means the element contains a phrase that should not be split up and can be added to our highlight list directly
                        if (phrase != string.Empty)
                        {
                            highlightTermList.Add(phrase);
                        }
                    }
                }
            }
            else
            {
                string[] terms = endecaTerms.Split(' ');
                foreach (string term in terms)
                {
                    highlightTermList.Add(term);
                }
            }
        }
        #endregion Private Methods
    }
}