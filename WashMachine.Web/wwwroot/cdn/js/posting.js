
$(function () {
    $('.datepicker').datepicker({
        format: 'MM/dd/yyyy'
    });

    var message = $('body').attr('data-message');
    if (message !== '') {
        showMessage(message);
    }
    $('.area-search input').keyup(function () {
        if (event.keyCode === 13) {
            $(this).parents('.area-search').find('button').click();
        }
    });
});

var myEditor;

$(document).ready(function () {
    bindCKEditor();
});
function bindCKEditor() {

    var editorArea = document.querySelector('.editor');
    if (editorArea) {
        ClassicEditor
            .create(document.querySelector('.editor'))
            .then(function (editor) {
                //console.log('Editor was initialized', editor);
                myEditor = editor;
                editor.model.document.on('change:data', function () {
                    var dataEditor = editor.getData();
                    $('.group-contents').find('input.hidden, textarea.hidden').val(dataEditor);
                    $('.group-contents').find('.field-validation-error').empty();
                });
            })
            .catch(function (err) {
                console.error(err.stack);
            });
    }
}
function testFunction() {
    var text = myEditor.getData();
}
var listFiles = [];
function showImages() {
    var inp = $('.file-images').last()[0];
    for (var i = 0; i < inp.files.length; i++) {
        var element = document.createElement("div");
        element.classList.add('posting-image');
        var image = document.createElement("IMG");
        image.src = URL.createObjectURL(inp.files[i]);
        element.appendChild(image);
        $('#list-images').append(element);
    }

    $(inp.parentElement).append(inp.outerHTML);
    var input = $('.file-images').last()[0];
    var number = $('.file-images').length - 1;
    input.setAttribute('name', 'Images[' + number + ']');
}
function selectFile() {
    var inputFile = $(event.target).parents('.image-selecting').find('[type="file"]')[0];
    inputFile.click();
}
function showImage() {
    var inputFile = $(event.target)[0];
    var src = URL.createObjectURL(inputFile.files[0]);
    $(inputFile.parentElement).find('img').attr('src', src);
}

function previewImage() {

    var previewTagId = $(event.target).attr("for-img");
    $('#' + previewTagId).attr("src", URL.createObjectURL(event.target.files[0]));
}