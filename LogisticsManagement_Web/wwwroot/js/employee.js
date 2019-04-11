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
    $('#txtEmployeeId').removeAttr('readonly');
});

$('#employee-list').on('click', '.btnEdit', function () {
    $('#txtEmployeeId').attr('readonly', true);
    var data = $(this).data('employee');

    $('#txtEmployeeId').val(data.id);
    $('#txtFirstName').val(data.firstName);
    $('#txtLastName').val(data.lastName);
    $('#txtDrivingLicenseNo').val(data.driverLicenseNo);
    $('#txtSocialInsuranceNo').val(data.socialInsuranceNo);
    $('#txtUnitNumber').val(data.unitNumber);
    $('#txtAddressLine').val(data.addressLine);
    $('#ddlCityId').val(data.cityId);
    $('#ddlProvinceId').val(data.provinceId);
    $('#ddlCountryId').val(data.countryId);
    $('#txtPostCode').val(data.postCode);
    $('#txtPhoneNumber').val(data.phoneNumber);
    $('#txtMobileNo').val(data.mobileNumber);
    $('#txtFaxNo').val(data.faxNumber);
    $('#txtEmailAddress').val(data.emailAddress);

    $('#ddlEmployeeTypeId').val(data.employeeTypeId);
    $('#chkIsHourlyPaid').val(data.employeeTypeId);
    $('#txtHourlyRate').val(data.id);
    $('#chkIsSalaryEmployee').val(data.isSalaried);
    $('#txtSalaryAmount').val(data.salaryAmount);
    $('#ddlSalaryTermId').val(data.salaryTerm);
    $('#chkIsCommissionProvided').val(data.isCommissionProvided);
    $('#txtCommissionAmount').val(data.commissionPercentage);
    $('#chkIsFuelProvided').val(data.isFuelChargeProvided);
    $('#txtFuelSurchargePercentage').val(data.fuelPercentage);
    $('#txtRadioInsuranceAmount').val(data.radioInsuranceAmount);
    $('#txtInsuranceAmount').val(data.insuranceAmount);

    $('#chkIsActive').val(data.isActive);


});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');

});

$('#frmEmployeeForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        UpdateEntry('Employee/Update', dataArray);
    }
    else {
        AddEntry('Employee/Add', dataArray);
    }
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    employeeId = $(this).data('employeeid');
    RemoveEntry('Employee/Remove', employeeId);
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
        isHourlyPaid: $('#chkIsHourlyPaid').is(':checked') ? 1 : 0,
        hourlyRate: $('#txtHourlyRate').val(),
        isSalaried: $('#chkIsSalaryEmployee').is(':checked') ? 1 : 0,
        salaryAmount: $('#txtSalaryAmount').val(),
        salaryTerm: $('#ddlSalaryTermId').val(),
        isCommissionProvided: $('#chkIsCommissionProvided').is(':checked') ? 1 : 0,
        commissionPercentage: $('#txtCommissionAmount').val(),
        isFuelChargeProvided: $('#chkIsFuelProvided').is(':checked') ? 1 : 0,
        fuelPercentage: $('#txtFuelSurchargePercentage').val(),
        radioInsuranceAmount: $('#txtRadioInsuranceAmount').val(),
        insuranceAmount: $('#txtInsuranceAmount').val(),

        isActive: $('#chkIsActive').is(':checked') ? 1 : 0

    };

    return [employeeData];
}

