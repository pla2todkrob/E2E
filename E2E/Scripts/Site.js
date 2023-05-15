let siteName = 'E2E';
let baseUrl = window.location.pathname.search(siteName) < 0 ? '' : `/${siteName}`;

let chat;

$(function () {
    let classEmpty = true;
    const urlPathName = window.location.pathname,
        urlRegExp = new RegExp(urlPathName.replace(/\/$/, '') + '$');
    const $navbarTop = $('#navbar_top');
    const $navbarTopUl = $navbarTop.find('ul.navbar-nav');
    $navbarTopUl.each(function () {
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
    const navService = $('#nav_service').load(`${baseUrl}/Configurations/_NavService`);
    const navDepartment = $('#nav_department').load(`${baseUrl}/Configurations/_NavDepartment`);
    const navNewTopic = $('#nav_Newtopic').load(`${baseUrl}/Topics/_Newtopic`);
    const reloadCountA = $('._reloadCountA').load(`${baseUrl}/Topics/_SortTopicAnnounce`);
    const reloadCountN = $('._reloadCountN').load(`${baseUrl}/Topics/_SortTopicNew`);

    await Promise.all([navService, navDepartment, navNewTopic, reloadCountA, reloadCountN]);
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
        document.querySelector("body").classList.add("disabled");
        target.appendChild(spinner.el);
    }
    else {
        document.querySelector("body").classList.remove("disabled");
        $(target).empty();
    }
}

function getQueryString() {
    if (window.location.search === "") {
        return '{}'
    }
    let pairs = window.location.search.substring(1).split('&'),
        obj = {};
    pairs.forEach(function (element) {
        if (element === '') return;
        pair = element.split('=');
        obj[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
    });
    return JSON.stringify(obj)
}

function getQueryStringName(param) {
    const urlSplit = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    const queryString = urlSplit.find(ele => ele.split('=')[0] === param);
    if (queryString) {
        return queryString.split('=')[1]
    }
    else {
        return 'empty'
    }
}

function clearQueryString() {
    history.pushState({}, null, location.href.split('?')[0]);
    location.reload();
}

// Helper function for creating a DataTable with the given options
async function createDataTable(tableId, options) {
    return await $(tableId).DataTable(options);
}

// Function for fetching and displaying data in a table
async function callTable(urlAjax, hasDate = false, hasButton = false, dateCol = [], blockId = '#datalist') {
    let url = urlAjax;
    // check if urlAjax already has query string
    if (urlAjax.indexOf("?") === -1) {
        url += "?filter=" + getQueryString();
    } else {
        url += "&filter=" + getQueryString();
    }

    try {
        // Make the GET request using the async/await pattern
        const res = await $.ajax({
            url: url,
            method: 'GET',
        });
        // append the data to blockId
        $(blockId).html(res);
        $(blockId).find('select').each(function () {
            $(this).select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        });
        let targetArr = [];
        let targetObj = {};
        if (hasDate) {
            $.each(dateCol, function (key, val) {
                targetObj = {};
                targetObj.targets = val;
                targetObj.type = 'datetime';
                targetArr.push(targetObj);
            });
        }
        if (hasButton) {
            targetObj = {};
            targetObj.targets = 0;
            targetObj.orderable = false;
            targetArr.push(targetObj);
        }

        // create datatable with options
        $(blockId).find('table').each(async function (i, v) {
            if (hasDate || hasButton) {
                await createDataTable(v, {
                    'columnDefs': targetArr,
                    'order': [[dateCol[0], 'desc']],
                    'scrollX': true
                });
            } else {
                await createDataTable(v, {
                    'scrollX': true
                });
            }
        });
    } catch (error) {
        // Handle the error
        console.error(error);
    }
}

async function callTable_Normal(urlAjax, blockId = '#datalist') {
    let url = urlAjax;
    // check if urlAjax already has query string
    if (urlAjax.indexOf("?") === -1) {
        url += "?filter=" + getQueryString();
    } else {
        url += "&filter=" + getQueryString();
    }

    try {
        // Make the GET request using the async/await pattern
        const res = await $.ajax({
            url: url,
            method: 'GET',
        });
        // append the data to blockId
        $(blockId).html(res);
        $(blockId).find('select').each(function () {
            $(this).select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        });
    } catch (error) {
        // Handle the error
        console.error(error);
    }
}

async function callTable_NoSort(urlAjax, blockId = '#datalist') {
    let url = urlAjax;
    // check if urlAjax already has query string
    if (urlAjax.indexOf("?") === -1) {
        url += "?filter=" + getQueryString();
    } else {
        url += "&filter=" + getQueryString();
    }

    try {
        // Make the GET request using the async/await pattern
        const res = await $.ajax({
            url: url,
            method: 'GET'
        });
        // append the data to blockId
        $(blockId).html(res);
        $(blockId).find('select').each(function () {
            $(this).select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        });

        let table;
        // create datatable with no sort
        $(blockId).find('table').each(async function (i, v) {
            table = await createDataTable(v, {
                'ordering': false,
                'scrollX': true
            });
        });
    } catch (error) {
        // Handle the error
        console.error(error);
    }
}

function callFilter(urlAjax, blockId = '#filter') {
    $.ajax({
        url: `${urlAjax}?filter=${getQueryString()}`,
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            const block = $(blockId);
            block.html(data);
            block.css({
                display: 'block',
                opacity: 0,
                transition: 'opacity 500ms'
            });
            block.animate({ opacity: 1 }, 500);
            block.find('select').css('width', '100%');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(`Failed to fetch data, status code: ${jqXHR.status}`);
        }
    });
}

async function callData(urlAjax, blockId = '#datalist') {
    try {
        const res = await $.ajax({
            url: urlAjax,
            method: 'GET'
        });
        // append the data to blockId
        $(blockId).html(res);
    } catch (e) {
        console.error(e);
    }
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

async function callModal(urlAjax, options = { bigSize: false, callback: null }) {
    try {
        const res = await $.ajax({
            url: urlAjax,
            async: true,
        });

        if (options.bigSize) {
            $('#modalContent').parent().addClass('modal-lg');
        } else {
            $('#modalContent').parent().removeClass('modal-lg');
        }

        $('#modalContent').html(res);
        $('#modalContent').find('select').each(function () {
            $(this).select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        });
        if (options.callback) {
            options.callback();
        }
        $('#modalArea').modal('show');
    } catch (error) {
        console.error(error);
    }
}

function callSubmitModal(urlAjax, form) {
    swal({
        title: 'Are you sure?',
        text: 'This information will be saved to the database.',
        buttons: true,
        icon: 'warning'
    }).then(function (confirmed) {
        if (confirmed) {
            const fd = new FormData(form);
            $.ajax({
                url: urlAjax,
                method: 'POST',
                data: fd,
                processData: false,
                contentType: false,
                dataType: 'json',
                success: function (json) {
                    swal({
                        title: json.Title,
                        text: json.Text,
                        icon: json.Icon,
                        button: json.Button,
                        dangerMode: json.DangerMode
                    }).then(function () {
                        if (json.Icon === 'success') {
                            $('#modalArea').modal('hide');
                            reloadCount().then(function () {
                                reloadTable();
                            });
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error(`An error occured while submitting the form: ${errorThrown}`);
                    swal({
                        title: 'Error',
                        text: 'An error occured while submitting the form.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}


function callSubmitPage(urlAjax, form) {
    swal({
        title: 'Are you sure?',
        text: 'This information will be saved to the database.',
        buttons: true,
        icon: 'warning'
    }).then((confirmed) => {
        if (confirmed) {
            const fd = new FormData(form);
            $.ajax({
                url: urlAjax,
                type: 'POST',
                data: fd,
                processData: false,
                contentType: false,
                success: (json) => {
                    swal({
                        title: json.Title,
                        text: json.Text,
                        icon: json.Icon,
                        button: json.Button,
                        dangerMode: json.DangerMode
                    }).then(() => {
                        if (json.Icon === 'success') {
                            window.location.reload();
                        }
                    });
                },
                error: (error) => {
                    console.error(error);
                    swal({
                        title: 'Error',
                        text: 'An error occured while submitting the form.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}


function callSubmitRedirect(urlAjax, form, urlRedirect) {
    const formData = new FormData(form);
    return $.ajax({
        url: urlAjax,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (json) {
            swal({
                title: json.Title,
                text: json.Text,
                icon: json.Icon,
                button: json.Button,
                dangerMode: json.DangerMode
            }).then(function () {
                if (json.Icon === 'success') {
                    if (json.Option != null) {
                        urlRedirect += '/' + json.Option;
                    }
                    window.location.href = urlRedirect;
                }
            });
        },
        error: function (xhr, status, error) {
            console.error(error);
            swal({
                title: 'Error',
                text: 'An error occurred while submitting the form.',
                icon: 'error'
            });
        }
    });
}


function callDeleteItem(urlAjax, reloadPage = false) {
    $.ajax({
        url: urlAjax,
        type: 'DELETE',
        success: function (json) {
            swal({
                title: json.Title,
                text: json.Text,
                icon: json.Icon,
                button: json.Button,
                dangerMode: json.DangerMode
            }).then(function () {
                if (json.Icon === 'success') {
                    $('#modalArea').modal('hide');
                    if (reloadPage) {
                        location.reload();
                    } else {
                        reloadTable();
                    }
                }
            });
        },
        error: function (error) {
            console.error(error);
            swal({
                title: 'Error',
                text: 'An error occured while deleting the item.',
                icon: 'error'
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

function getSelectOp(url, val, desSelectId) {
    const eSelect = $(desSelectId);
    eSelect.empty();
    $.ajax({
        url: url,
        type: 'GET',
        data: { id: val },
        dataType: 'json',
        success: function (data) {
            appendOptions(eSelect, data);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error(`Request failed with status code ${xhr.status}`);
        }
    });
}

function appendOptions(eSelect, options) {
    options.forEach(option => {
        eSelect.append(new Option(option.Text, option.Value));
    });
}

const warningConfirm = {
    title: 'Are you sure?',
    text: 'If is confirmed, it cannot be reversed.',
    icon: 'warning',
    buttons: true,
    dangerMode: false
};

async function confirmAndPerformAjaxRequest(urlAjax, action, option = { urlRedirect: '', isDangerous: false }) {
    warningConfirm.dangerMode = option.isDangerous;
    const confirm = await swal(warningConfirm);
    if (confirm) {
        try {
            const res = await $.ajax({
                url: urlAjax,
                async: true,
            });
            swal({
                title: res.Title,
                text: res.Text,
                icon: res.Icon,
                button: res.Button,
                dangerMode: res.DangerMode
            });
            if (res.Icon === 'success') {
                switch (action) {
                    case 'reloadPage':
                        location.reload();
                        break;
                    case 'reloadTable':
                        reloadTable();
                        break;
                    case 'redirect':
                        window.location.href = option.urlRedirect;
                        break;
                    default:
                        console.log("Invalid Action type")
                }
            }
        } catch (error) {
            console.error(error);
        }
    }
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

let lastScrollTop = window.pageYOffset;
const eleNav = document.querySelector('nav.navbar');
const topButton = document.getElementById('btnToTop');
window.addEventListener("scroll", debounce(scrollFunction));

function scrollFunction() {
    let totalScroll = document.documentElement.scrollHeight - window.innerHeight;
    totalScroll = totalScroll * 0.25;

    let scrollTop = window.pageYOffset;
    if (scrollTop > lastScrollTop) {
        eleNav.style.top = `-${eleNav.offsetHeight}px`;
    }
    else {
        eleNav.style.top = '0';
    }

    if (scrollTop > totalScroll) {
        topButton.style.bottom = '0';
    } else {
        topButton.style.bottom = '-100%';
    }
    
    lastScrollTop = scrollTop;
}

function debounce(func, wait = 50) {
    let timeout;
    return function () {
        clearTimeout(timeout);
        timeout = setTimeout(func, wait);
    }
}

// When the user clicks on the button, scroll to the top of the document
async function topFunction() {
    await window.scrollTo({ top: 0, behavior: 'smooth' });
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function setCookie(name, value, expires = 1) {
    var date = new Date();
    date.setTime(date.getTime() + (expires * 24 * 60 * 60 * 1000));
    var expires = "expires=" + date.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}
