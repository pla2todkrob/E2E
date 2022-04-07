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
