$(function () {
    adjustHeight();
    $(window).resize(function () {
        adjustHeight();
    });
});

async function adjustHeight() {
    if ($(document).find('#serviceInfo').length) {
        $('#serviceComment').innerHeight(0);
        var infoHeight = $('#serviceInfo').innerHeight();
        $('#serviceComment').innerHeight(infoHeight);
        $('#commentHis').animate({ scrollTop: 5000 });
    }

    if ($(document).find('#refSection').length) {
        $('#refSection').find('.refInfo').each(function () {
            $(this).find('.refComment').innerHeight(0);
            var dataHeight = $(this).find('.refData').innerHeight();
            $(this).find('.refComment').innerHeight(dataHeight);
        });
    }
}

function CheckNotClose(urlAjax, urlAjaxCHK, bigSize = false) {
    $.ajax({
        url: urlAjaxCHK,
        async: true,
        success: function (res) {
            if (res.option != null) {
                swal({
                    title: res.title,
                    text: res.text,
                    icon: res.icon,
                    button: res.button,
                    dangerMode: res.dangerMode
                }).then(function () {
                    if (res.icon == 'warning') {
                        window.location.href = baseUrl + '/Services/ServiceInfomation/' + res.option;
                    }
                });
            }
            else {
                callModalService(urlAjax, bigSize);
            }
        }
    });
    return false;
}

function callModalService(urlAjax, bigSize = false) {
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
                }).trigger('change.select2');
            });

            $('#User_Id').on('select2:select', function (e) {
                getSelectOp(baseUrl + '/Services/GetServiceRef', e.params.data.id, '#Ref_Service_Id');
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

function setDateRange(val) {
    $.ajax({
        url: baseUrl + '/Services/GetPriorityDateRange',
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