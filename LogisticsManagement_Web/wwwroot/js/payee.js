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
    $('#txtPayeeId').prop('readonly', true);
});

$('#btnClear').on('click', function () {
    $('#txtPayeeId').prop('readonly', false);
});



$('#payee-list').on('click', '.btnEdit', function () {
    $('#txtPayeeId').prop('readonly', true);

    var payeeId = $(this).data('payeeid');
    var payeeInfo = GetSingleById('Payee/GetPayeeById', payeeId);
    
    if (payeeInfo !== "") {
        payeeInfo = JSON.parse(payeeInfo);
    }
    else {
        bootbox.alert('The payee was not found. Please check or select from the list.');
        event.preventDefault();
        return;
    }

    FillPayeeInfo(payeeInfo);
});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Payee/PartialViewDataTable');

});

$('#frmPayeeForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmPayeeForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('Payee/Update', dataArray);
        bootbox.alert('Data updated successfully.');
    }
    else {
        PerformPostActionWithObject('Payee/Add', dataArray);
        bootbox.alert('Data saved successfully.');
    }
    $('#frmPayeeForm').trigger('reset');
    $('#loadDataTable').load('Payee/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    payeeId = $(this).data('payeeid');
    bootbox.confirm("Are you sure you want to delete the payee?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Payee/Remove', payeeId);
            $('#loadDataTable').load('Payee/PartialViewDataTable');
            $('#frmPayeeForm').trigger('reset');
        }
    });
});

function GetFormData() {
    var payeeData = {
        id: $('#txtPayeeId').val() === "" ? "0" : $('#txtPayeeId').val(),
        payeeName: $('#txtFirstName').val(),
        address: $('#txtLastName').val(),
        emailAddress: $('#txtDrivingLicenseNo').val(),
        phoneNumber: $('#txtSocialInsuranceNo').val(),
    };

    return [payeeData];
}

function FillPayeeInfo(payeeInfo) {

    $('#txtEmployeeId').val(payeeInfo.Id);
    $('#txtPayeeName').val(payeeInfo.PayeeName);
    $('#txtAddress').val(payeeInfo.Address);
    $('#txtEmailAddress').val(payeeInfo.EmailAddress);
    $('#txtPhoneNumber').val(payeeInfo.PhoneNumber);
}
