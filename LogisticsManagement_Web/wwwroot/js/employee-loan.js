
$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

$('#btnNew').on('click', function () {
    $('#hfEmployeeLoanId').val('');
});


$('#txtEmployeeIdForLoan').unbind('keypress').on('keypress', function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var employeeId = $('#txtEmployeeIdForLoan').val();
        var employeeInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
        if (employeeInfo !== "" && employeeInfo !== null) {
            employeeInfo = JSON.parse(employeeInfo);
            $('#txtEmployeeName').val(employeeInfo.FirstName + ' ' + (employeeInfo.LastName != null ? employeeInfo.LastName : '')); 
        }
        else {
            $('#txtEmployeeName').val('');
            bootbox.alert('The employee was not found. Please check.');
            return;
        }
    }
});

//$('#employee-loan-list').on('click', '.btnEdit', function () {
//    $('#txtEmployeeIdForLoan').prop('readonly', true);

//    var loanId = $(this).data('loanid');
//    var loanInfo = GetSingleById('EmployeeLoan/GetLoanInfoByLoanId', loanId);

//    if (loanInfo !== "") {
//        employeeLoanInfo = JSON.parse(loanInfo);
//        FillEmployeeLoanInfo(employeeLoanInfo);
//    }
//    else {
//        bootbox.alert('The employee was not found. Please check.');
//        event.preventDefault();
//        return;
//    }

//    FillEmployeeInfo(employeeInfo);
//});

$('#frmLoanForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmLoanForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();

    //if (dataArray[0].id > 0) {
    //    PerformPostActionWithObject('EmployeeLoan/Update', dataArray);
    //    bootbox.alert('Data updated successfully.');
    //}
    //else {
    var result = PerformPostActionWithObject('EmployeeLoan/Add', dataArray);
    if (result.length > 0) {
        bootbox.alert('Data saved successfully.');
        $('#frmLoanForm').trigger('reset');
        $('#loadEmployeeWithLoans').load('EmployeeLoan/PartialViewEmployeeLoans');
    } else {
        bootbox.alert('Failed! An error occurred during saving loan information.');
    }
    
    //}
    
});

$('#employee-loan-list .btnDeleteLoan').unbind().on('click', function () {
    loanId = $(this).data('loanid');
    bootbox.confirm("Are you sure you want to delete this loan entry?", function (result) {
        if (result === true) {
            PerformPostActionWithId('EmployeeLoan/Remove', loanId);
            $('#loadEmployeeWithLoans').load('EmployeeLoan/PartialViewEmployeeLoans');
            $('#frmLoanForm').trigger('reset');
        }
    });
});

function GetFormData() {
    var employeeLoanData = {
        id: $('#hfEmployeeLoanId').val() === "" ? "0" : $('#hfEmployeeLoanId').val(),
        employeeId: $('#txtEmployeeIdForLoan').val(),
        loanAmount: $('#txtNewSanctionAmnt').val(),
        loanTakenOn: $('#txtDateTaken').val(),
        remarks: $('#txtLoanRemarks').val()
    };

    return [employeeLoanData];
}

function FillEmployeeLoanInfo(employeeLoanInfo) {

    $('#hfEmployeeLoanId').val(employeeLoanInfo.Id);
    $('#txtEmployeeIdForLoan').val(employeeLoanInfo.EmployeeId);
    $('#txtNewSanctionAmnt').val(employeeLoanInfo.LoanAmount);
    $('#txtDateTaken').val(employeeLoanInfo.LoanTakenOn);
    $('#txtLoanRemarks').val(employeeLoanInfo.Remarks);
}

function ClearFields() {
    $('#hfEmployeeLoanId').val('');
    $('#txtEmployeeIdForLoan').val('');
    $('#txtNewSanctionAmnt').val('');
    $('#txtDateTaken').val('');
    $('#txtLoanRemarks').val('');
}