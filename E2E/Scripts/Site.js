//For external
const baseUrl = '/E2E';

//For local
//const baseUrl = '';
let chat;

$(function () {
    let classEmpty = true;
    const url = window.location.pathname,
        urlRegExp = new RegExp(url.replace(/\/$/, '') + '$');

    $('#navbar_top').find('ul.navbar-nav').each(function () {
        $(this).find('li.nav-item a').each(function () {
            if (classEmpty) {
                if (urlRegExp.test(this.href.replace(/\/$/, ''))) {
                    $(this).addClass('active');
                    classEmpty = false;
                }
            }
        });
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
            .columns.adjust();
    });
    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
            .columns.adjust();
    });

    reloadCount().then(function () {
        scrollFunction();
    });
});
$(document).ajaxStart(function () {
    callSpin(true);
}).ajaxStop(function () {
    callSpin(false);
});

async function reloadCount() {
    await $('#nav_service').load(baseUrl + '/Configurations/_NavService');
    await $('#nav_department').load(baseUrl + '/Configurations/_NavDepartment');
    await $('#nav_Newtopic').load(baseUrl + '/Topics/_Newtopic');
    await $('._reloadCountA').load(baseUrl + '/Topics/_SortTopicAnnounce');
    await $('._reloadCountN').load(baseUrl + '/Topics/_SortTopicNew');
}
async function callSpin(active) {
    const opts = {
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
        className: 'spinner', // The CSS class to assign to the spinner
        position: 'fixed', // Element positioning
    };

    const target = await document.getElementById('objSpin');
    const spinner = await new Spinner(opts).spin(target);

    if (active) {
        await target.appendChild(spinner.el);
    }
    else {
        await $(target).empty();
    }
}

function getQueryString() {
    let pairs = window.location.search.substring(1).split('&'),
        obj = {},
        pair,
        i;

    for (i in pairs) {
        if (pairs[i] === '') continue;

        pair = pairs[i].split('=');
        obj[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
    }

    return JSON.stringify(obj);
}

async function getQueryStringName(param) {
    const url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');

    for (let i = 0; i < url.length; i++) {
        let urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }

    return 'empty';
}

async function clearQueryString() {
    await history.pushState({}, null, location.href.split('?')[0]);
    return location.reload();
}

async function callTable(urlAjax, hasDate = false, hasButton = false, dateCol = [], blockId = '#datalist') {
    return $.ajax({
        url: urlAjax,
        async: true,
        data: {
            filter: getQueryString()
        },
        success: function (res) {
            $(blockId).html(res);
            $(blockId).find('select').each(function () {
                $(this).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            });

            let targetArr = [];
            let targetObj = {};

            $.each(dateCol, function (key, val) {
                targetObj = {};
                targetObj.targets = val;
                targetObj.type = 'date';
                targetArr.push(targetObj);
            });

            $(blockId).find('table').each(function () {
                if (hasDate && hasButton) {
                    targetObj = {};
                    targetObj.targets = 0;
                    targetObj.orderable = false;
                    targetArr.push(targetObj);

                    $(this).DataTable({
                        'columnDefs': targetArr,
                        'order': [[dateCol[0], 'desc']],
                        'scrollX': true
                    });
                }
                else if (hasDate) {
                    $(this).DataTable({
                        'columnDefs': targetArr,
                        'order': [[dateCol[0], 'desc']],
                        'scrollX': true
                    });
                }
                else if (hasButton) {
                    $(this).DataTable({
                        'columnDefs': [{ 'targets': 0, 'orderable': false }],
                        'scrollX': true
                    });
                }
                else {
                    $(this).DataTable({
                        'scrollX': true
                    });
                }
            });
        }
    });
}
async function callTable_NoSort(urlAjax, hasDate = false, dateCol = [], blockId = '#datalist') {
    return $.ajax({
        url: urlAjax,
        async: true,
        data: {
            filter: getQueryString()
        },
        success: function (res) {
            $(blockId).html(res);
            $(blockId).find('select').each(function () {
                $(this).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            });
            let table;
            $(blockId).find('table').each(function (i, v) {
                if (hasDate) {
                    let targetArr = [];
                    let targetObj = {};

                    $.each(dateCol, function (key, val) {
                        targetObj = {};
                        targetObj.targets = val;
                        targetObj.type = 'date';
                        targetArr.push(targetObj);
                    });

                    table = $(this).DataTable({
                        'columnDefs': targetArr,
                        'ordering': false,
                        'scrollX': true
                    });
                }
                else {
                    table = $(this).DataTable({
                        'ordering': false,
                        'scrollX': true
                    });
                }
            });
            table.columns.adjust();
        }
    });
}

async function callFilter(urlAjax, blockId = '#filter') {
    return $.ajax({
        url: urlAjax,
        async: true,
        cache: false,
        contentType: 'application/json',
        data: {
            filter: getQueryString()
        },
        success: function (res) {
            $(blockId).html(res).fadeIn(500);
            $(blockId).find('select').each(function () {
                $(this).select2({
                    width: '100%',
                    theme: 'bootstrap4'
                });
            });
        }
    });
}

async function setTable_File(tableId, bOrder = false, bSearch = false) {
    return table = await $(tableId).DataTable({
        'ordering': bOrder,
        'searching': bSearch
    });
}
async function setDropdown_Form() {
    return $('form').find('select').each(function () {
        $(this).select2({
            theme: 'bootstrap4',
            width: '100%'
        });
    });
}
async function callModal(urlAjax, bigSize = false, callback = null) {
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
                });
            });
            if (callback != null) {
                callback();
            }
            $('#modalArea').modal('show');
        }
    });
}
async function callModalTable(urlAjax, bigSize = false) {
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
                });
            });
            $('#modalContent').find('table').each(function () {
                $(this).DataTable();
            });
            $('#modalArea').modal('show');
        }
    });
}
async function callSubmitModal(urlAjax, form) {
    return swal({
        title: 'Are you sure?',
        text: 'This information is saved to the database.',
        buttons: true,
        icon: 'warning'
    }).then(function (cf) {
        if (cf) {
            const fd = new FormData(form);
            $.ajax({
                url: urlAjax,
                method: 'POST',
                async: true,
                data: fd,
                processData: false,
                contentType: false,
                traditional: true,
                success: function (res) {
                    swal({
                        title: res.Title,
                        text: res.Text,
                        icon: res.Icon,
                        button: res.Button,
                        dangerMode: res.DangerMode
                    }).then(function (e) {
                        if (res.Icon == 'success') {
                            $('#modalArea').modal('hide');
                            reloadCount().then(function () {
                                reloadTable();
                            });
                        }
                    });
                }
            });
        }
    });
}

