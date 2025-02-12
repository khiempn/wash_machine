
function setAppName() {
    $('body').attr('data-app', 'DropCoin');
}

function dropCoins(coinValue, type) {
    setTimeout(function () {
        //var coinValue = 1;
        if (detectWebview()) {
            alertA('window.external.RecieveToDropCoins(' + coinValue + ', ' + type + ')');
            window.external.RecieveToDropCoins(coinValue, type);
        } else {
            showFormResult("{\"Value\": 0, \"Message\":\"ERROR\"}");
        }
    }, 100);
}
function showFormResult(data) {
    var obj = JSON.parse(data);
    if (obj.Value === undefined) {
        obj = JSON.parse(obj);
    }
    const messageTag = $('#payment_status');
    messageTag.text(obj.Message);
}
function AssumePaymentSuccess(type, number) {
    dropCoins(number, type);
    
}
