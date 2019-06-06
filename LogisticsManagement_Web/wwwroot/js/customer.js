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

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
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

        if (customerInfo !== null) {
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

    if ($('#hfBillingAddressId').val() === '' && $('#hfMailingAddressId').val() === '') {
        bootbox.alert('Please select address from suggestions and try again.');
        event.preventDefault();
        return;
    }

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
        isActive: $('#chkIsActive').is(':checked') === true ? 1 : 0,
        mailingAddressId: $('#hfMailingAddressId').val(),
        billingAddressId: $('#hfBillingAddressId').val()

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

function FillCustomerInfo(customerInfo) {

    $('#hfBillingAddressId').val(customerInfo.BillingAddressId);
    $('#hfMailingAddressId').val(customerInfo.MailingAddressId);

    $('#txtCustomerId').val(customerInfo.Id);
    $('#txtCustomerName').val(customerInfo.CustomerName);
    $('#txtFuelSurcharge').val(customerInfo.FuelSurChargePercentage);
    $('#txtSpecialDiscount').val(customerInfo.DiscountPercentage);
    $('#txtInvoiceDueDays').val(customerInfo.InvoiceDueDays);

    if (customerInfo.BillingAddressId !== null)
    {

    }

    $('#txtAddressUnit').val(address.UnitNumber);
    $('#txtAddressLine').val(address.UnitNumber);
    $('#ddlCityId').val(address.UnitNumber);
    $('#ddlBillingProvinceId').val(address.UnitNumber);
    $('#ddlBillingCountryId').val(address.UnitNumber);
    $('#txtBillingPostCode').val(address.UnitNumber);
    $('#txtBillingContactPerson').val(address.UnitNumber);
    $('#txtBillingFaxNumber').val(address.UnitNumber);
    $('#txtBillingPrimaryPhoneNumber').val(address.UnitNumber);
    $('#txtBillingEmailAddress').val(address.UnitNumber);



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