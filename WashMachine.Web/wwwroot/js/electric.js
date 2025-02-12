function openPayments() {
    clearStacks();
    var dataValue = getDataMoney(event);
    openPopupPOS('/pos/PopupPayments?money=' + dataValue);
}
$.ajaxSetup({ cache: false });
function getDataMoney(ev) {
    var target = ev.target;
    if (!target.hasAttribute("data-money")) {
        target = $(target).closest('[data-money]');
    }
    var dataValue = $(target).attr('data-money');
    return dataValue;
}
function payByOctopus() {
    pushStacks();
    var dataValue = getDataMoney(event);
    openPopupPOS('/pos/OctopusPayment?money=' + dataValue);
}
function payByPayme() {
    pushStacks();
    var dataValue = getDataMoney(event);
    openPopupPOS('/pos/PaymePayment?money=' + dataValue);
}
function payByOther() {
    pushStacks();
    var dataValue = getDataMoney(event);
    openPopupPOS('/pos/AlipayPayment?money=' + dataValue);
}
function openInvoice() {
    var url = $('#modal-placeholder .modal[data-url]').attr('data-url');
    pushStacks(url);
    openPopupPOS('/pos/PopupInvoice');
}
function getModal() {
    return $('#modal-placeholder');
}

function openPopupPOS(link) {
    window.lastActionTime = undefined;
    $('.modal-backdrop.show').remove();
    if (link) url = link;
    if (!link) {
        link = $(event.target).data('url');
    }

    var placeholderElement = getModal();
    placeholderElement.empty();

    var path = link.split('?')[0];
    var viewData = $('[data-view="' + path + '"]');
    if (viewData.length > 0) {
        var htmlView = viewData.html();
        placeholderElement.html(htmlView);
        var params = getParams(link);
        for (var key in params) {
            placeholderElement.find('[data-' + key + ']').attr('data-' + key, params[key]);
        }
        placeholderElement.find('[data-url]').attr('data-url', link);
        placeholderElement.find('.modal').modal({ show: true });
        return;
    }
    $.get(url).done(function (data) {
        placeholderElement.html(data);
        placeholderElement.find('.modal').modal({ show: true });
        var form = placeholderElement.find('form');
        //$.validator.unobtrusive.parse(form);
        $('.modal').on('shown.bs.modal', function () {
            $('.focused').focus();
            if (url.indexOf('?paymentType') > 0) {
                var paymentType = url.substr((url.indexOf("=") + 1), url.length);
                switch (paymentType) {
                    case "1":
                        $('.modal').find('.header').text('Octopus Payment');
                        break;
                    case "2":
                        $('.modal').find('.header').text('Ali Payment');
                        break;
                    case "3":
                        $('.modal').find('.header').text('WeChat Payment');
                        break;
                    default:
                        break;
                }
            }
        });
    });
}

function backStep() {
    $(event.target).parents('.modal').modal('hide');
    var previousUrl = pullStacks();
    $('.modal-backdrop.show').remove();
    if (previousUrl !== undefined && previousUrl !== null) {
        openPopup(previousUrl);
    }
}

function backMain() {
    var target = $('.modal-body');
    if (event && event.target) target = $(event.target);
    target.parents('.modal').modal('hide');
    clearStacks();
    $('.modal-backdrop.show').remove();
    target.parents('.modal').empty();
}

function openPopup(link) {
    $('.modal-backdrop.show').remove();
    var url = $(event.target).data('url');
    if (link) url = link;

    var placeholderElement = getModal();
    placeholderElement.empty();

    var path = link.split('?')[0];
    var viewData = $('[data-view="' + path + '"]');
    if (viewData.length > 0) {
        var htmlView = viewData.html();
        placeholderElement.html(htmlView);
        var params = getParams(link);
        for (var key in params) {
            placeholderElement.find('[data-' + key + ']').attr('data-' + key, params[key]);
        }
        placeholderElement.find('[data-url]').attr('data-url', link);
        placeholderElement.find('.modal').modal({ show: true });
        return;
    }
    $.get(url).done(function (data) {
        placeholderElement.html(data);
        placeholderElement.find('.modal').modal({ show: true });
        var form = placeholderElement.find('form');
        $.validator.unobtrusive.parse(form);
        $('.modal').on('shown.bs.modal', function () {
            $('.focused').focus();
            if (url.indexOf('?paymentType') > 0) {
                var paymentType = url.substr((url.indexOf("=") + 1), url.length);
                switch (paymentType) {
                    case "1":
                        $('.modal').find('.header').text('Octopus Payment');
                        break;
                    case "2":
                        $('.modal').find('.header').text('Ali Payment');
                        break;
                    case "3":
                        $('.modal').find('.header').text('WeChat Payment');
                        break;
                    default:
                        break;
                }
            }
            //if (url.indexOf('/PopupQR') > 0) {
            //    setInterval(function () {
            //        $('.focused').focus();     
            //    }, 2000);
            //}
        });
    });
}

