(function (proxied) {
    //window.alert = function () {
    //    return alertMessage.apply(this, arguments);
    //};

    $(document).on("click", 'button[data-dismiss="modal"]', function () {
        $(this).parents(".modal:first").modal('hide');
    });
})(window.alert);
function alertMessage(text, title) {
    var placeholderElement = $('#modal-placeholder');
    var modal1 = $('#modal-placeholder .modal.show');
    if (modal1.length) placeholderElement = $('#modal-placeholder2');

    $('#alertTemplate .modal-body').html(text);
    if (title) {
        $('#alertTemplate .modal-title').html(title);
    }
    var htmlText = document.getElementById('alertTemplate').outerHTML;
    placeholderElement.html(htmlText);
    placeholderElement.find('.modal').modal('show');
}

function deleteItem(url,urlReload) {
    window.deleteUrl = url;
    window.urlReload = urlReload;
    var placeholderElement = $('#modal-placeholder');
    var modal1 = $('#modal-placeholder .modal.show');
    if (modal1.length) placeholderElement = $('#modal-placeholder2');

    var text = document.getElementById('exampleModal').outerHTML;
    placeholderElement.html(text);
    placeholderElement.find('.modal').modal('show');
}



function requestDelete() {
    $.ajax({
        url: window.deleteUrl,
        method: 'DELETE',
        contentType: 'application/json',
        success: function (data) {
            if (window.urlReload.length > 0) {
                window.location.href = window.urlReload;
            }
            else {
                reloadPage();
                if (data.success)
                    showSuccessMessage(data.message);
                else
                    showWarningMessage(data.message);
            }
        },
        error: function (request, msg, error) {
            showWarningMessage(error);
            // handle failure
        }
    });
}

function openPopupForm(link) {
    var modal1 = $('#modal-placeholder .modal.show');
    var url = $(event.target).data('url');
    if (link) url = link;

    var placeholderElement = $('#modal-placeholder');
    if (modal1.length) placeholderElement = $('#modal-placeholder2');
    $('.preloader').show();
    $.get(url).done(function (data) {
        placeholderElement.html(data);
        placeholderElement.find('.modal:first').modal('show');
        var form = placeholderElement.find('form');
        $.validator.unobtrusive.parse(form);
        loadDataReferUrl();
        $('.modal').on('shown.bs.modal', function () {
            $('.focused').focus();
        });
        $('.preloader').show();
    });
}
function savePopupForm() {
    event.preventDefault();
    var form = $(event.target).parents('.modal').find('form')[0];
    var isValid = $(form).valid();
    if (!isValid) return;

    var actionUrl = $(event.target).parents('.modal').find('form').attr('action');
    var formData = new FormData(form);
    $.ajax({
        type: "POST",
        enctype: 'multipart/form-data',
        url: actionUrl,
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        timeout: 600000,
        success: function (data) {
            var newBody = $('.modal-body', data);
            var placeholderElement = $('#modal-placeholder');

            var modal2 = $('#modal-placeholder2 .modal.show');
            if (modal2.length) placeholderElement = $('#modal-placeholder2');

            placeholderElement.find('.modal-body').replaceWith(newBody);

            // find IsValid input field and check it's value
            // if it's valid then hide modal window
            var isValid = newBody.find('[name="IsValid"]').val() === 'True';
            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
                placeholderElement.find('.modal').empty();
                showSuccessMessage("Successful implementation!");
                reloadPage();
            }

        }
    });
}

