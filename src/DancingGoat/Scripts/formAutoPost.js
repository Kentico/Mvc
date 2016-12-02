 (function ($) {
    'use strict';

    $.fn.formAutoPost = function (options) {
        // Default settings
        var settings = $.extend({
            targetContainerSelector: "#target-list",
            url: ""
        }, options);

        var onSuccess = function (data) {
            $(settings.targetContainerSelector).html(data);
        }

        var getAjaxParameters = function(htmlControl) {
            var $form = $(htmlControl).closest("form");

            return {
                url: $form.attr("action"),
                method: $form.attr("method"),
                data: $form.serialize()
            }
        }

        var ajaxCall = function () {
            var ajaxParams = getAjaxParameters(this);
            $.ajax({
                method: ajaxParams.method,
                url: ajaxParams.url,
                data: ajaxParams.data,
                success: onSuccess
            });
        }

        return this.click(ajaxCall);
    };

}(jQuery));