async function callSubmitPage(urlAjax, form) {
    return swal({
        title: 'Are you sure?',
        text: 'This information is saved to the database.',
        buttons: true,
        icon: 'warning'
    }).then(function (cf) {
        if (cf) {
            const fd = new FormData(form);

            $.ajax({
                url: urlAjax,
                method: 'POST',
                async: true,
                data: fd,
                processData: false,
                contentType: false,
                traditional: true,
                success: function (res) {
                    swal({
                        title: res.Title,
                        text: res.Text,
                        icon: res.Icon,
                        button: res.Button,
                        dangerMode: res.DangerMode
                    }).then(function (e) {
                        if (res.Icon == 'success') {
                            window.location.reload();
                        }
                    });
                }
            });
        }
    });
}
async function callSubmitRedirect(urlAjax, form, urlRedirect) {
    return swal({
        title: 'Are you sure?',
        text: 'This information is saved to the database.',
        buttons: true,
        icon: 'warning'
    }).then((cf) => {
        if (cf) {
            const fd = new FormData(form);

            $.ajax({
                url: urlAjax,
                method: 'POST',
                async: true,
                data: fd,
                processData: false,
                contentType: false,
                traditional: true,
                success: function (res) {
                    swal({
                        title: res.Title,
                        text: res.Text,
                        icon: res.Icon,
                        button: res.Button,
                        dangerMode: res.DangerMode
                    }).then(function (e) {
                        if (res.Icon == 'success') {
                            if (res.Option != null) {
                                urlRedirect += '/' + res.Option;
                            }

                            window.location.href = urlRedirect;
                        }
                    });
                }
            });
        }
    });
}
async function callDeleteItem(urlAjax, reloadPage = false) {
    return swal({
        title: 'Are you sure?',
        text: 'Once you delete this information, you cannot recover it.',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    }).then((cf) => {
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
                            $('#modalArea').modal('hide');
                            if (reloadPage) {
                                location.reload();
                            }
                            else {
                                reloadTable();
                            }
                        }
                    });
                }
            });
        }
    });
}
async function notifySignout(url) {
    return swal({
        title: 'Are you sure?',
        text: 'Signout',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    }).then((cf) => {
        if (cf) {
            location.href = url;
        }
    });
}
async function getSelectOp(urlAjax, val, desSelectId) {
    const eSelect = $(desSelectId);
    eSelect.empty();
    return $.ajax({
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
async function setSuccessByIdRePage(urlAjax) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true
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
                                location.reload();
                            }
                        });
                    }
                });
            }
        });
}

async function setDangerByIdRePage(urlAjax) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true,
        dangerMode: true
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
                                location.reload();
                            }
                        });
                    }
                });
            }
        });
}

async function setSuccessByIdReTable(urlAjax) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true
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
                                reloadTable();
                            }
                        });
                    }
                });
            }
        });
}

async function setDangerByIdReTable(urlAjax) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true,
        dangerMode: true
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
                                reloadTable();
                            }
                        });
                    }
                });
            }
        });
}

async function setSuccessByIdReDirect(urlAjax, urlRedirect) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true
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
                                if (res.Option != null) {
                                    urlRedirect += '/' + res.Option;
                                }

                                window.location.href = urlRedirect;
                            }
                        });
                    }
                });
            }
        });
}

async function setDangerByIdReDirect(urlAjax, urlRedirect) {
    return swal({
        title: 'Are you sure?',
        text: 'If is confirmed, it cannot be reversed.',
        icon: 'warning',
        buttons: true,
        dangerMode: true
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
                                if (res.Option != null) {
                                    urlRedirect += '/' + res.Option;
                                }

                                window.location.href = urlRedirect;
                            }
                        });
                    }
                });
            }
        });
}

async function callDeleteIMG_SC(urlAjax) {
    return swal({
        title: 'Are you sure?',
        text: 'Once you delete this information, you cannot recover it.',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    }).then((cf) => {
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
                            const id = res.Option;

                            $('#MediaSC').empty();
                            $('#' + id).empty();
                        }
                    });
                }
            });
        }
    });
}

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () { scrollFunction() };

async function scrollFunction() {
    const top = $('#btnToTop');
    if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 300) {
        return top.fadeIn();
    } else {
        return top.fadeOut();
    }
}

// When the user clicks on the button, scroll to the top of the document
async function topFunction() {
    await window.scrollTo({ top: 0, behavior: 'smooth' });
}
