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

$('#employee-list').on('click', '.btnEdit', function () {
    var data = $(this).data('employee');

    $('#txtCustomerId').val(data.id);
    $('#txtCustomerName').val(data.customerName);
    $('#txtSpecialDiscount').val(data.discountPercentage);
    $('#txtInvoiceDueDays').val(data.invoiceDueDays);
});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');
   
});

$('#frmEmployeeForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    AddEntry('Employee/Add', dataArray);
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

