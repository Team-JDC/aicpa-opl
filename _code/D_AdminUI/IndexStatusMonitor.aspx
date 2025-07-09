<%@ Page language="c#" Codebehind="IndexStatusMonitor.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI.IndexStatusMonitor" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>IndexStatusMonitor</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script>
			function replaceValues(newValues){

				var element = newValues.split(",");
				var siteInfo,position,status;
				var newValuesToMonitor = "";
				var parentTable = window.parent.document.getElementById("SiteGrid");

				for(a=0;a<element.length;a++)
				{
					siteInfo = element[a].split("|");
					position = parseInt(siteInfo[2]) + 1;
					status = siteInfo[0];
					id = siteInfo[1];

					if(status == "Building"){
						newValuesToMonitor += id + "|" + (position - 1) + ",";
					}
					if(parentTable.rows[position].cells.length < 5){
						position = position + 1;
					}
					parentTable.rows[position].cells[5].innerText = status;
				}

				newValuesToMonitor = newValuesToMonitor.length > 0 ? newValuesToMonitor.substring(0,newValuesToMonitor.length-1) : "";
				window.parent.parent.document.getElementById("indexStatusMonitor").value = newValuesToMonitor;
				window.parent.parent.document.getElementById("gettingStatusFlag").value = "false";

				if(newValuesToMonitor.length > 0 ){
					window.parent.monitoringValues();
				}
				
				return;
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:Label id="displaySites" runat="server">Label</asp:Label>
			<asp:Label id="jsLabel" runat="server" Visible="False">Label</asp:Label>
		</form>
	</body>
</HTML>
