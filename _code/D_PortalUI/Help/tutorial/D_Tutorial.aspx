<%@ Page language="c#" Codebehind="D_Tutorial.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.Help.tutorial.D_Tutorial" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>AICPA Online Publications</title>
    <link href="../../ASPNETPortal.css" type="text/css" rel="stylesheet">
    <script type="text/javascript" src="../../Scripts/Destroyer.js"></script>  
    <script>
    function searchSend(){
		var txt = document.getElementById("topSearchTxt");
		if(txt.value != "" && txt.value != null){
			window.location.href = "../../DesktopDefault.aspx?tabindex=3&tabid=5&Search="+txt.value;
		}
		return;
    }
    </script>  
  </head>
  <body MS_POSITIONING="FlowLayout"  leftmargin="0" bottommargin="0" rightmargin="0" topmargin="0" marginheight="0" marginwidth="0" onload="init();" onkeypress="pressedKey();">
	
    <form id="Form1" method="post" runat="server">
          <table width="100%" cellspacing="0" cellpadding="0" border="0" width=100%>
                <tr valign="top">
                    <td colspan="2">
<table width="100%" cellspacing="0" cellpadding="0" class="HeadBg" border="0">
	<tr>
		<td rowspan="3" width="10%" valign="middle" align="center">
			<img src="../../images/portal/mainLogo.gif">
		</td>
		<td align=right>
			<a href="../../DesktopDefault.aspx?tabindex=0&tabid=1" class="OtherTabs">Home</a>&nbsp;|&nbsp;<a href="../../DesktopDefault.aspx?tabindex=1&tabid=2" class="OtherTabs">Document</a>&nbsp;|&nbsp;<a href="../../DesktopDefault.aspx?tabindex=2&tabid=4" class="OtherTabs">Quick Find</a>&nbsp;|&nbsp;<a href="../../DesktopDefault.aspx?tabindex=3&tabid=5" class="OtherTabs">Search</a>&nbsp;|&nbsp;<a href="../../DesktopDefault.aspx?tabindex=4&tabid=98" class="OtherTabs">Help</a>&nbsp;|
		</td>
	</tr>
	<tr>
		<td class="bannerText" align="right" height="25">
			<b>Search</b>&nbsp;
			<input type="text" id="topSearchTxt" Class="bannerText" style="width:120px;"></input>
			<img src="../../images/portal/arrow-sm.gif" onclick="searchSend();">&nbsp;
		</td>
	</tr>
	<tr>
		<td height="25" valign="top" align="right" class="bannerText">
			<span id="todaysDate"></span>
		</td>
	</tr>
</table>
                    </td>
                </tr>
                <tr>
                    <td align=center>                        
						<!-- URL's used in the movie-->
						<!-- text used in the movie-->
						<OBJECT id=AICPA_tutorial codeBase=http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0 height=550 width=900 classid=clsid:D27CDB6E-AE6D-11cf-96B8-444553540000 VIEWASTEXT>
							<PARAM NAME="_cx" VALUE="23813">
							<PARAM NAME="_cy" VALUE="14552">
							<PARAM NAME="FlashVars" VALUE="">
							<PARAM NAME="Movie" VALUE="AICPA_tutorial.swf">
							<PARAM NAME="Src" VALUE="AICPA_tutorial.swf">
							<PARAM NAME="WMode" VALUE="Window">
							<PARAM NAME="Play" VALUE="-1">
							<PARAM NAME="Loop" VALUE="-1">
							<PARAM NAME="Quality" VALUE="High">
							<PARAM NAME="SAlign" VALUE="">
							<PARAM NAME="Menu" VALUE="-1">
							<PARAM NAME="Base" VALUE="">
							<PARAM NAME="AllowScriptAccess" VALUE="always">
							<PARAM NAME="Scale" VALUE="ShowAll">
							<PARAM NAME="DeviceFont" VALUE="0">
							<PARAM NAME="EmbedMovie" VALUE="0">
							<PARAM NAME="BGColor" VALUE="FFFFFF">
							<PARAM NAME="SWRemote" VALUE="">
							<PARAM NAME="MovieData" VALUE="">
							<PARAM NAME="SeamlessTabbing" VALUE="1">
							
							<EMBED src="AICPA_tutorial.swf" quality=high bgcolor=#FFFFFF  WIDTH="900" HEIGHT="550" NAME="AICPA_tutorial" ALIGN="" TYPE="application/x-shockwave-flash" PLUGINSPAGE="http://www.macromedia.com/go/getflashplayer">
							</EMBED>
							
						</OBJECT>
                    </td>
                </tr>
            </table>
     </form>
	
  </body>
</html>
