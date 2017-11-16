$(document).ready(function () {
    var url = "/heroes/teammmr/";

    $('#submit-team-id').on('click', function () {
        $('#loader img').attr("style", "display: block;").appendTo($(this).text(""));
        window.location.href = url + $('#team-id').val();
    });
    $('#team-id').on('input', function () {
        $('#submit-team-id').prop('disabled', !$(this).val())
    });
});