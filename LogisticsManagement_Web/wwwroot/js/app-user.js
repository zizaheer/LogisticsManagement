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

var imageFileInfo = null;

$('#btnNewUser').on('click', function () {
    $('#txtPassword').prop('disabled', false);
    $('#txtConfirmPassword').prop('disabled', false);
    $('#frmUserForm').trigger('reset');

    $('#modalUser').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#modalUser').draggable();
    $('#modalUser').modal('show');

    ResetDefaultProfilePicture();
});

$('#user-list').on('click', '.btnEdit', function () {
    var userId = $(this).data('userid');
    var userInfo = GetSingleById('User/GetUserById', userId);

    if (userInfo !== "") {
        userInfo = JSON.parse(userInfo);
        FillUserInfo(userInfo);

        $('#modalUser').modal({
            backdrop: 'static',
            keyboard: false
        });
        $('#modalUser').draggable();
        $('#modalUser').modal('show');
    }
    else {
        bootbox.alert('The user was not found. Please check or select from the bottom list of users.');
        event.preventDefault();
        return;
    }
});

$('#btnDownloadUserData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadUserDataTable').load('User/PartialViewDataTable');

});



$('#frmUserForm').unbind('submit').submit(function (event) {

    var dataArray = GetFormData();

    var existingUser = GetSingleById('User/GetUserByUserName', dataArray[0].userName);
    if (existingUser !== "") {
        bootbox.alert("User exist with the user name. Try a different user name.");
        event.preventDefault();
        return;
    }
    if (dataArray[0].firstName === "") {
        bootbox.alert("Please enter first name.");
        event.preventDefault();
        return;
    }
    if (dataArray[0].groupId === '0') {
        bootbox.alert("Please select user group.");
        event.preventDefault();
        return;
    }
    if (dataArray[0].userName === '') {
        bootbox.alert("Please enter user name.");
        event.preventDefault();
        return;
    }
    if (dataArray[0].password === '') {
        bootbox.alert("Please enter password.");
        event.preventDefault();
        return;
    }
    if ($('#txtPassword').val() !== $('#txtConfirmPassword').val()) {
        bootbox.alert("Password doesn't match with confirm password");
        event.preventDefault();
        return;
    }

    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('User/Update', dataArray);
    }
    else {
        PerformPostActionWithObject('User/Add', dataArray);
    }
    event.preventDefault();
    $('#loadUserDataTable').load('User/PartialViewDataTable');
    $('#modalUser').modal('hide');
});

$('.btnDelete').unbind().on('click', function () {
    userId = $(this).data('userid');
    bootbox.confirm("This user will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('User/Remove', userId);
            $('#loadUserDataTable').load('User/PartialViewDataTable');
        }
    });
});

function GetFormData() {

    var userData = {
        id: $('#txtUserId').val() === "" ? "0" : $('#txtUserId').val(),
        employeeId: $('#ddlEmployeeId').val(),
        userName: $('#txtUserName').val(),
        password: $('#txtPassword').val(),
        firstName: $('#txtFirstName').val(),
        lastName: $('#txtLastName').val(),
        groupId: $('#ddlUserGroupId').val(),
        addressLine: $('#txtAddressLine').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),
        phoneNumber: $('#txtPhoneNumber').val(),
        emailAddress: $('#txtEmailAddress').val(),
        isActive: $('#chkIsActive').is(':checked') === true ? 1 : 0

    };

    var profilePicture = $('#imgProfilePic').prop('src');

    return [userData, profilePicture];
}

function FillUserInfo(userInfo) {
    $('#txtPassword').prop('disabled', true);
    $('#txtConfirmPassword').prop('disabled', true);
    $('#txtUserId').val(userInfo.Id);
    $('#ddlEmployeeId').val(userInfo.EmployeeId);
    $('#txtUserName').val(userInfo.UserName);
    $('#txtPassword').val('**********');
    $('#txtConfirmPassword').val('**********');
    $('#txtFirstName').val(userInfo.FirstName);
    $('#txtLastName').val(userInfo.LastName);
    $('#ddlUserGroupId').val(userInfo.GroupId);
    $('#txtAddressLine').val(userInfo.AddressLine);
    $('#ddlCityId').val(userInfo.CityId);
    $('#ddlProvinceId').val(userInfo.ProvinceId);
    $('#ddlCountryId').val(userInfo.CountryId);
    $('#txtPostCode').val(userInfo.PostCode);
    $('#txtPhoneNumber').val(userInfo.PhoneNumber);
    $('#txtEmailAddress').val(userInfo.EmailAddress);

    if (userInfo.ProfilePicture !== null) {
        $('#imgProfilePic').prop('src', 'data:image/png;base64,' + userInfo.ProfilePicture);
    }
    else {
        ResetDefaultProfilePicture();
    }


    if (userInfo.IsActive) {
        $('#chkIsActive').prop('checked', true);
    } else {
        $('#chkIsActive').prop('checked', false);
    }

}

function GetImageFile(imgPath) {
    var fileInfo = null;

    if (imgPath.files && imgPath.files[0]) {

        var reader = new FileReader();
        reader.onload = function (e) {
            fileInfo = e.target.result;
            $('#imgProfilePic').attr('src', fileInfo);
        };

        reader.readAsDataURL(imgPath.files[0]);
    }
    return fileInfo;
}

function ResetDefaultProfilePicture() {
    $('#imgProfilePic').prop('src', '../images/others/no-image.png');
}

$('#fileProfilePic').on('change', function () {
    GetImageFile(this);
});

