function isElementInViewport(el) {
    //special bonus for those using jQuery
    if (typeof jQuery === "function" && el instanceof jQuery) {
        el = el[0];
    }
    if (!el) return false;
    var rect = el.getBoundingClientRect();
    return (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
        rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
    );
}
function onVisibilityChange(el, callback) {
    var old_visible;
    return function () {
        var visible = isElementInViewport(el);
        if (visible != old_visible) {
            old_visible = visible;
            if (typeof callback == 'function') {
                callback();
            }
        }
    }
}

$(document).on('change', '[data-change-slug]', '', function () {
    var text = event.target.value;
    var unsignedText = slug(text);
    var name = $(event.target).attr('data-change-slug');
    var slugText = $('[name=' + name + ']').val();
    if (slugText === '') {
        $('[name=' + name + ']').val(unsignedText.toLowerCase());
    }
});

function getSlug(text) {
    if (text === undefined) return '';
    text = text.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().replace(/đ/g, 'd').replace(/,/g, '').trim();

    text = text.replace(/[^A-Za-z0-9 -]/g, '') // remove invalid chars
        .replace(/\s+/g, '-') // collapse whitespace and replace by -
        .replace(/-+/g, '-')// collapse dashes
        .replace(/[^\w-]+/g, '');
    return text;
};
$(document).on('focus', 'input', function (e) {
    var text = event.target.value;
    $(event.target).attr('data-old', text);
});

$(document).on('input', '.alphanumeric', function (e) {
    var text = event.target.value;
    var validText = text.replace(/[^a-z0-9]/gi, '');
    event.target.value = validText
});

function getParams() {
    var params = {};
    window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) { params[key] = value; });
    return params;
}