function getStacks() {
    var textItems = localStorage.getItem('stacks-url');
    if (!textItems) {
        textItems = '[]';
    }
    var list = JSON.parse(textItems);
    return list;
}
function pushStacks(url) {
    if (!url) {
        url = $('#modal-placeholder .modal[data-url]').attr('data-url');
    }
    if (!url) return;
    var list = getStacks();
    list.push(url);
    localStorage.setItem("stacks-url", JSON.stringify(list));
}
function pullStacks() {
    var list = getStacks();
    var item = list.pop();
    localStorage.setItem("stacks-url", JSON.stringify(list));
    return item;
}
function clearStacks() {
    localStorage.setItem("stacks-url", '[]');
}

function payOctopus() {
    var url = $('#modal-placeholder .modal[data-url]').attr('data-url');
    pushStacks(url);
    openPopupPOS('/pos/OctopusPayment');
}

function showBrowser() {
    //return;
    //var target = document.getElementById('browser');
    //if (target) {
    //    var text = navigator.userAgent;
    //    target.innerText = text;

    //    //document.getElementById('app-version').innerText = navigator.appVersion + '===' + window.msCrypto;


    //}
}
showBrowser();
function setCookieShop(value) {
    setCookie("ShopCode", value, 1000);
}
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}


function xFileLocked() {
    $(document.body).addClass('xfile-running');
}
function xFileResuming(data) {
    $(document.body).removeClass('xfile-running');
    if (data === undefined) {
        return;
    }
    if (document.domain === 'WashMachine') {
        showSuccessMessage('Assume: Send email generation success!');
        return;
    }
    var obj = JSON.parse(data);
    if (obj !== null) {
        var url = '/POS/SendEmailGenerationError';
        $.ajax({
            type: 'POST',
            data: obj,
            url: url,
            cache: false,
            success: function (data) {
                showSuccessMessage(data.Message);
                if (data.Success) {
                    setTimeout(function () {
                        window.external.SendEmailDone(obj.Time, 'xFileResuming');
                    }, 100);
                }
            },
            error: function (a, b, c, d, e) {
                console.log('Error');
            }
        });
    }
}

function sendEmailUploadFail(data) {
    if (data === undefined) {
        return;
    }
    if (document.domain === 'WashMachine') {
        showSuccessMessage('Assume: Send email success: ' + data.emailType);
        return;
    }
    var obj = JSON.parse(data);
    if (obj !== null) {
        var url = '/POS/SendErrorEmail';
        $.ajax({
            type: 'POST',
            data: obj,
            url: url,
            cache: false,
            success: function (data) {
                showSuccessMessage(data.Message);
                if (data.Success) {
                    setTimeout(function () {
                        window.external.SendEmailDone(obj.Time, 'sendEmailUploadFail');
                    }, 100);
                }
            },
            error: function (a, b, c, d, e) {
                console.log('Error');
            }
        });
    }
}
function sendErrorEmail(data) {
    if (data === undefined) {
        return;
    }
    if (document.domain === 'WashMachine') {
        showSuccessMessage('Assume: Send email success: ' + data.emailType);
        return;
    }
    var obj = JSON.parse(data);
    if (obj !== null) {
        var url = '/POS/SendErrorEmail';
        $.ajax({
            type: 'POST',
            data: obj,
            url: url,
            cache: false,
            success: function (data) {
                showSuccessMessage(data.Message);
                if (data.Success) {
                    setTimeout(function () {
                        window.external.SendEmailDone(obj.Time, 'sendErrorEmail');
                    }, 100);
                }
            },
            error: function (a, b, c, d, e) {
                console.log('Error');
            }
        });
    }
}
