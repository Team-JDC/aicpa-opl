function toggleSubscriptionBooks(rowId, toggleLinkId)
{
	var SHOW_BOOKS_HTML = "show books";
	var HIDE_BOOKS_HTML = "hide books";
	var oRows = document.getElementsByTagName("tr");

	for(var i=0; i<oRows.length; i++)
	{
		if(oRows[i].id.indexOf(rowId) >= 0)
		{
			oRows[i].style.display = (oRows[i].style.display == "block")?"none":"block";
		}
	}
	
	var oAnchor = document.getElementById(toggleLinkId)
	oAnchor.innerHTML = (oAnchor.innerHTML == SHOW_BOOKS_HTML?HIDE_BOOKS_HTML:SHOW_BOOKS_HTML);
}

function sendToTutorial(){
	window.location.href = "Help/tutorial/D_Tutorial.aspx";
	return;
}

function docframe_onload(frame, bookList, bookDelim, fallbackSuffix)
{
	reScrollDocument();

	//adjust the frame hieght
	frame.frameElement.height = frame.document.body.scrollHeight + 100;

	//delete trailing semicolon from book list
	if(bookList.lastIndexOf(bookDelim) == bookList.length -1)
	{
		bookList = bookList.substring(0, bookList.length - 1);
	}

	//split book list into array
	var bookNames = bookList.split(bookDelim);
	
	//go through each image in the document
	var oImages = frame.document.getElementsByTagName("img");
	for(var i=0; i<oImages.length; i++)
	{
		var oImage = oImages[i];
		var imageClassName = oImage.className;
		var foundIt = false;
		for(var j=0;j<bookNames.length; j++)
		{
			var bookName = bookNames[j];
			if(bookName.indexOf(fallbackSuffix) > 0)
			{
				bookName = bookName.substring(0, bookName.indexOf(fallbackSuffix));
			}
			
			if(bookName == imageClassName)
			{
				foundIt = true;
				break;
			}
		}
		if(!foundIt)
		{
			oImage.style.display = "inline";
		}
	}
}

function scrollToNamedAnchor(anchorName)
{
	window.location = '#' + anchorName;
}

function syncTocToPath(tocSyncPath)
{
	if(tocSyncPath != "")
	{
		window.location = setQuery("t_sp", tocSyncPath);
	}
}

function getFormat(formatId)
{
	window.location = "D_DownloadDocument.aspx?d_ft=" + formatId;
}

function setWindowDocument()
{
	window.location = "D_Document.aspx";
}

function setWindowRefLink()
{
	window.location = "D_RefLink.aspx";
}
function setWindowArchive()
{
	window.location = "D_Archive.aspx";
}
				

/* query string routines */
function setQuery(key, val)
{
    var l = window.location + '';
    var q = window.location.search + '';
    var f = false;
    var x = 0;
    var r = '';
    var c;
    var p;
    if (q.length == 0)
    {
        r = l + '?' + escape(key) + '=' + escape(val);
    }
    else  {
        l = l.substr(0, l.indexOf('?'));
        c = q.substr(1).split('&');
        r = l + '?';
        for (x = 0; x < c.length; x++)
        {
            p = c[x].split('=');
            if (p[0] == key)
            {
                p[1] = val;
                f = true;
            }
            r += escape(p[0]) + '=' + escape(p[1]) + '&';
        }
        if (!f)r += escape(key) + '=' + escape(val);
    }
    if (r.substr(r.length - 1) == '&')r = r.substr(0, r.length - 1);
    return r;
}

function parseQuery(str)
{
    var field;
    var x;
    str = str ? str : location.search;
    if (rightChar(str) == '&')str = str.substr(0, str.length - 1);
    var query = str.charAt(0) == '?' ? str.substring(1) : str;
    var args = new Object();
    if (query) {
        var fields = query.split('&');
        for (x = 0; x < fields.length; x++)
        {
            field = fields[x].split('=');
            args[unescape(field[0].replace(/\+/g, ' '))] = unescape(field[1].replace(/\+/g, ' '));
        }
    }
    return args;
}

function getQuery(key)
{
    var args = parseQuery();
    return args[key];
}


