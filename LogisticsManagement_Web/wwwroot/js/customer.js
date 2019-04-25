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
    //FillEmployeeDropDown();

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
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

$('#chkIsSameAsMailing').on('change', function () {
    if ($('#chkIsSameAsMailing').is(':checked') === true) {
        $('#hfBillingAddressId').val($('#hfMailingAddressId').val());
        $('#txtBillingAddressUnit').val($('#txtMailingAddressUnit').val());
        $('#txtBillingAddressLine').val($('#txtMailingAddressLine').val());
        $('#ddlBillingCityId').val($('#ddlMailingCityId').val());
        $('#ddlBillingProvinceId').val($('#ddlMailingProvinceId').val());
        $('#ddlBillingCountryId').val($('#ddlMailingCountryId').val());
        $('#txtBillingPostCode').val($('#txtMailingPostCode').val());
        $('#txtBillingContactPerson').val($('#txtMailingContactPerson').val());
        $('#txtBillingFaxNumber').val($('#txtMailingFaxNumber').val());
        $('#txtBillingPrimaryPhoneNumber').val($('#txtMailingPrimaryPhoneNumber').val());
        $('#txtBillingEmailAddress').val($('#txtMailingEmailAddress').val());
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

$('#btnNew').on('click', function () {
    $('#txtCustomerId').prop('readonly', true);
});

$('#btnClear').on('click', function () {
    $('#txtCustomerId').prop('readonly', false);
});


$('#txtCustomerId').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var customerId = $('#txtCustomerId').val();
        var customerInfo = GetSingleObjectById('Customer/GetCustomerById', customerId);

        if (customerInfo !== "" && customerInfo !== null) {
            customerInfo = JSON.parse(customerInfo);
        }
        else {
            bootbox.alert('The customer was not found. Please check or select from the bottom list of customers.');
            event.preventDefault();
            return;
        }

        if (customerInfo !== null)
        { 
            FillCustomerInfo(customerInfo);
        }
    }
});


$('#btnDownloadData').unbind().on('click', function () {
    $('#loadDataTable').load('Customer/PartialViewDataTable');
});


$('#frmCustomerForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmCustomerForm').unbind('submit').submit(function () {

    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        UpdateEntry('Customer/Update', dataArray);
    }
    else {
        AddEntry('Customer/Add', dataArray);
    }
    event.preventDefault();
    $('#loadDataTable').load('Customer/PartialViewDataTable');

});

$('#customer-list').on('click', '.btnEdit', function () {
    $('#txtCustomerId').prop('readonly', true);

    var customerId = $(this).data('customerid');

    var customerInfo = GetSingleObjectById('Customer/GetCustomerById', customerId);
    if (customerInfo !== "") {
        customerInfo = JSON.parse(customerInfo);
    }
    else {
        bootbox.alert('The employee was not found. Please check or select from the bottom list of employees.');
        event.preventDefault();
        return;
    }

    FillCustomerInformation(customerInfo);


});

$('.btnDelete').unbind().on('click', function () {
    customerId = $(this).data('customerid');
    RemoveEntry('Customer/Remove', customerId);
    $('#loadDataTable').load('Customer/PartialViewDataTable');
});



