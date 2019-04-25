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

$('#btnNew').on('click', function () {
    $('#txtUserId').prop('readonly', true);
});

$('#btnClear').on('click', function () {
    $('#txtUserId').prop('readonly', false);
});


$('#txtUserId').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var userId = $('#txtUserId').val();
        var userInfo = GetSingleObjectById('User/GetUserById', userId);
        if (userInfo !== "" && userInfo !== null) {
            userInfo = JSON.parse(userInfo);
        }
        else {
            bootbox.alert('The user was not found. Please check or select from the bottom list of users.');
            event.preventDefault();
            return;
        }

        if (userInfo !== null) {
            FillUserInfo(userInfo);
        }
    }
});

$('#user-list').on('click', '.btnEdit', function () {
    $('#txtUserId').prop('readonly', true);

    var userId = $(this).data('userid');
    var userInfo = GetSingleObjectById('User/GetUserById', userId);

    if (userInfo !== "") {
        userInfo = JSON.parse(userInfo);
    }
    else {
        bootbox.alert('The user was not found. Please check or select from the bottom list of users.');
        event.preventDefault();
        return;
    }

    FillUserInfo(userInfo);
});

$('#btnDownloadUserData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadUserDataTable').load('User/PartialViewDataTable');

});

$('#frmUserForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmUserForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        UpdateEntry('User/Update', dataArray);
    }
    else {
        AddEntry('User/Add', dataArray);
    }
    event.preventDefault();
    $('#loadUserDataTable').load('User/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    userId = $(this).data('userid');
    RemoveEntry('User/Remove', userId);
    $('#loadUserDataTable').load('User/PartialViewDataTable');

});

function GetFormData() {

    var userData = {

        id: $('#txtEmployeeId').val() === "" ? "0" : $('#txtEmployeeId').val(),
        firstName: $('#txtFirstName').val(),
        lastName: $('#txtLastName').val(),
        driverLicenseNo: $('#txtDrivingLicenseNo').val(),
        socialInsuranceNo: $('#txtSocialInsuranceNo').val(),
        unitNumber: $('#txtUnitNumber').val(),
        addressLine: $('#txtAddressLine').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),
        phoneNumber: $('#txtPhoneNumber').val(),
        mobileNumber: $('#txtMobileNo').val(),
        faxNumber: $('#txtFaxNo').val(),
        emailAddress: $('#txtEmailAddress').val(),
        isActive: $('#chkIsActive').is(':checked') ? 1 : 0

    };

    return userData;
}

function FillUserInfo(userInfo) {

    $('#txtEmployeeId').val(employeeInfo.Id);
    $('#txtFirstName').val(employeeInfo.FirstName);
    $('#txtLastName').val(employeeInfo.LastName);
    $('#txtDrivingLicenseNo').val(employeeInfo.DriverLicenseNo);
    $('#txtSocialInsuranceNo').val(employeeInfo.SocialInsuranceNo);
    $('#txtUnitNumber').val(employeeInfo.UnitNumber);
    $('#txtAddressLine').val(employeeInfo.AddressLine);
    $('#ddlCityId').val(employeeInfo.CityId);
    $('#ddlProvinceId').val(employeeInfo.ProvinceId);
    $('#ddlCountryId').val(employeeInfo.CountryId);
    $('#txtPostCode').val(employeeInfo.PostCode);
    $('#txtPhoneNumber').val(employeeInfo.PhoneNumber);
    $('#txtMobileNo').val(employeeInfo.MobileNumber);
    $('#txtFaxNo').val(employeeInfo.FaxNumber);
    $('#txtEmailAddress').val(employeeInfo.EmailAddress);

    $('#ddlEmployeeTypeId').val(employeeInfo.EmployeeTypeId);

  

}