function showDesc(id)
{
    var imgName = "img_"+ id;
    var rowid = "desc_" + id; 
    
    var row = document.getElementById(rowid);
    var img = document.getElementById(imgName);
    if (row.className.indexOf("hide")  >  -1)
	{
	      newClassName = "showBookDesc";
          newImgSrc = "images/topicbar_up.gif";
    }
    else
    {
       newClassName = "hideBookDesc";
       newImgSrc = "images/topicbar_down.gif";
    }
    
    row.className = newClassName;
    img.src = newImgSrc; 
}

function showPubs(rowid)
{
    var imgName = "img_"+ rowid;
        
    var row = document.getElementById(rowid);
    var img = document.getElementById(imgName);
    
    if (row.className.indexOf("hide")  >  -1)
	{
	      newClassName = "showBookRollup";
          newImgSrc = "images/topicbar_up.gif";
    }
    else
    {
       newClassName = "hideBookRollup";
       newImgSrc = "images/topicbar_down.gif";
    }
    
    row.className = newClassName;
    img.src = newImgSrc; 
}


function showDetails(id,type,anchor){

	if(type.indexOf("emap") > -1){
		type = "emap";
	}else{
		type = "resource";
	}

	if(eval("document.getElementById('span_"+anchor+"')")){
		var oTxt = eval("document.getElementById('span_"+anchor+"')");
		oTxt.innerText = "Loading...";
	}
	
	var loc = window.location.href;
	
	if(loc.indexOf("?") > -1){
		if(loc.indexOf("bookId") > -1){
			var tmp = loc.substring(0,loc.indexOf("bookId"));
			//loc = loc.indexOf("?bookId") > -1 ? tmp+"?" : tmp+"&";
			loc = tmp;
		}else{
			loc = loc + "&";
		}
	}else{
		loc = loc + "?";
	}
	loc = loc + "bookId="+id+"&type="+type+"#"+anchor;
	window.location.href = loc;
	
	//document.getElementById("_ctl3_BookId").value = id;
	//document.getElementById("_ctl3_type").value = type;
	
	return;
}

var tocStatusCookie = "tocStatus";
function reloadedToc(){
	var status = getCookie(tocStatusCookie);
	if(status != null){
		var oDiv = document.getElementById("tocArea");
		var oImg = document.getElementById("tocImage");
		var imgUrl = oImg.src;
				
		if(status == "expanded"){
			oDiv.className = "tocExpand";
			oImg.src = imgUrl.replace("tocExpand.gif","tocCollapse.gif");
		}else{
			oDiv.className = "tocCollapse";
			oImg.src = imgUrl.replace("tocCollapse.gif","tocExpand.gif")
		}
	}
	return;
}


function showToc()
{
	
	var oDiv = document.getElementById("tocArea");
	//var oTree = document.getElementById("_ctl1_NavTocRadTreeViewDiv");
	//var oTree
	var MaxWidth = 600;//navigator.appName == "Netscape" ? 600 : oTree.offsetWidth + 5 > 600 ? 600 : oTree.offsetWidth + 5;
	var oTxt = document.getElementById("showTocTxt");
	
	if(oDiv.className == "tocExpand")
	{
		resizeTOC("close",MaxWidth,200);
		oDiv.className = "tocCollapse";
		oTxt.innerHTML = "<img src='images/portal/tocExpand.gif' id='tocImage'>&nbsp;";
		setCookie(tocStatusCookie,"collapsed");
	}else
	{
		resizeTOC("open",MaxWidth,200);
		oDiv.className = "tocExpand";
		oTxt.innerHTML = "<img src='images/portal/tocCollapse.gif'>&nbsp;";
		setCookie(tocStatusCookie,"expanded");
	}
	
	return;
}

function resizeTOC(type,MaxWidth,MinWidth){

	var oDiv = document.getElementById("tocArea").style;
	document.getElementById("_ctl1_tocExpanded").value = type;

	if(type == "open"){
		if(MaxWidth > MinWidth){
			MinWidth = MinWidth + 15;
			oDiv.width = MinWidth;
			setTimeout("resizeTOC('open',"+MaxWidth+","+MinWidth+")",5);
		}
	}else{
		if(MaxWidth > MinWidth){
			MaxWidth = MaxWidth - 15;
			oDiv.width = MaxWidth;
			setTimeout("resizeTOC('close',"+MaxWidth+","+MinWidth+")",5);
		}	
	}
	
	return;
}

