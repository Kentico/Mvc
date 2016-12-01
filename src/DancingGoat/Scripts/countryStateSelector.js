(function () {
    'use strict';

    $('.js-country-selector').change(function () {
        var $countrySelector = $(this),
            $countryStateSelector = $countrySelector.parent('.js-country-state-selector'),
            $stateSelector = $countryStateSelector.find('.js-state-selector'),
            $stateSelectorContainer = $countryStateSelector.find('.js-state-selector-container'),
            selectedStateId = $countryStateSelector.data('stateselectedid'),
            url = $countryStateSelector.data('statelistaction'),
            postData = {
                countryId: $countrySelector.val()
            };

        $stateSelectorContainer.hide();

        if (!postData.countryId) {
            return;
        }

        $.post(url, postData, function (data) {
            $countryStateSelector.data('stateselectedid', 0);
            $stateSelector.val(null);

            if (!data.length) {
                return;
            }

            fillStateSelector($stateSelector, data);
            $stateSelectorContainer.show();

            if (selectedStateId > 0) {
                $stateSelector.val(selectedStateId);
            }
        });
    });

    $('.js-country-state-selector').each(function () {
        var $selector = $(this),
            $countrySelector = $selector.find('.js-country-selector'),
            countryId = $selector.data('countryselectedid');

        if (countryId > 0) {
            $countrySelector.val(countryId);
        }

        $countrySelector.change();
        $selector.data('countryselectedid', 0);
    });

    function fillStateSelector($stateSelector, data) {
        var items = '<option>' + $stateSelector.find('option:first').html() + '</option>';

        $.each(data, function (i, state) {
            items += '<option value="' + state.id + '">' + state.name + '</option>';
        });

        $stateSelector.html(items);
    }
}());