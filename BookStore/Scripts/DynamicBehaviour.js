$(function(){
    $('#BookList').on('click', function () {
        if ($(this).is(':clicked')) {
            $('#ListOfRelatedBooks').show();
        } else {
            $('#ListOfRelatedBooks').hide();
        }
    })
});
