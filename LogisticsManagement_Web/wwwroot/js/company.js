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

    FillCompanyInfo();
});

$('#frmCompanyForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmCompanyForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        PerformPostActionWithObject('CompanyInfo/Update', dataArray);
        bootbox.alert('Data updated successfully.');
    }
    //else {
    //    PerformPostActionWithObject('CompanyInfo/Add', dataArray);
    //    bootbox.alert('Data saved successfully.');
    //}
    location.reload();
});

function GetFormData() {
    var companyData = {
        id: $('#txtCompanyId').val() === "" ? "0" : $('#txtCompanyId').val(),
        CompanyName: $('#txtCompanyName').val(),
        //CompanyLogo: $('#imgCompanyLogo').prop('src'),
        MainAddress: $('#txtAddress').val(),
        EmailAddress: $('#txtEmailAddress').val(),
        Telephone: $('#txtPhoneNumber').val(),
    };
    var logo = $('#imgCompanyLogo').prop('src');
    return [companyData, logo];
}

function GetImageFile(imgPath) {
    var fileInfo = null;

    if (imgPath.files && imgPath.files[0]) {

        var reader = new FileReader();
        reader.onload = function (e) {
            fileInfo = e.target.result;
            $('#imgCompanyLogo').attr('src', fileInfo);
        };

        reader.readAsDataURL(imgPath.files[0]);
    }
    return fileInfo;
}

function ResetDefaultProfilePicture() {
    $('#imgCompanyLogo').prop('src', '');
}

$('#fileCompanyLogo').on('change', function () {
    GetImageFile(this);
});

function FillCompanyInfo() {
    var companyId = 1;
    var companyInfo = GetSingleById('CompanyInfo/GetCompanyDataById', companyId);

    if (companyInfo !== "") {
        companyInfo = JSON.parse(companyInfo);
        $('#txtCompanyId').val(companyInfo.Id);
        $('#txtCompanyName').val(companyInfo.CompanyName);
        $('#txtAddress').val(companyInfo.MainAddress);
        $('#txtEmailAddress').val(companyInfo.EmailAddress);
        $('#txtPhoneNumber').val(companyInfo.Telephone);

        if (companyInfo.CompanyLogo !== null) {
            $('#imgCompanyLogo').prop('src', 'data:image/png;base64,' + companyInfo.CompanyLogo);
        }
        else {
            ResetDefaultProfilePicture();
        }
    }
}