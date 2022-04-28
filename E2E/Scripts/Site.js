$(function () {
    AdjustMenu();
    $(window).resize(function () {
        AdjustMenu();
    });

    var classEmpty = true;
    var url = window.location.pathname,
        urlRegExp = new RegExp(url.replace(/\/$/, '') + "$");

    $('li.nav-item a').each(function () {
        if (classEmpty) {
            if (urlRegExp.test(this.href.replace(/\/$/, ''))) {
                $(this).addClass('active');
                classEmpty = false;
            }
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
            .columns.adjust();
    });
    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
            .columns.adjust();
    });
});
$(document).ajaxStart(function () {
    callSpin(true);
}).ajaxStop(function () {
    callSpin(false);
});
function AdjustMenu() {
    var sidebarHeight = $('#sidebar').innerHeight();
    var brandHeight = $('#brand').innerHeight();
    var menuHeight = sidebarHeight - brandHeight;
    $('#menu').innerHeight(menuHeight);
}
function reloadCount() {
    $('#nav_service').load(link_navService);
}
function callSpin(active) {
    var opts = {
        lines: 13, // The number of lines to draw
        length: 38, // The length of each line
        width: 17, // The line thickness
        radius: 45, // The radius of the inner circle
        scale: 1, // Scales overall size of the spinner
        corners: 1, // Corner roundness (0..1)
        speed: 1, // Rounds per second
        rotate: 0, // The rotation offset
        animation: 'spinner-line-fade-quick', // The CSS animation name for the lines
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#ffffff', // CSS color or array of colors
        fadeColor: 'transparent', // CSS color or array of colors
        top: '50%', // Top position relative to parent
        left: '50%', // Left position relative to parent
        shadow: '0 0 1px transparent', // Box-shadow for the lines
        zIndex: 2000000000, // The z-index (defaults to 2e9)
        className: 'spinner', // The CSS class to assign to the spinner
        position: 'absolute', // Element positioning
    };

    var target = document.getElementById('objSpin');
    var spinner = new Spinner(opts).spin(target);

    if (active) {
        target.appendChild(spinner.el);
    }
    else {
        $(target).empty();
    }
}
async function callTable(urlAjax, hasDate = false, hasButton = false, dateCol = 0, blockId = '#datalist') {
    $.ajax({
        url: urlAjax,
        async: true,
        success: function (res) {
            $(blockId).html(res);
            var table;
            $(blockId).find('table').each(function () {
                if (hasDate && hasButton) {
                    table = $(this).DataTable({
                        "columnDefs": [{ "targets": dateCol, "type": "date" }, { "targets": 0, "orderable": false }],
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
                else if (hasDate) {
                    table = $(this).DataTable({
                        "columnDefs": [{ "targets": dateCol, "type": "date" }],
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
                else if (hasButton) {
                    table = $(this).DataTable({
                        "columnDefs": [{ "targets": 0, "orderable": false }],
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
                else {
                    table = $(this).DataTable({
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
            });
            table.columns.adjust();
            reloadCount();
        }
    });
    return true;
}
async function callTable_NoSort(urlAjax, hasDate = false, dateCol = 0, blockId = '#datalist') {
    $.ajax({
        url: urlAjax,
        async: true,
        success: function (res) {
            $(blockId).html(res);
            var table;
            $(blockId).find('table').each(function (i, v) {
                if (hasDate) {
                    table = $(this).DataTable({
                        "columnDefs": [{ "targets": dateCol, "type": "date" }],
                        "ordering": false,
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
                else {
                    table = $(this).DataTable({
                        "ordering": false,
                        "scrollX": true,
                        "autoWidth": false
                    });
                }
            });
            table.columns.adjust();
            reloadCount();
        }
    });
    return true;
}
function setTable_File(tableId,bOrder = false,bSearch = false) {
    var table = $(tableId).DataTable({
        "ordering": bOrder,
        "searching": bSearch
    });
}
function setDropdown_Form() {
    $('form').find('select').each(function () {
        $(this).select2({
            theme: 'bootstrap4',
            width: '100%'
        });
    });
}
function callModal(urlAjax, bigSize = false) {
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
            $('#modalArea').modal('show');
        }
    });
    return false;
}
function callModalTable(urlAjax, bigSize = false) {
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
            $('#modalContent').find('table').each(function () {
                $(this).DataTable();
            });
            $('#modalArea').modal('show');
        }
    });
    return false;
}
function callSubmit(urlAjax, reloadPage = false) {
    var form = $('form')[0];
    var fd = new FormData(form);

    $.ajax({
        url: urlAjax,
        method: "POST",
        async: true,
        data: fd,
        processData: false,
        contentType: false,
        traditional: true,
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
function callDeleteItem(urlAjax, reloadPage = false) {
    swal({
        title: "Are you sure?",
        text: "Once you delete this information, you cannot recover it.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((cf) => {
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
    });
}
function SignoutNotify(url) {
    swal({
        title: "Are you sure?",
        text: "Signout",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((cf) => {
        if (cf) {
            location.href = url;
        }
    });
}
function getSelectOp(urlAjax, val, desSelectId) {
    var eSelect = $(desSelectId);
    eSelect.empty();
    $.ajax({
        url: urlAjax,
        data: {
            id: val
        },
        async: true,
        success: function (res) {
            $.each(res, function (i, v) {
                eSelect.append(new Option(v.Text, v.Value));
            });
        }
    });
}