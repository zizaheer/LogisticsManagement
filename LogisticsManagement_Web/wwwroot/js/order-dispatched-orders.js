//#region document.ready

$(document).ready(function () {

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

var passOnEmployeeId = 0;
var orderId = 0;
var wayBillNumber = 0;
$('.btnPickup').unbind().on('click', function () {

    //console.log('teste');
    ClearModal();

    orderId = $("input[name='rdoWaybillNo']:checked").data('orderid');
    wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    $('#txtWayBillNoForPickupModal').val(wayBillNumber);

    if (orderId < 1 || orderId === undefined) {
        bootbox.alert('Please select an order.');
        return;
    }

    $('#hfOrderIdForOrderStatusUpdate').val(orderId);

    var orderStatusInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForPickupModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleObjectById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
        if (empInfo.LastName == null) {
            empInfo.LastName = '';
        }
        $('#txtDispatchEmployeeNameForPickupModal').val(empInfo.FirstName + ' ' + empInfo.LastName);

        if (orderStatusInfo.PickupDatetime !== null) {
            $('#txtPickupDateTime').val(orderStatusInfo.PickupDatetime);
        }
        else {
            $('#txtPickupDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }

        $('#txtPickupWaitTime').val(orderStatusInfo.PickupWaitTimeHour);

        $('#orderPickup').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#orderPickup').modal('show');
    }
    else {
        bootbox.alert('Order information was not found.');
        return;
    }
});
$('#btnSavePickup').unbind().on('click', function () {
    var waitTime = $('#txtPickupWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var dispatchedDate = $('#txtDispatchDateTimeForPickupModal').val();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

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

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    bootbox.confirm("Pickup information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemovePickupStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
            $('#orderPickup').modal('hide');
        }
    });
});

$('.btnPasson').unbind().on('click', function () {

    ClearModal();

     orderId = $("input[name='rdoWaybillNo']:checked").data('orderid');
     wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    $('#txtWayBillNoForPassOnModal').val(wayBillNumber);

    if (orderId < 1 || orderId === undefined) {
        bootbox.alert('Please select an order.');
        return;
    }

    $('#hfOrderIdForOrderStatusUpdate').val(orderId);

    var orderStatusInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderStatusInfo.PickupDatetime === null) {
        bootbox.alert('The order is not picked-up yet.');
        return;
    }
    

    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForPassOnModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleObjectById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
        if (empInfo.LastName == null) {
            empInfo.LastName = '';
        }
        $('#txtDispatchEmployeeNameForPassOnModal').val(empInfo.FirstName + ' ' + empInfo.LastName);

        if (orderStatusInfo.PassOffDatetime !== null) {
            $('#txtPassOnDateTime').val(orderStatusInfo.PassOffDatetime);
        }
        else {
            $('#txtPassOnDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }
        $('#txtPickupDateTimeForPassOnModal').val(orderStatusInfo.PickupDatetime);

        if (orderStatusInfo.PassedOffToEmployeeId > 0) {

            var passOnEmp = JSON.parse(GetSingleObjectById('Employee/GetEmployeeById', orderStatusInfo.PassedOffToEmployeeId));
            if (passOnEmp.LastName == null) {
                passOnEmp.LastName = '';
            }
            $('#txtPassOnEmployeeName').val(passOnEmp.FirstName + ' ' + passOnEmp.LastName);
            $('#txtPassOnEmployeeName').attr('data-employeeid', passOnEmployeeId);

        } else {
            $('#txtPassOnEmployeeName').val('');
        }

        $('#txtPassOnWaitTime').val(orderStatusInfo.PassOffWaitTimeHour);

        $('#orderPassOn').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#orderPassOn').modal('show');
    }
    else {
        bootbox.alert('Order information was not found.');
        return;
    }
});

$('#txtPassOnEmployeeName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $('#txtPassOnEmployeeName').val();
    passOnEmployeeId = $('#dlPassOnEmployees option').filter(function () {
        return this.value === valueSelected;
    }).data('employeeid');

    $('#txtPassOnEmployeeName').attr('data-employeeid', passOnEmployeeId);

});



$('#btnSavePassOn').unbind().on('click', function () {
    var waitTime = $('#txtPassOnWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForPassOnModal').val();
    var passOnDate = $('#txtPassOnDateTime').val();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();
    passOnEmployeeId = $('#txtPassOnEmployeeName').data('employeeid');

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

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    bootbox.confirm("Pass-on information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemovePassonStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
            $('#orderPassOn').modal('hide');
        }
    });

});

