$(document).ready(function () {

    var teamMmr = 0;
    var players = 0;

    (function getPlayerMmr() {
        var urlRoot = '/heroes/playermmr?playerid='
        $.each($('.player-row'), function (index, value) {
            var url = urlRoot + $(value).attr('player-id');
            $.getJSON(url, function (data) {
                var jsonData = JSON.parse(data);
                var heroLeague = jsonData['HeroLeague'] || 0; 
                var teamLeague = jsonData['TeamLeague'] || 0;
                var unrankedDraft = jsonData['UnrankedDraft'] || 0;
                var weightedMmr = (0.5 * heroLeague) + (0.3 * teamLeague) + (0.2 * unrankedDraft);

                $(value)
                    .append($('<td/>').text(heroLeague))
                    .append($('<td/>').text(teamLeague))
                    .append($('<td/>').text(unrankedDraft))
                    .append($('<td/>').text(weightedMmr.toFixed(0)).attr('class', 'weighted-mmr'))
                    .append($('<td/>')
                        .append($('<input/>').attr('type', 'checkbox').attr('class', 'row-select')));

                updateTeamMmr(weightedMmr);
            });
        });
    })();

    function updateTeamMmr(mmr) {
        ++players;
        teamMmr += mmr;
        $('#team-mmr').text('Team MMR: ' + (teamMmr / players).toFixed(0));
    }

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

        total = (total / selectedRows.length).toFixed(0);

        $('#selected-mmr').text(baseHeader + total);

    }

    // Select all the available heroes when the 'select all' checkbox is checked
    $('.row-select-all').change(function () {
        $(this).closest('table').find('.row-select').prop('checked', $(this).is(':checked')).trigger('change');
    });

    // When a player is selected/deselected, update the selected average MMR
    $('div.container-fluid tbody').on('change', '.player-row .row-select', updateSelectedMmr);
    //$('.player-row .row-select').change(updateSelectedMmr);


    // Collapse/expand player hero stats tables
    $("button.collapse-player-row").click(function (ev) {
        ev.preventDefault();
        var attr = this.getAttribute('data-target');
        $(attr).toggleClass("in");
    });

});