//#region document.ready

$(document).ready(function () {

    $('#txtDispatchDatetimeForNewOrders').val(ConvertDatetimeToUSDatetime(new Date));
    

    //$(function () {
    //    var canvas = document.querySelector('#signatureCanvas');
    //    var pad = new SignaturePad(canvas);

    //    $('#btnOk').click(function () {
    //        var data = pad.toDataURL();
    //        $('#imgSignature').val(data);
    //        pad.off();
    //    });

    //    $('#btnCancel').click(function () {
    //        pad.clear();
    //        pad.on();
    //        $('#imgSignature').val('');
    //    });
    //});

});

//#endregion 

//#region Events 

var passOnEmployeeId = 0;
var orderId = 0;
var wayBillNumber = 0;
var selectedOrdersForDispatch = [];

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

    var orderStatusInfo = JSON.parse(GetSingleById('Order/GetOrderStatusByOrderId', orderId));

    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForPickupModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
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
        $('#orderPickup').draggable();
        $('#orderPickup').modal('show');
    }
    else {
        bootbox.alert('Order information was not found.');
        return;
    }
});
$('#btnSavePickup').unbind().on('click', function (event) {
    event.preventDefault();

    var waitTime = $('#txtPickupWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var dispatchedDate = $('#txtDispatchDateTimeForPickupModal').val();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    if (pickupDate <= dispatchedDate) {
        bootbox.alert('Pickup date must be greater than dispatch date.');
        return;
    }

    var dataArray = [wayBillNumber, waitTime, pickupDate, orderId];

    var result = PerformPostActionWithObject('Order/UpdatePickupStatus', dataArray);
    if (result.length > 0) {
        $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        $('#orderPickup').modal('hide');
        location.reload();
    }

});
$('#btnRemovePickup').unbind().on('click', function (event) {
    event.preventDefault();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    bootbox.confirm("Pickup information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            var status = PerformPostActionWithId('Order/RemovePickupStatus', orderId);
            if (status.length > 0) {
                $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
                $('#orderPickup').modal('hide');
                location.reload();
            }
        }
    });
});

