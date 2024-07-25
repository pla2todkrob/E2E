$(function () {
    adjustHeight().then(function () {
        $(window).resize(function () {
            adjustHeight();
        });
    });
});

async function adjustHeight() {
    if ($(document).find('#serviceInfo').length) {
        $('#serviceComment').innerHeight(0);
        var infoHeight = $('#serviceInfo').innerHeight();
        $('#serviceComment').innerHeight(infoHeight);
        await bottomFunction('#commentHis');
    }

    if ($(document).find('#refSection').length) {
        $('#refSection').find('.refInfo').each(function () {
            $(this).find('.refComment').innerHeight(0);
            var dataHeight = $(this).find('.refData').innerHeight();
            $(this).find('.refComment').innerHeight(dataHeight);
        });
    }
}

async function CheckNotClose(urlAjax, urlAjaxCHK, bigSize = false) {
    $.ajax({
        url: urlAjaxCHK,
        async: true,
        success: function (res) {
            if (res.Option != null) {
                swal({
                    title: res.Title,
                    text: res.Text,
                    icon: res.Icon,
                    button: res.Button,
                    dangerMode: res.DangerMode
                }).then(function () {
                    if (res.Icon == 'warning') {
                        return window.location.href = baseUrl + '/Services/ServiceInfomation/' + res.Option;
                    }
                });
            }
            else {
                return callModalService(urlAjax, bigSize);
            }
        }
    });
}

async function callModalService(urlAjax, bigSize = false) {
    return $.ajax({
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
}

async function deleteFile(urlAjax, urlLoad) {
    swal({
        title: 'Are you sure?',
        text: 'Once deleted, you will not be able to recover this file',
        icon: 'warning',
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
                            title: res.Title,
                            text: res.Text,
                            icon: res.Icon,
                            button: res.Button,
                            dangerMode: res.DangerMode
                        }).then(function () {
                            if (res.Icon == 'success') {
                                return $('#fileTable').load(urlLoad);
                            }
                        });
                    }
                });
            }
        });
}

async function setDateRange(val) {
    return $.ajax({
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

async function setRequired(urlAjax, urlRedirect) {
    swal({
        title: `Are you sure?`,
        text: `This item will be sent to the requester's department manager.`,
        buttons: true,
        icon: `warning`
    })
        .then((cf) => {
            if (cf) {
                $.ajax({
                    url: urlAjax,
                    async: true,
                    success: function (res) {
                        swal({
                            title: res.Title,
                            text: res.Text,
                            icon: res.Icon,
                            button: res.Button,
                            dangerMode: res.DangerMode
                        }).then(function () {
                            if (res.Icon == 'success') {
                                return window.location.replace(urlRedirect);
                            }
                        });
                    }
                });
            }
        });
}

async function setCommitToDepartment(urlAjax, urlRedirect) {
    swal({
        title: 'Are you sure?',
        text: 'This item request will be imported to your department.',
        buttons: true,
        icon: 'warning'
    })
        .then((cf) => {
            if (cf) {
                $.ajax({
                    url: urlAjax,
                    async: true,
                    success: function (res) {
                        swal({
                            title: res.Title,
                            text: res.Text,
                            icon: res.Icon,
                            button: res.Button,
                            dangerMode: res.DangerMode
                        }).then(function () {
                            if (res.Icon == 'success') {
                                return window.location.replace(urlRedirect);
                            }
                        });
                    }
                });
            }
        });
}

async function resendEmail(urlAjax) {
    swal({
        title: 'Are you sure?',
        text: 'An email will be sent to the creator of the request again.',
        buttons: true,
        icon: 'warning'
    })
        .then((cf) => {
            if (cf) {
                $.ajax({
                    url: urlAjax,
                    async: true,
                    success: function (res) {
                        swal({
                            title: res.Title,
                            text: res.Text,
                            icon: res.Icon,
                            button: res.Button,
                            dangerMode: res.DangerMode
                        }).then(function () {
                            console.log('Notification closed');
                            location.reload(true);
                        });
                    }
                });
            }
        });
}
