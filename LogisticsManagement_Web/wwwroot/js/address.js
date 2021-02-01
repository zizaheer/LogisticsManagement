var employeeData;

$(document).ready(function () {

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

$('#btnNewAddress').on('click', function () {
    $('#frmAddressForm').trigger('reset');
    $('#addressModal').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#addressModal').draggable({
        handle: ".modal-header"
    });
    $('#addressModal').modal('show');
});

$('#address-list').on('click', '.btnEdit', function () {
    var addressId = $(this).data('addressid');
    var addressInfo = GetSingleById('Address/GetAddressById', addressId);

    if (addressInfo !== "") {
        addressInfo = JSON.parse(addressInfo);
    }
    else {
        bootbox.alert('The address was not found. Please check or select from the bottom list of addresses.');
        event.preventDefault();
        return;
    }

    FillAddressInfo(addressInfo);

    $('#addressModal').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#addressModal').draggable();
    $('#addressModal').modal('show');
});

$('#btnDownloadAddressData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadAddressDataTable').load('Address/PartialViewDataTable');
});

$('#frmAddressForm').unbind('submit').submit(function (event) {
    var data = GetFormData();
    
    if (data.id > 0) {
        PerformPostActionWithObject('Address/Update', data);
    }
    else {
        PerformPostActionWithObject('Address/Add', data);
    }
    event.preventDefault();
    location.reload();
});

$('.btnDelete').unbind().on('click', function () {
    addressId = $(this).data('addressid');
    bootbox.confirm("This user will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Address/Remove', addressId);
            $('#loadAddressDataTable').load('Address/PartialViewDataTable');
        }
    });
});

function GetFormData() {

    var addressData = {
        id: $('#txtAddressId').val() === "" ? "0" : $('#txtAddressId').val(),
        unitNumber: $('#txtUnitNumber').val(),
        addressLine: $('#txtAddressLine').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),

        contactPersonName: $('#txtContactPersonName').val(),
        fax: $('#txtFaxNo').val(),
        primaryPhoneNumber: $('#txtPhoneNumber').val(),
        mobileNumber: $('#txtMobileNo').val(),
        emailAddress1: $('#txtEmailAddress1').val(),
        emailAddress2: $('#txtEmailAddress2').val()
    };

    return addressData;
}

function FillAddressInfo(addressInfo) {

    $('#txtAddressId').val(addressInfo.Id);
    $('#txtUnitNumber').val(addressInfo.UnitNumber);
    $('#txtAddressLine').val(addressInfo.AddressLine);
    $('#ddlCityId').val(addressInfo.CityId);
    $('#ddlProvinceId').val(addressInfo.ProvinceId);
    $('#ddlCountryId').val(addressInfo.CountryId);
    $('#txtPostCode').val(addressInfo.PostCode);

    $('#txtContactPersonName').val(addressInfo.ContactPersonName);
    $('#txtFaxNo').val(addressInfo.Fax);
    $('#txtPhoneNumber').val(addressInfo.PrimaryPhoneNumber);
    $('#txtMobileNo').val(addressInfo.MobileNumber);
    $('#txtEmailAddress1').val(addressInfo.EmailAddress1);
    $('#txtEmailAddress2').val(addressInfo.EmailAddress2);

}
