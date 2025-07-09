/***********************************************************************************************************************
DOCUMENT: includes/javascript.js
DEVELOPED BY: Ryan Stemkoski
COMPANY: Zipline Interactive
EMAIL: ryan@gozipline.com
PHONE: 509-321-2849
DATE: 2/26/2009
DESCRIPTION: This is the JavaScript required to create the accordion style menu.  Requires jQuery library
************************************************************************************************************************/

$(document).ready(function() {
	
	/********************************************************************************************************************
	SIMPLE ACCORDIAN STYLE MENU FUNCTION
	********************************************************************************************************************/	
	$('div.accordionButton').click(function() {
		$('div.accordionContent').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton2').click(function() {
		$('div.accordionContent2').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton3').click(function() {
		$('div.accordionContent3').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton4').click(function() {
		$('div.accordionContent4').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton5').click(function() {
		$('div.accordionContent5').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton6').click(function() {
		$('div.accordionContent6').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton7').click(function() {
		$('div.accordionContent7').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	$('div.accordionButton8').click(function() {
		$('div.accordionContent8').slideUp('normal');	
		$(this).next().slideDown('normal');
	});
	
	
	
	/********************************************************************************************************************
	CLOSES ALL DIVS ON PAGE LOAD
	********************************************************************************************************************/	
	$("div.accordionContent").hide();

});
