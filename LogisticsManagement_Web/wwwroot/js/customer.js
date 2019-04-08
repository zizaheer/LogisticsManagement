var customerData;
var addressData;
var employeeData;
var cityData;
var provinceData;
var countryData;
var addressLineForAutocomplete;

$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    FillEmployeeDropDown();
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

    $('#txtMailingAddressLine').autocomplete({
        minLength: 0,
        source: JSON.parse(addressLineForAutocomplete),
        focus: function (event, ui) {
            $('#txtMailingAddressLine').val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            $('#hfMailingAddressId').val(ui.item.Id);
            $('#txtMailingAddressLine').val(ui.item.AddressLine);
            $('#txtMailingAddressUnit').val(ui.item.UnitNumber);

            $('#txtMailingPostCode').val(ui.item.PostCode);
            $('#txtMailingPrimaryPhoneNumber').val(ui.item.PrimaryPhoneNumber);
            $('#txtMailingFaxNumber').val(ui.item.Fax);
            $('#txtMailingContactPerson').val(ui.item.ContactPersonName);
            $('#txtMailingEmailAddress').val(ui.item.EmailAddress1);
            $('#ddlMailingCityId').val(ui.item.CityId);
            $('#ddlMailingProvinceId').val(ui.item.ProvinceId);
            $('#ddlMailingCountryId').val(ui.item.CountryId);

            return false;
        }
    });







































});



$('#chkIsSameAsBilling').on('change', function () {
    if ($('#chkIsSameAsBilling').is(':checked') === true) {

        $('#hfMailingAddressId').val($('#hfBillingAddressId').val());
        $('#txtMailingAddressUnit').val($('#txtBillingAddressUnit').val());
        $('#txtMailingAddressLine').val($('#txtBillingAddressLine').val());
        $('#ddlMailingCityId').val($('#ddlBillingCityId').val());
        $('#ddlMailingProvinceId').val($('#ddlBillingProvinceId').val());
        $('#ddlMailingCountryId').val($('#ddlBillingCountryId').val());
        $('#txtMailingPostCode').val($('#txtBillingPostCode').val());
        $('#txtMailingContactPerson').val($('#txtBillingContactPerson').val());
        $('#txtMailingFaxNumber').val($('#txtBillingFaxNumber').val());
        $('#txtMailingPrimaryPhoneNumber').val($('#txtBillingPrimaryPhoneNumber').val());
        $('#txtMailingEmailAddress').val($('#txtBillingEmailAddress').val());
    }
});






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


$('.btnEdit').on('click', function () {
    var data = $(this).data('customer');




});

$('#loadData').on('click', function () {

    var table = $('#customer-list').DataTable({
        "ajax": GetListObject('Customer/GetCustomers'),

        "columnDefs": [{
            "targets": -1,
            "data": null,
            "defaultContent": "<button>Click!</button>"
        }]
    });

    $('#loadData').on('click', function () {

        var data = table.row($(this).parents('tr')).data();

        jQuery("#tblEntAttributes tbody").append(newRowContent);

    });
    //var customers = GetListObject('Customer/GetCustomers');
    //var parsedData = JSON.parse(customers);

    //$('#loadDataTable').load('Customer/PartialViewDataTable');
    

});

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

function FillEmployeeDropDown()
{
    var employees = GetListObject('Employee/GetEmployees');
    var employeedropDown = $('#ddlEmployeeId');
    $.each(employees, function (index, item) {
        $employeedropDown.append($('<option/>').val(this.Id).text(this.FirstName + this.LastName));
    });

}