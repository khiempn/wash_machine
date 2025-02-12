
$(document).on('focus', '.select-types', function () {
    $(this).children('ul').show();
});
$(document).on('click', '.select-types span', function () {
    var selectedLi = $(this).parent('li');
    selected(selectedLi);
});
$(document).on('blur', '.select-types', function () {
    if ($(this).attr('prevent-blur')) return;
    $('.select-types > ul').hide();
    $(this).parent().find('.field-validation-error').empty();

    var group = $(this);
    var text = group.find('input[type="text"]:not([name])').val();
    if (text === '') {
        var oldData = $(event.target).attr('data-old')
        if (oldData !== ''){
            selected(event.target);
        }
    }
});

$(document).on('focus', '.select-types input', function () {
    $(event.target).attr('data-old', $(event.target).val());
    var value = '';
    var group = $(this).parents('.select-types');
    group.find('li').filter(function () {
        $(this).toggle(normalize($(this).text()).indexOf(value) > -1)
    });
});
$(document).ready(function () {
    $(".select-types input").on("keyup", function () {
        var value = normalize($(this).val().toLowerCase());
        var group = $(this).parents('.select-types');
        group.find('li').filter(function () {
            $(this).toggle(normalize($(this).text()).indexOf(value) > -1)
        });
    });
    fillSelectTypes();
    //$.validator.setDefaults({ ignore: '' });
});
function fillSelectTypes() {
    $('.select-types').each(function (index) {
        var id = $(this).find('.hidden, hidden').val();
        //var item = $(this).find('.hidden, hidden')[0].outerHTML;
        //var aa = $(this).find('.hidden, [type="hidden"]')[0].value;
        var hidden11 = $(this).find('[type="hidden"]');

        if (id !== '') {
            var selectedLi = $(this).find('[data-id = "' + id + '"]');
            selected(selectedLi);
        }
    });
}
function selected(selectedLi) {
    var name = $(selectedLi).find('span').first().text().trim();
    var id = $(selectedLi).attr('data-id');
    var group = $(selectedLi).parents('.select-types');

    group.find('.active').removeClass('active');
    $(selectedLi).addClass('active');

    group.find('input[type="text"]:not([name])').val(name);
    group.find('input[type="hidden"]').val(id);
    group.find('input.hidden').val(id);
    group.find('input.hidden, input[type="hidden"]').attr('data-text', id);

    $('.select-types > ul').hide();
    group.find('input[type="text"], [type="hidden"]').trigger("change");
}
function normalize(text) {
    if (text === undefined) return '';
    return text.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().replace(/đ/g, 'd').replace(/,/g, '');
}