
$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

$('#btnNewTariff').on('click', function () {
    $('#frmTariffForm').trigger('reset');

    $('#tariffUpdate').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#tariffUpdate').draggable();
    $('#tariffUpdate').modal('show');

});



$('#tariff-list').on('click', '.btnEdit', function () {
    $('#txtTariffId').prop('readonly', true);

    var tariffId = $(this).data('tariffid');
    var tariffInfo = GetSingleById('Tariff/GetTariffById', tariffId);

    if (tariffInfo !== "") {
        tariffInfo = JSON.parse(tariffInfo);
    }
    else {
        bootbox.alert('The tariff was not found. Please check or select from the bottom list of tariffs.');
        event.preventDefault();
        return;
    }

    FillTariffInfo(tariffInfo);

    $('#tariffUpdate').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#tariffUpdate').draggable();
    $('#tariffUpdate').modal('show');
});


$('#btnDownloadTariffData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');
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
        PerformPostActionWithObject('Tariff/Update', data);
    }
    else {
        var result = PerformPostActionWithObject('Tariff/Add', data);
        if (result.length > 0) {
            $('#txtTariffId').val(result);
        }
    }

    $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');
    location.reload();
});

$('.btnDelete').unbind().on('click', function () {
    tariffId = $(this).data('tariffid');
    bootbox.confirm("This tariff will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Tariff/Remove', tariffId);
            $('#loadTariffDataTable').load('Tariff/PartialViewDataTable');
        }
    });
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

