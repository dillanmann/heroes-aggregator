$(document).ready(function () {

    function updateSelectedMmr() {

        var baseHeader = "Selected MMR: ";

        var selectedCheckboxes = $('.row-select').filter(function () {
            return $(this).is(':checked');
        });

        if (!selectedCheckboxes || selectedCheckboxes.length === 0) {
            $('#selected-mmr').text(baseHeader + 0);
            return;
        }

        var selectedRows = selectedCheckboxes.closest('.player-row').find('.weighted-mmr');

        var total = 0;
        for (var i = 0; i < selectedRows.length; ++i) {
            var value = parseFloat($(selectedRows[i]).text());
            total += value;
        }

        total = (total / selectedRows.length).toFixed(2);

        $('#selected-mmr').text(baseHeader + total);

    }

    $('.row-select-all').change(function () {
        $(this).closest('table').find('.row-select').prop('checked', $(this).is(':checked')).trigger('change');
    });

    $('.player-row .row-select').change(updateSelectedMmr);
});