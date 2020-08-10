$(document).ready(function () {
    $(".mainNav").on('click', function (e) {
        $('.tabDivClass').hide();
        var tag = e.currentTarget;
        $('.mainNav').removeClass('active');
        let newClasses = 'active ' + $(tag).attr('class');
        $(tag).attr('class', newClasses);
        let classlist = $(tag).attr('class').split(' ');
        let className = classlist[classlist.length - 1];
        if ($('.' + className).hasClass('d-none')) {
            $('.' + className).removeClass('d-none');
        }
        $('#' + className).show();
    });
});