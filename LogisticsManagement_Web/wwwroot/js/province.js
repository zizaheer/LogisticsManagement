$(document).ready(function () {
    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
    $(document).ajaxStart(function () {
        $("#addressSpinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#addressSpinnerLoadingDataTable").css("display", "none");
    });
});

$('#btnCloseModal').on('click', function () {
    $('#provinceInformation').modal('hide');
});

$('#btnNewProvince').on('click', function () {
    $('#txtProvinceId').prop('readonly', true);
    $('#txtProvinceId').val('');

    $('#frmProvinceForm').trigger('reset');

    $('#provinceInformation').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#provinceInformation').draggable();
    $('#provinceInformation').modal('show');

});



$('#frmProvinceForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmProvinceForm').unbind('submit').submit(function () {
    event.preventDefault();
    var dataArray = GetFormData();

    if (dataArray[0].provinceName === '') {
        event.preventDefault();
        bootbox.alert('Please enter province name.');
        return;
    }

    var result = '';
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        result = PerformPostActionWithObject('Province/Update', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the province. Please check your data and try again.');
        }
    }
    else {
        result = PerformPostActionWithObject('Province/Add', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the province. Please check your data and try again.');
        }
    }

});

$('#prov-list').on('click', '.btnEdit', function () {
    $('#txtProvinceId').prop('readonly', true);
    var provinceId = $(this).data('provinceid');

    if (provinceId !== '') {
        var provinceInfo = GetSingleById('Province/GetProvinceById', provinceId);
        if (provinceInfo != null && provinceInfo !== '') {
            FillProvinceInformation(JSON.parse(provinceInfo));
            $('#provinceInformation').modal({
                backdrop: 'static',
                keyboard: false
            });
            $('#provinceInformation').draggable();
            $('#provinceInformation').modal('show');
        }
        else {
            bootbox.alert('The province was not found. Please check and try again.');
            event.preventDefault();
            return;
        }
    }
});

$('.btnDelete').unbind().on('click', function () {
    var provinceId = $(this).data('provinceid');
    bootbox.confirm("This province will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Province/Remove', provinceId);
            location.reload();
        }
    });
});


function GetFormData() {
    var provinceData = {
        id: $('#txtProvinceId').val() === "" ? "0" : $('#txtProvinceId').val(),
        provinceName: $('#txtProvinceName').val(),
        shortCode: $('#txtShortCode').val(),
        countryId: $('#ddlCountryId').val()
    };

    return [provinceData];
}


function FillProvinceInformation(provinceInfo) {
    $('#txtProvinceId').val(provinceInfo.Id);
    $('#txtProvinceName').val(provinceInfo.ProvinceName);
    $('#txtShortCode').val(provinceInfo.ShortCode);
    $('#ddlCountryId').val(provinceInfo.CountryId);
}
