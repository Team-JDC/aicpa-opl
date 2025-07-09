#region Imported Namespaces

using AICPA.Destroyer.Shared;
using MainUI.WS;
using NUnit.Framework;

#endregion

namespace MainUITests
{
    ///<summary>
    ///  This is a test class for EndecaServicesTest and is intended
    ///  to contain all EndecaServicesTest Unit Tests
    ///</summary>
    [TestFixture]
    public class EndecaServicesTest
    {
        /// <summary>
        ///   Endecas the navigational search test paging.
        /// </summary>
        [Test]
        public void EndecaNavigationalSearchTestPaging()
        {
            //EndecaServices endecaServices = new EndecaServices();
            //EndecaServices.SearchResultResponse searchResultResponse = new EndecaServices.SearchResultResponse();
            ////Pass All Dimension value.
            //searchResultResponse = endecaServices.EndecaAdvancedSearch("4294946395", "fee", 1, 10, 10, 10, 1, 1);
            //Assert.Greater(searchResultResponse.HitCount, 1);
            //Assert.IsNotNull(searchResultResponse.DimensionResults);
            //Assert.IsNotNull(searchResultResponse.WordIntepretations);
        }

        /// <summary>
        ///   Endecas the navigational search test paging.
        /// </summary>
        [Test]
        public void SetSearchCriteriaTest()
        {
            //EndecaServices endecaServices = new EndecaServices();
            //string[] arrayOfString = {"4294946395"};
            //Assert.IsTrue(endecaServices.SetSearchCriteria(arrayOfString, "fee", SearchType.AnyWords, 10, 10, 10, true,
            //                                               true));
        }

        /// <summary>
        ///   Endecas the navigational search test.
        /// </summary>
        [Test]
        public void EndecaNavigationalSearchTest()
        {
            //EndecaServices endecaServices = new EndecaServices();
            //EndecaServices.SearchResultResponse searchResultResponse = new EndecaServices.SearchResultResponse();
            ////Pass All Dimension value.
            //searchResultResponse = endecaServices.EndecaAdvancedSearch("4294946395", "fee", 1, 10, 10, 0, 1, 1);
            //Assert.Greater(searchResultResponse.HitCount, 1);
            //Assert.IsNotNull(searchResultResponse.DimensionResults);
            //Assert.IsNotNull(searchResultResponse.WordIntepretations);
        }
    }
}