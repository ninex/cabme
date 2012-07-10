
$(document).ready(function () {
    $('#tab1').show();
    $('#tab2').hide();
    $('#tab3').hide();
    $('#htab1').addClass('current');

    $('#htab1').click(function () {
        $('h3').removeClass('current');
        $('#tab2').hide();
        $('#tab3').hide();
        $('#tab1').fadeIn();
        $('#htab1').addClass('current');
    });
    $('#htab2').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab3').hide();
        $('#tab2').fadeIn();
        $('#htab2').addClass('current');
    });
    $('#htab3').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab2').hide();
        $('#tab3').fadeIn();
        $('#htab3').addClass('current');
    });
});