function reloadPage(refreshUrl) {
    var refreshArea = $('[data-url-refresh="' + refreshUrl + '"]').last();
    if (refreshArea.length === 0) {
        refreshArea = $('[data-url-refresh]');
    }
    if (refreshUrl === undefined) {
        refreshUrl = refreshArea.attr('data-url-refresh');
    }
    $.get(refreshUrl).done(function (data) {
        var refreshedData = $(data).find('[data-url-refresh]').html();
        refreshArea.html(refreshedData);
    });

    if (refreshArea.length === 0) location.reload();
}
function reloadGridPage(pageIndex) {
    var refreshArea = $(event.target).parents('[data-url-refresh]');
    var refreshUrl = refreshArea.attr('data-url-refresh');
    if (refreshUrl.indexOf("?") === -1) {
        refreshUrl += '?';
    }
    var pattern = /page=[0-9]+/g;
    refreshUrl = refreshUrl.replace(pattern, '');

    var lastChar = refreshUrl.substr(-1);
    if (lastChar !== '?' && lastChar != '&') refreshUrl += '&';

    refreshUrl += 'page=' + pageIndex;
    refreshArea.attr('data-url-refresh', refreshUrl);
    if (refreshUrl.indexOf(location.pathname) === 0) {
        window.history.pushState('page2', 'Title', refreshUrl);
    }
    reloadPage(refreshUrl);
}
function executeSearch() {
    var refreshUrl = window.location.pathname;
    var area = $(event.target).parents('.filter_area');
    var refreshArea = area.find('[data-url-refresh]');
    if (refreshArea.length === 0) {
        refreshArea = $('[data-url-refresh]');
    }
    refreshUrl = refreshArea.attr('data-url-refresh').split('?')[0];
    var separator = '?';
    $(area.find('input')).each(function (index) {
        var name = $(this).attr('name');
        var text = $(this).val();
        if (text === '' || name === undefined) return;

        text = text.trim().replace(/ /gi, "+");
        refreshUrl = refreshUrl + separator + name + '=' + text;
        separator = '&';
    });
    //AnhNT
    $(area.find('select')).each(function (index) {
        var name = $(this).attr('name');
        var text = $(this).val();
        if (text === '' || name === undefined) return;

        text = text.trim().replace(/ /gi, "+");
        refreshUrl = refreshUrl + separator + name + '=' + text;
        separator = '&';
    });
    // End AnhNT
    refreshArea.attr('data-url-refresh', refreshUrl);
    if (refreshUrl.indexOf(location.pathname) === 0) {
        window.history.pushState('page2', 'Title', refreshUrl);
    }
    reloadPage(refreshUrl);
}
$(document).on('keyup', '.filter_area input', function () {
    if (event.keyCode === 13) {
        var area = $(event.target).parents('.filter_area');
        area.find('button').trigger("click");
        $(event.target).focus();
    }
});
$(document).on('click', '[data-url-refresh] .manager-paging a', function () {
    event.preventDefault();
    var refreshElement = $(event.target).parents('[data-url-refresh]').first();
    var refreshUrl = event.target.href;
    refreshElement.attr('data-url-refresh', refreshUrl);
    reloadPage(refreshUrl);
});
function loadDataReferUrl() {
    
    var referAreas = $('[data-refer-url]');
    referAreas.each(function (i, item) {
        var referUrl = $(item).attr('data-refer-url');
        $('.preloader').show();
        $.get(referUrl).done(function (data) {
            $(item).html(data);
            $('.preloader').hide();
        });
    });
}
$(document).on('click', '[role="tab"]', function () {
    var href = $(event.target).attr('href');
    var arr = location.href.split('#');
    var link = arr[0] + href;
    window.location.replace(link);
    $('form').attr('action', link);
});

$(document).ready(function () {
    $('.readonly').attr('readonly', '');
    $('.clicked_onload').each(function (index) {
        $(this)[0].click();
    });

    if (window.location.hash !== undefined && window.location.hash !== '') {
        var currentTab = $('[href="' + window.location.hash + '"]');
        $('.tab-pane, .nav-link').removeClass('active');
        currentTab.addClass('active');
        $(window.location.hash).addClass('active');
        $(window.location.hash).addClass('show');
    }
});


function showSuccessMessage(message) {

    $.gritter.add({
        position: 'bottom-right',
        time: 3000,
        title: '<i class="fa fa-check-circle"></i>  Message',
        text: message,
        class_name: 'gritter-light'
    });

    $('.alert-dismissable').remove();
}
function showWarningMessage(message, center, timeOut) {
    if (!timeOut) {
        timeOut = 3000;
    }
    var centerClass = "gritter-bottom ";
    if (center == null || !center)
        centerClass = "";

    $.gritter.add({
        position: 'bottom-right',
        time: timeOut,
        title: '<i class="fa fa-check-circle"></i>  Message',
        text: message,
        class_name: centerClass + 'gritter-warning'
    });

    $('.alert-dismissable').remove();
}