$('.btnDeliver').unbind().on('click', function () {

    ClearModal();

     orderId = $("input[name='rdoWaybillNo']:checked").data('orderid');
     wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    $('#txtWayBillNoForDeliverModal').val(wayBillNumber);

    if (orderId < 1 || orderId === undefined) {
        bootbox.alert('Please select an order.');
        return;
    }

    $('#hfOrderIdForOrderStatusUpdate').val(orderId);

    var orderStatusInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));
    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderInfoByOrderId', orderId));
    var shipperInfo = JSON.parse(GetSingleObjectById('Customer/GetCustomerById', orderInfo.ShipperCustomerId));
    var consigneeInfo = JSON.parse(GetSingleObjectById('Customer/GetCustomerById', orderInfo.ConsigneeCustomerId));

    var shipperAddInfo = JSON.parse(GetSingleObjectById('Address/GetAddressById', orderInfo.ShipperAddressId));
    var consigneeAddInfo = JSON.parse(GetSingleObjectById('Address/GetAddressById', orderInfo.ConsigneeAddressId));


    $('#txtShipperInfo').val(shipperInfo.CustomerName + '\n' + shipperAddInfo.AddressLine );
    $('#txtConsigneeInfo').val(consigneeInfo.CustomerName + '\n' + consigneeAddInfo.AddressLine );

    if (orderStatusInfo.PickupDatetime === null) {
        bootbox.alert('The order is not picked-up yet.');
        return;
    }

    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForDeliverModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleObjectById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
        if (empInfo.LastName == null) {
            empInfo.LastName = '';
        }
        $('#txtDispatchEmployeeNameForDeliverModal').val(empInfo.FirstName + ' ' + empInfo.LastName);

        if (orderStatusInfo.PassedOffToEmployeeId > 0) {
            var passOnEmp = JSON.parse(GetSingleObjectById('Employee/GetEmployeeById', orderStatusInfo.PassedOffToEmployeeId));
            if (passOnEmp.LastName == null) {
                passOnEmp.LastName = '';
            }
            $('#txtPassOnEmployeeNameForDeliverModal').val(passOnEmp.FirstName + ' ' + passOnEmp.LastName);
            $('#txtDispatchEmployeeNameForDeliverModal').val(passOnEmp.FirstName + ' ' + passOnEmp.LastName);

        } else {
            $('#txtPassOnEmployeeNameForDeliverModal').val('');
        }

        $('#txtPickupDateTimeForDeliverModal').val(orderStatusInfo.PickupDatetime);
        if (orderStatusInfo.DeliveredDatetime !== null) {
            $('#txtDeliveryDateTime').val(orderStatusInfo.DeliveredDatetime);
        }
        else {
            $('#txtDeliveryDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }

        $('#txtDeliveryWaitTime').val(orderStatusInfo.DeliveryWaitTimeHour);
        $('#txtReceivedByName').val(orderStatusInfo.ReceivedByName);
        $('#txtDeliveryNote').val(orderStatusInfo.ProofOfDeliveryNote);
        DrawSignatureImage(orderStatusInfo.ReceivedBySignature);

        $('#orderDeliver').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#orderDeliver').modal('show');
    }
    else {
        bootbox.alert('Order information was not found.');
        return;
    }
});
$('#btnSaveDeliver').unbind().on('click', function () {
    var waitTime = $('#txtDeliveryWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForDeliverModal').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

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
$('#btnRemoveDeliver').unbind().on('click', function () {

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    bootbox.confirm("Delivery information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemoveDeliveryStatus', orderId);
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');

            $('#orderDeliver').modal('hide');
        }
    });

    
});

$('.btnRemoveDispatch').unbind().on('click', function () {

    wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    if (wayBillNumber == null || wayBillNumber < 1) {
        bootbox.alert('Please select an order to remove.');
        return;
    }

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





