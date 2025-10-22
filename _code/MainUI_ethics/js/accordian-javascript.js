/***********************************************************************************************************************
DOCUMENT: includes/javascript.js
DEVELOPED BY: Ryan Stemkoski
COMPANY: Zipline Interactive
EMAIL: ryan@gozipline.com
PHONE: 509-321-2849
DATE: 2/26/2009
DESCRIPTION: This is the JavaScript required to create the accordion style menu.  Requires jQuery library
************************************************************************************************************************/

$(function() {
	
	/********************************************************************************************************************
	SIMPLE ACCORDIAN STYLE MENU FUNCTION
	********************************************************************************************************************/	
	$('div.accordionButton').on("click", function() {
		$('div.accordionContent').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton2').on("click", function() {
		$('div.accordionContent2').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton3').on("click", function() {
		$('div.accordionContent3').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton4').on("click", function() {
		$('div.accordionContent4').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton5').on("click", function() {
		$('div.accordionContent5').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton6').on("click", function() {
		$('div.accordionContent6').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton7').on("click", function() {
		$('div.accordionContent7').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton8').on("click", function() {
		$('div.accordionContent8').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	
	
	/********************************************************************************************************************
	CLOSES ALL DIVS ON PAGE LOAD
	********************************************************************************************************************/	
	$("div.accordionContent").hide();

});