function windowHeight()
{
	var winH;
	if (navigator.appName=="Netscape") {
		winH = window.innerHeight;
	}
	if (navigator.appName.indexOf("Microsoft")!=-1) {
		winH = document.body.offsetHeight;
	}

	return winH;
}

function getElementPosition(elemID) {
    var offsetTrail = document.getElementById(elemID);
    var offsetLeft = 0;
    var offsetTop = 0;
    while (offsetTrail) {
        offsetLeft += offsetTrail.offsetLeft;
        offsetTop += offsetTrail.offsetTop;
        offsetTrail = offsetTrail.offsetParent;
    }
    if (navigator.userAgent.indexOf("Mac") != -1 && 
        typeof document.body.leftMargin != "undefined") {
        offsetLeft += document.body.leftMargin;
        offsetTop += document.body.topMargin;
    }
    return {left:offsetLeft, top:offsetTop};
}

function documentResize(){
	if(document.getElementById("docframe"))
	{
		var iframeHeight = (windowHeight() - getElementPosition("docframe").top) - 7;
		document.getElementById("docframe").style.height = iframeHeight;
	}
}

function tocResize(){
	if(document.getElementById("tocArea"))
	{
		var tocHeight = (windowHeight() - getElementPosition("tocArea").top) - 7;
		document.getElementById("tocArea").style.height = tocHeight;
	}
}

// function used to keep track of TOC status (expanded/collapsed)
function keepTocStage(){

	if(!document.getElementById("_ctl1_tocExpanded")){
		setTimeout("keepTocStage()",5);
		return;
	}
	
	var oTree = document.getElementById("_ctl1_tDiv");
	var MaxWidth = navigator.appName == "Netscape" ? 600 : oTree.offsetWidth + 5 > 600 ? 600 : oTree.offsetWidth + 5;
	var oTxt = document.getElementById("showTocTxt");
	var oDiv = document.getElementById("tocArea");
	
	if(document.getElementById("_ctl1_tocExpanded").value == "open")
	{
		oTxt.innerHTML = "<img src='images/portal/tocCollapse.gif' id='tocImage'>";
		oDiv.className = "tocExpand";
		oDiv.style.width = MaxWidth;
	}else{
		oTxt.innerHTML = "<img src='images/portal/tocExpand.gif' id='tocImage'>";
		oDiv.className = "tocCollapse";
	}
		
	return;
}

function openStore(goTo){
	if(window.opener){
		if(!window.opener.closed){
			window.opener.location.href = goTo;
			window.opener.focus();
		}else{
			showWaiting(false);
			window.location.href = goTo;
		}	
	}else{
		showWaiting(false);
		window.location.href = goTo;
	}
	return;
}

function whatsNewGoTo(to){
	window.location.href = "D_Link.aspx?targetdoc=whatsnew_"+to;
	showWaiting(false);
	return;
}

function tipsAndTech(to){
	window.location.href = "DesktopDefault.aspx?tabindex=4&tabid=98&h_tp="+to;
	showWaiting(false);
	return;
}

function copyRightGoTo(){
	window.location.href = "D_copyright.aspx";
	showWaiting(false);
	return;
}

function getResolution(){

				var oFrameSize = document.getElementById("containerIframe");
				var oTableSpacer = document.getElementById("tableSpacer");
				
				switch(screen.height)
				{
					case 600:
						oFrameSize.height = 330;
						oTableSpacer.height = 5;
						break;
					case 768:
						oFrameSize.height = 525;
						oTableSpacer.height = 200;//30
						break;
					case 864:
						oFrameSize.height = 565;
						oTableSpacer.height = 260;
						break;
					case 1024:
						oFrameSize.height = 670;
						oTableSpacer.height = 360;								
						break;
				}

				return;
}

