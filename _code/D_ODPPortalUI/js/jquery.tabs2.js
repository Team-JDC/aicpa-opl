$(document).ready(function() {

	//When page loads...
	$(".tab_content2").hide(); //Hide all content
	$(".tabs2 li:first").addClass("active").show(); //Activate first tab
	$(".tab_content2:first").show(); //Show first tab content

	//On Click Event
	$(".tabs2 li").bind("click",function() {

		$(".tabs2 li").removeClass("active"); //Remove any "active" class
		$(this).addClass("active"); //Add "active" class to selected tab
		$(".tab_content2").hide(); //Hide all tab content

		var activeTab = $(this).find("a").attr("href"); //Find the href attribute value to identify the active tab + content
		$(activeTab).fadeIn(); //Fade in the active ID content
		return false;
	});
	
		$("#flipPad a:not(.revert)").bind("click",function(){
					var $this = $(this);
					$("#flipbox").flip({
						direction: $this.attr("rel"),
						color: $this.attr("rev"),
						content: $this.attr("title"),//(new Date()).getTime(),
						onBefore: function(){$(".revert").show()}
					})
					return true;
				});
				
				$(".revert").bind("click",function(){
					$("#flipbox").revertFlip();
					return true;
				});


				
		

});

//$(document).ready(function() {
function LoadMyDocuments() {
    $.ajax({
        type: "POST",
        url: "TestWebService.asmx/GetMyDocuments",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(msg) {
            //instantiate a template with data
            ApplyTemplate(msg);
        },
        error: AjaxFailed
        
    });
    //      });
}
function LoadMyDocuments_orig() {
    $.ajax({
        type: "POST",
        url: "Content.aspx/GetMyDocuments3",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(msg) {
            //instantiate a template with data
            ApplyTemplate(msg);
        }
    });
    //      });
}
function ApplyTemplate(msg) {
//    alert("applying templates");
//    alert("msg: " + msg);
      $('#doctoolbar').setTemplate($("#NewDocTool").html());
      $('#doctoolbar').processTemplate(msg);
  }
  function AjaxFailed(result) {
      alert('failed' + result.status + ' ' + result.statusText);
  }