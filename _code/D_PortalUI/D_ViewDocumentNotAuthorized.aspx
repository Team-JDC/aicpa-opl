<%@ Page language="c#" Codebehind="D_ViewDocumentNotAuthorized.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.D_ViewDocumentNotAuthorized" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>D_ViewDocumentNotAuthorized</title>
		<script language="javascript">
		function sendToCatalog(fromUrl, toUrl, reqBookName, refSite, userGuid)
		{
			var domain = isCombination(reqBookName);
			
			if (refSite == "C2b")
			{
				var windowPath = window.location.protocol + "//" + "www.cpa2biz.com" + "/standard/redirector.asp?domain=" +  domain + "&returnurl=" + fromUrl +"&linkurl=" + toUrl;
				if(typeof(parent.window.opener)!="undefined")
				{
					if(parent.window.opener.closed == false)
					{
						parent.window.opener.location = windowPath;
						parent.window.opener.focus();
						document.location=("Logout.aspx");
					}
					else
					{
						//var Url = document.location.protocol + "://" +document.location.host + "/autologin/D_C2Bcatalog.asp?Domain=" +  domain + "&Guid=" + userGuid + "&returnurl=" + fromUrl +"&linkurl=" + toUrl;
						var Url = "../autologin/D_C2Bcatalog.asp?Domain=" +  domain + "&Guid=" + userGuid + "&returnurl=" + fromUrl +"&linkurl=" + toUrl;
						//alert(Url);
						window.open(Url,"reSource",width=400, height=200, scrollbars="yes", resizable="yes")
						document.location=("Logout.aspx");	
					}	
				}
				else
				{
					var Url = "../autologin/D_C2Bcatalog.asp?Domain=" +  domain + "&Guid=" + userGuid + "&returnurl=" + fromUrl +"&linkurl=" + toUrl;
					//alert(Url);
					window.open(Url,"reSource",width=400, height=200, scrollbars="yes", resizable="yes")
					document.location=("Logout.aspx");	
				}			
			}
			else
			{
				var Url = "C2Bcatalog.asp?domain=" +  domain + "&Guid=" + userGuid + "&returnurl=" + fromUrl +"&linkurl=" + toUrl;
				window.open(Url,"reSource",width=400, height=200, scrollbars="yes", resizable="yes")
				document.location=("Logout.aspx");		
			}			
		}
		function isCombination(Domain)
		{
			posaag = Domain.indexOf("aag");
			posara = Domain.indexOf("ara");
			posemap = Domain.indexOf("emap");
			if (posemap != -1)
			{
				Domain = "emap";
			}	
			
			if (posaag != -1)
			{
				if (Domain.indexOf("dep") != -1)
				{
					Domain = Domain + ";ara-dep";
				}	
				if (Domain.indexOf("brd") != -1)
				{
					Domain = Domain + ";ara-brd";
				}
				if (Domain.indexOf("con") != -1)
				{
					Domain = Domain + ";ara-con";
				}
				if (Domain.indexOf("ebp") != -1)
				{
					Domain = Domain + ";ara-ebp";
				}
				if (Domain.indexOf("hco") != -1)
				{
					Domain = Domain + ";ara-hco";
				}
				if (Domain.indexOf("inv") != -1)
				{
					Domain = Domain + ";ara-inv";
				}
				if (Domain.indexOf("lhi") != -1)
				{
					Domain = Domain + ";ara-ins";
				}
				if (Domain.indexOf("npo") != -1)
				{
					Domain = Domain + ";ara-npo";
				}
				if (Domain.indexOf("pli") != -1)
				{
					Domain = Domain + ";ara-ins";
				}
				if (Domain.indexOf("slg") != -1)
				{
					Domain = Domain + ";ara-slg";
				}
				if (Domain.indexOf("slv") != -1)
				{
					Domain = Domain + ";ara-slg";
				}
				if (Domain.indexOf("sla") != -1)
				{
					Domain = Domain + ";ara-sga";
				}
				if (Domain.indexOf("cir") != -1)
				{
					Domain = Domain + ";ara-cir";
				}
				
			}
			if (posara != -1)
			{
				if (Domain.indexOf("dep") != -1)
				{
					Domain = "aag-dep;" + Domain;
				}	
				if (Domain.indexOf("brd") != -1)
				{
					Domain = "aag-brd;" + Domain;
				}
				if (Domain.indexOf("con") != -1)
				{
					Domain = "aag-con;" + Domain;
				}
				if (Domain.indexOf("ebp") != -1)
				{
					Domain = "aag-ebp;" + Domain;
				}
				if (Domain.indexOf("hco") != -1)
				{
					Domain = "aag-hco;" + Domain;
				}
				if (Domain.indexOf("inv") != -1)
				{
					Domain = "aag-lhi;" + Domain;
				}
				if (Domain.indexOf("npo") != -1)
				{
					Domain = "aag-npo;" + Domain;
				}
				if (Domain.indexOf("slg") != -1)
				{
					Domain = "aag-slv;" + Domain;
				}
				if (Domain.indexOf("sga") != -1)
				{
					Domain = "aag-sla;" + Domain;
				}
				if (Domain.indexOf("cir") != -1)
				{
					Domain = "aag-cir;" + Domain;
				}
			}

			return Domain;
		}
		
		function exists(x)
		{
			return (typeof(x) != "undefined");
		}
		  
		</script>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" name="vs_targetSchema">
		<LINK href="ASPNETPortal.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body topMargin="0" MS_POSITIONING="FlowLayout">
		<form id="Form1" method="post" runat="server">
			<IMG src="images/portal/icon_warning_24.gif">
			<span class="noAccess">Our apologies...</span>
			<P>You do not have access to the document <b><i>
						<%=requestedDocTitle%>
					</i></b>in the publication <b><i>
						<%=requestedBookTitle%>
					</i></b>as you have requested.</P>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr runat="server" id="purchaseRow1">
					<td colSpan="2" id="instructions">Please do one of the following:</td>
				</tr>
				<tr runat="server" id="purchaseRow2">
					<td colspan="2">
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td valign="middle"><IMG src="images/portal/icon_tip_24.gif">&nbsp;</td>
								<td>
									<ul class="subscription_listings">
										<li>
											Return to your <A href="javascript:history.back();">previous location</A>.
										<li id="purchaseLinkMsg">
											Go to the CPA2Biz store to purchase <i><b>
													<asp:hyperlink id="purchaseLink" Runat="server" on></asp:hyperlink></b></i>.
										</li>
									</ul>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td align="center">
						<div runat="server" id="fasbGasbMsg">
							<p style='FONT-SIZE:12pt;COLOR:darkred;FONT-FAMILY:Arial;TEXT-ALIGN:left'>To obtain 
								access to this <% =fasbGasbLibraryName %> title, you must upgrade your subscription to include the 
								<% =fasbGasbLibraryName %> Library.<BR><BR>
								<B>To upgrade, please call 1-888-777-7077</B> (9am to 6pm ET,Monday-Friday).<BR>
							</p>
							<p style='FONT-SIZE:8pt;COLOR:darkred;FONT-FAMILY:Arial;TEXT-ALIGN:left'>[Please 
								Note: You will be charged for a new 1-year subscription and a credit will be 
								issued for the unused portion of your current subscription.]</p>
							<p style='FONT-SIZE:8pt;COLOR:darkred;FONT-FAMILY:Arial;TEXT-ALIGN:left'>
								Return to your <A href="javascript:history.back();">previous location</A>.
							</p>
						</div>
					</td>
					<td align="right" width="300"><IMG src="images\portal\main-lockBig.gif">
					</td>
				</tr>
			</table>
			<asp:Label ID="jsLabel" Runat="server" Visible="False"></asp:Label>
		</form>
	</body>
</HTML>
