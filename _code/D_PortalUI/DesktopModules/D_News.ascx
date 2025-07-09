<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_news" CodeBehind="D_news.ascx.cs" AutoEventWireup="True" enableViewState="True"%>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<%@ Register TagPrefix="radr" Namespace="Telerik.WebControls" Assembly="RadRotator" %>
<asp:label id="removeWhatsNew" Runat="server" Visible="True">
	<div style="WIDTH: 90%" id="wnDiv">
		<div class="newsContainer">
			<div class="newsTop">
				<img src="images/portal/wnTop.gif" alt="" title="" style="BORDER-RIGHT:0px; BORDER-TOP:0px; BORDER-LEFT:0px; BORDER-BOTTOM:0px"	width="300">
			</div>
			<div class="wnewTopBG">
				<div class="newsUpperContent">
					<table border="0" cellpadding="0" cellspacing="0" width="270px" id="whatsNewTable" runat="server" class="whatsNewTable">
					</table>
				</div>
			</div>
			<div>
				<img src="images/portal/wnBottom2.gif" width="300">
			</div>
		</div>
	</div>
</asp:label>
<!-- Store ---><asp:label id="removeStore" Runat="server" Visible="True">
	<div style="WIDTH: 90%">
		<div class="newsTop"><img src="images/portal/storeTop.gif"></div>
		<div class="storeModule">
			<radR:RadRotator id="storeMain" ScrollDirection="Left" SmoothScrollDelay="10" runat="server" ContentFile="xmlFiles/homepage/storeInfo.xml"
				TransitionType="scroll" FrameTimeout="12000" width="257" height="235" AutoPostBack="true">
				<FrameTemplate>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td colspan="2" class='storeTitleTxt' valign="top">
								<b>
									<%# DataBinder.Eval(Container.DataItem, "title") %>
								</b>
							</td>
						</tr>
						<tr>
							<td colspan="2" class='storeTitle2Txt' valign="top" style="border-bottom:1px solid silver;padding-bottom:4px;">
								<%# DataBinder.Eval(Container.DataItem, "title2") %>
							</td>
						</tr>
						<tr>
							<td rowspan="2" valign="top" style="cursor:hand;cursor:pointer;padding-top:8px;">
								<img src='<%# DataBinder.Eval(Container.DataItem, "image") %>' border=1>
							</td>
							<td valign="top" align="left" class="storeTxt" height="155px" style="cursor:hand;cursor:pointer;padding-top:6px;">
								<radR:RadTicker runat="server" ID="Radticker1" NAME="Radticker1" TickSpeed="15">
									<%# DataBinder.Eval(Container.DataItem, "description") %>
								</radR:RadTicker>
								<br>
								<radR:RadTicker runat="server" ID="Radticker2" NAME="Radticker2" TickSpeed="15">
									<%# DataBinder.Eval(Container.DataItem, "description2") %>
								</radR:RadTicker>
							</td>
						</tr>
						<tr>
							<td align="right" class="storeTxt" valign="bottom"><font color="darkgray">Click for 
									Details</font></td>
						</tr>
					</table>
				</FrameTemplate>
			</radR:RadRotator>
		</div>
		<div>
			<img src="images/portal/storeBottom.gif">&nbsp;
		</div>
	</div>
</asp:label>
<!-- Tips and Techniques -->
<div style="WIDTH: 90%">
	<div class="newsTop"><IMG title="" style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; BORDER-BOTTOM: 0px"
			alt="" src="images/portal/ttTop.gif" width="300"></div>
	<div class="module"><radr:radrotator id="RadRotator3" runat="server" FrameTimeout="13000" TransitionType="scroll" ScrollDirection="right"
			height="43px" width="270px">
			<FrameTemplate>
				<span class="tipTitle">
					<li class="diamond">
						<%# DataBinder.Eval(Container.DataItem, "ttImgTop") %>
						<%# DataBinder.Eval(Container.DataItem, "ttItem") %>
						<%# DataBinder.Eval(Container.DataItem, "ttImgBtn") %>
					</li>
				</span>
			</FrameTemplate>
		</radr:radrotator><radr:radrotator id="RadRotator4" runat="server" FrameTimeout="13000" TransitionType="scroll" ScrollDirection="left"
			height="43px" width="274px">
			<FrameTemplate>
				<span class="tipTitle">
					<li class="diamond">
						<%# DataBinder.Eval(Container.DataItem, "ttImgTop") %>
						<%# DataBinder.Eval(Container.DataItem, "ttItem") %>
						<%# DataBinder.Eval(Container.DataItem, "ttImgBtn") %>
					</li>
				</span>
			</FrameTemplate>
		</radr:radrotator></div>
	<div><IMG src="images/portal/ttBottom.gif">
	</div>
</div>
<asp:label id="jsLabel" Runat="server" Visible="False"></asp:label>
