function callModalTopics(urlAjax, urlLoad = '', bigSize = false) {
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
    return false;
}

function savegallery_T(urlAjax, urlLoad) {
    var arr = [];
    $('table#tableGal').find('tbody').each(function () {
        console.log($(this));
        $(this).find('tr').each(function () {
            console.log($(this));
            $(this).find('td#rowVal').each(function () {
                console.log($(this));
                var obj = {};
                obj.TopicGallery_Id = $(this).find('#item_TopicGallery_Id').val();
                obj.TopicGallery_Seq = $(this).find('#item_TopicGallery_Seq').val();
                arr.push(obj);
            });
        });
    });
    console.log(arr);
    $.ajax({
        url: urlAjax,
        async: true,
        data: {
            model: JSON.stringify(arr)
        },
        success: function (res) {
            swal({
                title: res.title,
                text: res.text,
                icon: res.icon,
                button: res.button,
                dangerMode: res.dangerMode
            }).then(function (e) {
                console.log(e);
                if (res.icon == 'success') {
                    reloadTable();
                    callFileCollection(urlLoad);
                }
            });
            
        }
    });
}

function checkPin(ele) {
    if ($(ele).is(':checked')) {
        $('#Topic_Pin_EndDate').attr('required', 'required');
        $('#Pins').slideDown();
    } else {
        $('#Topic_Pin_EndDate').removeAttr('required');
        $('#Pins').slideUp();
    }
}

function callFileCollection(urlLoad) {
    $('#fileCollection').load(urlLoad);
}

function deleteFiles(urlAjax, urlLoad) {
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
                        }).then(function (e) {
                            console.log(e);
                            if (res.icon == 'success') {
                                reloadTable();
                                callFileCollection(urlLoad);
                            }
                        });
                        
                    }
                });
                return false;
            }
        });
}

function previewMultiple(event) {
    $('#galImage').empty();

    var saida = document.getElementById("fileImage");
    var quantos = saida.files.length;
    for (i = 0; i < quantos; i++) {
        var urls = URL.createObjectURL(event.target.files[i]);

        var filetype = event.target.files[i].type.split('/')[0];
        if (filetype == 'image') {
            document.getElementById("galImage").innerHTML += '<img src="' + urls + '"class="img-fluid img-thumbnail mr-1" title="' + event.target.files[i].name +'">';
        }
        else {
            document.getElementById("galImage").innerHTML += '<i class="fa fa-file-text-o fa-5x" title="' + event.target.files[i].name +'"></i>';
        }
    }
}

function callSubmit_Reply(urlAjax, form) {

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
            }).then(function () {
                location.reload();
            });
        }
    });

    return false;
}