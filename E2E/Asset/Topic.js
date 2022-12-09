async function callModalTopics(urlAjax, urlLoad = '', bigSize = false) {
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

            checkPin($('#Topic_Pin'));

            $('#Topic_Pin').on('change', function () {
                checkPin(this);
            });

            if (urlLoad != '') {
                callFileCollection(urlLoad);
            }

            $('#modalArea').modal('show');
        }
    });
}

async function savegallery_T(urlAjax, urlLoad) {
    var arr = [];
    await $('table#tableGal').find('tbody').each(function () {
        $(this).find('tr').each(function () {
            $(this).find('td#rowVal').each(function () {
                var obj = {};
                obj.TopicGallery_Id = $(this).find('#item_TopicGallery_Id').val();
                obj.TopicGallery_Seq = $(this).find('#item_TopicGallery_Seq').val();
                arr.push(obj);
            });
        });
    });
    return $.ajax({
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
            }).then(function (e) {
                if (res.Icon == 'success') {
                    reloadTable();
                    callFileCollection(urlLoad);
                }
            });
        }
    });
}

async function checkPin(ele) {
    if ($(ele).is(':checked')) {
        await $('#Topic_Pin_EndDate').attr('required', 'required');
        return $('#Pins').slideDown();
    } else {
        await $('#Topic_Pin_EndDate').removeAttr('required');
        return $('#Pins').slideUp();
    }
}

async function callFileCollection(urlLoad) {
    return $('#fileCollection').load(urlLoad);
}

async function deleteFiles(urlAjax, urlLoad) {
    return swal({
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
                        }).then(function (e) {
                            if (res.Icon == 'success') {
                                reloadTable();
                                callFileCollection(urlLoad);
                            }
                        });
                    }
                });
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
