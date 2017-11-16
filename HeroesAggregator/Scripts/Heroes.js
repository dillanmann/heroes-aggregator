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

    // Select all the available heroes when the 'select all' checkbox is checked
    $('.row-select-all').change(function () {
        $(this).closest('table').find('.row-select').prop('checked', $(this).is(':checked')).trigger('change');
    });

    // When a player is selected/deselected, update the selected average MMR
    $('.player-row .row-select').change(updateSelectedMmr);


    // Collapse/expand player hero stats tables
    $("button.collapse-player-row").click(function (ev) {
        ev.preventDefault();
        var attr = this.getAttribute('data-target');
        $(attr).toggleClass("in");
    });

});