/**
 * This jQuery plugin creates an animated slider with the child elements as frames.
 *
 * @author Justin Jones (justin(at)jstnjns(dot)com)
 * @version 1.1
 *
 * New Features from 1.0:
 * 	- Vertical scrolling
 * 	- Buttons are optional
 * 	- Speed settings
 *
 */
(function($){
	$.fn.simpleslider2 = function(settings) {
		
		var defaults = {
			auto: false,
			interval: 2000,
			speed: 500,
			direction: 'horizontal',
			buttons: true,
			autobuttonsize: false,
			previoustext: 'Previous',
			nexttext: 'Next'
			},
			settings = $.extend({}, defaults, settings); 
		
		$(this).each(function() {

			// ----------------------------------------------------------------|| Setting Vars ||
			$container = $(this);
			$frame = $container.children().addClass('frame');
			
			// Wraps entire element
			$wrapper = $container.wrap('<div id="'+$container.attr('id')+'-wrapper" />').parent('#'+$container.attr('id')+'-wrapper')

			// Find frame count
			frameCount = $frame.size();

			// Find the greatest height of all frames
			wrapperHeight = 0;
			frameHeight = 0;
			
			$frame.each(function() {
				if($(this).height() > frameHeight) frameHeight = $(this).height();
				if($(this).outerHeight(true) > wrapperHeight) wrapperHeight = $(this).outerHeight(true);
			});
			
			// Find frame width
			wrapperWidth = $container.width();
			frameWidth = $frame.width();
			
			// Set to first frame
			frameNumber = 1;
			
			// ----------------------------------------------------------------|| Wrapper Details ||			
			
			// Sets strucural CSS for wrapper
			$wrapper.css({
				width: wrapperWidth,
				height: wrapperHeight,
				overflow: 'hidden'
			});			
				
			// ----------------------------------------------------------------|| Container Details ||			
			// Set structural CSS for container
			if(settings.direction == 'horizontal') {
				containerWidth = wrapperWidth * frameCount;
				containerHeight = wrapperHeight;
			} else if(settings.direction == 'vertical') {
				containerWidth = wrapperWidth;
				containerHeight = wrapperHeight * frameCount;
			}
			
			$container.css({
				width: containerWidth,
				height: containerHeight
			});

			// ----------------------------------------------------------------|| Frame Details ||			

			// Set frames to display correctly
			$frame.css({
				display: 'block',
				float: 'left',
				height: frameHeight,
				width: frameWidth
			});
			
			// ----------------------------------------------------------------|| Button Details ||	
			// Creates buttons
			if(settings.buttons) {
				$wrapper
				.before('<input type="image" id="'+$container.attr('id')+'-previous_button" class="button"  value="" src="images/btn-back.gif" />')
				.after('<input type="image" id="'+$container.attr('id')+'-next_button" class="button" value="" src="images/btn-next.gif" />');
			}
			
			if(settings.autobuttonsize && settings.direction == 'horizontal') {
				$wrapper.parent().find('.button').css({
					height: containerHeight
				});
			} else if(settings.autobuttonsize && (settings.direction == 'vertical')) {
				$wrapper.parent().find('.button').css({
					width: containerWidth
				});
			}

			// Creates buttons functionality
			$('#'+$container.attr('id')+'-previous_button').click(function(e){ e.preventDefault(); prevFrame(); stopTimer(); });
			$('#'+$container.attr('id')+'-next_button').click(function(e){ e.preventDefault(); nextFrame(); stopTimer(); });
			
			// ----------------------------------------------------------------|| Actions ||			
			// Initializes actions
			checkButtons();
			
			if(settings.auto == true) {
				startTimer(settings.interval);
				checkHover();
			}

			// Moves to frame
			function move(toFrame) {
				if(toFrame !== undefined) {
					if(settings.direction == 'horizontal') {
						$container.animate({
							marginLeft: (-1)*(toFrame - 1)*wrapperWidth+'px'
						}, settings.speed);
					} else if(settings.direction == 'vertical') {
						$container.animate({
							marginTop: (-1)*(toFrame - 1)*wrapperHeight+'px'
						}, settings.speed);
					}
				} else {
					if(settings.direction == 'horizontal') {
						$container.animate({
							marginLeft: (-1)*(frameNumber - 1)*wrapperWidth+'px'
						}, settings.speed);
					} else if(settings.direction == 'vertical') {
						$container.animate({
							marginTop: (-1)*(frameNumber - 1)*wrapperHeight+'px'
						}, settings.speed);						
					}
				}
			}
			
			// Moves to previous frame
			function prevFrame() {
				frameNumber--;
				if(frameNumber > 0) {
					move();
				} else {
					move(frameCount);
					frameNumber = frameCount;
				}
				checkButtons();
			}
			
			// Moves to next frame
			function nextFrame() {
				frameNumber++;
				if(frameNumber <= frameCount) {
					move();
				} else {
					move(1);
					frameNumber = 1;
				}
				checkButtons();
			}
			
			function checkButtons() {
				// Sets 'previous' button to disabled if on first frame
				if(frameNumber == 1) {
					$('#'+$container.attr('id')+'-previous_button').attr('disabled', 'disabled').addClass('disabled');
				} else {
					$('#'+$container.attr('id')+'-previous_button').removeAttr('disabled').removeClass('disabled');
				}
					
				// Sets 'next' button to disabled if on last frame
				if(frameNumber == frameCount) {
					$('#'+$container.attr('id')+'-next_button').attr('disabled', 'disabled').addClass('disabled');
				} else {
					$('#'+$container.attr('id')+'-next_button').removeAttr('disabled').removeClass('disabled');;
				}
			}

			function startTimer(interval) {
				auto = setInterval(nextFrame, interval);
			}
			
			function stopTimer() {
				if(settings.auto !== false){
					clearInterval(auto);
				}
			}
			
			function checkHover() {
				$wrapper.hover(function() {
					stopTimer();
				}, function() {
					startTimer(settings.interval);
				});
			}
			
		});
		
		// Allows chainability
		return this;
	}
})(jQuery);