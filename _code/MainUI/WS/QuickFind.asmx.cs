using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;

namespace MainUI.WS
{
    /// <summary>
    ///   Summary description for QuickFind
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class QuickFind : AicpaService
    {
        protected const string TAXONOMY = "Taxonomy";
        protected const string TAXONOMY_VALUE = "TaxonomyValue";
        protected const string DOCUMENT = "Document";
        protected const string TARGET_DOCUMENT = "targetdoc";
        protected const string TARGET_POINTER = "targetptr";
        protected const string TEXT = "Text";
        protected const string ID = "id";
        protected const string TITLE = "title";
        protected const string ACCESS = "access";
        protected const string IS_LINK = "isLink";

        protected const string DOCUMENT_TYPES = "document_types";
        protected const string SUBJECT_MATTER = "subject_matter";

        [WebMethod(true, Description = "Gets the Quick Find DocumentType and SubjectMatter lists")]
        public QuickFindLists GetQuickFindLists()
        {
            XDocument xmlDoc = XDocument.Load(ContextManager.XmlFile_Taxonomy);

            var documentTypes = from taxonomyValue in xmlDoc.Descendants(TAXONOMY_VALUE)
                                where taxonomyValue.Ancestors(TAXONOMY).Single().Attribute(ID).Value == DOCUMENT_TYPES
                                select new TaxonomyValue
                                           {
                                               Id = taxonomyValue.Attribute(ID).Value,
                                               Title = taxonomyValue.Attribute(TITLE).Value,
                                               //Access = taxonomyValue.Attribute(ACCESS).Value
                                           };

            var subjectMatters = from taxonomyValue in xmlDoc.Descendants(TAXONOMY_VALUE)
                                 where taxonomyValue.Parent.Name == TAXONOMY &&
                                       taxonomyValue.Parent.Attribute(ID).Value == SUBJECT_MATTER
                                 select new TaxonomyValue
                                            {
                                                Id = taxonomyValue.Elements().First().Name == TAXONOMY_VALUE ?
                                                        taxonomyValue.Elements().First().Attribute(ID).Value :
                                                        taxonomyValue.Attribute(ID).Value,
                                                Title = taxonomyValue.Attribute(TITLE).Value,
                                                //Access = taxonomyValue.Attribute(ACCESS).Value,
                                                Children = taxonomyValue.Elements().First().Name == TAXONOMY_VALUE ?
                                                               (from child in taxonomyValue.Elements()
                                                                select new TaxonomyValue
                                                                           {
                                                                               Id = child.Attribute(ID).Value,
                                                                               Title = child.Attribute(TITLE).Value
                                                                               //Access = child.Attribute(ACCESS).Value
                                                                           }).ToList() :
                                                               new List<TaxonomyValue>()
                                            };

            return new QuickFindLists
                       {
                           DocumentTypeList = documentTypes.ToList(),
                           SubjectMatterList = subjectMatters.ToList(),
                       };
        }

        [WebMethod(true, Description = "Gets the Quick Find results for a selected taxonomy value")]
        public QuickFindResults GetQuickFindResults(string selectedTaxonomy)
        {
            XDocument xmlDoc = XDocument.Load(ContextManager.XmlFile_Taxonomy);

            var results = from doc in xmlDoc.Descendants(DOCUMENT)
                          where doc.Parent.Attribute(ID).Value == selectedTaxonomy &&
                            IsInSubscription(doc.Attribute(TARGET_DOCUMENT).Value, CurrentSite)
                          select new QuickFindResult
                                     {
                                         TargetDocument =
                                            (Convert.ToBoolean(doc.Element(TEXT).Attribute(IS_LINK).Value)) ?
                                            doc.Attribute(TARGET_DOCUMENT).Value :
                                            string.Empty,
                                         TargetPointer =
                                            (Convert.ToBoolean(doc.Element(TEXT).Attribute(IS_LINK).Value)) ?
                                            doc.Attribute(TARGET_POINTER).Value :
                                            string.Empty,
                                         Document = doc.Element(TEXT).Value,
                                         //Access = doc.Parent.Attribute(ACCESS).Value
                                     };

            return new QuickFindResults
                       {
                           QuickFindResultList = results.ToList(),
                           SelectedTitle = (from taxonomyValue in xmlDoc.Descendants(TAXONOMY_VALUE)
                                            where taxonomyValue.Attribute(ID).Value == selectedTaxonomy
                                            select taxonomyValue.Attribute(TITLE).Value).SingleOrDefault() ?? string.Empty
                       };
        }

        private static bool IsInSubscription(string targetDoc, AICPA.Destroyer.Content.Site.ISite site)
        {
            bool isInSubscription = true;

            if (!string.IsNullOrEmpty(targetDoc))
            {
                isInSubscription = site.Books[targetDoc] != null;
            }

            return isInSubscription;
        }

        [WebMethod(true, Description = "Test TaxonomyValue IDs are unique")]
        public TestResults TestTaxonomyValueIdsAreDistinct()
        {
            XDocument xmlDoc = XDocument.Load(ContextManager.XmlFile_Taxonomy);

            var results = from doc in xmlDoc.Descendants(TAXONOMY_VALUE)
                          group doc by doc.Attribute(ID).Value
                            into grp
                          select new
                          {
                              Id = grp.Key,
                              Count = grp.Select(x => x.Attribute(ID)).Distinct().Count()
                          };

            return new TestResults
                       {
                           Problems = (from item in results
                                       where item.Count > 1
                                       select string.Format("id: '{0}', count: {1}", item.Id, item.Count)).ToArray()
                       };
        }

        [Serializable]
        public struct TestResults
        {
            public string[] Problems;
        }
    }
    

    [Serializable]
    public struct QuickFindResult
    {
        //public string Access;
        public string Document;
        public string TargetDocument;
        public string TargetPointer;
    }

    [Serializable]
    public struct TaxonomyValue
    {
        //public string Access;
        public string Id;
        public string Title;
        public List<TaxonomyValue> Children;
    }

    [Serializable]
    public struct QuickFindResults
    {
        public List<QuickFindResult> QuickFindResultList;
        public string SelectedTitle;
    }

    [Serializable]
    public struct QuickFindLists
    {
        public List<TaxonomyValue> DocumentTypeList;
        public List<TaxonomyValue> SubjectMatterList;
    }
}