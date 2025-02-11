﻿async function callFileCollections(urlLoad) {
    return $('#fileCollections').load(urlLoad);
}

async function savegallery(urlAjax, urlLoad) {
    var arr = [];
    await $('table#tableGal').find('tbody').each(function () {
        $(this).find('tr').each(function () {
            $(this).find('td#rowVal').each(function () {
                var obj = {};
                obj.EForm_Gallery_Id = $(this).find('#item_EForm_Gallery_Id').val();
                obj.EForm_Gallery_Seq = $(this).find('#item_EForm_Gallery_Seq').val();
                arr.push(obj);
            });
        });
    });
    $.ajax({
        url: urlAjax,
        async: true,
        data: {
            model: JSON.stringify(arr)
        },
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
                    return callFileCollections(urlLoad);
                }
            });
        }
    });
}

async function deleteFiles(urlAjax, urlLoad) {
    swal({
        title: 'Are you sure?',
        text: 'Once deleted, you will not be able to recover this file',
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
                            reloadTable();
                            return callFileCollections(urlLoad);
                        }
                    });
                }
            });
        }
    });
}

async function callModalEForms(urlAjax, urlLoad = '', bigSize = false) {
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

            if (urlLoad != '') {
                callFileCollections(urlLoad);
            }

            $('#modalArea').modal('show');
        }
    });
}

async function previewMultiple(event) {
    $('#galImage').empty();

    var saida = await document.getElementById('fileImage');
    var quantos = await saida.files.length;
    for (i = 0; i < quantos; i++) {
        var urls = URL.createObjectURL(event.target.files[i]);

        var filetype = event.target.files[i].type.split('/')[0];
        if (filetype == 'image') {
            document.getElementById('galImage').innerHTML += '<img src="' + urls + '"class="img-fluid img-thumbnail mr-1" style="height:100px" title="' + event.target.files[i].name + '">';
        }
        else {
            document.getElementById('galImage').innerHTML += '<i class="fa fa-file-text-o fa-5x" style="height:100px" title="' + event.target.files[i].name + '"></i>';
        }
    }
}
