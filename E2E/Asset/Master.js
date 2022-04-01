function callModalDepartment(urlAjax, bigSize = false) {
    $('#Master_Divisions_Plant_Id').trigger('select2:select');
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

            $('#modalArea').modal('show');

            $('#Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Division_Id');
                getDivisions(objSelect, $(this).val());
            });

        }
    });
    return false;
}

function callModalSection(urlAjax, bigSize = false) {
    $('#Master_Departments_Master_Divisions_Plant_Id').trigger('select2:select');
    $('#Master_Departments_Division_Id').trigger('select2:select');
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

            $('#modalArea').modal('show');

            $('#Master_Departments_Division_Id').on('select2:select', function () {
                var objSelect = $('#Department_Id');
                getDepartments(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Master_Departments_Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Master_Departments_Division_Id');
                getDivisions(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

        }
    });
    return false;
}

function callModalProcesses(urlAjax, bigSize = false) {
    $('#Master_Sections_Master_Departments_Master_Divisions_Plant_Id').trigger('select2:select');
    $('#Master_Sections_Master_Departments_Division_Id').trigger('select2:select');
    $('#Master_Sections_Department_Id').trigger('select2:select');

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

            $('#modalArea').modal('show');

            $('#Master_Sections_Master_Departments_Division_Id').on('select2:select', function () {
                var objSelect = $('#Master_Sections_Department_Id');
                getDepartments(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Master_Sections_Master_Departments_Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Master_Sections_Master_Departments_Division_Id');
                getDivisions(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Master_Sections_Department_Id').on('select2:select', function () {
                var objSelect = $('#Section_Id');
                getSections(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

        }
    });
    return false;
}

function callModalUser(urlAjax, bigSize = false) {
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
            $('#modalArea').modal('show');

            $('#Users_LineWork_Id').on('select2:select', function () {
                var objSelect = $('#Users_Grade_Id');
                getGrades(objSelect, $(this).val());
            });

            $('#Users_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Users_Division_Id');
                getDivisions(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Users_Division_Id').on('select2:select', function () {
                var objSelect = $('#Users_Department_Id');
                getDepartments(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Users_Department_Id').on('select2:select', function () {
                var objSelect = $('#Users_Section_Id');
                getSections(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });

            $('#Users_Section_Id').on('select2:select', function () {
                var objSelect = $('#Users_Process_Id');
                getProcesses(objSelect, $(this).val());
                objSelect.trigger('select2:select');
            });
        }
    });
    return false;
}

function getGrades(objSelect, selectVal) {
    objSelect.empty();
    objSelect.append(new Option('Select Grade', ''));
    if (selectVal != '') {
        $.ajax({
            url: '/Masters/Users_GetSelectGrades',
            data: {
                id: selectVal
            },
            async: true,
            success: function (res) {
                $.each(res, function (n, v) {
                    objSelect.append(new Option(v.Text, v.Value));
                });
            }
        });
    }

    return false;
}

function getDivisions(objSelect, selectVal) {
    objSelect.empty();
    objSelect.append(new Option('Select Division', ''));
    if (selectVal != '') {
        $.ajax({
            url: '/Masters/Users_GetSelectDivisions',
            data: {
                id: selectVal
            },
            async: true,
            success: function (res) {
                $.each(res, function (n, v) {
                    objSelect.append(new Option(v.Text, v.Value));
                });
            }
        });
    }
    return false;
}

function getDepartments(objSelect, selectVal) {
    objSelect.empty();
    objSelect.append(new Option('Select Department', ''));
    if (selectVal != '') {
        $.ajax({
            url: '/Masters/Users_GetSelectDepartments',
            data: {
                id: selectVal
            },
            async: true,
            success: function (res) {
                $.each(res, function (n, v) {
                    objSelect.append(new Option(v.Text, v.Value));
                });
            }
        });
    }
    return false;
}

function getSections(objSelect, selectVal) {
    objSelect.empty();
    objSelect.append(new Option('Select Section', ''));
    if (selectVal != '') {
        $.ajax({
            url: '/Masters/Users_GetSelectSections',
            data: {
                id: selectVal
            },
            async: true,
            success: function (res) {
                $.each(res, function (n, v) {
                    objSelect.append(new Option(v.Text, v.Value));
                });
            }
        });
    }

    return false;
}

function getProcesses(objSelect, selectVal) {
    objSelect.empty();
    objSelect.append(new Option('Select Process', ''));
    if (selectVal != '') {
        $.ajax({
            url: '/Masters/Users_GetSelectProcesses',
            data: {
                id: selectVal
            },
            async: true,
            success: function (res) {
                $.each(res, function (n, v) {
                    objSelect.append(new Option(v.Text, v.Value));
                });
            }
        });
    }

    return false;
}
