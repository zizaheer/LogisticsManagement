//#region document.ready

$(document).ready(function () {

    $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(new Date));
    $('#txtDispatchDatetimeForNewOrders').val(ConvertDatetimeToUSDatetime(new Date));

    $(function () {
        var canvas = document.querySelector('#signatureCanvas');
        var pad = new SignaturePad(canvas);

        $('#btnOk').click(function () {
            var data = pad.toDataURL();
            $('#imgSignature').val(data);
            pad.off();
        });

        $('#btnCancel').click(function () {
            pad.clear();
            pad.on();
            $('#imgSignature').val('');
        });
    });

});

//#endregion 

//#region Events 

$('.btnPickup').unbind().on('click', function () {

    //console.log('teste');
    ClearModal();

    var orderId = $(this).data('orderid');
    $('#txtOrderIdForPickupModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForPickupModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForPickupModal').val(orderInfo.DispatchedDatetime);

        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForPickupModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        if (orderInfo.PickupDatetime !== null) {
            $('#txtPickupDateTime').val(orderInfo.PickupDatetime);
        }
        else {
            $('#txtPickupDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }

        $('#txtPickupWaitTime').val(orderInfo.PickupWaitTimeHour);
    }
    ////$('#orderPickup').toggle('modal');
    //$('#orderPickup').modal('show');
    ////$('#orderPickup').show();

});
$('#btnSavePickup').unbind().on('click', function () {
    var waitTime = $('#txtPickupWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var dispatchedDate = $('#txtDispatchDateTimeForPickupModal').val();
    var wayBillNumber = $('#txtWayBillNoForPickupModal').val();
    var orderId = $('#txtOrderIdForPickupModal').val();

    if (pickupDate <= dispatchedDate) {
        bootbox.alert('Pickup date must be greater than dispatch date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, pickupDate, orderId];

    UpdateEntry('Order/UpdatePickupStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemovePickup').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForPickupModal').val();

    bootbox.confirm("Pickup information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemovePickupStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        }
    });
});

$('.btnPasson').unbind().on('click', function () {

    ClearModal();

    var orderId = $(this).data('orderid');
    $('#txtOrderIdForPassOnModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForPassOnModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForPassOnModal').val(orderInfo.DispatchedDatetime);

        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForPassOnModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        if (orderInfo.PassOffDatetime !== null) {
            $('#txtPassOnDateTime').val(orderInfo.PassOffDatetime);
        }
        else {
            $('#txtPassOnDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }
        $('#txtPickupDateTimeForPassOnModal').val(orderInfo.PickupDatetime);
        if (orderInfo.PassedOffToEmployeeId > 0) {
            $('#ddlPassOnEmployeeId').val(orderInfo.PassedOffToEmployeeId);
        } else {
            $('#ddlPassOnEmployeeId').val(0);
        }

        $('#txtPassOnEmployeeNumber').val($('#ddlPassOnEmployeeId').val());
        $('#txtPassOnWaitTime').val(orderInfo.PassOffWaitTimeHour);
    }
});
$('#txtPassOnEmployeeNumber').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        $('#ddlPassOnEmployeeId').val($('#txtPassOnEmployeeNumber').val());
    }

});
$('#btnSavePassOn').unbind().on('click', function () {
    var waitTime = $('#txtPassOnWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForPassOnModal').val();
    var passOnDate = $('#txtPassOnDateTime').val();
    var passOnEmployeeId = $('#ddlPassOnEmployeeId').val();
    var orderId = $('#txtOrderIdForPassOnModal').val();
    var wayBillNumber = $('#txtWayBillNoForPassOnModal').val();

    if (passOnDate <= pickupDate) {
        bootbox.alert('Pass-on date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, passOnEmployeeId, passOnDate, orderId];

    UpdateEntry('Order/UpdatePassonStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemovePassOn').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForPassOnModal').val();
    bootbox.confirm("Pass-on information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemovePassonStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        }
    });

});

$('.btnDeliver').unbind().on('click', function () {

    ClearModal();

    var orderId = $(this).data('orderid');
    $('#txtOrderIdForDeliverModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForDeliverModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForDeliverModal').val(orderInfo.DispatchedDatetime);


        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForDeliverModal').val($("#ddlEmployeeId option:selected").text());
        //$('#ddlEmployeeId').val(0); // reset <select>

        $('#ddlEmployeeId').val(orderInfo.PassedOffToEmployeeId);
        $('#txtPassOnEmployeeNameForDeliverModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        $('#txtPickupDateTimeForDeliverModal').val(orderInfo.PickupDatetime);
        if (orderInfo.DeliveredDatetime !== null) {
            $('#txtDeliveryDateTime').val(orderInfo.DeliveredDatetime);
        }
        else {
            $('#txtDeliveryDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }

        $('#txtDeliveryWaitTime').val(orderInfo.DeliveryWaitTimeHour);
        $('#txtReceivedByName').val(orderInfo.ReceivedByName);
        $('#txtDeliveryNote').val(orderInfo.ProofOfDeliveryNote);
        DrawSignatureImage(orderInfo.ReceivedBySignature);

    }
});
$('#btnSaveDeliver').unbind().on('click', function () {
    var waitTime = $('#txtDeliveryWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForDeliverModal').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();
    var orderId = $('#txtOrderIdForDeliverModal').val();

    var wayBillNumber = $('#txtWayBillNumber').val();

    if (deliveryDate <= pickupDate) {
        bootbox.alert('Delivery date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, deliveryDate, deliveryNote, receivedByName, receivedBySign, orderId];

    UpdateEntry('Order/UpdateDeliveryStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemoveDelivery').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForDeliverModal').val();

    bootbox.confirm("Pass-on information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemoveDeliveryStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        }
    });

    
});

$('.btnRemoveDispatch').unbind().on('click', function () {
    var wayBillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This will also remove the pickup and delivery status. Are you sure you want to remove?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemoveDispatchStatus', wayBillNumber);
            $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        }
    });
});

//#endregion


//#region Private methods

function ClearModal() {
    $('#txtWayBillNoForPickupModal').val('');
    $('#txtOrderIdForPickupModal').val('');
    $('#txtDispatchDateTimeForPickupModal').val('');
    $('#txtDispatchEmployeeNameForPickupModal').val('');
    $('#txtPickupDateTime').val('');
    $('#txtPickupWaitTime').val('');
    $('#txtWayBillNoForPassOnModal').val('');
    $('#txtOrderIdForPassOnModal').val('');
    $('#txtDispatchDateTimeForPassOnModal').val('');
    $('#txtPickupDateTimeForPassOnModal').val('');
    $('#txtDispatchEmployeeNameForPassOnModal').val('');
    $('#txtPassOnDateTime').val('');
    $('#ddlPassOnEmployeeId').val(0);
    $('#txtPassOnEmployeeNumber').val('');
    $('#txtPassOnWaitTime').val('');
    $('#txtWayBillNoForDeliverModal').val('');
    $('#txtOrderIdForDeliverModal').val('');
    $('#txtPickupDateTimeForDeliverModal').val('');
    $('#txtDispatchDateTimeForDeliverModal').val('');
    $('#txtDispatchEmployeeNameForDeliverModal').val('');
    $('#txtPassOnEmployeeNameForDeliverModal').val('');
    $('#txtDeliveryDateTime').val('');
    $('#txtDeliveryWaitTime').val('');
    $('#txtReceivedByName').val('');
    $('#txtDeliveryNote').val('');

    var canvas = document.querySelector('#signatureCanvas');
    var pad = new SignaturePad(canvas);
}

function DrawSignatureImage(base64String) {
    var canvas = document.getElementById('signatureCanvas');
    var canvasContext = canvas.getContext('2d');
    var image = new Image();
    image.onload = function () {
        canvasContext.drawImage(image, 0, 0);
    };
    image.src = "data:image/png;base64," + base64String;

}

//#endregion 





