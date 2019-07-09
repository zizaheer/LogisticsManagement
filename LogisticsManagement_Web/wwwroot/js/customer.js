var customerData;
var addressData;
var employeeData;
var cityData;
var provinceData;
var countryData;
var addressLineForAutocomplete;
var defaultObjectLoadCount = 20;

$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

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

    var addressLinesForAutoComplete = GetList('Address/GetAddressForAutoComplete');
    if (addressLinesForAutoComplete !== null) {
        var addressLines = JSON.parse(addressLinesForAutoComplete);

        $.each(addressLines, function (i, item) {
            $('#addresses').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
            //$('#addresses').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
        });
    }

});

$('#txtCustomerId').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var customerId = '';
        customerId = $('#txtCustomerId').val();

        if (customerId !== '') {
            var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);

            if (customerInfo !== "" && customerInfo !== null && customerInfo !== undefined) {
                FillCustomerInfo(JSON.parse(customerInfo));
            }
            else {
                bootbox.alert('The customer was not found. Please check and try again.');
                event.preventDefault();
                return;
            }
        }
    }
});


$('#btnLoadCustomerData').unbind().on('click', function (event) {
    event.preventDefault();

    var count = $('#ddlLoadCustomerCount').val();
    $('#loadCustomerDataTable').load('Customer/LoadCustomerData/' + count);
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

    if (dataArray[0].customerName === '') {
        event.preventDefault();
        bootbox.alert('Please enter customer name.');
        return;
    }

    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('Customer/Update', dataArray);
    }
    else {
        result = PerformPostActionWithObject('Customer/Add', dataArray);
        $('#frmCustomerForm').trigger('reset');
    }
    event.preventDefault();
    $('#loadCustomerDataTable').load('Customer/LoadCustomerData/' + defaultObjectLoadCount);

});

$('#customer-list').on('click', '.btnEdit', function () {
    //$('#txtCustomerId').prop('readonly', true);

    var customerId = $(this).data('customerid');

    if (customerId !== '') {
        var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);
        if (customerInfo !== null && customerInfo !== undefined) {
            FillCustomerInformation(JSON.parse(customerInfo));
        }
        else {
            bootbox.alert('The employee was not found. Please check or select from the bottom list of employees.');
            event.preventDefault();
            return;
        }
    }

});

$('.btnDelete').unbind().on('click', function () {

    var customerId = $(this).data('customerid');

    bootbox.confirm("This customer will be deleted with all relavant data. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Customer/Remove', customerId);
            $('#loadCustomerDataTable').load('Customer/LoadCustomerData/' + defaultObjectLoadCount);
        }
    });

});



$('#btnAddAddress').unbind().on('click', function (event) {
    event.preventDefault();
    $('#hfAddressId').val(0);
    ClearAddressForm();

    $('#addAddress').modal({
        backdrop: 'static',
        keyboard: false
    });

    var customerId = $('#txtCustomerId').val();
    var customerName = $('#txtCustomerName').val();
    $('#txtCustomerIdForAddress').val(customerId);
    $('#txtCustomerNameForAddress').val(customerName);

    if (customerId === '' || customerId === undefined) {
        //$('#addAddress').modal('hide');
        bootbox.alert('Please select a customer to add/view address');
        return;
    }
    else {
        $('#addAddress').modal('show');
    }

    $('#loadAddressDataTable').load('Customer/LoadCustomerAddressData/' + customerId);
});

$('#btnNewAddress').unbind().on('click', function (event) {
    event.preventDefault();
    ClearAddressForm();
});

$('#txtAddressLine').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $('#txtAddressLine').val();
    var addressId = $('#addresses option').filter(function () {
        return this.value === valueSelected;
    }).data('addressid');

    FillAddress(addressId);
});

$('input[name=rdoAddressType]').on('change', function () {

    var selectedValue = $('input[name=rdoAddressType]:checked').val();

    if (selectedValue === '1') {
        $('#lblIsDefault').text('Default billing address');
    }
    else if (selectedValue === '2') {
        $('#lblIsDefault').text('Default shipping address');
    }
    else if (selectedValue === '4') {
        $('#lblIsDefault').text('Default warehouse address');
    }


});


$('#txtCustomerIdForAddress ').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var customerId = '';
        customerId = $('#txtCustomerIdForAddress').val();

        if (customerId !== '') {
            var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);

            if (customerInfo !== "" && customerInfo !== null && customerInfo !== undefined) {
                var customer = JSON.parse(customerInfo);
                $('#txtCustomerIdForAddress').val(customer.Id);
                $('#txtCustomerNameForAddress').val(customer.CustomerName);
            }
            else {
                bootbox.alert('The customer was not found. Please check and try again.');
                event.preventDefault();
                return;
            }
        }
    }
});

$('#frmCustomerAddress').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmCustomerAddress').unbind('submit').submit(function () {

    var dataArray = GetAddressData();
    var customerId = dataArray[0].customerId;
    if (dataArray[0].addressLine === '' || dataArray[0].cityId === '0' || dataArray[0].provinceId === '0' || dataArray[0].countryId === '0') {
        event.preventDefault();
        bootbox.alert('Please enter address correctly with address line, city, province and country');
        return;
    }

    if (customerId === '0') {
        event.preventDefault();
        bootbox.alert('Customer id was not found to enter the address. Please check and try again');
        return;
    }

    result = PerformPostActionWithObject('Customer/AddAddress', dataArray);
    $('#frmCustomerAddress').trigger('reset');

    event.preventDefault();
    $('#loadAddressDataTable').load('Customer/LoadCustomerAddressData/' + customerId);

});


