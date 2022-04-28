﻿function callModalForm(urlAjax, bigSize = false) {
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

function callModalCommit(urlAjax, urlLoad) {
    $.ajax({
        url: urlAjax,
        async: true,
        success: function (res) {
            if (!$('#modalContent').parent().hasClass('modal-lg')) {
                $('#modalContent').parent().addClass('modal-lg');
            }

            $('#modalContent').html(res);
            $('#modalContent').find('select').each(function () {
                $(this).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            });

            $('#serviceRefList').load(urlLoad);

            $('#modalArea').modal('show');
        }
    });
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

function setDateRange(urlAjax, val) {
    $.ajax({
        url: urlAjax,
        data: {
            id: val
        },
        async: true,
        success: function (res) {
            var now = new Date();
            $('#Service_DueDate').attr('min', '');
        }
    });
}

function setRequired(urlAjax) {
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
                $('#modalArea').modal('hide');
                if (reloadPage) {
                    location.reload();
                }
                else {
                    reloadTable();
                }
            }
        }
    });
    return false;
}