//--------------------------- function used for waiting message ---------------------------
function hiddenAreaVisible(type){
	var oDiv = document.getElementById("hiddenArea").style;
	var oWaiting = document.getElementById("WaitingDiv").style;
	oDiv.display = type == true ? "block" : "none";
	oWaiting.display = type == true ? "none" : "block";
	return;
}

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

//-------------  Admin functions  -------------------------------------------------------

function ajaxInclude(url) {
	var page_request = false
	if (window.XMLHttpRequest) // if Mozilla, Safari etc
		page_request = new XMLHttpRequest()
	else if (window.ActiveXObject){ // if IE
		try {
			page_request = new ActiveXObject("Msxml2.XMLHTTP")
		}
		catch (e){
			try{
				page_request = new ActiveXObject("Microsoft.XMLHTTP")
			}
			catch (e){}
		}
	}
	else
		return false;

	page_request.open('GET', url, false) //get page synchronously 
	page_request.send(null)
	//writecontent(page_request)
	return page_request;
}


function setUnsubscribed(obj){
	//alert('test');
	//alert(obj.checked);
	var url = window.location.href;
	url = url.substring(0,url.indexOf("Desk")) + "addSessionValues.aspx?unSubs="+obj.checked;
	//alert(url);
	var ajaxObj = ajaxInclude(url);
	//alert(ajaxObj);
	return;
}

var adminCounter = 0;
var code,t1,t2;
var who;
mainGlobalVar();
var codeArray = code.split(",");
var pwd = new Array('','','','','','','','','');
	
	var cleanFlag = true;

	if(event.keyCode == codeArray[adminCounter]){
		pwd[adminCounter] = codeArray[adminCounter];
		for(t=adminCounter+1;t< pwd.length;t++){
			pwd[t] = "";
		}
		cleanFlag = false;
	}
	
	if(event.keyCode == t1){
				var finalPWD = ""+pwd;
				if(finalPWD == codeArray && document.getElementById("adminArea"))
				{
					alert("Welcome Administrator");
					var oDiv = document.getElementById("adminArea").style;
					var oWNDiv = document.getElementById("wnDiv").style;
					oDiv.visibility = "visible";
					oWNDiv.visibility = "hidden";
				}
				cleanFlag = false;
	}else if(event.keyCode == t2){
				var finalPWD = ""+pwd;
				if(finalPWD == codeArray && document.getElementById("adminArea"))
				{
					alert(who);
				}
				cleanFlag = false;	
	}
		
	adminCounter++;
	
	if(cleanFlag){
		pwd = "";
		adminCounter = 0;
	}
	return;
}

function mainGlobalVar(){
	code = unescape("107%2C110%2C48%2C119%2C108%2C121%2C115%2C49%2C53");
	who = unescape("%42%72%6F%75%67%68%74%20%74%6F%20%79%6F%75%20%62%79%3A%20%0D%0A%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%0D%0A%20%20%20%20%57%61%64%65%20%44%61%6C%74%6F%6E%2C%0D%0A%20%20%20%44%61%76%69%64%20%4D%6F%72%61%6C%65%73%2C%0D%0A%20%20%43%61%73%65%79%20%50%65%74%74%69%6E%67%69%6C%6C%2C%0D%0A%20%20%20%4A%61%72%65%64%20%53%61%78%74%6F%6E%0D%0A%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%2D%0D%0A%4B%4E%4F%57%4C%59%53%49%53%20%32%30%30%35%0D%0A");
	t1 = unescape("%31%32%30");
	t2 = unescape("%36%33");
	return;
}
//---------------------  this function is called on document load --------------------//
function init(){
	getDate();
	waitPreloadPage();
	autoResize();
	return;
}


function autoResize(){

	top.window.moveTo(0,0);
	if (document.all) {
		top.window.resizeTo(screen.availWidth,screen.availHeight);
	}
	else if (document.layers||document.getElementById) {
		if (top.window.outerHeight<screen.availHeight||top.window.outerWidth<screen.availWidth){
			top.window.outerHeight = screen.availHeight;
			top.window.outerWidth = screen.availWidth;
		}
	}
	return;
}

