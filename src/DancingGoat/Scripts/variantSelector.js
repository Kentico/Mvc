(function () {
    'use strict';

    var url = $('.cart-item-selector').data('variant-action'),
        $stockMessage = $("#stockMessage"),
        $totalPrice = $('#totalPrice'),
        $selectedVariant = $('#selectedVariantID'),
        inStockClass = "available",
        outOfStockClass = "unavailable",
        parentProductID = $('#js-parent-product-id').val(),
        $submitButton = $('#js-submit-add-to-cart'),
        $beforeDiscount = $("#js-before-discount"),
        $savings = $("#js-savings"),
        $dicountWrapper = $('.discount-price');


    // Reset selection when back button is pressed
    $(function () {
        $('select.js-variant-selector').each(function () {
            $(this).val($(this).find("option[selected]").val());
        });

        $('.js-variant-selector input').each(function() {
            $(this).prop('checked', $(this).attr('checked'));
        });
    });

    $('.js-variant-selector').change(function () {
        var selectedOptionIDs = [];
        // Collect selected options in categories in order to create variant
        $('.js-variant-selector option:selected, .js-variant-selector input:checked').each(function () {
            selectedOptionIDs.push($(this).val());
        });

        updateVariantSpecificData(selectedOptionIDs);
    });

    function updateVariantSpecificData(optionIDs) {
        $.post(url, { options: optionIDs, parentProductID: parentProductID }, function (data) {
            $stockMessage.text(data.stockMessage);
            if (data.inStock) {
                $stockMessage.removeClass(outOfStockClass).addClass(inStockClass);
                $submitButton.removeClass('btn-disabled').removeAttr('disabled');
            } else {
                $stockMessage.removeClass(inStockClass).addClass(outOfStockClass),
                $submitButton.addClass('btn-disabled').attr('disabled', 'disabled');
            }

            $totalPrice.text(data.totalPrice);
            // Used to add variant to the shopping cart
            $selectedVariant.val(data.variantSKUID);

            // Update discount price info
            if (data.savings) {
                $beforeDiscount.text(data.beforeDiscount);
                $savings.text(data.savings);
                $dicountWrapper.removeClass('hidden');
            } else {
                $dicountWrapper.addClass('hidden');
            }
        });
    }
}());