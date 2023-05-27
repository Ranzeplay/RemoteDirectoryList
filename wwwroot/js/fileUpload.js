document.getElementById("fileUpload").addEventListener("change", function (e) {
    // Get the selected file from the input element
    var file = e.target.files[0];

    // Create a new tus upload
    var upload = new tus.Upload(file, {
        endpoint: "/api/upload",
        retryDelays: [0, 3000, 5000, 10000, 20000],
        metadata: {
            filename: file.name,
            filetype: file.type,
            tabId: tabId,
            path: path,
        },
        onError: function (error) {
            console.error("Failed because: " + error);
        },
        onProgress: function (bytesUploaded, bytesTotal) {
            var percentage = (bytesUploaded / bytesTotal * 100).toFixed(2);
            console.log(bytesUploaded, bytesTotal, percentage + "%");
            document.getElementById("upload-progress").style = "width: " + percentage + "%";
            document.getElementById("uploadModalLabel").innerHTML = "Upload file (" + percentage + "%)";
        },
        onSuccess: function () {
            console.log("Download %s from %s", upload.file.name, upload.url)
            document.getElementById("upload-progress").classList.remove("progress-bar-animated");
            document.getElementById("upload-progress").classList.remove("bg-info");
            document.getElementById("upload-progress").classList.remove("progress-bar-striped");
            document.getElementById("upload-progress").classList.add("bg-success");
        }
    })

    // Check if there are any previous uploads to continue.
    upload.findPreviousUploads().then(function (previousUploads) {
        // Found previous uploads so we select the first one.
        if (previousUploads.length) {
            upload.resumeFromPreviousUpload(previousUploads[0]);

            upload.start();
        }
    })

    // Start the upload
    upload.start();
    document.getElementById("upload-progress").classList.remove("bg-success");
    document.getElementById("upload-progress").classList.add("progress-bar-animated");
    document.getElementById("upload-progress").classList.add("bg-info");
    document.getElementById("upload-progress").classList.add("progress-bar-striped");
})