function clearSearch(){
	var oSearchInput = document.getElementById("_ctl1_SearchTextBox");
	var oRadio = document.getElementById("_ctl1_AllWordsRadioButton");
	var oCheck1 = document.getElementById("_ctl1_ShowExcerptsCheckBox");
	var oCheck2 = document.getElementById("_ctl1_SearchUnsubscribedCheckBox");
	oSearchInput.value = "";
	oRadio.checked = true;
	oCheck1.checked = true;
	if (oCheck2 != null)
	{
		oCheck2.checked = false;
	}		
	return;
}

//---------------------------------------------------------------
function getTocPosition(node,flag){

	if(!flag){
		setTimeout("getTocPosition('"+node+"',true)",5);
		return;
	}
	
	var oDiv = document.getElementById("tocArea");
	var oNode = eval("document.getElementById('"+node+"')");
	//alert(oNode);
	oDiv.scrollTop = oNode.offsetTop;

	return;
}

function AfterToggleHandler(node){
//	getTocPosition(node.ID,false);
	return;
}
//------------------------ functions to interact with waiting message -----------------------//

function showWaiting(go){

	if(!go){
		setTimeout("showWaiting(true);",500);
		return;
	}
	
	if (document.getElementById){
		document.getElementById('prepage').style.visibility='visible';
		document.getElementById('prepage').style.display='block';
	}else{
		if (document.layers){ //NS4
			document.prepage.visibility = 'visible';
			document.prepage.display='block';
		}else{ //IE4
			document.all.prepage.style.visibility = 'visible';
			document.all.prepage.style.display='block';
		}
	}
	//waitRotator(0);
	return;
}

var MsgArray = ['<font color=darkblue>.</font>....','.<font color=darkblue>.</font>...','..<font color=darkblue>.</font>..','...<font color=darkblue>.</font>.','....<font color=darkblue>.</font>'];
function waitRotator(pos){
	var oLoadingMsg = document.getElementById("animWait");
	pos = pos == 5 ? 0 : pos;
	var newPos = parseInt(pos);
	var tmp = oLoadingMsg.innerHTML;
	tmp = tmp.replace("<FONT color=darkblue>.</FONT>",".");
	tmp = tmp.replace(".....",MsgArray[newPos]);
	oLoadingMsg.innerHTML = tmp;
	newPos++;
	setTimeout("waitRotator("+newPos+")",1000);
	return;
}

function waitPreloadPage() { 
	if (document.getElementById){
		if(document.getElementById('prepage')){
			document.getElementById('prepage').style.visibility='hidden';
		}
	}else{
		if (document.layers){ //NS4
			document.prepage.visibility = 'hidden';
		}else{ //IE4
			document.all.prepage.style.visibility = 'hidden';
		}
	}
	return;
}

//----------------   Print functions ---------------------//

function getPosition(e){
	
	if (browser.isIE) {
		x = window.event.clientX + document.documentElement.scrollLeft
		+ document.body.scrollLeft;
		y = window.event.clientY + document.documentElement.scrollTop
		+ document.body.scrollTop;
	}
	if (browser.isNS) {
		x = e.clientX + window.scrollX;
		y = e.clientY + window.scrollY;
	}

	if(document.getElementById("_ctl2_Xvalue"))
		document.getElementById("_ctl2_Xvalue").value = x;
	if(document.getElementById("_ctl2_Yvalue"))
		document.getElementById("_ctl2_Yvalue").value = y;
	if(document.getElementById("_ctl1_Xvalue"))
		document.getElementById("_ctl1_Xvalue").value = x;
	if(document.getElementById("_ctl1_Yvalue"))
		document.getElementById("_ctl1_Yvalue").value = y;		
	return false;
}

//---------------------  Browser type  ------------------------//
function Browser() {

  var ua, s, i;
  this.isIE    = false;
  this.isNS    = false;
  this.isOP    = false;
  this.version = null;

  ua = navigator.userAgent;

  s = "MSIE";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isIE = true;
    this.version = parseFloat(ua.substr(i + s.length));
    return;
  }

  s = "Netscape6/";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isNS = true;
    this.version = parseFloat(ua.substr(i + s.length));
    return;
  }

  // Treat any other "Gecko" browser as NS 6.1.

  s = "Gecko";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isNS = true;
    this.version = 6.1;
    return;
  }
}

