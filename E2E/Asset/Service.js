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
            $('#modalArea').modal('show');
        }
    });
    return false;
}