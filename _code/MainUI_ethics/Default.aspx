<%@ Page Title="Default" Language="C#" MasterPageFile="~/ODPMaster.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="MainUI.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphLeft" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            loadToc();
            loadWhatsNew();
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphRight" runat="server">
                        <%--<div class="widget">
                            <div class="widget_header">
                                <h3>What's New?</h3>
                            </div>
                            <div class="widget_inner">
                                <ul class="listing">
                                    <li>
                                        <p class="date">3/8/12</p>
                                        <a href="">What's New in Practice Aids and Toolkits</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                    <li>
                                        <p class="date">10/8/10</p>
                                        <a href="">What's New in Audit Risk Alerts</a>
                                    </li>
                                </ul>
                            </div>
                        </div>
--%>

</asp:Content>
