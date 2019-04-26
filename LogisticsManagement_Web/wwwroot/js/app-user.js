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
    var data = GetFormData();

    if (data.id > 0) {
        UpdateEntry('User/Update', data);
    }
    else {
        AddEntry('User/Add', data);
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

        id: $('#txtUserId').val() === "" ? "0" : $('#txtUserId').val(),
        userName: $('#txtUserName').val(),
        password: $('#txtPassword').val(),
        //confirmPassword: $('#txtConfirmPassword').val(),
        firstName: $('#txtFirstName').val(),
        lastName: $('#txtLastName').val(),
        userGroupId: $('#ddlUserGroupId').val(),
        addressLine: $('#txtAddressLine').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),
        phoneNumber: $('#txtPhoneNumber').val(),
        emailAddress: $('#txtEmailAddress').val(),
        profilePicture: $('#fileProfilePic').val(),
        isActive: $('#chkIsActive').is(':checked') ===true ? 1 : 0

    };

    return userData;
}

function FillUserInfo(userInfo) {

    $('#txtUserId').val(userInfo.Id);
    $('#txtUserName').val(userInfo.UserName);
    $('#txtPassword').val('**********');
    $('#txtConfirmPassword').val('**********');
    $('#txtFirstName').val(userInfo.FirstName);
    $('#txtLastName').val(userInfo.LastName);
    $('#ddlUserGroupId').val(userInfo.GroupId);
    $('#txtAddressLine').val(userInfo.Address);
    $('#ddlCityId').val(userInfo.CityId);
    $('#ddlProvinceId').val(userInfo.ProvinceId);
    $('#ddlCountryId').val(userInfo.CountryId);
    $('#txtPostCode').val(userInfo.PostCode);
    $('#txtPhoneNumber').val(userInfo.PhoneNumber);
    $('#txtEmailAddress').val(userInfo.EmailAddress);
    $('#fileProfilePic').val(userInfo.ProfilePicture);
    if (userInfo.IsActive) {
        $('#chkIsActive').prop('checked', true);
    } else {
        $('#chkIsActive').prop('checked', false);
    }

}