function GetFormData() {
    var customerData = {
        id: $('#txtCustomerId').val() === "" ? "0" : $('#txtCustomerId').val(),
        customerName: $('#txtCustomerName').val(),
        fuelSurChargePercentage: $('#txtFuelSurcharge').val(),
        discountPercentage: $('#txtSpecialDiscount').val(),
        invoiceDueDays: $('#txtInvoiceDueDays').val(),
        isGstApplicable: $('#isGstApplicable').is(':checked') ? 1 : 0,
        isActive: 1, //$('#chkIsActive').is(':checked') ? 1 : 0,
        mailingAddressId: $('#hfMailingAddressId').val(),
        billingAddressId: $('#hfBillingAddressId').val()
        //employeeNumber: $('#ddlEmployeeId').val()
        //createDate: $('#hfCreateDate').val()
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

function FillEmployeeDropDown() {
    var employees = JSON.parse(GetListObject('Employee/GetEmployees'));
    var employeeDropDown = $('#ddlEmployeeId');

    for (var i = 0; i < employees.length; i++) {
        employeeDropDown.append('<option value=' + employees[i].Id + '>' + employees[i].FirstName + ' ' + (employees[i].LastName === null ? '' : employees[i].LastName) + '  (' + employees[i].EmployeeNumber + ') ' + '</option>');
    }
}

function FillCustomerInfo(customerInfo) {

    $('#txtCustomerId').val('');
    $('#txtCustomerName').val('');
    $('#txtFuelSurcharge').val('');
    $('#txtSpecialDiscount').val('');
    $('#txtInvoiceDueDays').val('');

    $('#hfBillingAddressId').val('');

    $('#txtBillingAddressUnit').val('');
    $('#txtBillingAddressLine').val('');
    $('#ddlBillingCityId').val('0');
    $('#ddlBillingProvinceId').val('0');
    $('#ddlBillingCountryId').val('0');
    $('#txtBillingPostCode').val('');
    $('#txtBillingContactPerson').val('');
    $('#txtBillingFaxNumber').val('');
    $('#txtBillingPrimaryPhoneNumber').val('');
    $('#txtBillingEmailAddress').val('');

    $('#hfMailingAddressId').val('');

    $('#txtMailingAddressUnit').val('');
    $('#txtMailingAddressLine').val('');
    $('#ddlMailingCityId').val('0');
    $('#ddlMailingProvinceId').val('0');
    $('#ddlMailingCountryId').val('0');
    $('#txtMailingPostCode').val('');
    $('#txtMailingContactPerson').val('');
    $('#txtMailingFaxNumber').val('');
    $('#txtMailingPrimaryPhoneNumber').val('');
    $('#txtMailingEmailAddress').val('');

    $('#txtCustomerId').val(customerInfo.Id);
    $('#txtCustomerName').val(customerInfo.CustomerName);
    $('#txtFuelSurcharge').val(customerInfo.FuelSurChargePercentage);
    $('#txtSpecialDiscount').val(customerInfo.DiscountPercentage);
    $('#txtInvoiceDueDays').val(customerInfo.InvoiceDueDays);

    if (customerInfo.BillingAddressId !== null && customerInfo.BillingAddressId !== undefined) {
        $('#hfBillingAddressId').val(customerInfo.BillingAddressId);

        var billingAddressRaw = GetSingleObjectById('Address/GetAddressById', customerInfo.BillingAddressId);
        if (billingAddressRaw !== null) {
            var billingAddress = JSON.parse(billingAddressRaw);
            $('#txtBillingAddressUnit').val(billingAddress.UnitNumber);
            $('#txtBillingAddressLine').val(billingAddress.AddressLine);
            $('#ddlBillingCityId').val(billingAddress.CityId);
            $('#ddlBillingProvinceId').val(billingAddress.ProvinceId);
            $('#ddlBillingCountryId').val(billingAddress.CountryId);
            $('#txtBillingPostCode').val(billingAddress.PostCode);
            $('#txtBillingContactPerson').val(billingAddress.ContactPersonName);
            $('#txtBillingFaxNumber').val(billingAddress.Fax);
            $('#txtBillingPrimaryPhoneNumber').val(billingAddress.PrimaryPhoneNumber);
            $('#txtBillingEmailAddress').val(billingAddress.EmailAddress1);
        }

    }

    if (customerInfo.MailingAddressId !== null && customerInfo.MailingAddressId !== undefined) {
        $('#hfMailingAddressId').val(customerInfo.MailingAddressId);

        var mailingAddressRaw = GetSingleObjectById('Address/GetAddressById', customerInfo.MailingAddressId);
        if (mailingAddressRaw !== null) {
            var mailingAddress = JSON.parse(mailingAddressRaw);
            $('#txtMailingAddressUnit').val(mailingAddress.UnitNumber);
            $('#txtMailingAddressLine').val(mailingAddress.AddressLine);
            $('#ddlMailingCityId').val(mailingAddress.CityId);
            $('#ddlMailingProvinceId').val(mailingAddress.ProvinceId);
            $('#ddlMailingCountryId').val(mailingAddress.CountryId);
            $('#txtMailingPostCode').val(mailingAddress.PostCode);
            $('#txtMailingContactPerson').val(mailingAddress.ContactPersonName);
            $('#txtMailingFaxNumber').val(mailingAddress.Fax);
            $('#txtMailingPrimaryPhoneNumber').val(mailingAddress.PrimaryPhoneNumber);
            $('#txtMailingEmailAddress').val(mailingAddress.EmailAddress1);
        }
    }
}