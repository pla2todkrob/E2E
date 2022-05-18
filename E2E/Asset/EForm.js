function callFileCollections(urlLoad) {
    $('#fileCollections').load(urlLoad);
}

function deleteFileEF(urlAjax, urlLoad) {
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
                            reloadTable();
                            callFileCollections(urlLoad);
                        }
                    }
                });
                return false;
            }
        });
}

function callModalEForms(urlAjax, urlLoad = '', bigSize = false) {
    $.ajax({
        url: urlAjax,
        async: true,
        success: function (res) {
            if (bigSize) {
                $('#modalContent').parent().addClass('modal-lg');
            }
            else {
                $('#modalContent').parent().removeClass('modal-lg');
            }

            $('#modalContent').html(res);
            $('#modalContent').find('select').each(function () {
                $(this).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            });

            if (urlLoad != '') {
                callFileCollections(urlLoad);
            }

            $('#modalArea').modal('show');
        }
    });
    return false;
}