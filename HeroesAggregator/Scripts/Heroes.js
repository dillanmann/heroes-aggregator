$(document).ready(function () {

    var teamMmr = 0;
    var players = 0;

    function createCollapsibleTableRow(target) {
        var row = $('<tr/>').attr('class', 'player-collapse ' + target);
        return row.append($('<td/>').append($('<table/>').attr('class', 'table')
            .append($('<thead>')
                .append($('<th/>').text('Hero'))
                .append($('<th/>').text('Games Played'))
                .append($('<th/>').text("Win %")))
                .append($('<tbody/>'))));
    }

    (function getPlayerDetails() {
        var urlRoot = '/heroes/playermmr?playerid=';
        $.each($('.player-row'), function (index, value) {

            var playerRow = $(this);
            var playerId = $(value).attr('player-id');

            // Build the MMR table
            var mmrUrl = urlRoot + playerId;
            $.getJSON(mmrUrl, function (data) {
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

            // Build the collapsible hero stats table
            var playerName = $(this).find(".player-name").text();
            var collapseTarget = "heroes-pref-" + playerName.replace(' ', '-');
            var statsUrl = '/heroes/playerherostats?playerid=' + playerId;
            $.getJSON(statsUrl, function (data) {
                var row = createCollapsibleTableRow(collapseTarget);
                var tbody = row.find('tbody');

                if (!data || data == "null") {
                    tbody.append($('<tr/>').text('Failed to find HotSLogs profile for player ' + playerName));
                }
                else {
                    var heroStats = JSON.parse(data).HeroStats;
                    heroStats.forEach(function (obj) {
                        tbody.append($('<tr/>')
                            .append($('<td/>').text(obj['Name']))
                            .append($('<td/>').text(obj['GamesPlayed']))
                            .append($('<td/>').text(obj['WinPercent'])));
                    });
                }

                playerRow.after(row);
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