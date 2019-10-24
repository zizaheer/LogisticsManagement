

$(document).ready(function () {

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    var currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 7);
    $('#txtStartDate').val(ConvertDateToUSFormat(currentDate));
    $('#txtToDate').val(ConvertDateToUSFormat(new Date));

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

var wayBillNumberArray = [];

$('#frmPayrollGenerationForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmPayrollGenerationForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('Employee/Update', dataArray);
        bootbox.alert('Data updated successfully.');
    }
    else {
        PerformPostActionWithObject('Employee/Add', dataArray);
        bootbox.alert('Data saved successfully.');
    }
    $('#frmEmployeeForm').trigger('reset');
    $('#loadDataTable').load('Employee/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    employeeId = $(this).data('employeeid');
    bootbox.confirm("Are you sure you want to delete the employee?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Employee/Remove', employeeId);
            $('#loadDataTable').load('Employee/PartialViewDataTable');
            $('#frmEmployeeForm').trigger('reset');
        }
    });
});






$('#btnLoadEmployee').unbind().on('click', function (event) {
    event.preventDefault();

    var empType = $('#ddlEmployeeTypeIdForPayroll').val();

    $('#loadEmployeeList').load('EmployeePayroll/PartialLoadEmployees?empTypeId=' + empType);

});

$('#employee-list .btnSelect').unbind().on('click', function () {

    var empId = $(this).data('employeeid');
    var employeeInfo = GetSingleById('Employee/GetEmployeeById', empId);
    if (employeeInfo !== '') {
        var employee = JSON.parse(employeeInfo);

        $('#txtEmployeeIdForPayroll').val(employee.Id);
        $('#txtEmployeeNameForPayroll').val(employee.FirstName);
    } else {
        $('#txtEmployeeIdForPayroll').val('');
        $('#txtEmployeeNameForPayroll').val('');
    }
});

$('#btnLoadDelivery').unbind().on('click', function () {

    var empId = $('#txtEmployeeIdForPayroll').val();
    var fromDate = $('#txtStartDate').val();
    var toDate = $('#txtToDate').val();

    $('#loadEmployeeOrderList').load('EmployeePayroll/PartialLoadEmployeeOrders?empId=' + empId + '&fromDate=' + fromDate + '&toDate=' + toDate);

});

$('#btnChangeOrderShare').unbind().on('click', function () {

    if (wayBillNumberArray.length > 1) {
        bootbox.alert('Please select only one order to modify.');
        return;
    }
    if (wayBillNumberArray.length < 1) {
        bootbox.alert('Please select an order to modify.');
        return;
    }

    var orderId = wayBillNumberArray[0];
    
   

});



$('#delivery-list .chkSelectedOrder').on('change', function (event) {
    event.preventDefault();

    var wbNumber =
    {
        wbillNumber: $(this).data('waybillnumber')
    };

    var isChecked = $(this).is(':checked');

    var index = wayBillNumberArray.findIndex(c => c.wbillNumber === wbNumber.wbillNumber);
    if (index >= 0) {
        wayBillNumberArray.splice(index, 1);
    }

    if (isChecked) {
        wayBillNumberArray.push(wbNumber);
    }

});
$('#delivery-list .chkCheckAllOrders').on('change', function (event) {
    event.preventDefault();
    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkSelectedOrder').prop('checked', true);
        var wbArrayString = $('#hfWaybillArray').val();
        wayBillNumberArray = [];
        var wbArray = wbArrayString.split(',');
        $.each(wbArray, function (i, item) {
            if (item !== '') {
                wayBillNumberArray.push({ wbillNumber: parseInt(item) });
            }
        });
    } else {
        $('.chkSelectedOrder').prop('checked', false);
        wayBillNumberArray = [];
    }
});

function GetFormData() {

    var employeeData = {

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

        employeeTypeId: $('#ddlEmployeeTypeId').val(),
        isHourlyPaid: $('#chkIsHourlyPaid').is(':checked') === true ? 1 : 0,
        hourlyRate: $('#txtHourlyRate').val(),
        isSalaried: $('#chkIsSalaryEmployee').is(':checked') === true ? 1 : 0,
        salaryAmount: $('#txtSalaryAmount').val(),
        salaryTerm: $('#ddlSalaryTermId').val(),
        isCommissionProvided: $('#chkIsCommissionProvided').is(':checked') === true ? 1 : 0,
        commissionPercentage: $('#txtCommissionAmount').val(),
        isFuelChargeProvided: $('#chkIsFuelProvided').is(':checked') === true ? 1 : 0,
        fuelPercentage: $('#txtFuelSurchargePercentage').val(),
        radioInsuranceAmount: $('#txtRadioInsuranceAmount').val(),
        insuranceAmount: $('#txtInsuranceAmount').val(),

        isActive: $('#chkIsActive').is(':checked') ? 1 : 0

    };

    return [employeeData];
}

function FillEmployeeInfo(employeeInfo) {

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

    if (employeeInfo.IsHourlyPaid) {
        $('#chkIsHourlyPaid').prop('checked', true);
        $('#txtHourlyRate').prop('disabled', false);
    }
    else {
        $('#chkIsHourlyPaid').prop('checked', false);
    }

    $('#txtHourlyRate').val(employeeInfo.HourlyRate);

    if (employeeInfo.IsSalaried) {
        $('#chkIsSalaryEmployee').prop('checked', true);
        $('#txtSalaryAmount').prop('disabled', false);
    }
    else {
        $('#chkIsSalaryEmployee').prop('checked', false);
    }

    $('#txtSalaryAmount').val(employeeInfo.SalaryAmount);
    $('#ddlSalaryTermId').val(employeeInfo.SalaryTerm);

    if (employeeInfo.IsCommissionProvided) {
        $('#chkIsCommissionProvided').prop('checked', true);
        $('#txtCommissionAmount').prop('disabled', false);
    }
    else {
        $('#chkIsCommissionProvided').prop('checked', false);
    }

    $('#txtCommissionAmount').val(employeeInfo.CommissionPercentage);

    if (employeeInfo.IsFuelChargeProvided) {
        $('#chkIsFuelProvided').prop('checked', true);
        $('#txtFuelSurchargePercentage').prop('disabled', false);
    }
    else {
        $('#chkIsFuelProvided').prop('checked', false);
    }

    $('#txtFuelSurchargePercentage').val(employeeInfo.FuelPercentage);

    $('#txtRadioInsuranceAmount').val(employeeInfo.RadioInsuranceAmount);
    $('#txtInsuranceAmount').val(employeeInfo.InsuranceAmount);

    if (employeeInfo.IsActive) {
        $('#chkIsActive').prop('checked', true);
    }
    else {
        $('#chkIsActive').prop('checked', false);
    }

}
