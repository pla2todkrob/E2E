function callModalAddService(urlAjax, modalSize = '') {
    $.ajax({
        url: urlAjax,
        async: true,
        success: function (res) {
            if (modalSize != '') {
                $('#modalContent').parent().addClass(modalSize);
            }
            else {
                $('#modalContent').parent().removeClass(modalSize);
            }

            $('#modalContent').html(res);
            $('#modalContent').find('select').each(function () {
                $(this).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            });

            $('#Ref_Service_Id').on('select2:select', function () {
            });

            $('#modalArea').modal('show');
        }
    });
    return false;
}

function getOwner(val) {
    $.ajax({
        url: '/Services/GetOwner/' + val,
        async: true,
        success: function (res) {
            $('#User_Id').val(res);
        }
    });
    return false;
}