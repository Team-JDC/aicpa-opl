var waitForWidthChangeFlag = false;
function getSize(){
return;
	var theSize = document.body.scrollHeight;
	window.parent.resizeIframe(theSize);
	return;
}

function showDesc(who,id)
{
	document.getElementById("Id").value = id;
	document.getElementById("EditWho").value = who;
	window.open('descContainer.htm','Description_Edit','status=no,toolbar=no,menubar=no,scrollbars=no,height=200,width=350,top=250,left=300');
	
	return;
}

function showButton(uniqueId, row, tableName)
{
	var tableRow = parseInt(row) + 1;
	var btnStr = "";
	var myTable = eval("document.getElementById('"+tableName+"').rows");	
	var myEditContent = "";

	document.getElementById("Id").value = uniqueId;
	for(t=0;t<myTable[0].cells.length;t++)
	{
		uniqueId = uniqueId.substring(0,uniqueId.length-1);
		uniqueId = uniqueId+t;
		myEditContent = myTable[tableRow].cells[t].innerHTML;
		var txt = myEditContent.indexOf("id=");
		var uId = myEditContent.substring(txt+3,myEditContent.length);
		txt = uId.indexOf(" ");
		uId = uId.substring(0,txt);
		var odis = eval("document.getElementById('"+ uId +"')");
		if(odis != null && odis.type=="text")
		{
			var disabledString = (odis.disabled)?" disabled ":"";		
			btnStr = "<input class='treeView_button' type='button' " + disabledString + " id='editBtn' value='...' OnClick='showDesc(\""+myTable[0].cells[t].innerText+"\",\""+uId+"\");'>";
			tmp = myEditContent.split("> <");
			myEditContent = myEditContent.indexOf("> <") > -1 ? tmp[0] + ">" + btnStr + "<" + tmp[1] : myEditContent + btnStr;
			myTable[tableRow].cells[t].innerHTML = myEditContent;
		}		
	}	
	return;
}

var maxDisplayLength = 200;

function getDescArea(tableName){

	var myTable = eval("document.getElementById('"+tableName+"')");
	var titleName = "";
	var columnsToEvaluate = myTable.rows[0].cells.length-1;
		
	for(a=1;a<myTable.rows.length;a++){
		for(z=0;z<columnsToEvaluate;z++){
			if(!(tableName == "SiteGrid" && z==5) && !(tableName == "DataGridBooks" && z==2)){
				titleName = myTable.rows[0].cells[z].innerText + ":\n";
				if(myTable.rows[a].cells[z]){
					txt = myTable.rows[a].cells[z].innerText;
					reWrite = myTable.rows[a].cells[z].innerHTML;			
					if(reWrite.indexOf("editBtn") == -1){
						reWriteTemp = reWrite.split(">");
						reWrite = reWriteTemp[0];
						txt = txt.indexOf("'") > -1 ? txt.replace("'","\`") : txt;
						reWrite += txt.length > maxDisplayLength ? " title='"+titleName+txt+"'>"+txt.substring(0,maxDisplayLength)+"&nbsp;&nbsp;<b>...</b>" : ">"+txt;
						reWrite += "</SPAN>";
						if(tableName != "DataGridBooks")
							myTable.rows[a].cells[z].innerHTML = reWrite;
						else if(z < 5)
							myTable.rows[a].cells[z].innerHTML = reWrite;
						else if(z == 5)
							myTable.rows[a].cells[z].innerHTML = getFileName(myTable.rows[a].cells[z].innerText);	
						else if(z == 6)
							myTable.rows[a].cells[z].innerHTML = getDateOnly(myTable.rows[a].cells[z].innerText);
						
					}
				}
			}
		}
	}
	waitForWidthChangeFlag = true;
	return;
}

function getFileName(loc){
	var fileName = "";
	var splitArray = loc.split("\\");
	loc = "File Path:\n"+loc;
	var temp = splitArray[splitArray.length-1];
	temp = temp.length > maxDisplayLength ? "... " + temp.substring(temp.length - maxDisplayLength,temp.length) : temp;
	fileName = "<span title='"+loc+"'><b>...</b> \\"+temp+"</span>";
	return fileName;
}

function getDateOnly(pubDate){
	var dateOnly = "";
	dateOnlyArray = pubDate.split(" ");
	pubDate = "Publish Date:\n"+pubDate;
	dateOnly = "<span title='"+pubDate+"'>"+dateOnlyArray[0]+"</span>";
	return dateOnly;
}

//-------------------------- tree functions ------------------------------
function resizeNice(type,id){
	if(!document.getElementById("treeFrame")){
		showTree(globalRow - 2,globalId,globalTreeType);
		return;
	}
	
	var treeHeight = document.getElementById("treeFrame");
	var i = parseInt(treeHeight.height);
	if(type == "open"){
		if(maxHeight > i){
			i = i + 5;
			treeHeight.height = i;
			setTimeout("resizeNice('open')",5);
		}
	}else{
		if(minHeight < i){
			i = i - 5;
			treeHeight.height = i;
			setTimeout("resizeNice('close','"+id+"')",5);
		}else{
			var myTable = document.getElementById("SiteGrid") ? document.getElementById("SiteGrid") : document.getElementById("DataGridBooks");
			myTable.deleteRow(globalRow);
			var oImg = eval("document.getElementById('"+id+"')");
			oImg.src = "images/toc_expand.gif";
		}			
	}
	return;
}

