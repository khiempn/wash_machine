
var language = langCh;
//var eftError = eftLangEn;

window.eftLocked = false;
const base_error = 100000;
const wattingErrors = [100016, 100017, 100020, 100032, 100034, 100019, 100021, 100024, 100035, 100022, 100048, 100049];
const retryErrors = [100016, 100017, 100020, 100032, 100034, 100022];
const messageTag = $('#payment_status');
function showPaymentResult(data) {
    var obj = JSON.parse(data);
    if (obj.Value === undefined) {
        obj = JSON.parse(obj);
    }
    showTracking(paymentCouter + ' - ' + obj.Value);
    $('.dependent_payment').removeClass('payment-running');
    var closeTimeout = 10000;

    if (obj.Value < base_error) {
        createNewOrder(obj);
        return;
    }

    if (obj.Value === 100999) {
        autoClosePopup(closeTimeout);
        return;
    }

    if (obj.Value !== 100032 && !obj.CardIdRequired) {
        paymentCouter = 0;
    }

    var textMessage = language[obj.Value];
    if (!textMessage) textMessage = language[100099];
    var message = obj.Value + '-' + textMessage;

    var message2 = undefined;
    if (obj.CardIdRequired) {
        message2 = language[1000221].replace('{0}', obj.CardIdRequired);
    }
    showErrorPopup(message, message2);

    if (paymentCouter >= 7) {
        autoClosePopup(closeTimeout);
        return;
    }
    var timeout = 100;
    //timeout = 2000; // Need to remove
    if (obj.CardIdRequired && wattingErrors.indexOf(obj.Value) >= 0) {
        //$('.dependent_payment').addClass('payment-running');
        if (obj.Value === 100022) timeout = 2000;
        setTimeout(function () {
            callPaymentMethod();
        }, timeout);
    } else if (retryErrors.indexOf(obj.Value) >= 0) {
        //$('.dependent_payment').addClass('payment-running');
        setTimeout(callPaymentMethod, timeout);
    } else {
        autoClosePopup(closeTimeout);
    }
}
function autoClosePopup(timeout) {
    alertA('Started auto close: ' + timeout)
    var time = new Date().getTime();
    window.lastActionTime = time;

    timeout = 3000;

    if (!timeout) timeout = 10000;
    $('.dependent_payment').removeClass('payment-running');
    $('.dependent_payment').addClass('closing');
    setTimeout(function () {
        var closeTime = new Date().getTime();
        var timeSpace = closeTime - window.lastActionTime;
        if (timeSpace < timeout) {
            alertA('Can not close.');
            return;
        }
        alertA('CLOSED');
        if ($('.dependent_payment').hasClass('closing')) {
            $('.dependent_payment').removeClass('closing');
        }
        backMain();
    }, timeout);
}
function extendTimePayment() {
    $('.dependent_payment').removeClass('questioning');
    paymentCouter = 0;
    setTimeout(function () {
        callPaymentMethod();
    }, 100);
    //showPaymentResult(window.octopusCached);
}

function createNewOrder(obj) {
    showQRMessage(language.PaymentSuccess);
    var postUrl = $('[data-url]').attr('data-url').replace('OctopusPayment?', 'CreateOctopusOrder?').replace('ScanQR', 'CreateOctopusOrder');
    postUrl = postUrl.replace('PaymePayment', 'CreateEftOrder');
    postUrl = postUrl.replace('AlipayPayment', 'CreateEftOrder');

    var paymentValue = $('[data-payment]').attr('data-payment');
    var paymentInt = parseInt(paymentValue);

    var paymentType = $('[data-payment-type]').attr('data-payment-type');
    postUrl += '&PaymentTypeText=' + paymentType + '&RemainValue=' + obj.Value + '&PaymentId=' + obj.PaymentId;
    postUrl += '&TicketCode=' + obj.RefNo;
    var postData = {
        Received: JSON.stringify(obj)
    };

    //alertA('Start call ajax: ' + postUrl);
    $.ajax({
        url: postUrl,
        type: 'POST',
        data: postData,
        success: function (data) {
            alertA('Create order success');
            //showQRMessage(language.CreateOrderSuccess, true);

            $('.print').remove();
            $(document.body).addClass('preview-print');
            $('body').prepend(data);
            printInvoice();
            dropCoins(paymentInt,"ANY");
        },
        error: function (request, msg, error) {
            //showWarningMessage(msg);
            window.eftLocked = false;
            alertA('Ajax error: ' + msg);
        }
    });
}