$('.btnPasson').unbind().on('click', function (event) {
    event.preventDefault();

    ClearModal();
    orderId = $("input[name='rdoWaybillNo']:checked").data('orderid');
    wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    $('#txtWayBillNoForPassOnModal').val(wayBillNumber);

    if (orderId < 1 || orderId === undefined) {
        bootbox.alert('Please select an order.');
        return;
    }

    $('#hfOrderIdForOrderStatusUpdate').val(orderId);

    var orderStatusInfo = JSON.parse(GetSingleById('Order/GetOrderStatusByOrderId', orderId));

    if (orderStatusInfo.PickupDatetime === null) {
        bootbox.alert('The order is not picked-up yet.');
        return;
    }


    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForPassOnModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
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

            var passOnEmp = JSON.parse(GetSingleById('Employee/GetEmployeeById', orderStatusInfo.PassedOffToEmployeeId));
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
        $('#orderPassOn').draggable();
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

$('#btnSavePassOn').unbind().on('click', function (event) {
    event.preventDefault();
    var waitTime = $('#txtPassOnWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForPassOnModal').val();
    var passOnDate = $('#txtPassOnDateTime').val();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();
    passOnEmployeeId = $('#txtPassOnEmployeeName').data('employeeid');

    if (passOnDate <= pickupDate) {
        bootbox.alert('Pass-on date must be greater than pickup date.');
        return;
    }

    if (passOnEmployeeId == undefined || passOnEmployeeId == '' || passOnEmployeeId < 1) {
        bootbox.alert('Please select a valid employee to pass-on the delivery.');
        return;
    }

    var dataArray = [wayBillNumber, waitTime, passOnEmployeeId, passOnDate, orderId];

    var result = PerformPostActionWithObject('Order/UpdatePassonStatus', dataArray);
    if (result.length > 0) {
        event.preventDefault();
        $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        $('#orderPassOn').modal('hide');
        location.reload();
    }

});
$('#btnRemovePassOn').unbind().on('click', function (event) {
    event.preventDefault();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();
    bootbox.confirm("Pass-on information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            var status = PerformPostActionWithId('Order/RemovePassonStatus', orderId);
            if (status.length > 0) {
                $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
                $('#orderPassOn').modal('hide');
                location.reload();
            }
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

    var orderStatusInfo = JSON.parse(GetSingleById('Order/GetOrderStatusByOrderId', orderId));
    var orderInfo = JSON.parse(GetSingleById('Order/GetOrderInfoByOrderId', orderId));
    var shipperInfo = JSON.parse(GetSingleById('Customer/GetCustomerById', orderInfo.ShipperCustomerId));
    var consigneeInfo = JSON.parse(GetSingleById('Customer/GetCustomerById', orderInfo.ConsigneeCustomerId));

    var shipperAddInfo = JSON.parse(GetSingleById('Address/GetAddressById', orderInfo.ShipperAddressId));
    var consigneeAddInfo = JSON.parse(GetSingleById('Address/GetAddressById', orderInfo.ConsigneeAddressId));

    console.log(orderInfo);

    //$('#lblShipperInfo').html(shipperInfo.CustomerName + '<br/>' + shipperAddInfo.AddressLine + ' &nbsp;&nbsp; // ' + orderInfo.UnitQuantity + ' ' + orderInfo.UnitTypeId + ', ' + orderInfo.SkidQuantity + ' SKD, ' + orderInfo.WeightTotal + ' ' + orderInfo.WeightScaleId);
    $('#lblShipperInfo').html(shipperInfo.CustomerName + '<br/>' + shipperAddInfo.AddressLine);
    $('#lblConsigneeInfo').html(consigneeInfo.CustomerName + '<br/>' + consigneeAddInfo.AddressLine);

    if (orderStatusInfo.PickupDatetime === null) {
        bootbox.alert('The order is not picked-up yet.');
        return;
    }

    if (orderStatusInfo !== null) {
        $('#txtDispatchDateTimeForDeliverModal').val(orderStatusInfo.DispatchedDatetime);

        var empInfo = JSON.parse(GetSingleById('Employee/GetEmployeeById', orderStatusInfo.DispatchedToEmployeeId));
        if (empInfo.LastName == null) {
            empInfo.LastName = '';
        }
        $('#txtDispatchEmployeeNameForDeliverModal').val(empInfo.FirstName + ' ' + empInfo.LastName);

        if (orderStatusInfo.PassedOffToEmployeeId > 0) {
            var passOnEmp = JSON.parse(GetSingleById('Employee/GetEmployeeById', orderStatusInfo.PassedOffToEmployeeId));
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
        //DrawSignatureImage(orderStatusInfo.ReceivedBySignature);

        $('#orderDeliver').modal({
            backdrop: 'static',
            keyboard: false
        });
        $('#orderDeliver').draggable({ handle:'#deliverHeader',cursor:'move'});
        $('#orderDeliver').modal('show');
    }
    else {
        bootbox.alert('Order information was not found.');
        return;
    }
});
$('#btnSaveDeliver').unbind().on('click', function (event) {
    event.preventDefault();

    var waitTime = $('#txtDeliveryWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForDeliverModal').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();
    var printWaybill = $('#chkPrintWaybill').is(':checked');

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();

    if (deliveryDate <= pickupDate) {
        bootbox.alert('Delivery date must be greater than pickup date.');
        return;
    }

    if (receivedByName === '') {
        bootbox.alert('Received by field cannot be empty.');
        return;
    }

    var dataArray = [wayBillNumber, waitTime, deliveryDate, deliveryNote, receivedByName, receivedBySign, orderId];

    var result = PerformPostActionWithObject('Order/UpdateDeliveryStatus', dataArray);

    if (result.length > 0) {
        if (printWaybill === true) {
            selectedOrdersForDispatch.push(parseInt(wayBillNumber));
            var printUrl = 'Order/PrintWaybillAsPdf';
            var printOption = {
                numberOfcopyOnEachPage: 2,
                numberOfcopyPerItem: 1,
                ignorePrice: 0,
                isMiscellaneous: 0,
                viewName: 'PrintDeliveryWaybill',
                printUrl: printUrl
            };

            var dataArray = [selectedOrdersForDispatch, printOption];

            $.ajax({
                'async': false,
                url: "Order/PrintWaybillAsPdf",
                type: 'POST',
                data: JSON.stringify(dataArray),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result.length > 0) {
                        window.open(result, "_blank");
                    }
                },
                error: function (result) {
                    bootbox.alert('Printing failed! There are some eror occurred while processing the printing.');
                }
            });

        }
        
        $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        $('#orderDeliver').modal('hide');
        location.reload();
    }

});
$('#btnRemoveDeliver').unbind().on('click', function (event) {
    event.preventDefault();

    orderId = $('#hfOrderIdForOrderStatusUpdate').val();
    bootbox.confirm("Delivery information related to this order will be deleted. Are you sure to proceed?", function (result) {
        if (result === true) {
            var status = PerformPostActionWithId('Order/RemoveDeliveryStatus', orderId);
            if (status.length > 0) {
                $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
                $('#orderDeliver').modal('hide');
                location.reload();
            }
        }
    });

});

$('.btnRemoveDispatch').unbind().on('click', function (event) {
    event.preventDefault();

    wayBillNumber = $("input[name='rdoWaybillNo']:checked").data('waybillnumber');

    if (wayBillNumber == null || wayBillNumber < 1) {
        bootbox.alert('Please select an order to remove.');
        return;
    }

    bootbox.confirm("This will also remove the pickup and delivery status. Are you sure you want to remove?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Order/RemoveDispatchStatus', wayBillNumber);
            $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
            location.reload();
        }
    });
});

$('.btnEdit').on('click', function (event) {
    event.preventDefault();

    ClearForm();
    $('#frmOrderForm').trigger('reset');

    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
            $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
        });
    }
    var wbNumber = $(this).data('waybillnumber');
    if (wbNumber > 0) {
        GetAndFillOrderDetailsByWayBillNumber(wbNumber, 1);
    }

    $('#newOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newOrder').draggable();
    $('#newOrder').modal('show');

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
    setTimeout(function () { $('input[name="txtReceivedByName"]').focus() }, 1000);

    //var canvas = document.querySelector('#signatureCanvas');
    //var pad = new SignaturePad(canvas);
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





