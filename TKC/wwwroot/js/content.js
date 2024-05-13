
$(document).ready(function () {
    // Find all editor nodes
    var editors = $('.editor');
    // Loop through each div and initialize TinyMCE editor
    editors.each(function (index, element) {
        initTinyForElement(element);
    });
});

function initTinyForElement(element) {

    var contentId = $(element).attr('contentid');

    $(element).tinymce({
        license_key: 'gpl',
        inline: true,
        plugins_url: '/tinymce/plugins/',
        plugins: ['advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount'],
        toolbar: 'undo redo | blocks | formatselect | bold italic | alignleft aligncenter alignright | bullist numlist outdent indent | link image | code',
        menubar: false,
        setup: function (editor) {
            editor.on('change', function () {
                saveEditorChanges(contentId, editor.getContent());
            });

            editor.on('drop', function (event) {
                var droppedFiles = event.dataTransfer.files;
                if (droppedFiles.length > 0) {
                    handleDroppedImage(editor, droppedFiles[0]);
                }
            });

            editor.on('PreInit', function () {
                editor.parser.addNodeFilter('iframe', function (nodes) {
                    nodes.forEach(function (node) {
                        node.attr('sandbox', 'allow-scripts allow-same-origin');
                    });
                });
            });

        }
    });
}

function handleDroppedImage(editor, file) {
    // Upload the image to your API
    uploadImageToAPI(file, function (response) {
        if (response.success) {
            // Insert the uploaded image into the editor
            var imageUrl = response;
            editor.execCommand('mceInsertContent', false, '<img src="' + imageUrl + '">');
        } else {
            // Handle error
            console.error('Failed to upload image:', response.error);
        }
    });
}

function saveEditorChanges(contentId, content) {
    saveContent(contentId, content);
}

function uploadImageToAPI(file, callback) {
    // Replace '/upload-image-api' with the actual endpoint of your API
    var apiUrl = '/api/htmlcontent/';

    var formData = new FormData();
    formData.append('file', file);

    $.ajax({
        url: apiUrl,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            callback(response);
        },
        error: function (xhr, status, error) {
            callback({ success: false, error: error });
        }
    });
}

function saveContent(id, content) {

    var formData = new FormData();
    formData.append('html', content);

    var url = '/api/htmlcontent/' + id;

    $.ajax({
        url: url,
        type: 'PATCH',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            location.reload();
        },
        error: function (xhr, status, error) {
            alert("Failed to save. Error: " + error);
        }
    });
}