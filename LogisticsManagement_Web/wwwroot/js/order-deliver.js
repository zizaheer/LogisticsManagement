
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

    $('#txtDeliveryDateTime').val(ConvertDatetimeToUSDatetime(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });


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

$('#btnClear').on('click', function () {
    $('#txtWayBillNumber').removeAttr('readonly');
});

$('#txtWayBillNumber').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var wayBillNumber = $('#txtWayBillNumber').val();
        var orderStatus = GetSingleObjectById('OrderDeliver/GetOrderByWayBillId', wayBillNumber);
        if (orderStatus !== "") {
            orderStatus = JSON.parse(orderStatus);
        }
        else {
            bootbox.alert('The order number was not found. Please check and try again.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderDispatched === null) {
            bootbox.alert('The order has not yet dispatched.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderPickedup === null) {
            bootbox.alert('The order has not yet been picked up.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderDelivered !== null) {
            bootbox.alert('The order has already been delivered. Please check in the delivered list below.');
            //event.preventDefault();
            //return;
        }

        $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
        $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
        $('#txtPickupDateTime').val(orderStatus.PickupDatetime);

        $('#txtWaitTime').val(orderStatus.DeliveryWaitTimeInHour);
        if (orderStatus.DeliverDatetime !== null)
        {
            $('#txtDeliveryDateTime').val(orderStatus.DeliverDatetime);
        }
        $('#txtReceivedByName').val(orderStatus.ReceivedByName);
        $('#txtDeliveryNote').val(orderStatus.ProofOfDeliveryNote);
        DrawSignatureImage(orderStatus.ReceivedBySignature);
    }
});

$('#frmOrderDeliverForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderDeliverForm').unbind('submit').submit(function (event) {
    var waitTime = $('#txtWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();
    var orderTypeId = $('#ddlOrderTypeId').val();

    var wayBillNumber = $('#txtWayBillNumber').val();

    if (deliveryDate <= pickupDate) {
        bootbox.alert('Delivery date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, deliveryDate, deliveryNote, receivedByName, receivedBySign, orderTypeId];

    UpdateEntry('OrderDeliver/Update', dataArray);

    event.preventDefault();
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');
});

$('#deliver-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');
    var orderStatus = GetSingleObjectById('OrderDeliver/GetOrderByWayBillId', wbNumber);
    if (orderStatus !== "") {
        orderStatus = JSON.parse(orderStatus);
    }
    $('#txtWayBillNumber').val(wbNumber);
    $('#txtWayBillNumber').attr('readonly', true);

    $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
    $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
    $('#txtPickupDateTime').val(orderStatus.PickupDatetime);

    $('#txtWaitTime').val(orderStatus.DeliveryWaitTimeInHour);
    $('#txtDeliveryDateTime').val(orderStatus.DeliverDatetime);
    $('#txtReceivedByName').val(orderStatus.ReceivedByName);
    $('#txtDeliveryNote').val(orderStatus.ProofOfDeliveryNote);
    //$('#imgSignature').val(orderStatus.ReceivedBySignature);
    DrawSignatureImage(orderStatus.ReceivedBySignature);
});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderDeliver/Remove', waybillNumber);
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');
});

$('#btnDownloadDataDeliveredData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');

});

function DrawSignatureImage(base64String) {
    var canvas = document.getElementById('signatureCanvas');
    var canvasContext = canvas.getContext('2d');
    var image = new Image();
    image.onload = function () {
        canvasContext.drawImage(image, 0, 0);
    };
    image.src = "data:image/png;base64," + base64String;

}
