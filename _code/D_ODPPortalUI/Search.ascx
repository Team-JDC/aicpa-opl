<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="D_ODPPortalUI.Search" %>
<div class="searchbar-left">
</div>
<div class="searchbar-mid">
    <p class="copyright float">
        Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights
        Reserved</p>
    <div id="searchForm" class="right30 ">
        <input type="text" class="search float" value="New Search" />
        <ul class="tabs2 float">
            <li><a href="#results">
                <input type="image" src="images/btn-search.gif" class="submit-search float" /></a></li>
            <li><a href="#results">
                <img src="images/btn-results.gif" alt="See search results float" border="0" /></a></li>
            <li><a href="#" onclick="history.go(-1)">
                <img src="images/btn-doc.gif" alt="See search results float" border="0" /></a></li>
        </ul>
        <asp:ImageButton ID="ibtResults" runat="server" OnClick="ibtResults_Click" ImageUrl="~/images/btn-results.gif" />
    </div>
</div>