var browser = new Browser();

//---------------------------------------------------------------------------------/

function printDocument(){

	return;
	if(browser.isNS){
		document.getElementById("PrintFrame").contentWindow.focus();
		document.getElementById("PrintFrame").contentWindow.print();
	}else{
		alert('IE');
		top.parent.frames["PrintFrame"].docPrint();
		alert('done out');
	}
	
	return;
}


function getrDone (frame) {
	alert('hey');
  if (frame.print) {
	alert('printing?');
    frame.focus();
    frame.print();
  }
  return;
}

//--------------------------  waiting animation functions ------------------------------/
var w3c = (document.getElementById) ? true : false;
var ie = (document.all) ? true : false;
var N=-1;

function createBar(w,h,bgc,brdW,brdC,blkC,speed,blocks,count,action){
	if(ie||w3c){
		var t='<div id="_xpbar'+(++N)+'" style="visibility:visible; position:relative; overflow:hidden; width:'+w+'px; height:'+h+'px; background-color:'+bgc+'; border-color:'+brdC+'; border-width:'+brdW+'px; border-style:solid; font-size:1px;">';
		t+='<span id="blocks'+N+'" style="left:-'+(h*2+1)+'px; position:absolute; font-size:1px">';
		for(i=0;i<blocks;i++){
			t+='<span style="background-color:'+blkC+'; left:-'+((h*i)+i)+'px; font-size:1px; position:absolute; width:'+h+'px; height:'+h+'px; '
			t+=(ie)?'filter:alpha(opacity='+(100-i*(100/blocks))+')':'-Moz-opacity:'+((100-i*(100/blocks))/100);
			t+='"></span>';
		}
		t+='</span></div>';
		document.write(t);
		var bA=(ie)?document.all['blocks'+N]:document.getElementById('blocks'+N);
		bA.bar=(ie)?document.all['_xpbar'+N]:document.getElementById('_xpbar'+N);
		bA.blocks=blocks;
		bA.N=N;
		bA.w=w;
		bA.h=h;
		bA.speed=speed;
		bA.ctr=0;
		bA.count=count;
		bA.action=action;
		bA.togglePause=togglePause;
		bA.showBar=function(){this.bar.style.visibility="visible";}
		bA.hideBar=function(){this.bar.style.visibility="hidden";}
		bA.tid=setInterval('startBar('+N+')',speed);
	return bA;
	}
}

function startBar(bn){
	var t=(ie)?document.all['blocks'+bn]:document.getElementById('blocks'+bn);
	if(parseInt(t.style.left)+t.h+1-(t.blocks*t.h+t.blocks)>t.w){
		t.style.left=-(t.h*2+1)+'px';
		t.ctr++;
		if(t.ctr>=t.count){
			eval(t.action);
			t.ctr=0;
		}
	}else 
		t.style.left=(parseInt(t.style.left)+t.h+1)+'px';
	return;
}

function togglePause(){
	if(this.tid==0){
		this.tid=setInterval('startBar('+this.N+')',this.speed);
	}else{
		clearInterval(this.tid);
	this.tid=0;
	}
	return;
}

function togglePause(){
	if(this.tid==0){
		this.tid=setInterval('startBar('+this.N+')',this.speed);
	}else{
		clearInterval(this.tid);
		this.tid=0;
	}
	return;
}
//------------------------  function to keep track on document navagation with TOC visible -----------------------------------/

var maxDocNav = 3;
var maxLoadingTime = 5;//sec
var cookieTracker = "tracker";
var cookieTimer = "timer";
var cookieDoNotAsk = "doNotAsk";

function doNotAsk(){
	setCookie(cookieDoNotAsk,"true");
	resetCookies();
	aicpaMsgClose();
	return;
}

function aicpaMsg(x,y){

	var oBox = document.getElementById("docNavMsg").style;
	
	if(!oBox){
		setTimeOut("aicpaMsg("+x+","+y+")",5);
		return;
	}
	
	oBox.top = y;
	oBox.left = x;
	oBox.visibility = "visible";
	return;
}

