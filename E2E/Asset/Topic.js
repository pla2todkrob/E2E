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