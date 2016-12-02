(function () {
    'use strict';

    $('.js-address-selector-div').change(function () {
        var $selectorDiv = $(this),
            $addressDiv = $selectorDiv.parent(),
            $selector = $selectorDiv.find('.js-address-selector'),
            url = $selectorDiv.data('statelistaction'),
            postData = {
                addressId: $selector.val()
            };

        if (!postData.addressId) {
            eraseFields($addressDiv);
            return;
        }

        $.post(url, postData, function (data) {
            fillFields($addressDiv, data);
        });
    });

    function fillFields($addressDiv, data) {
        fillBasicFields($addressDiv, data);
        fillCountryStateFields($addressDiv, data);
    }

    function fillBasicFields($addressDiv, data) {
        var basicFields = $addressDiv.data('fields'),
            addressType = $addressDiv.data('addresstype');

        $.each(basicFields, function (i, val) {
            var fieldId = '#' + addressType + '_' + addressType + val,
                fieldVal = data[val];

            $(fieldId).val(fieldVal);
        });
    }

    function fillCountryStateFields($addressDiv, data) {
        var $countryStateSelector = $addressDiv.find('.js-country-state-selector'),
            countryField = $countryStateSelector.data('countryfield'),
            stateField = $countryStateSelector.data('statefield'),
            $countrySelector = $countryStateSelector.find('.js-country-selector');

        $countryStateSelector.data('stateselectedid', data[stateField]);
        $countrySelector.val(data[countryField]).change();
    }

    function eraseFields($addressDiv) {
        var data = {};
        fillFields($addressDiv, data);
    }
}());
