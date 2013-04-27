/* 
//////////////////////////////////////////
switchControl.js is a jQuery plug-in ( Free to use )
  
* Created by Serkan Karaarslan 
- Blog : http://skaraarslan.blogspot.com/
- Linkedin : http://www.linkedin.com/in/serkankaraarslan
- Twitter : http://twitter.com/sekanet
- More Information about Switch Control : http://code.technolatte.net/SwitchControl 
* Depends :
- jquery.js ( http://code.jquery.com/jquery-latest.js )  

//////////////////////////////////////////
*/
(function ($) {
    $.fn.switchControl = function (options) {
        var settings = $.extend({
            'left': 'ON',
            'right': 'OFF',
            'initialState': '', // if it's not determine, right option is used as default value
            'speed': 'fast', // fast | slow
            'size': 'm' // Small | Medium | Large
        }, options);

        return this.each(function () {
            var element = $(this);

            var state = '';
            var sliderPlace = '';
            var size = settings.size.toLowerCase();
            
            if (settings.initialState.length === 0) {
                settings.initialState = settings.right;
            }

            if (element.attr("data-state") === undefined) {
                state = settings.initialState;
                element.attr("data-state", state);
            }
            else {
                state = element.attr("data-state");
            }


            if (state === settings.left)
                sliderPlace = "left";
            else
                sliderPlace = "right";


            var layout = "<div class='switch-left-" + size + "'>" + settings.left + "</div><div class='switch-right-" + size + "'>" + settings.right + "</div>" +
                    "<div class='switch-slider-" + size + " switch-slider-" + sliderPlace + "-" + size + "'></div>";


            element.html(layout).addClass("switch-box-" + size);

            element.click(function () {
                var slider = $("#" + $(this).attr("id") + " .switch-slider-" + size);

                if ($(this).attr("data-state") === settings.right) {
                    slider.animate({ "left": "+=" + $(this).css("width").replace('px', '') / 2 + "px" }, settings.speed);
                    $(this).attr("data-state", settings.left);
                }
                else {
                    slider.animate({ "left": "-=" + $(this).css("width").replace('px', '') / 2 + "px" }, settings.speed);
                    $(this).attr("data-state", settings.right);
                }
            });
        });
    };
})(jQuery);