function printInvoice() {
    var isPrint = $(document.body).hasClass('preview-print');
    if (isPrint) {
        if (detectWebview()) {
            //window.external.PrintInvoice();
        } else {
            //window.print();
        }
        setTimeout(function () {
            location.reload();
            //$('.print').remove();
            //$(document.body).removeClass('preview-print');
        }, 10000);
    }
}
function testInvoiceConfirm() {
    var theUrl = '/Pos/GetOrderInvoice?id=633';
    $.ajax({
        url: theUrl,
        type: 'GET',
        cache: false,
        success: function (data) {
            //showQRMessage(language.CreateOrderSuccess, true);
            $('.print').remove();
            $(document.body).addClass('preview-print');
            $('body').prepend(data);

            printInvoice();
        },
        error: function (request, msg, error) {
            showWarningMessage(msg);
            // handle failure
        }
    });
}

// This function only use for testing
function assumeError() {
    var data = '{"DeductedValue":0,"Value":100019,"PaymentId":"37","CardType":null,"CardId":null,"DeviceId":null,"DeductionTime":null,"LastAddType":null,"LastAddDate":null,"CardIdRequired":"123456789"}';
    showPaymentResult(data);
}

function showConfirm() {

}
function assumePaymentSuccess(barcode) {
    var paymentType = $('[data-payment-type]').attr('data-payment-type');
    if (paymentType === 'Octopus') {
        var obj = {
            Value: 100, PaymentId: 100
        };
        createNewOrder(obj);
    }

    if (paymentType === 'Payme') {
        var data = '{"DeductedValue":0,"Value":100019,"PaymentId":"37","CardType":null,"CardId":null,"DeviceId":null,"DeductionTime":null,"LastAddType":null,"LastAddDate":null,"CardIdRequired":"123456789"}';
        barcodeFormResult(data);
    };

    if (paymentType === "AliPay" || paymentType === "WeChatPay") {
        if (paymentType === "AliPay") barcode = '123112121';
        if (paymentType === "WeChatPay") barcode = '13000000000000000011';
        processBarcode(barcode);
    }
}
function assumeCatchBarcode(barcode) {
    processBarcode(barcode);
}

function createBarcodeOrder(barcode) {
    var indexTime = barcode.indexOf('-Time:');
    var paymentType = $('[data-payment-type]').attr('data-payment-type');
    if (paymentType === 'TicketRedeem') {
        if (barcode.length < 12 && indexTime < 0) {
            showQrWarning('The code is invalid');
            return;
        }
    }
    var paymentValue = $('[data-payment]').attr('data-payment');
    var paymentInt = parseInt(paymentValue);
    if (paymentInt === 0) {
        showQrWarning(language.OrderZeroPrice);
        return;
    }

    var postUrl = $('[data-url]').attr('data-url').replace('PopupQR?', 'CreateNewOrder?').replace('ScanQR', 'CreateNewOrder');
    postUrl += '&TicketCode=' + barcode + '&Money=' + paymentValue;
    $.ajax({
        url: postUrl,
        method: 'POST',
        contentType: 'application/json',
        success: function (data) {
            if (data.Success === false) {
                showQrWarning(data.Message, true);
                return;
            }
            //showQRMessage(language.CreateOrderSuccess, true);
            $('.print').remove();
            $(document.body).addClass('preview-print');
            $('body').append(data);
            printInvoice();
        },
        error: function (request, msg, error) {
            showWarningMessage(msg);
            // handle failure
        }
    });
}


var paymentCouter = 0;
function preCallPaymentMethod() {
    paymentCouter = 0;
    var paymentType = $('[data-payment-type]').attr('data-payment-type');
    if (paymentType === 'Octopus') {
        var isWebview = detectWebview();
        if (isWebview) {
            callPaymentMethod();
        } else {
            showFormResult("{\"Value\": 0, \"Message\":\"ERROR\"}");
        }
    }
}
function showErrorPopup(message, message2) {
    if (message2 !== undefined) {
        message += '<br />----------<br />' + message2;
    }
    var messageEl = $('#qr_scan_message');
    $(messageEl).find('span').html(message);
    $(messageEl).removeClass('hidden');
}
function callPaymentMethod() {
    if (detectWebview()) {
        $('.dependent_payment').addClass('payment-running');
        var postUrl = '/Pos/GetNewOtopusPayment'
        $.ajax({
            url: postUrl,
            method: 'GET',
            cache: false,
            contentType: 'application/json',
            success: function (data) {
                var paymentValue = $('[data-payment]').attr('data-payment');
                if (paymentValue) {
                    paymentCouter++;
                    var paymentId = data.Id;
                    setTimeout(function () {
                        window.external.ReceiveOctopusCard(paymentValue, paymentId);
                    }, 100);

                }
            },
            error: function (request, msg, error) {
                showWarningMessage(msg);
            }
        });
    }
}

function detectWebview() {
    var data = $('body').attr('data-app');
    if (data) {
        return true;
    }
    return false;
}

