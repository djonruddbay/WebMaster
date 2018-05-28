
$('.PhoneNumber').on('input propertychange', function () {

    pnValue = $(this).val().replace(/\D/g, '');
    if (pnValue.length > 0) {
        pnValue = "(" + pnValue;

        if (pnValue.length > 3) {
            pnValue = pnValue.slice(0, 4) + ") " + pnValue.slice(4)

            if (pnValue.length > 8) {
                pnValue = pnValue.slice(0, 9) + "-" + pnValue.slice(9)

                if (pnValue.length > 14) {
                    pnValue = pnValue.slice(0, 14)
                }
            }
        }
    }

    $(this).val(pnValue);

});
