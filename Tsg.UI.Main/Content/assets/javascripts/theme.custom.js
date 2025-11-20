/* Add here all your JS customizations */

$(function () {

    // basic
    $("#form").validate({
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        }
    });

    var closeMessageBtn = $("#btn-close-message");
    if (closeMessageBtn.length > 0) {
        closeMessageBtn.click(function () {
            $.ajax({
                type: 'POST',
                url: '/User/HideMessage',
                data: 'display=false',
                success: function (response) {
                    //document.location.reload();
                }
            });
        });
    }
    

});
