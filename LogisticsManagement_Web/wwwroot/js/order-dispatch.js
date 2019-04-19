﻿
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    //FillEmployeeDropDown();
    $('#txtDispatchDateTime').val(ConvertDatetimeToUSDatetime(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

var wayBillNumberArray = [];
var employeeNumber;


$('#btnDownloadOrderData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Order/PartialViewDataTable');

});



$('#orderdispatch-list').on('click', '.chkOrderSelected', function (event) {
    //event.preventDefault();

    var wbNumber =
    {
        wbillNumber: $(this).data('waybillnumber')
    };


    var index = wayBillNumberArray.findIndex(c => c.wbillNumber === wbNumber.wbillNumber);
    if (index >= 0) {
        wayBillNumberArray.splice(index, 1);
    }

    wayBillNumberArray.push(wbNumber);
});

$('#txtEmployeeNumber').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        $('#ddlEmployeeId').val($('#txtEmployeeNumber').val());
    }

});

$('#frmOrderDispatchForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderDispatchForm').unbind('submit').submit(function (event) {
    employeeNumber = $('#ddlEmployeeId').val();
    dispatchDate = $('#txtDispatchDateTime').val();
    if (employeeNumber < 1) {
        alert('Please select employee.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumberArray, employeeNumber, dispatchDate];

    UpdateEntry('OrderDispatch/Update', dataArray);

    event.preventDefault();
    $('#loadDataTable').load('OrderDispatch/PartialViewDataTable');
});



function FillEmployeeDropDown() {
    var employees = JSON.parse(GetListObject('Employee/GetEmployees'));
    var employeeDropDown = $('#ddlEmployeeId');

    for (var i = 0; i < employees.length; i++) {
        employeeDropDown.append('<option value=' + employees[i].Id + '>' + employees[i].FirstName + ' ' + (employees[i].LastName === null ? '' : employees[i].LastName) + '  (' + employees[i].EmployeeNumber + ') ' + '</option>');
    }
}