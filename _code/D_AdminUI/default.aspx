<%@ Register TagPrefix="DestroyerHeader" TagName="Banner" Src="adminBanner.ascx" %>
<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI._default" %>
<%@ Register TagPrefix="radmp" Namespace="Telerik.WebControls" Assembly="RadMultiPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AICPA Administration Site</title>
		<LINK href="styles.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="scripts/common.js"></script>
		<script>
			function changeContent(tab)
			{
				var oDoc = document.getElementById("containerIframe");

				switch(tab){
					case 1:
							oDoc.src = "ManageSite.aspx";
						break;
					case 2:
							oDoc.src = "ManageBooks.aspx";
						break;
					case 3:
							oDoc.src = "ManageSubscriptions.aspx";
						break;
					case 4:
							oDoc.src = "reporting/reportsMain.aspx";
						break;
				}
				//forcerReload(oDoc.src);
				return(false)
			}
			
			function forcerReload(where){
				//alert('forcing...');
				var who = document.frames["containerIframe"].location.href;
				//alert("who: "+who);
				//alert("where: "+where);
				var compare = who.split("/");
				//alert(compare.length);
				if(compare[compare.length-1] == where){
					//alert('found');
					var o = document.frames["containerIframe"];
					o.location.reload();
				}else{
					//alert('not found');
					setTimeout("forcerReload('"+where+"')",10);
					return;
				}
				
				return;
			};
			
			function init(){
				getDisplaySize();
				getDate();
			}
			
			function getDisplaySize(){

				var oFrameSize = document.getElementById("containerIframe");
				
				switch(screen.height)
				{
					case 600:
						oFrameSize.height = 410;
						break;
					case 768:
						oFrameSize.height = 490;
						break;
					case 864:
						oFrameSize.height = 565;
						break;
					case 1024:
						oFrameSize.height = 660;
						break;
				}

				return;
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" topmargin=0 rightmargin=0 leftmargin=0 onLoad="init()">
		<form id="Memory" method="post" runat="server">
			<input id="indexStatusMonitor" type="hidden" NAME="indexStatusMonitor">
			<input id="gettingStatusFlag" type="hidden" value="false">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td bgcolor="white">
						<DestroyerHeader:Banner runat="server" id="Banner"></DestroyerHeader:Banner>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
