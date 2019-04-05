var customerData;
var addressData;
var employeeData;
var cityData;
var provinceData;
var countryData;
var addressLineForAutocomplete;

$(document).ready(function () {
    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
});


$(function () {
    addressLineForAutocomplete = GetListObject('Address/GetAddressesForAutoComplete');
    $('#txtBillingAddressLine').autocomplete({
        minLength: 0,
        source: JSON.parse(addressLineForAutocomplete),
        focus: function (event, ui) {
            $('#txtBillingAddressLine').val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            $('#hfBillingAddressId').val(ui.item.Id);
            $('#txtBillingAddressLine').val(ui.item.AddressLine);
            $('#txtBillingAddressUnit').val(ui.item.UnitNumber);

            $('#txtBillingPostCode').val(ui.item.PostCode);
            $('#txtBillingPrimaryPhoneNumber').val(ui.item.PrimaryPhoneNumber);
            $('#txtBillingFaxNumber').val(ui.item.Fax);
            $('#txtBillingContactPerson').val(ui.item.ContactPersonName);
            $('#txtBillingEmailAddress').val(ui.item.EmailAddress1);
            $('#ddlBillingCityId').val(ui.item.CityId);
            $('#ddlBillingProvinceId').val(ui.item.ProvinceId);
            $('#ddlBillingCountryId').val(ui.item.CountryId);
            
            return false;
        }
    });





    
    
    
    
    
    
    
    


    txtMailingAddressLine
    txtMailingAddressUnit
    ddlMailingCityId
    ddlMailingProvinceId
    ddlMailingCountryId
    txtMailingPostCode
    txtMailingPrimaryPhoneNumber
    txtMailingFaxNumber
    txtMailingContactPerson
    txtMailingEmailAddress


















});




//var dataCust = JSON.parse(GetListObject('Customer/GetAll'));
//$.each(dataCust, function (k, v) {
//    var zone = JSON.stringify(k, v);
//    //zone = zone.replace(/"/g, '');
//    //$('#zone').append('<option value="' + zone + '">' + zone + '</option>');
//    console.log(zone);

//});
$('#txtBillingAddressLine').on('change', function () {


});


$(document).ready(function () {
    ClearForm();
    $('#cardMailingAddress *').attr('disabled', 'disabled');
});

function GetFormData() {
    var customerData = {
        id: $('#txtCustomerId').val() === "" ? "0" : $('#txtCustomerId').val(),
        customerName: $('#txtCustomerName').val(),
        discountPercentage: $('#txtSpecialDiscount').val(),
        invoiceDueDays: $('#txtInvoiceDueDays').val(),
        isGstApplicable: $('#chkIsGstApplicable').is(':checked') ? 1 : 0,
        isActive: $('#chkIsGstApplicable').is(':checked') ? 1 : 0,
        mailingAddressId: $('#hfMailingAddressId').val(),
        billingAddressId: $('#hfBillingAddressId').val(),
        employeeNumber: $('#ddlEmployeeId').val()
    };

    var billingAddressData = {
        billingAddressId: $('#hfBillingAddressId').val(),
        unitNumber: $('#txtBillingAddressUnit').val(),
        addressLine: $('#txtBillingAddressLine').val(),
        cityId: $('#ddlBillingCityId').val(),
        provinceId: $('#ddlBillingProvinceId').val(),
        countryId: $('#ddlBillingCountryId').val(),
        postCode: $('#txtBillingPostCode').val(),
        contactPersonName: $('#txtBillingContactPerson').val(),
        fax: $('#txtBillingFaxNumber').val(),
        primaryPhoneNumber: $('#txtBillingPrimaryPhoneNumber').val(),
        emailAddress1: $('#txtBillingEmailAddress').val()
    };

    var mailingAddressData = {
        billingAddressId: $('#hfMailingAddressId').val(),
        unitNumber: $('#txtMailingAddressUnit').val(),
        addressLine: $('#txtMailingAddressLine').val(),
        cityId: $('#ddlMailingCityId').val(),
        provinceId: $('#ddlMailingProvinceId').val(),
        countryId: $('#ddlMailingCountryId').val(),
        postCode: $('#txtMailingPostCode').val(),
        contactPersonName: $('#txtMailingContactPerson').val(),
        fax: $('#txtMailingFaxNumber').val(),
        primaryPhoneNumber: $('#txtMailingPrimaryPhoneNumber').val(),
        emailAddress1: $('#txtMailingEmailAddress').val()
    };


    return [customerData, billingAddressData, mailingAddressData];
}

function ClearForm() {

    $('#txtCustomerId').attr('disabled', 'disabled');
    $('#txtCustomerName').val();
    $('#txtSpecialDiscount').val();
    $('#txtInvoiceDueDays').val();
    $('#chkIsGstApplicable').val();
    $('#hfMailingAddressId').val();
    $('#hfBillingAddressId').val();
    $('#ddlEmployeeId').val();
   // $('#txtPerUnitPrice').val();

}

$('input[type=radio][name=addressType]').change(function () {

    var addOption = $("input[name='addressType']:checked").val();
    if (addOption === '1') {
        $('#cardBillingAddress *').removeAttr('disabled');
        $('#cardMailingAddress *').attr('disabled', 'disabled');
    }
    else if (addOption === '2') {
        $('#cardBillingAddress *').attr('disabled', 'disabled');
        $('#cardMailingAddress *').removeAttr('disabled');
    }
    else {
        $('#cardBillingAddress *').removeAttr('disabled');
        $('#cardMailingAddress *').removeAttr('disabled');
    }

});

$('#btnNew').on('click', function () {
    ClearForm();
});

$('.btnEdit').on('click', function () {
    var data = $(this).data('customer');
    $('#txtTariffId').val(data.id);
    $('#ddlDeliveryOptionId').val(data.deliveryOptionId);
    $('#ddlCityId').val(data.cityId);
    $('#ddlVehicleTypeId').val(data.vehicleTypeId);
    $('#ddlUnitTypeId').val(data.unitTypeId);
    $('#ddlWeightScaleId').val(data.weightScaleId);
    $('#txtFirstUnitPrice').val(data.firstUnitPrice);
    $('#txtPerUnitPrice').val(data.perUnitPrice);
});



//$('#ddlSelectAddressId').editableSelect();

$('#frmCustomerForm').submit(function (event) {
    var data = GetFormData();
    $.ajax({
        url: 'Customer/AddOrUpdate',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            //SetAlertType('Success', 'Data has been removed.');
            console.log('Success');
            window.location.href = 'Customer/Index';
        },
        error: function (result) {
            //SetAlertType('Failed', 'An error occured during deleting the data.');
        }
    });
});

$('.btnDelete').on('click', function () {
    SetAlertType('Warning', 'The data will be deleted. Are you sure you want ot continue?');
    customerData = $(this).data('customer');
});
$('#btnProceed').on('click', function () {
    if (customerData !== null) {
        RemoveCustomer(customerData);
    }
});

function RemoveCustomer(customerData) {
    $.ajax({
        url: 'Customer/Remove',
        type: 'POST',
        data: JSON.stringify([customerData]),
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
