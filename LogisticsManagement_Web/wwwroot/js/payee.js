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

$('#btnNewPayee').on('click', function () {
    $('#frmPayeeForm').trigger('reset');
    $('#modalPayee').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#modalPayee').draggable();
    $('#modalPayee').modal('show');

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

$('#frmPayeeForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();

    if (dataArray[0].payeeName == "") {
        bootbox.alert('Please enter payee name');
        event.preventDefault();
        return;
    }

    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('Payee/Update', dataArray);
    }
    else {
        PerformPostActionWithObject('Payee/Add', dataArray);
    }

    $('#loadDataTable').load('Payee/PartialViewDataTable');
    $('#modalPayee').modal('hide');
});

$('.btnDelete').unbind().on('click', function () {
    payeeId = $(this).data('payeeid');
    bootbox.confirm("Are you sure you want to delete the payee?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Payee/Remove', payeeId);
            $('#loadDataTable').load('Payee/PartialViewDataTable');
        }
    });

});

function GetFormData() {
    var payeeData = {
        id: $('#txtPayeeId').val() === "" ? "0" : $('#txtPayeeId').val(),
        payeeName: $('#txtPayeeName').val(),
        address: $('#txtAddress').val(),
        emailAddress: $('#txtEmailAddress').val(),
        phoneNumber: $('#txtPhoneNumber').val(),
    };

    return [payeeData];
}

function FillPayeeInfo(payeeInfo) {

    $('#txtPayeeId').val(payeeInfo.Id);
    $('#txtPayeeName').val(payeeInfo.PayeeName);
    $('#txtAddress').val(payeeInfo.Address);
    $('#txtEmailAddress').val(payeeInfo.EmailAddress);
    $('#txtPhoneNumber').val(payeeInfo.PhoneNumber);

    $('#modalPayee').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#modalPayee').draggable();
    $('#modalPayee').modal('show');
}
