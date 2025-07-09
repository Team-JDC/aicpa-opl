<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SearchResultsPage.aspx.cs" Inherits="D_ODPPortalUI.SearchResultsPage" %>
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="~/SearchResults.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMainContainer" runat="server">
<uc:SearchResults ID="ucSearchResults" runat="server" />
</asp:Content>
