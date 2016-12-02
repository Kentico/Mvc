(function () {
    'use strict';

    var url = $('.cart-item-selector').data('variant-action'),
        stockMessage = $("#stockMessage"),
        totalPrice = $('#totalPrice'),
        selectedSKUID = $('#selectedVariantID');

    $('.js-variant-selector').change(function () {
        var id = $(this).val();
        updateVariantSelection(id);
    });

    function updateVariantSelection(variantId) {
        $.post(url, { variantID: variantId }, function (data) {
            stockMessage.text(data.stockMessage);

            totalPrice.text(data.totalPrice);
            selectedSKUID.val(variantId);
        });
    }

}());