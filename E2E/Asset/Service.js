function callModalForm(urlAjax, urlGetDate, urlGetRef, bigSize = false) {
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
                setUserRef(urlGetRef, e.params.data.id);
            });

            $('#Priority_Id').on('select2:select', function (e) {
                setDateRange(urlGetDate, e.params.data.id);
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
                        }).then(function () {
                            if (res.icon == 'success') {
                                $('#fileTable').load(urlLoad);
                            }
                        });
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
            now.setDate(now.getDate() + res);

            $('#Service_DueDate').attr('min', moment(now).format('YYYY-MM-DD'));
        }
    });
}

function setRequired(urlAjax, urlRedirect) {
    swal({
        title: "Are you sure?",
        text: "This item will be sent to the requester's department manager.",
        buttons: true,
        icon: "warning"
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
                        }).then(function () {
                            if (res.icon == 'success') {
                                window.location.replace(urlRedirect);
                            }
                        });
                    }
                });
            }
        });

    return false;
}

function setCommitToDepartment(urlAjax, urlRedirect) {
    swal({
        title: "Are you sure?",
        text: "This item request will be imported to your department.",
        buttons: true,
        icon: "warning"
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
                        }).then(function () {
                            if (res.icon == 'success') {
                                window.location.replace(urlRedirect);
                            }
                        });
                    }
                });
            }
        });

    return false;
}

function setApprove(urlAjax) {
    swal({
        title: "Are you sure?",
        text: "If approval is confirmed, it cannot be reversed.",
        icon: "warning",
        buttons: true
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
                        }).then(function () {
                            if (res.icon == 'success') {
                                location.reload();
                            }
                        });
                        
                    }
                });
                return false;
            }
        });
}

function setUserRef(urlAjax, val) {
    $('#User_Id').val('').trigger('change');
    if (val != '') {
        $.ajax({
            url: urlAjax,
            data: {
                id: val
            },
            async: true,
            success: function (res) {
                $('#User_Id').val(res).trigger('change');
                $('#User_Id').prop('disabled', true);
            }
        });
    }
    else {
        $('#User_Id').prop('disabled', false);
    }
}