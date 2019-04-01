var customerData;


$(document).ready(function () {
    ClearForm();
});

function GetFormData() {
    var data = {
        id: $('#txtCustomerId').val(),
        customerName: $('#txtCustomerName').val(),
        discountPercentage: $('#txtSpecialDiscount').val(),
        invoiceDueDays: $('#txtInvoiceDueDays').val(),
        isGstApplicable: $('#chkIsGstApplicable').val(),
        isActive: $('#chkIsGstApplicable').is(':checked') ? 1 : 0,
        mailingAddressId: $('#chkIsGstApplicable').val(),
        billingAddressId: $('#chkIsGstApplicable').val(),
        unitNumber: $('#chkIsGstApplicable').val(),
        addressLine: $('#chkIsGstApplicable').val(),
        cityId: $('#chkIsGstApplicable').val(),
        provinceId: $('#chkIsGstApplicable').val(),
        countryId: $('#chkIsGstApplicable').val(),
        postCode: $('#chkIsGstApplicable').val(),
        contactPersonName: $('#chkIsGstApplicable').val(),
        fax: $('#chkIsGstApplicable').val(),
        fax: $('#chkIsGstApplicable').val(),
        fax: $('#chkIsGstApplicable').val(),
        Fax


        deliveryOptionId: $('#ddlDeliveryOptionId').val(),
        cityId: $('#ddlCityId').val(),
        vehicleTypeId: $('#ddlVehicleTypeId').val(),
        unitTypeId: $('#ddlUnitTypeId').val(),
        weightScaleId: $('#ddlWeightScaleId').val(),
        firstUnitPrice: $('#txtFirstUnitPrice').val(),
        perUnitPrice: $('#txtPerUnitPrice').val(),
        createDate: $('#hfCreateDate').val(),
        createdBy: $('#hfCreatedBy').val()
    };

    return data;
}

function ClearForm() {

    $('#txtTariffId').attr('disabled', 'disabled');
    $('#txtTariffId').val();
    $('#ddlDeliveryOptionId').val();
    $('#ddlCityId').val();
    $('#ddlVehicleTypeId').val();
    $('#ddlUnitTypeId').val();
    $('#ddlWeightScaleId').val();
    $('#txtFirstUnitPrice').val();
    $('#txtPerUnitPrice').val();

}

$('#btnNew').on('click', function () {
    ClearForm();
});

$('.btnEdit').on('click', function () {
    var data = $(this).data('tariff');
    $('#txtTariffId').val(data.id);
    $('#ddlDeliveryOptionId').val(data.deliveryOptionId);
    $('#ddlCityId').val(data.cityId);
    $('#ddlVehicleTypeId').val(data.vehicleTypeId);
    $('#ddlUnitTypeId').val(data.unitTypeId);
    $('#ddlWeightScaleId').val(data.weightScaleId);
    $('#txtFirstUnitPrice').val(data.firstUnitPrice);
    $('#txtPerUnitPrice').val(data.perUnitPrice);
});



$('#frmTariffForm').submit(function (event) {
    var data = GetFormData();
    $.ajax({
        url: 'Tariff/AddOrUpdate',
        type: 'POST',
        data: JSON.stringify([data]),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            //SetAlertType('Success', 'Data has been removed.');
            console.log('Success');
            window.location.href = 'Tariff/Index';
        },
        error: function (result) {
            //SetAlertType('Failed', 'An error occured during deleting the data.');
        }
    });
});

$('.btnDelete').on('click', function () {
    SetAlertType('Warning', 'The data will be deleted. Are you sure you want ot continue?');
    tariffData = $(this).data('tariff');
});
$('#btnProceed').on('click', function () {
    if (tariffData !== null) {
        RemoveTariff(tariffData);
    }
});

function RemoveTariff(tariffData) {
    $.ajax({
        url: 'Tariff/Remove',
        type: 'POST',
        data: JSON.stringify([tariffData]),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            SetAlertType('Success', 'Data has been removed.');

        },
        error: function (result) {
            SetAlertType('Failed', 'An error occured during deleting the data.');
        }
    });
}
