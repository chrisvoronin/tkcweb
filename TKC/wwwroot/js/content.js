
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
        automatic_uploads: true,
        plugins_url: '/tinymce/plugins/',
        plugins: ['advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount'],
        toolbar: 'undo redo | blocks | formatselect | bold italic | alignleft aligncenter alignright | bullist numlist outdent indent | link image | code',
        file_picker_types: 'image',
        images_file_types: 'jpg,png,webp,svg',
        block_unsupported_drop: true,
        contextmenu: 'link image table',
        menubar: false,
        /* and here's our custom image picker*/
        file_picker_callback: (cb, value, meta) => {
            const input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');

            input.addEventListener('change', (e) => {
                const file = e.target.files[0];

                const reader = new FileReader();
                reader.addEventListener('load', () => {
                    /*
                      Note: Now we need to register the blob in TinyMCEs image blob
                      registry. In the next release this part hopefully won't be
                      necessary, as we are looking to handle it internally.
                    */
                    const id = 'blobid' + (new Date()).getTime();
                    const blobCache = tinymce.activeEditor.editorUpload.blobCache;
                    const base64 = reader.result.split(',')[1];
                    const blobInfo = blobCache.create(id, file, base64);
                    blobCache.add(blobInfo);

                    /* call the callback and populate the Title field with the file name */
                    cb(blobInfo.blobUri(), { title: file.name });
                });
                reader.readAsDataURL(file);
            });

            input.click();
        },
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
            toastBootstrap.show();
        },
        error: function (xhr, status, error) {
            alert("Failed to save. Error: " + error);
        }
    });
}

const toastLiveExample = document.getElementById('liveToast');
const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLiveExample);