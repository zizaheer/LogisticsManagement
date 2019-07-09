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
    $('#txtEmployeeId').prop('readonly', true);
});

$('#btnClear').on('click', function () {
    $('#txtEmployeeId').prop('readonly', false);
});


$('#txtEmployeeId').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var employeeId = $('#txtEmployeeId').val();
        var employeeInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
        if (employeeInfo !== "" && employeeInfo !== null)
        {
            employeeInfo = JSON.parse(employeeInfo);
        }
        else
        {
            bootbox.alert('The employee was not found. Please check or select from the bottom list of employees.');
            event.preventDefault();
            return;
        }

        if (employeeInfo !== null)
        {
            FillEmployeeInfo(employeeInfo);
        }
    }
});

$('#chkIsHourlyPaid').on('change', function () {

    var isChecked = $('input[name=chkIsHourlyPaid]').is(':checked');
    if (!isChecked) {

        $('#txtHourlyRate').val('');
        $('#txtHourlyRate').prop('disabled', true);
    }
    else {
        $('#txtHourlyRate').prop('disabled', false);
    }
});
$('#chkIsSalaryEmployee').on('change', function () {

    var isChecked = $('input[name=chkIsSalaryEmployee]').is(':checked');
    if (!isChecked) {

        $('#txtSalaryAmount').val('');
        $('#txtSalaryAmount').prop('disabled', true);
    }
    else {
        $('#txtSalaryAmount').prop('disabled', false);
    }
});
$('#chkIsCommissionProvided').on('change', function () {

    var isChecked = $('input[name=chkIsCommissionProvided]').is(':checked');
    if (!isChecked) {

        $('#txtCommissionAmount').val('');
        $('#txtCommissionAmount').prop('disabled', true);
    }
    else {
        $('#txtCommissionAmount').prop('disabled', false);
    }
});
$('#chkIsFuelProvided').on('change', function () {

    var isChecked = $('input[name=chkIsFuelProvided]').is(':checked');
    if (!isChecked) {

        $('#txtFuelSurchargePercentage').val('');
        $('#txtFuelSurchargePercentage').prop('disabled', true);
    }
    else {
        $('#txtFuelSurchargePercentage').prop('disabled', false);
    }
});

$('#employee-list').on('click', '.btnEdit', function () {
    $('#txtEmployeeId').prop('readonly', true);

    var employeeId = $(this).data('employeeid');
    var employeeInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
    
    if (employeeInfo !== "") {
        employeeInfo = JSON.parse(employeeInfo);
    }
    else {
        bootbox.alert('The employee was not found. Please check or select from the bottom list of employees.');
        event.preventDefault();
        return;
    }

    FillEmployeeInfo(employeeInfo);
});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');

});

$('#frmEmployeeForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmEmployeeForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('Employee/Update', dataArray);
    }
    else {
        PerformPostActionWithObject('Employee/Add', dataArray);
    }
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    employeeId = $(this).data('employeeid');
    PerformPostActionWithId('Employee/Remove', employeeId);
    $('#loadDataTable').load('Employee/PartialViewDataTable');

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
