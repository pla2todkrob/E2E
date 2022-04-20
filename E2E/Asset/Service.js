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

            $('#Ref_Service_Id').on('select2:select', function (e) {
            });

            $('#Priority_Id').on('select2:select', function (e) {
                setDateRange(e.params.data.id);
            });

            $('#modalArea').modal('show');
        }
    });
    return false;
}

function deleteFile(urlAjax, urlLoad) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this file",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((cf) => {
            if (cf) {
                $.ajax({
                    url: urlAjax,
                    async: true,
                    success: function (res) {
                        swal({
                            title: res.title,
                            text: res.text,
                            icon: res.icon,
                            button: res.button,
                            dangerMode: res.dangerMode
                        });
                        if (res.icon == 'success') {
                            $('#fileTable').load(urlLoad);
                        }
                    }
                });
                return false;
            }
        });
}

function setDateRange(id) {
    $.ajax({
        url: '',
        async: true,
        success: function (res) {
            var now = new Date();
            $('#Service_DueDate').attr('min', '');
        }
    });
}