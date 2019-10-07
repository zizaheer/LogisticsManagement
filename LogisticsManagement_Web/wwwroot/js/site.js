// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

// Please refer to bundleconfig.json where the js files are being minified as site.min.js which is referenced on _Layout template.
$(document).ready(function () {
    //displayClock();
    
    setInterval(CallServer, 10000);
});


function CallServer()
{
    PerformPostAction("Home/KeepMeAlive");
    //console.log('call server');
}

$('#lnkEmployeeClockIn').on('click', function () {
    displayClock();
    setInterval(displayClock, 1000);

    var employeeId = '00';

    var userClockInInfo = GetSingleById('EmployeeTimesheet/GetEmployeeClockInInfo', employeeId);
    if (userClockInInfo !== '' && userClockInInfo != null) {
        var parsedData = JSON.parse(userClockInInfo);
        $('#txtEmployeeClockInId').val(parsedData.EmployeeId);
        $('#txtClockInTime').val(parsedData.SignInDatetime);
        $('#txtClockOutTime').val(parsedData.SignOutDatetime);

    }
    $('#employeeClockIn').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#employeeClockIn').draggable();
    $('#employeeClockIn').modal('show');
});
function displayClock() {
    var currentDate = new Date();
    document.getElementById('lblCurrentTime').innerHTML = currentDate.toLocaleString();
}


$('#btnClockIn').on('click', function (event) {
    event.preventDefault();
    var data = GetClockInData();
    console.log(data);

    if (data.clockInTime != null && data.clockInTime.length > 0) {
        bootbox.alert('You have already clocked-in. To change existing timing please contact admin');
        return;
    }
    else {
        var result = PerformPostActionWithObject('EmployeeTimesheet/Add', [data]);
        if (result.length > 0) {
            bootbox.alert('You have successfully clocked-in. ');
        }
    }
});

$('#btnClockOut').on('click', function (event) {
    event.preventDefault();

    var data = GetClockInData();

    if (data.clockInTime.length < 1) {
        bootbox.alert('Please clock-in first. ');
        return;
    }
    else {
        var result = PerformPostActionWithObject('EmployeeTimesheet/Update', [data]);
        if (result.length > 0) {
            bootbox.alert('You have successfully clocked-out. ');
        }
    }
});

function GetClockInData() {
    var empId = $('#txtEmployeeClockInId').val();
    var clockInTime = $('#txtClockInTime').val();
    var clockOutTime = $('#txtClockOutTime').val();
    var remarks = ""; //$('#txtRemarks').val();
    var breakTime = ""; //$('#txtBreaKTime').val();

    var clockInData = {
        empId: empId,
        clockInTime: clockInTime,
        clockOutTime: clockOutTime,
        remarks: remarks,
        breakTime: breakTime
    };

    return clockInData;
}


$('#lnkChangePassword').on('click', function (event) {
    event.preventDefault();
    $('#changePassword').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#changePassword').draggable();
    $('#changePassword').modal('show');
});
$('#btnSavePassword').on('click', function (event) {
    event.preventDefault();

    var newPass = $('#txtNewPassword').val();
    var confirmNewPass = $('#txtConfirmNewPassword').val();

    if (newPass !== confirmNewPass) {
        bootbox.alert('New password must match with Confirm new password. Please check and try again');
        return;
    }

    var currentPass = $('#txtCurrentPassword').val();
    var isCurrentPasswordValid = GetSingleById('User/ValidateCurrentUserByPassword', currentPass);
    if (isCurrentPasswordValid == null || isCurrentPasswordValid.length < 1) {
        bootbox.alert('Your current password is invalid');
        return;
    }

    var data = {
        newPass: newPass,
        currentPass: currentPass
    };

    var updateStatus = PerformPostActionWithObject('User/UpdatePassword', [data]);
    if (updateStatus.length > 0) {
        bootbox.alert('Your password has been changed successfully');
    }
    else {
        bootbox.alert('Failed! Something went wrong. Please try again later.');
        return;
    }

});