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
    $('#cityInformation').modal('hide');
});

$('#btnNewCity').on('click', function () {
    $('#txtCityId').prop('readonly', true);
    $('#txtCityId').val('');

    $('#frmCityForm').trigger('reset');

    $('#cityInformation').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#cityInformation').draggable();
    $('#cityInformation').modal('show');

});



$('#frmCityForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmCityForm').unbind('submit').submit(function () {
    event.preventDefault();
    var dataArray = GetFormData();

    if (dataArray[0].cityName === '') {
        event.preventDefault();
        bootbox.alert('Please enter city name.');
        return;
    }

    var result = '';
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        result = PerformPostActionWithObject('City/Update', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the city. Please check your data and try again.');
        }
    }
    else {
        result = PerformPostActionWithObject('City/Add', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the city. Please check your data and try again.');
        }
    }

});

$('#city-list').on('click', '.btnEdit', function () {
    $('#txtCityId').prop('readonly', true);
    var cityId = $(this).data('cityid');

    if (cityId !== '') {
        var cityInfo = GetSingleById('City/GetCityById', cityId);
        if (cityInfo != null && cityInfo !== '') {
            FillCityInformation(JSON.parse(cityInfo));
            $('#cityInformation').modal({
                backdrop: 'static',
                keyboard: false
            });
            $('#cityInformation').draggable();
            $('#cityInformation').modal('show');
        }
        else {
            bootbox.alert('The city was not found. Please check and try again.');
            event.preventDefault();
            return;
        }
    }
});

$('.btnDelete').unbind().on('click', function () {
    var cityId = $(this).data('cityid');
    bootbox.confirm("This city will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('City/Remove', cityId);
            location.reload();
        }
    });
});


function GetFormData() {
    var cityData = {
        id: $('#txtCityId').val() === "" ? "0" : $('#txtCityId').val(),
        cityName: $('#txtCityName').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val()
    };
    
    return [cityData];
}


function FillCityInformation(cityInfo) {
    $('#txtCityId').val(cityInfo.Id);
    $('#txtCityName').val(cityInfo.CityName);
    $('#ddlProvinceId').val(cityInfo.ProvinceId);
    $('#ddlCountryId').val(cityInfo.CountryId);
}