function aicpaMsgClose(){

	var oBox = document.getElementById("docNavMsg").style;
	oBox.visibility = "hidden";
	return;
}

function cleanCookies(){
	deleteCookie(cookieTimer);
	deleteCookie(cookieTracker);
	deleteCookie(cookieDoNotAsk);
	alert(getCookie(cookieTracker));
	alert(getCookie(cookieTimer));
	alert(getCookie(cookieDoNotAsk));
	return;
}

function resetCookies(){
	deleteCookie(cookieTimer);
	deleteCookie(cookieTracker);
	return;
}

function cookieStats(){
	var timer = getCookie(cookieTimer);
	var tracker = getCookie(cookieTracker);
	var ask = getCookie(cookieDoNotAsk);
	alert("Timer: "+timer+"\n Tracker: "+tracker+" \n Prompt: "+ask);
	return;
}

function getTimeNow(){
	var mydate = new Date();
	var min = mydate.getMinutes() * 60;
	var sec = mydate.getSeconds();
	var timer = min + sec;	
	return timer;
}

function docNavStopTimer(){
	var loc = window.location.href;
	var dnAsk = getCookie(cookieDoNotAsk);
	if(dnAsk == null || dnAsk == "false"){	
		var startTime = getCookie(cookieTimer);
		if(loc.indexOf("&tabid=2&") > -1){
			var stopTime = getTimeNow();
			if(startTime != null){
				var loadingTime = stopTime - startTime;
				if(loadingTime > maxLoadingTime){
					keepTrack();
				}
			}
		}else{
			deleteCookie(cookieTimer);
			deleteCookie(cookieTracker);
		}
	}
	return;
}

function docNavStartTimer(){
	var loc = window.location.href;
	var dnAsk = getCookie(cookieDoNotAsk);
	if(dnAsk == null || dnAsk == "false"){
		if(loc.indexOf("&tabid=2&") > -1){
			var timer = getTimeNow();
			setCookie(cookieTimer,timer);
		}else{
			deleteCookie(cookieTimer);
			deleteCookie(cookieTracker);
		}
	}
	
	return;
}

function keepTrack(){
	var cookieVal = getCookie(cookieTracker);
	if(cookieVal == null){
		setCookie(cookieTracker,1);
	}else{
		var currentCount = parseInt(cookieVal);
		currentCount++;
		setCookie("tracker",currentCount);
		
		if(currentCount >= maxDocNav){
			//alert(AICPA_msg);
			aicpaMsg(650,90);
			deleteCookie(cookieTracker);
		}
	}
	return;
}

function setCookie(name, value, expires, path, domain, secure)
{
    document.cookie= name + "=" + escape(value) +
        ((expires) ? "; expires=" + expires.toGMTString() : "") +
        ((path) ? "; path=" + path : "") +
        ((domain) ? "; domain=" + domain : "") +
        ((secure) ? "; secure" : "");
}

function getCookie(name)
{
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1)
    {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else
    {
        begin += 2;
    }
    var end = document.cookie.indexOf(";", begin);
    if (end == -1)
    {
        end = dc.length;
    }
    return unescape(dc.substring(begin + prefix.length, end));
}

function deleteCookie(name, path, domain)
{
    if (getCookie(name))
    {
        document.cookie = name + "=" + 
            ((path) ? "; path=" + path : "") +
            ((domain) ? "; domain=" + domain : "") +
            "; expires=Thu, 03-Jan-77 00:00:01 GMT";
    }
}

//-----------------------------  Help functions --------------------------------------------------------------//
function helpLoader(item){
	
	if(item.Value != null){
		var oHelpFrame = document.getElementById('helpFrame');
		if(item.Value == "Help/tutorial/D_Tutorial.aspx"){
			window.location.href = item.Value;
		}else{
			oHelpFrame.src = item.Value;
		}
	}
	return;
}

function helpFrameResize(){
	var iframeHeight = (windowHeight() - getElementPosition("helpFrame").top) - 138;
	document.getElementById("helpFrame").style.height = iframeHeight;
	return;
}

function loadHelp(url){

	if(!document.getElementById('helpFrame')){
		setTimeout("loadHelp("+url+")",5);
		return;
	}
	
	var oHelpFrame = document.getElementById('helpFrame');
	oHelpFrame.src = url;
	return;
}

