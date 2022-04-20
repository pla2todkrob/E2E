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

function previewMultiple(event) {
    $('#galImage').empty();
    var saida = document.getElementById("fileImage");
    var quantos = saida.files.length;
    for (i = 0; i < quantos; i++) {
        var urls = URL.createObjectURL(event.target.files[i]);
        document.getElementById("galImage").innerHTML += '<img src="' + urls + '"class="img-fluid">';
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
