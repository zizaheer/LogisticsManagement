
$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});


$('#btnAddNewAddService').on('click', function () {
    $('#txtServiceId').prop('readonly', true);
    $('#txtServiceId').val('');
    $('#frmAddServiceForm').trigger('reset');

    $('#additionalService').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#additionalService').draggable();
    $('#additionalService').modal('show');
});

$('#chkUnitPriceApplicable').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#txtUnitPrice').prop('disabled', false);
    } else {
        $('#txtUnitPrice').prop('disabled', true);
    }
});

$('#additional-service-list').on('click', '.btnEdit', function () {
    $('#txtServiceId').prop('readonly', true);
    var serviceInfo = null;
    var serviceid = $(this).data('serviceid');
    var serviceInfoRaw = GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceid);

    if (serviceInfoRaw !== "") {
        serviceInfo = JSON.parse(serviceInfoRaw);
    }
    else {
        bootbox.alert('The service was not found.');
        event.preventDefault();
        return;
    }

    FillServiceInfo(serviceInfo);

    $('#additionalService').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#additionalService').draggable();
    $('#additionalService').modal('show');
});

$('#btnCloseModal').on('click', function () {
    $('#additionalService').modal('hide');
});


$('#frmAddServiceForm').unbind('submit').submit(function (event) {
    event.preventDefault();

    var data = GetFormData();

    if (data.serviceName ==='') {
        bootbox.alert('Please enter service name');
        return;
    }

    if (data.id > 0) {
        PerformPostActionWithObject('AdditionalService/Update', data);
    }
    else {
        PerformPostActionWithObject('AdditionalService/Add', data);
    }

    location.reload();
});

$('.btnDelete').unbind().on('click', function () {
    serviceId = $(this).data('serviceid');
    bootbox.confirm("This user will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('AdditionalService/Remove', serviceId);
            location.reload();
        }
    });
});

function GetFormData() {
    var data = {
        id: $('#txtServiceId').val() === '' ? 0 : $('#txtServiceId').val(),
        serviceCode: $('#txtShortCode').val(),
        serviceName: $('#txtServiceName').val(),
        IsPriceApplicable: $('#chkUnitPriceApplicable').is(':checked'),
        UnitPrice: $('#txtUnitPrice').val() == "" ? 0 : parseFloat($('#txtUnitPrice').val()),

        isTaxApplicable: $('#chkIsTaxApplicable').is(':checked') === true ? 1 : 0,
        payToDriver: $('#chkPayToDriver').is(':checked') === true ? 1 : 0,
        isApplicableForStorage: $('#chkIsApplicableForStorage').is(':checked') === true ? 1 : 0,
        isActive: $('#chkIsActive').is(':checked') === true ? 1 : 0
    };

    return data;
}

function FillServiceInfo(serviceInfo) {
    $('#txtServiceId').val(serviceInfo.Id);
    $('#txtShortCode').val(serviceInfo.ServiceCode);
    $('#txtServiceName').val(serviceInfo.ServiceName);
    $('#txtUnitPrice').val(serviceInfo.UnitPrice);

    if (serviceInfo.IsPriceApplicable === true) {
        $('#chkUnitPriceApplicable').prop('checked', true);
        $('#txtUnitPrice').prop("disabled", false);
    } else {
        $('#chkUnitPriceApplicable').prop('checked', false);
        $('#txtUnitPrice').prop("disabled", true);
    }

    if (serviceInfo.IsTaxApplicable === true) {
        $('#chkIsTaxApplicable').prop('checked', true);
    } else {
        $('#chkIsTaxApplicable').prop('checked', false);
    }

    if (serviceInfo.PayToDriver === true) {
        $('#chkPayToDriver').prop('checked', true);
    } else {
        $('#chkPayToDriver').prop('checked', false);
    }

    if (serviceInfo.IsApplicableForStorage === true) {
        $('#chkIsApplicableForStorage').prop('checked', true);
    } else {
        $('#chkIsApplicableForStorage').prop('checked', false);
    }

    if (serviceInfo.IsActive === true) {
        $('#chkIsActive').prop('checked', true);
    } else {
        $('#chkIsActive').prop('checked', false);
    }
}

