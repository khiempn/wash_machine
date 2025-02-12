
function nextMonth() {
    var monthText = $('#FromMonth').val();
    var arr = monthText.split('/');
    var d = new Date(parseInt(arr[1]), parseInt(arr[0]), 1);
    month = d.getMonth();
    var year = d.getFullYear();
    $('#FromMonth').val(month + 1 + '/' + year);
    filterData();
}
function previewMonth() {
    var monthText = $('#FromMonth').val();
    var arr = monthText.split('/');
    var d = new Date(parseInt(arr[1]), parseInt(arr[0]), 1);
    month = d.getMonth();
    d.setMonth(month - 2);
    month = d.getMonth();
    var year = d.getFullYear();
    $('#FromMonth').val(month + 1 + '/' + year);
    filterData();
}
function nextDay() {
    var monthText = $('#FromDate').val();
    var arr = monthText.split('/');
    var d = new Date(parseInt(arr[2]), parseInt(arr[1]) - 1, parseInt(arr[0]));
    var date = d.getDate();
    d.setDate(date + 1);
    date = d.getDate();
    var month = d.getMonth();
    var year = d.getFullYear();
    $('#FromDate').val(date + '/' + (month + 1) + '/' + year);
    filterData();
}
function previousDay() {
    var monthText = $('#FromDate').val();
    var arr = monthText.split('/');
    var d = new Date(parseInt(arr[2]), parseInt(arr[1]) - 1, parseInt(arr[0]));
    var date = d.getDate();
    d.setDate(date - 1);
    date = d.getDate();
    var month = d.getMonth();
    var year = d.getFullYear();
    $('#FromDate').val(date + '/' + (month + 1) + '/' + year);
    filterData();
}
function addShopUser() {
    var dataId = $(event.target).parents('[data-id]').attr('data-id');
    var excludeElement = $('.available-users [name="Exclude"]');
    var excludeIds = excludeElement.val() + dataId + '-';
    excludeElement.val(excludeIds);

    var includeElement = $('.current-shop [name="Include"]');
    var includeIds = includeElement.val() + dataId + '-';
    includeElement.val(includeIds);
    $('form [name="UserIds"]').val(includeIds);

    $('.current-shop [onclick="executeSearch()"], .available-users [onclick="executeSearch()"]').each(function (i, item) {
        $(item)[0].dispatchEvent(new Event('click'));
    });
}
function removeShopUser() {
    var dataId = $(event.target).parents('[data-id]').attr('data-id');
    var excludeElement = $('.available-users [name="Exclude"]');
    var excludeIds = excludeElement.val();
    dataId = '-' + dataId + '-';
    excludeIds = excludeIds.replace(dataId, "-");
    excludeElement.val(excludeIds);

    var includeElement = $('.current-shop [name="Include"]');
    var includeIds = includeElement.val();

    dataId = '-' + dataId + '-';
    includeIds = excludeIds.replace(dataId, "-");
    includeElement.val(includeIds);
    $('form [name="UserIds"]').val(includeIds);

    $('.current-shop [onclick="executeSearch()"], .available-users [onclick="executeSearch()"]').each(function (i, item) {
        $(item)[0].dispatchEvent(new Event('click'));
    });
}
function saveShopOwrner() {
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
            isValid = true;
            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
                placeholderElement.find('.modal').empty();
                showSuccessMessage("Successful implementation!");
                location.reload();
            }

        }
    });
}