$('#customer-address-list').unbind().on('click', '.btnEditAddress', function (event) {
    //$('#txtCustomerId').prop('readonly', true);
    event.preventDefault();

    var customerId = $(this).data('customerid');
    var addressId = $(this).data('addressid');
    var addressTypeId = $(this).data('addresstypeid');
    var isDefault = $(this).data('isdefault').toLowerCase();

    if (isDefault === 'true') {
        $('#chkIsDefault').prop('checked', true);
    } else {
        $('#chkIsDefault').prop('checked', false);
    }

    if (addressTypeId === 1) {
        $('#rdoBilling').prop('checked', true);
        $('#lblIsDefault').text('Default billing address');
    }
    else if (addressTypeId === 2) {
        $('#rdoShipping').prop('checked', true);
        $('#lblIsDefault').text('Default shipping address');
    }
    else if (addressTypeId === 4) {
        $('#rdoWarehouse').prop('checked', true);
        $('#lblIsDefault').text('Default warehouse address');
    }
    if (addressId !== '') {
        FillAddress(addressId);
    }

});

$('.btnDeleteAddress').unbind().on('click', function () {

    var custId = $(this).data('customerid');

    var custAddData = {
        customerId: custId,
        addressId: $(this).data('addressid'),
        addressTypeId: $(this).data('addresstypeid')
    };

    bootbox.confirm("Selected address will be deleted from this customer. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithObject('Customer/RemoveAddress', [custAddData]);
            $('#loadAddressDataTable').load('Customer/LoadCustomerAddressData/' + custId);
            ClearAddressForm();
        }
    });

});


function GetFormData() {
    var customerData = {
        id: $('#txtCustomerId').val() === "" ? "0" : $('#txtCustomerId').val(),
        customerName: $('#txtCustomerName').val(),
        fuelSurChargePercentage: $('#txtFuelSurcharge').val(),
        discountPercentage: $('#txtSpecialDiscount').val(),
        invoiceDueDays: $('#txtInvoiceDueDays').val(),
        isGstApplicable: $('#isGstApplicable').is(':checked') ? 1 : 0,
        isActive: $('#chkIsActive').is(':checked') === true ? 1 : 0
    };

    return [customerData];
}

function GetAddressData() {
    var addressData = {
        customerId: $('#txtCustomerIdForAddress').val() === "" ? "0" : $('#txtCustomerIdForAddress').val(),
        addressTypeId: $('input[name="rdoAddressType"]:checked').val(),
        addressId: $('#hfAddressId').val(),
        isDefault: $('#chkIsDefault').is(':checked') ? 1 : 0,

        addressLine: $('#txtAddressLine').val(),
        unitNumber: $('#txtAddressUnit').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),
        contactPersonName: $('#txtContactPerson').val(),
        emailAddress1: $('#txtEmailAddress').val(),
        primaryPhoneNumber: $('#txtPrimaryPhoneNumber').val(),
        fax: $('#txtFaxNumber').val()
    };

    return [addressData];
}

function FillCustomerInformation(customerInfo) {

    $('#txtCustomerId').val(customerInfo.Id);
    $('#txtCustomerName').val(customerInfo.CustomerName);
    $('#txtFuelSurcharge').val(customerInfo.FuelSurChargePercentage);
    $('#txtSpecialDiscount').val(customerInfo.DiscountPercentage);
    $('#txtInvoiceDueDays').val(customerInfo.InvoiceDueDays);
}

function FillAddress(addressId) {
    var addressInfo = GetSingleById('Address/GetAddressById', addressId);

    if (addressInfo !== null && addressInfo !== undefined) {
        var address = JSON.parse(addressInfo);
        $('#hfAddressId').val(addressId);
        $('#txtAddressUnit').val(address.UnitNumber);
        $('#txtAddressLine').val(address.AddressLine);
        $('#ddlCityId').val(address.CityId);
        $('#ddlProvinceId').val(address.ProvinceId);
        $('#ddlCountryId').val(address.CountryId);
        $('#txtPostCode').val(address.PostCode);
        $('#txtContactPerson').val(address.ContactPersonName);
        $('#txtFaxNumber').val(address.Fax);
        $('#txtPrimaryPhoneNumber').val(address.PrimaryPhoneNumber);
        $('#txtEmailAddress').val(address.EmailAddress1);
    }
}

function ClearAddressForm() {
    $('#hfAddressId').val('');
    $('#txtAddressLine').val('');
    $('#txtAddressUnit').val('');
    $('#ddlCityId').val('335');
    $('#ddlProvinceId').val('7');
    $('#ddlCountryId').val('41');
    $('#txtPostCode').val('');
    $('#txtContactPerson').val('');
    $('#txtEmailAddress').val('');
    $('#txtPrimaryPhoneNumber').val('');
    $('#txtFaxNumber').val('');
    $('#chkIsDefault').prop('checked', false);
    $('#rdoBilling').prop('checked', true);

}