function showQRMessage(message, forever) {
    var messageEl = $('#qr_scan_message');
    messageEl.remove('qr-waning');

    $(messageEl).attr('data-time', new Date().valueOf());
    if (forever !== undefined) {
        $(messageEl).attr('data-time', new Date().valueOf() + 1000000);
    }
    $(messageEl).find('span').html(message);
    $(messageEl).removeClass('hidden');
    setTimeout(function () {
        //$(messageEl).attr('data-time');
        var lastTime = parseInt($(messageEl).attr('data-time'));
        var currentTime = new Date().valueOf();
        if (currentTime - lastTime >= 3000) {
            $(messageEl).addClass('hidden');
        }
    }, 3000);
}
function showQrWarning(message, forever) {
    if (!message) return;
    var messageEl = $('#qr_scan_message');
    //messageEl.addClass('qr-waning');

    $(messageEl).attr('data-time', new Date().valueOf());
    if (forever !== undefined) {
        $(messageEl).attr('data-time', new Date().valueOf() + 1000000);
    }
    $(messageEl).find('span').html(message);
    $(messageEl).removeClass('hidden');
    setTimeout(function () {
        //$(messageEl).attr('data-time');
        var lastTime = parseInt($(messageEl).attr('data-time'));
        var currentTime = new Date().valueOf();
        if (currentTime - lastTime >= 3000) {
            $(messageEl).addClass('hidden');
        }
    }, 3000);
}

function confirmQuestion(isOk) {
    window.answerOk = isOk;
    $('.boomer').removeClass('showing');
    if (!isOk) {
        location.reload();
        return;
    }
    var isPrint = $(document.body).hasClass('preview-print');
    if (isPrint) {
        $(document.body).removeClass('preview-print');
        if (detectWebview()) {
            window.external.PrintInvoice();
        } else {
            window.print();
        }
        setTimeout(function () {
            location.reload();
        }, 1000);
        return;
    }

    $('.boomer').hide();
    extendTimePayment();
}
function showQuestion(message) {
    $('.boomer').show();
    $('.boomer').addClass('showing');
    $('.boomer .question-top').text(message);
    setTimeout(function () {
        var isShowing = $('.boomer').hasClass('showing');
        if (isShowing) confirmQuestion(false);
    }, 6000);
}

function showPaymentQuestion() {
    $('.dependent_payment').addClass('questioning');
    $('.payments_continue .question-top').text(language.QuestionContinuePayment);
    setTimeout(function () {
        if ($('.dependent_payment').hasClass('questioning')) {
            location.reload();
        }
    }, 15000);
}


//==============================EFT==============================//

var langEft = langEftEn;

function processBarcode(barcode) {
    window.lastActionTime = undefined;
    if (window.eftLocked) return;
    window.eftLocked = true;

    const messageTag = $('#payment_status');
    messageTag.text("Checking order ...");
    var paymentType = $('[data-payment-type]').attr('data-payment-type');
    var amount = $('[data-payment]').attr('data-payment');
    if (paymentType === "Payme" || paymentType === "Alipay") {
        var paymentTypeInt = 1; // Alipay
        if (paymentType === "Payme") {
            paymentTypeInt = 6; // Payme
        }
        var postUrl = '/pos/GetPendingOrder?PaymentType=' + paymentTypeInt + '&Amount=' + amount;
        $.ajax({
            url: postUrl,
            type: 'POST',
            data: {},
            success: function (data) {
                messageTag.text("Payment is processing ...");
                setTimeout(function () {
                    if (detectWebview()) {
                        window.external.RecieveBarcode(data.Result.Data.ShopCode, paymentType, barcode, amount, data.Result.Data.TicketCode);
                    } else {
                        const messageTag = $('#payment_status');
                        messageTag.text("ERROR ACTION");
                        window.eftLocked = false;
                    }

                }, 100);

            },
            error: function (request, msg, error) {
                showWarningMessage(msg);
                window.eftLocked = false;
                // handle failure
            }
        });

    }
}

function barcodeFormResult(data) {

    const messageTag = $('#payment_status');
    messageTag.text('');

    window.lastActionTime = undefined;
    var obj = JSON.parse(data);
    if (obj.Value === undefined) {
        obj = JSON.parse(obj);
    }

    if (obj.Type === 'Payme' || obj.Type === 'Alipay') {
        if (obj.Value === 0 && obj.ResponeCode === "00") {
            //alertA('createNewOrder');
            createNewOrder(obj);
            return;
        }
    }

    var responseCode = obj.Value;
    if (responseCode === 0) {
        responseCode = parseInt(obj.ResponeCode);
    }

    var textMessage = langEft[responseCode+""];
    if (!textMessage) textMessage = obj.Message;
    var message = responseCode + ':' + textMessage;

    messageTag.text(message);
    window.eftLocked = false;
}


$(document).ready(function () {
    setInterval(checkAutoClosePopup, 1000);
});

function checkAutoClosePopup() {
    var time = new Date().getTime();
    if (!window.lastActionTime) window.lastActionTime = time;
    var timeSpace = time - window.lastActionTime;
    if (timeSpace >= 15000) {
        var autoCloseElement = $('.autoclose');
        if (autoCloseElement.length) {
            backMain();
        }
        window.lastActionTime = time;
    }
}