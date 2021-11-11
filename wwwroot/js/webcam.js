function startVideo(src) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
            let video = document.getElementById(src);
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                video.src = window.URL.createObjectURL(stream);
            }
            video.onloadedmetadata = function (e) {
                video.play();
            };
            //mirror image
            video.style.webkitTransform = "scaleX(-1)";
            video.style.transform = "scaleX(-1)";
            video.style.visibility = "collapse"
            video.style.position = "fixed"
        });
    }
}

function getFrame(src, dest) {
    let video = document.getElementById(src);
    var canvas = document.createElement("canvas");

    canvas.width = 1920;
    canvas.height = 1080;

    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

    let dataUrl = canvas.toDataURL("image/jpeg");
    return dataUrl;
}


window.PlayAudio = (elementName) => {
    document.getElementById(elementName).play();
}