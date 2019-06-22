
$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

$('#btnNew').on('click', function () {
    $('#txtTariffId').prop('readonly', true);
    $('#txtTariffId').val('');
});

$('#btnClear').on('click', function () {
    $('#txtTariffId').prop('readonly', false);
});


$('#txtTariffId').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var tariffId = $('#txtTariffId').val();
        var tariffInfo = GetSingleObjectById('Tariff/GetTariffById', tariffId);
        if (tariffInfo !== "" && tariffInfo !== null) {
            tariffInfo = JSON.parse(tariffInfo);
        }
        else {
            bootbox.alert('The tariff was not found. Please check or select from the bottom list of tariffs.');
            event.preventDefault();
            return;
        }

        if (tariffInfo !== null) {
            FillTariffInfo(tariffInfo);
        }
    }
});


$('#tariff-list').on('click', '.btnEdit', function () {
    $('#txtTariffId').prop('readonly', true);

    var tariffId = $(this).data('tariffid');
    var tariffInfo = GetSingleObjectById('Tariff/GetTariffById', tariffId);
    console.log(tariffInfo);
    if (tariffInfo !== "") {
        tariffInfo = JSON.parse(tariffInfo);
    }
    else {
        bootbox.alert('The tariff was not found. Please check or select from the bottom list of tariffs.');
        event.preventDefault();
        return;
    }

    FillTariffInfo(tariffInfo);
});

$('#btnDownloadTariffData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');

});

$('#frmTariffForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmTariffForm').unbind('submit').submit(function (event) {
    event.preventDefault();

    var data = GetFormData();

    if (data.cityId < 1) {
        bootbox.alert('Please select city to add tariff');
        return;
    }

    if (data.firstUnitPrice == null || data.firstUnitPrice === '' || data.firstUnitPrice <= 0) {
        bootbox.alert('Please enter first unit price');
        return;
    }

    if (data.perUnitPrice == null || data.perUnitPrice == '' || data.perUnitPrice <= 0) {
        bootbox.alert('Please enter per unit price');
        return;
    }

    if (data.id > 0) {
        UpdateEntry('Tariff/Update', data);
    }
    else {
        var result = AddEntry('Tariff/Add', data);
        if (result.length > 0) {
            $('#txtTariffId').val(result);
        }
    }
   
    $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    tariffId = $(this).data('tariffid');
    RemoveEntry('Tariff/Remove', tariffId);
    $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');

});

function GetFormData() {
    var data = {
        id: $('#txtTariffId').val() === '' ? 0 : $('#txtTariffId').val(),
        deliveryOptionId: $('#ddlDeliveryOptionId').val(),
        cityId: $('#ddlCityId').val(),
        vehicleTypeId: $('#ddlVehicleTypeId').val(),
        unitTypeId: $('#ddlUnitTypeId').val(),
        weightScaleId: $('#ddlWeightScaleId').val(),
        firstUnitPrice: $('#txtFirstUnitPrice').val(),
        perUnitPrice: $('#txtPerUnitPrice').val()
    };

    return data;
}

function FillTariffInfo(tariffInfo) {

    $('#txtTariffId').val(tariffInfo.Id);
    $('#ddlDeliveryOptionId').val(tariffInfo.DeliveryOptionId);
    $('#ddlCityId').val(tariffInfo.CityId);
    $('#ddlVehicleTypeId').val(tariffInfo.VehicleTypeId);
    $('#ddlUnitTypeId').val(tariffInfo.UnitTypeId);
    $('#ddlWeightScaleId').val(tariffInfo.WeightScaleId);
    $('#txtFirstUnitPrice').val(tariffInfo.FirstUnitPrice);
    $('#txtPerUnitPrice').val(tariffInfo.PerUnitPrice);

}

