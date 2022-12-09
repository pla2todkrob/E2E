async function callModalDepartment(urlAjax, bigSize = false) {
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

            $('#modalArea').modal('show');

            $('#Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Division_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDivisions', $(this).val(), objSelect);
            });
        }
    });
}

async function callModalSection(urlAjax, bigSize = false) {
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

            $('#modalArea').modal('show');

            $('#Master_Departments_Division_Id').on('select2:select', function () {
                var objSelect = $('#Department_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDepartments', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Master_Departments_Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Master_Departments_Division_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDivisions', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });
        }
    });
}

async function callModalProcesses(urlAjax, bigSize = false) {
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

            $('#modalArea').modal('show');

            $('#Master_Sections_Master_Departments_Division_Id').on('select2:select', function () {
                var objSelect = $('#Master_Sections_Department_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDepartments', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Master_Sections_Master_Departments_Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Master_Sections_Master_Departments_Division_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDivisions', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Master_Sections_Department_Id').on('select2:select', function () {
                var objSelect = $('#Section_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectSections', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });
        }
    });
}

async function callModalUser(urlAjax, bigSize = false) {
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
            $('#modalArea').modal('show');

            $('#Users_Master_Grades_LineWork_Id').on('select2:select', function () {
                var objSelect = $('#Users_Grade_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectGrades', $(this).val(), objSelect);
            });

            $('#Users_Master_Processes_Master_Sections_Master_Departments_Master_Divisions_Plant_Id').on('select2:select', function () {
                var objSelect = $('#Users_Master_Processes_Master_Sections_Master_Departments_Division_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDivisions', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Users_Master_Processes_Master_Sections_Master_Departments_Division_Id').on('select2:select', function () {
                var objSelect = $('#Users_Master_Processes_Master_Sections_Department_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectDepartments', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Users_Master_Processes_Master_Sections_Department_Id').on('select2:select', function () {
                var objSelect = $('#Users_Master_Processes_Section_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectSections', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });

            $('#Users_Master_Processes_Section_Id').on('select2:select', function () {
                var objSelect = $('#Users_Process_Id');
                getSelectOp(baseUrl + '/Masters/Users_GetSelectProcesses', $(this).val(), objSelect);
                objSelect.trigger('select2:select');
            });
        }
    });
}
