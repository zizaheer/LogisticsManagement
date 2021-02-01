$(document).ready(function () {
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
});

$('#btnCloseModal').on('click', function () {
    $('#bankInformation').modal('hide');
});

$('#btnNewBank').on('click', function () {
    $('#frmBankForm').trigger('reset');
    $('#bankInformation').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#bankInformation').draggable();
    $('#bankInformation').modal('show');
});

$('#frmBankForm').unbind('submit').submit(function () {
    event.preventDefault();
    var dataArray = GetFormData();

    if (dataArray[0].bankName === '') {
        event.preventDefault();
        bootbox.alert('Please enter bank name.');
        return;
    }

    var result = '';

    if (dataArray[0].id > 0) {
        result = PerformPostActionWithObject('Bank/Update', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the bank. Please check your data and try again.');
        }
    }
    else {
        result = PerformPostActionWithObject('Bank/Add', dataArray);
        if (result.length > 0) {
            location.reload();
        } else {
            bootbox.alert('Failed! Something went wrong during adding the bank. Please check your data and try again.');
        }
    }

});

$('#bank-list').on('click', '.btnEdit', function () {
    $('#txtBankId').prop('readonly', true);
    var bankId = $(this).data('bankid');

    if (bankId !== '') {
        var bankInfo = GetSingleById('Bank/GetBankById', bankId);
        if (bankInfo != null && bankInfo !== '') {
            FillBankInformation(JSON.parse(bankInfo));
            $('#bankInformation').modal({
                backdrop: 'static',
                keyboard: false
            });
            $('#bankInformation').draggable();
            $('#bankInformation').modal('show');
        }
        else {
            bootbox.alert('The bank was not found. Please check and try again.');
            event.preventDefault();
            return;
        }
    }
});

$('.btnDelete').unbind().on('click', function () {
    var bankId = $(this).data('bankid');
    bootbox.confirm("This bank will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Bank/Remove', bankId);
            location.reload();
        }
    });
});


function GetFormData() {
    var bankData = {
        id: $('#txtBankId').val() === "" ? "0" : $('#txtBankId').val(),
        bankName: $('#txtBankName').val(),
        instituteNumber: "",
    };

    return [bankData];
}


function FillBankInformation(bankInfo) {
    $('#txtBankId').val(bankInfo.Id);
    $('#txtBankName').val(bankInfo.BankName);
}
