
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    exdays = 1;
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$(document).ready(function () {
    $('.manager-box').each(function () {
        var ulContent = $(this).find('ul').text().trim();
        if (ulContent === '') $(this).hide();
    });
    //selectPackageType();
});

function filterData() {
    var baseUrl = window.location.origin + window.location.pathname;
    var url = baseUrl;
    var peg = '?';
    var elements = $('.filter_area [name]');
    for (var i = 0; i < elements.length; i++) {
        var item = $(elements[i]);
        var value = item.val();
        if (value !== '') {
            var name = item.attr('name');
            url += peg + name + '=' + value;
            peg = '&';
        }
    }
    window.location = url;
}
function exportData() {
    var url = location.href;
    url = url.replace('reports/', 'reports/export');
    location.href = url;
}

function changeShopCode() {
    var group = $(event.target).parents('.select-types');
    var shopCode = group.find('input[name]').val();
    if (shopCode === undefined) return;

    setCookie('CurrentShopCode', shopCode);
    console.log('Value: ' + shopCode);
    location.reload();
}
function setShopCode() {
    var shopCode = localStorage.getItem("CurrentShopCode");
    console.log('Current: ' + shopCode);
}

function requestSearch() {
    var area = $(event.target).parents('.filter_area');
    area.find('button').click();
}
function selectShop() {
    var area = $(event.target).parents('.select-types');
    var code = area.find('.hidden, hidden').val();
    if (code != undefined) {
        $('.shop_code').val(code);
    }
}
function selectPackageType() {
    var area = $('.package_type');
    if (area.length > 0) {

        var code = area.find('.hidden, hidden').val();
        if (code === "2") {
            //debugger;
            $('.shop_item').show();
            //$('#ShopCode').val('');
        } else {
            $('.shop_item').hide();
            //$('#ShopCode').val('SYSTEM');
        }
    }
}
//update js 18/03/2021
function togglenav() {
    if (!$('.mannager-content').hasClass('opennav') && !$('.manager-menu').hasClass('opennav')) {
        $('.mannager-content').addClass('opennav')
        $('.manager-menu').addClass('opennav')
    }
    else {
        $('.mannager-content').removeClass('opennav')
        $('.manager-menu').removeClass('opennav')
    }
}