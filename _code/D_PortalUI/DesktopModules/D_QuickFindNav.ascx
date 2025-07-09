<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<%@ Register TagPrefix="radt" Namespace="Telerik.WebControls" Assembly="RadTreeView" %>
<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_QuickFindNav" CodeBehind="D_QuickFindNav.ascx.cs" AutoEventWireup="True" enableViewState="True" %>
<%@ Register TagPrefix="radp" Namespace="Telerik.WebControls" Assembly="RadPanelbar" %>
<script>
	function getDiv(id){
		
		var divId = "_ctl1_div_"+id;
		var imgId = "img_"+id
		
		oDiv = eval("document.getElementById('"+divId+"')");
		oImg = eval("document.getElementById('"+imgId+"')");
		
		//oDiv.style.display = oDiv.style.display == "block" || oDiv.style.display == "" ? "none" : "block";
		oDiv.className = oDiv.className == "divVisibleClass" ? "divHiddenClass" : "divVisibleClass";

		var tmp = oImg.src;
		if(tmp.indexOf("up") > 0){
			oImg.src = "images/portal/topicbar_down.gif";
		}else if(tmp.indexOf("down") > 0){
			oImg.src = "images/portal/topicbar_up.gif";
		}else if(tmp.indexOf("plus") > 0){
			oImg.src = "images/minus.gif";
		}else if(tmp.indexOf("minus") > 0){
			oImg.src = "images/plus.gif";
		}
		return;
	}
	
	function resizeFrame(){
	
	}

	var lastUIdChanged = "";
	function getLink(id,uId){
		if(lastUIdChanged != ""){
			var oldLink = eval("document.getElementById('"+lastUIdChanged+"')");
			oldLink.className = "subElement";
		}
		var oLink = eval("document.getElementById('"+uId+"')");
		oLink.className = "subElementSelected";
		lastUIdChanged = uId;
		var oFrame = document.getElementById("frameHolder");
		oFrame.src = "quickFind_links.aspx?nodeId="+id;
		return;
	}
	
	function resizeQFiframe(){
		var currentfr=document.getElementById('frameHolder');
		
		if (currentfr){
			currentfr.style.display="block";
			
			if (currentfr.contentDocument && currentfr.contentDocument.body.offsetHeight){
				currentfr.style.height = currentfr.contentDocument.body.offsetHeight+FFextraHeight;
			}else if (currentfr.Document && currentfr.Document.body.scrollHeight){
				currentfr.style.height = currentfr.Document.body.scrollHeight;
			}
	
		}
	}	
	
</script>
<table border=0 cellpadding=0 cellspacing=0 width=100%>
<tr>
	<td id="linkHolder" valign=top width="25%" class="taxonomiesTable">
		<!--<div id="blocker" class="under">-->
			<table id="QFTable" cellSpacing="0" cellPadding="0" width="100%" border="0" runat="server">
			</table>
		<!--</div>-->
	</td>
	<td valign=top>
		<iframe id="frameHolder" onload='resizeQFiframe();' src="quickFind_links.aspx?nodeId=quickFindHome" style="height:850px" width=100% height=100% frameborder=0></iframe>
	</td>
</tr>
</table>
<input type="hidden" id="divIds" value="" runat="server">
<script>
	//resizeFrame();
</script>