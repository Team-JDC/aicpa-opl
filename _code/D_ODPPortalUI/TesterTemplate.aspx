<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="TesterTemplate.aspx.cs" Inherits="D_ODPPortalUI.TesterTemplate" %>
<%@ Register src="ucTOC.ascx" tagname="ucTOC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContainer" runat="server">
    <uc1:ucTOC ID="ucTOC1" runat="server" />
</asp:Content>