var globalId;
var globalTreeType;

function showTree(row,Id,treeType){
	
	if(!waitForWidthChangeFlag){
		setTimeout("showTree("+row+","+Id+","+treeType+")",5);
		return;
	}
	
	globalId = Id;
	globalTreeType = treeType;
	var myTable = treeType == 0 ? document.getElementById("DataGridBooks") : document.getElementById("SiteGrid");
	var treeFrameSrc = treeType == 0 ? "siteTreeViewer.aspx?Id="+Id+"&treeType=0" : "siteTreeViewer.aspx?Id="+Id+"&treeType=1";
	var oTree = document.getElementById("treeContainer");

	oRow = myTable.insertRow(row+2);
	oRow.insertCell(0);
	
	myTable.rows[row+2].cells[0].colSpan= myTable.rows[0].cells.length;
	var frameHolder = "<table width='100%' border=0 cellpadding=0 cellspacing=0 class='tree_viewer'><tr><td>";
	frameHolder += "<iframe frameborder=0 id='treeFrame' width=100% height=1 src='"+treeFrameSrc+"'></iframe>";
	frameHolder += "</td></tr></table>";
	myTable.rows[row+2].cells[0].innerHTML = frameHolder;
	globalRow = row+2;
	resizeNice("open");
	return;
}
//------------------------------------------------------------------------
function getDate(){

	var dayarray = new Array("Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday");
	var montharray = new Array("January","February","March","April","May","June","July","August","September","October","November","December");
	var mydate = new Date();
	var year = mydate.getYear();
	var day = mydate.getDay();
	var month = mydate.getMonth();
	var daym = mydate.getDate();
	var str = "";
	var oShowDate = document.getElementById("todaysDate");

	if (year < 1000)
		year+=1900;

	if (daym<10)
		daym="0"+daym;
	
	str = dayarray[day]+", "+montharray[month]+" "+daym+", "+year;
	oShowDate.innerHTML = str;
	return;
}
//----------------------------  Div tag for tree action objects -------------------------------------------
		var pageLoadedFlag = false;
		var defaultMenuWidth="150px" //set default menu width.

		var ie5=document.all && !window.opera
		var ns6=document.getElementById


		function iecompattest(){
			return (document.compatMode && document.compatMode.indexOf("CSS")!=-1)? document.documentElement : document.body
		}

		function showmenu(xPos,yPos){

			if(!pageLoadedFlag){
				setTimeout("showmenu("+xPos+","+yPos+")",5);
				return;
			}
			
			if (!document.all&&!document.getElementById)
			return
			clearhidemenu()
			menuobj=ie5? document.all.addNewFolder : document.getElementById("addNewFolder")

			menuobj.style.width=(typeof optWidth!="undefined")? optWidth : defaultMenuWidth
			menuobj.contentwidth=menuobj.offsetWidth
			menuobj.contentheight=menuobj.offsetHeight;

			var eventX=xPos;// - menuobj.contentwidth;
			var eventY=yPos;
			
			var rightedge=ie5? iecompattest().clientWidth-eventX : window.innerWidth-eventX
			var bottomedge=ie5? iecompattest().clientHeight-eventY : window.innerHeight-eventY

			if (rightedge<menuobj.contentwidth && iecompattest().scrollLeft > 0)
				menuobj.style.left=ie5? iecompattest().scrollLeft+eventX-menuobj.contentwidth+"px" : window.pageXOffset+eventX-menuobj.contentwidth+"px"
			else
				menuobj.style.left=ie5? iecompattest().scrollLeft+eventX+"px" : window.pageXOffset+eventX+"px"

			if (bottomedge<menuobj.contentheight)
				menuobj.style.top=ie5? iecompattest().scrollTop+eventY-menuobj.contentheight+"px" : window.pageYOffset+eventY-menuobj.contentheight+"px"
			else
				menuobj.style.top=ie5? iecompattest().scrollTop+eventY+"px" : window.pageYOffset+eventY+"px"

			menuobj.style.visibility="visible"

			return false
			
		}

		function contains_ns6(a, b) {
			while (b.parentNode)
				if ((b = b.parentNode) == a)
					return true;
			return false;
		}

		function hidemenu(){
			if (window.menuobj)
				menuobj.style.visibility="hidden";
		}

		function dynamichide(e){
			if (ie5&&!menuobj.contains(e.toElement))
				hidemenu();
			else if (ns6&&e.currentTarget!= e.relatedTarget&& !contains_ns6(e.currentTarget, e.relatedTarget))
				hidemenu();
		}

		function delayhidemenu(){
			delayhide=setTimeout("hidemenu()",500)
		}

		function clearhidemenu(){
			if (window.delayhide)
				clearTimeout(delayhide);
		}

		function setGlobals(){
			pageLoadedFlag = true;
			return;
		}
//------------------------------------------------------------------------