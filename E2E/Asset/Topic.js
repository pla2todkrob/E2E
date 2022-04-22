

function callModalTopics(urlAjax, bigSize = false) {
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


            $('#Topic_Pin').on('change', function () {
                if ($(this).is(':checked')) {
                    $('#Topic_Pin_EndDate').attr('required', 'required');
                } else {
                    $('#Topic_Pin_EndDate').removeAttr('required');
                }
            });


            $('#modalArea').modal('show');
        }

    });
    return false;
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
                        });
                        if (res.icon == 'success') {
                            reloadTable();

                            $('#fileTables').empty();
                            $('#reloads').load(urlLoad);
                        }
                    }
                });
                return false;
            }
        });
}

function previewMultiple(event) {
   /*     $('#galImage').empty();*/

    $("#fileImage").on("click", function () {
        $('#galImage').empty();
    });

    var saida = document.getElementById("fileImage");
    var quantos = saida.files.length;
    for (i = 0; i < quantos; i++) {
        var urls = URL.createObjectURL(event.target.files[i]);
        
        var filetype = event.target.files[i].type.split('/')[0];
        console.log(filetype);
        console.log(event.target.files[i]);
        if (filetype == 'image') {
            document.getElementById("galImage").innerHTML += '<img src="' + urls + '"class="img-fluid img-thumbnail mr-1">';
        }
        else {
            document.getElementById("galImage").innerHTML += '<i class="fa fa-file-text-o fa-5x"></i>';
        }
       
    }
}

function callSubmit_Reply(urlAjax, reloadPage = false) {
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
            }).then(function () {
                location.reload();
            });
        }
    });

    return false;
}
