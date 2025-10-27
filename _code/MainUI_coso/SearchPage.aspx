<%@ Page Title="" Language="C#" MasterPageFile="~/ODPMaster.Master" AutoEventWireup="true" CodeBehind="Tools.aspx.cs" Inherits="MainUI.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphLeft" runat="server">
                    <input type="hidden" id="searchMode" name="searchMode" value="" />
                    <input type="hidden" id="showExcerpts" name="showExcerpts" value="" />                    
                    <input type="hidden" id="showUnsubscribed" name="showUnsubscribed" value="" />
                    <input type="hidden" id="dimensionId" name="dimensionId" value="" />
                    <input type="hidden" id="searchTermsAdvanced" name="searchTermsAdvanced" value="" />
                   

    <script type="text/javascript">
        $().ready(function () {
            function getIntValue(str, defValue) {
                var result = defValue;
                try {
                    result = parseInt(str);
                } catch (err) {
                    result = defValue;
                }
                if (isNaN(result)) {
                    result = defValue;
                }
                return result;                
            }

            $("#main").addClass("search"); // added to update class for outer 
            //            
            //            var routeTargetDoc = '<asp:Literal ID="lTarg22etDoc" runat="server" Text="<..%$RouteValue:targetdoc%>" />';

            //            var query = '<asp:Literal ID="lTa22rgetDoc" runat="server" Text="<..%$Request.QueryString:q%>" />';
            //            var dimensionId = '<asp:Literal ID="lTarget22Doc" runat="server" Text="<...%$Request.QueryString:dimensionId%>" />';
            //doAdvancedNavigationalSearch('', document.getElementById('searchTerms').value, 1, 100, 10, 0, 1, 1,0);
            <%--
            did = dimensionid
            q = keywords
            sm = searchMode
            mh = maxHits
            ps = pageSize
            po = pageOffset
            se = showExcerpts
            su = showUnsubscribed
            na = nonAuthoritative
             --%>
            var did ='<%=Request.QueryString["did"]%>';
            var q = '<%=Request.QueryString["q"]%>';
            var sm = getIntValue('<%=Request.QueryString["sm"]%>', 1);
            var mh = getIntValue('<%=Request.QueryString["mh"]%>', 100);
            var ps = getIntValue('<%=Request.QueryString["ps"]%>', 10);
            var po = getIntValue('<%=Request.QueryString["po"]%>', 0);
            var se = getIntValue('<%=Request.QueryString["se"]%>', 1);
            var su = getIntValue('<%=Request.QueryString["su"]%>', 1);
            var redo = getIntValue('<%=Request.QueryString["redo"]%>', 1)
            $("#searchMode").val(sm);
            $("#showExcerpts").val(se);
            $("#showUnsubscribed").val(su);
            $("#dimensionId").val(did);
            $("#searchTermsAdvanced").val(q);
            <%-- 
//doAdvancedNavigationalSearch(dimensionId, keywords, 
//searchMode, 
//maxHits, 
//pageSize, 
//pageOffset, 
//showExcerpts, 
//showUnsubscribed, 
//nonauthoritative, 
//callback) {            
 --%>       
           
            if (redo == 1) {
                doAdvancedNavigationalSearchInt(did,q,
                   sm, 
                   mh, 
                   ps, 
                   po, 
                   se,  
                   su,
                   0);
            } else if (redo == 0){
                doSearchWithCurrentCriteria();
            }
        });
    </script>
                    
                    <%-- Brandon: min-height:55vh was placed here for testing. Can you update this? --%>
                    <div id="leftcol" class="col-sm-9" style="min-height:55vh">
                        <div id="divLeftColInner" class="leftcol_inner">
                        <p>Loading.</p>
                        <%--
                            <div class="leftcol_header">
                                <p>
                                    Search results for:
                                    <a href="" class="btn">Save this search</a>
                                    <span></span>
                                </p>
                            </div>
                            <div class="leftcol_content">
                                <div class="results_header">
                                    <div class="results_header_inner">
                                        <p class="within">Search Filters: 
                                            <br>
                                            
                                        </p>
                                        <div class="found_docs">
                                            <p data-toggle="collapse"  href="#filters" aria-expanded="false" aria-controls="filters" id="triggerFilters" class="collapsed">
                                                <i class="fa fa-plus" id="expandFilters"></i>
                                                <i class="fa fa-minus" id="collapseFilters"></i> 
                                                Add Filter
                                            </p>
                                            <div id="filters" class="panel-collapse collapse clearfix">
                                                <ul class="col-sm-6">
                                                    <li><a href=""><i class="fa fa-plus"></i> Sole Proprietorships</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> Limited Liability Companies</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> Corporations</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> S Corporations</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> S Corporations</a></li>
                                                </ul>
                                                <ul class="col-sm-6">
                                                    <li><a href=""><i class="fa fa-plus"></i> Business Valuation</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> Investment Vehicles</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> Bankruptcy/Insolvency</a></li>
                                                    <li><a href=""><i class="fa fa-plus"></i> Employee Retirement & Deferred Compensation Plans</a></li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="results">
                                    <p class="total_results"><span>37</span> search results:</p>
                                    <div class="clearfix">
                                        <div class="pages">Page<input type="text" value="1" />of <span>3</span></div>
                                        <div class="prev_next">
                                            <a class="btn prev off" href="">Prev Page</a><a class="btn next" href="">Next Page</a>
                                        </div>
                                    </div>
                                    <hr />
                                    <ol>
                                        <li>
                                            <a class="title" href="">AT Section 601 &mdash; Compliance Attestation</a>
                                            <p class="meta">Path: AICPA Online Professional Library &gt; AICPA Audit &amp; Accounting Literature &gt; PCAOB Standards and Related Rules &gt; PCAOB Standards and Related Rules &gt; Interim Standards &gt; AT STATEMENTS ON STANDARDS FOR ATTESTATION ENGAGEMENTS &gt; AT Section 601 &mdash; Compliance Attestation</p>
                                            <p class="summary">AT Section 601 AT Section 601 <span class="highlight">Compliance</span> Attestation Source: SSAE No. 10. Effective when the subject matter or assertion is as of or for a period ending on or after June 1, 2001. Earlier application is permitted. Introduction and Applicability...</p>
                                        </li>
                                        <li>
                                            <a class="title" href="">AT Section 600 &mdash; Other Types of Reports</a>
                                            <p class="meta">Path: AICPA Online Professional Library &gt; AICPA Audit &amp; Accounting Literature &gt; PCAOB Standards and Related Rules &gt; PCAOB Standards and Related Rules &gt; Interim Standards &gt; U.S. AUDITING &gt; AU Section 600 &mdash; Other Types of Reports</p>
                                            <p class="summary">...reports issued in connection with the following: a. Financial statements that are prepared in conformity with a comprehensive basis of accounting other than generally accepted accounting principles (paragraphs .02 through .10) b.  <span class="highlight">Compliance</span>...</p>
                                        </li>
                                        <li>
                                            <a class="title" href="">AT Section 600 &mdash; Other Types of Reports</a>
                                            <p class="meta">Path: AICPA Online Professional Library &gt; AICPA Audit &amp; Accounting Literature &gt; PCAOB Standards and Related Rules &gt; PCAOB Standards and Related Rules &gt; Interim Standards &gt; U.S. AUDITING &gt; AU Section 600 &mdash; Other Types of Reports</p>
                                            <p class="summary">...reports issued in connection with the following: a. Financial statements that are prepared in conformity with a comprehensive basis of<span class="highlight">Compliance</span> with aspects of contractual agreements or <span class="highlight">regulatory</span> requirements related to audited financial statements (paragraphs .19 through .21)</p>
                                        </li>
                                    </ol>
                                </div>
                                <div class="clearfix">
                                    <div class="prev_next">
                                        <a class="btn prev off" href="">Prev Page</a><a class="btn next" href="">Next Page</a>
                                    </div>
                                </div>
                            </div><!-- .leftcol_content -->

                            --%>
                        </div><!-- .leftcol_inner -->
                    </div>


                    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphRight" runat="server">
                        <div id="searchWidgetId" class="widget"></div>
</asp:Content>

