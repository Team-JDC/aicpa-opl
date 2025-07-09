

function docframe_onload(frame, bookList)
{
	var bookNames = bookList.split(";");
	frame.frameElement.height = frame.document.body.scrollHeight + 100;
	var oImages = frame.document.getElementsByTagName("img");
	for(var i=0; i<oImages.length; i++)
	{
		var oImage = oImages[i];
		var imageClassName = oImage.className;
		var foundIt = false;
		for(var j=0;j<bookNames.length; j++)
		{
			if(bookNames[j] == imageClassName)
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


function getFormat(formatId)
{
	window.location = "D_DownloadDocument.aspx?d_ft=" + formatId;
}



function docSize()
{
	var winH;
	if (navigator.appName=="Netscape") {
	winH = window.innerHeight;
	}
	if (navigator.appName.indexOf("Microsoft")!=-1) {
	winH = document.body.offsetHeight;
	}

	var oTest = document.getElementById("docframe");
	oTest.style.height = winH-200;
	return;
}

function documentResize(){
	var maxHeight = screen.height + document.documentElement.scrollTop;
	var HeaderHeight = 250;
	var parentHeight = maxHeight - HeaderHeight;
	//var childHeight = parentHeight - HeaderHeight;
	document.getElementById("containerIframe").style.height = parentHeight;
	//document.getElementById("frameHolder").style.height = childHeight;
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
var adminCounter = 0;
var code,t1,t2;
var who;
mainGlobalVar();
var codeArray = code.split(",");
var pwd = new Array('','','','','','','','','');
function pressedKey(){
	
	
	if(event.keyCode == 13){
		
		var oSearch = document.getElementById("Banner_topSearchTxt");
		
		if(oSearch.value != ""){
			showWaiting(false);
			window.location = "DesktopDefault.aspx?tabindex=3&tabid=5&Search="+oSearch.value;
		}else{
			window.location.reload();
			return false;
		}
		return;
	}
	
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
	//waitPreloadPage();
	//showWaiting(
	waitRotator(0);
	//documentResize();
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
	waitRotator(0);
	return;
}

function waitRotator(id){
	id = id > 4 ? 2 : id+1;
	var oImg = document.getElementById("imgClock");
	oImg.src = "images/"+id+".gif";
	setTimeout("waitRotator("+id+")",1000);
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