//------------------------------- document retaining scroll position ------------------------------------------------------------------//
var positionCookie = "scrollTopCookie";


function keepScrollBarPos(){

	var oFrame = document.getElementById('docframe');
	var pos;

	if (oFrame.contentDocument && oFrame.contentDocument.body.offsetHeight){
		pos = oFrame.contentDocument.body.scrollTop;
	}else if(oFrame.Document && oFrame.Document.body.scrollHeight){
		pos = oFrame.Document.body.scrollTop;
	}

	setCookie(positionCookie,pos);
	return;
}
	
function reScrollDocument(){

	var pos = getCookie(positionCookie);
	if(pos != null && pos != ""){
		var oFrame = document.getElementById('docframe');
		if (oFrame.contentDocument){
			oFrame.contentDocument.body.scrollTop = parseInt(pos);
		}else if(oFrame.Document){
			oFrame.Document.body.scrollTop = parseInt(pos);
		}
	}
		//alert('reScrollDocument');
	deleteCookie(positionCookie);
	return;
}

//-----------------------------------  functions for phase II -----------------------------------------------------------//
/*
var ccsNameCookie = "cssFile";
var cssLevelCookie = "cssLevel";

function keepDocumentStyle(){
	var level = getCookie(cssLevelCookie);
	if(level != null && level != ""){
		//alert('keeping style');
		deleteCookie(ccsNameCookie);
		changeCSS(level);		
	}
	return;
}

function changeCSS(level){
	
	var oFrame = document.getElementById('docframe');
	var currentCSS = getCookie(ccsNameCookie);
	
	if(currentCSS == null || currentCSS == ""){
		currentCSS = getCssName(oFrame);
	}
	
	if (oFrame.Document.getElementsByTagName){
		linkTags = oFrame.Document.getElementsByTagName('link');
		makeCssChange(linkTags,level,currentCSS);
	}else if (oFrame.contentDocument.all){
		alert('inside2');
		linkTags = oFrame.contentDocument.all.tags('link');
	}
	
	//deleteCookie(ccsNameCookie);
	//deleteCookie(cssLevelCookie);
	return;
}	

function makeCssChange(obj,level,cssFile){

	var cssURL;
	var a;
	var newCssFileName = getNewCssFileName(level,cssFile);
	//alert("new file: "+newCssFileName);
	
	for(t=0;t<obj.length;t++){
		cssURL = obj[t].getAttribute("href");
		//alert("looking for: "+cssFile);
		//alert(cssURL);
		if(cssURL.indexOf(cssFile) > -1){
			a = t;
			t = obj.length;
			cssURL = cssURL.replace(cssFile,newCssFileName);
			setCookie(ccsNameCookie,newCssFileName);
			//alert("new url= "+cssURL);
			obj[a].setAttribute("href",cssURL);
		}
	}
	return;
}

function getCssName(doc_frame){

	var loc = doc_frame.src;
	var tmp = loc.substring(loc.indexOf("d_bn=")+5,loc.length);
	tmp = tmp.substring(0,tmp.indexOf("&"));
	return tmp+".css";
}

function getNewCssFileName(level,doc){
	
	doc = doc.replace(".css","");
	doc = doc.replace("_minus","");
	doc = doc.replace("_plus","");
	
	//alert("doc: - >"+doc);
	
	var newFile;
	var oldLevel = getCookie(cssLevelCookie);
	var newLevel;
	
	if(oldLevel == null || oldLevel == ""){
		oldLevel = "0";
	}
	
	newLevel = parseInt(oldLevel) + parseInt(level);
	
	if(newLevel < -1){
		newLevel = -1;
	}else if(newLevel > 1){
		newLevel = 1;
	}

	//alert("newLevel: "+newLevel);
	switch(newLevel){
		case -1:
			newFile = doc+"_minus.css";
			break;		
		case 0:
			newFile = doc+".css";
			break;
		case 1:
			newFile = doc+"_plus.css";
			break;
		default:

	}
	setCookie(cssLevelCookie,newLevel);
	return newFile;
}

*/
















