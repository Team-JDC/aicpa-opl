$(document).ready(function () {

    //When page loads...
    $(".tab_content").hide(); //Hide all content
    $(".tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab_content:first").show(); //Show first tab content

    //On Click Event
    $(".tabs li").click(function () {
        if (isToolbarHidden()) {
            doToolbarToggle();
        }

        $(".tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content

        var activeTab = $(this).find("a").attr("href"); //Find the href attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active ID content
        return false;
    });

});

/*
$(document).ready(function () {

    //When page loads...
    $(".tab_content2").hide(); //Hide all content
    $(".tabs2 li:first").addClass("active").show(); //Activate first tab
    $(".tab_content2:first").show(); //Show first tab content

    //On Click Event
    $(".tabs2 li").bind("click", function () {

        $(".tabs2 li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content2").hide(); //Hide all tab content

        //var activeTab = $(this).find("a").attr("href"); //Find the href attribute value to identify the active tab + content
        //$(activeTab).fadeIn(); //Fade in the active ID content
        return false;
    });
});
*/
