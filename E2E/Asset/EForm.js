function callFileCollections(urlLoad) {
    $('#fileCollections').load(urlLoad);
}

function savegallery(urlAjax,urlLoad) {
    var arr = [];
    $('table#tableGal').find('tbody').each(function () {
        console.log($(this));
        $(this).find('tr').each(function () {
            console.log($(this));
            $(this).find('td#rowVal').each(function () {
                console.log($(this));
                var obj = {};
                obj.EForm_Gallery_Id = $(this).find('#item_EForm_Gallery_Id').val();
                obj.EForm_Gallery_Seq = $(this).find('#item_EForm_Gallery_Seq').val();
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
            }).then(function () {
                if (res.icon == 'success') {
                    reloadTable();
                    callFileCollections(urlLoad);
                }
            });
            
        }
    });
}

function deleteFileEF(urlAjax, urlLoad) {
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
                        }).then(function () {
                            if (res.icon == 'success') {
                                reloadTable();
                                callFileCollections(urlLoad);
                            }
                        });
                        
                    }
                });
                return false;
            }
        });
}

function callModalEForms(urlAjax, urlLoad = '', bigSize = false) {
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

            if (urlLoad != '') {
                callFileCollections(urlLoad);
            }

            $('#modalArea').modal('show');
        }
    });
    return false;
}