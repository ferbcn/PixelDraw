var stateLabel = document.getElementById("stateLabel");
var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";
var connectionUrl = scheme + "://" + document.location.hostname + port + "/wsclick";

var mainColor = '#DD3333'; // red
var baseColor = '#FFFFFF'; // white
var highColor = '#AAAAAA'; // lightgrey
var prevColor; // to store the previous color of the cell when hovering over it


// WebSockets connection and event handling

stateLabel.innerHTML = "Connecting...";
socket = new WebSocket(connectionUrl);

socket.onopen = function (event) {
    updateState();
    console.log("Connection opened");
};

socket.onclose = function (event) {
    updateState();
    console.log('Connection closed. Code: ' + event.code + '. Reason: ' + event.reason);
};

socket.onerror = updateState;

socket.onmessage = function (event) {
    console.log('Socket data received ' + event.data);
    // updtae the revieved cell
    var cell = event.data.split(':');
    var i = cell[0];
    var j = cell[1];
    var receivedColor = cell[2];
    var cell = document.getElementById('cell_' + i + "/" + j);
    cell.style.backgroundColor = receivedColor;
    prevColor = receivedColor; // avoid overwriting the new color on mouseleave
};

function updateState() {

    if (!socket) {
        disable();
    } else {
        switch (socket.readyState) {
            case WebSocket.CLOSED:
                stateLabel.innerHTML = "Closed";
                break;
            case WebSocket.CLOSING:
                stateLabel.innerHTML = "Closing...";
                break;
            case WebSocket.CONNECTING:
                stateLabel.innerHTML = "Connecting...";
                break;
            case WebSocket.OPEN:
                stateLabel.innerHTML = "Open";
                break;
            default:
                stateLabel.innerHTML = "Unknown WebSocket State: " + socket.readyState;
                break;
        }
    }
}


function clickCell (i, j) {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("socket not connected");
    }
    var data = i + ":" + j + ":" + mainColor;
    prevColor = mainColor;
    socket.send(data);
    console.log('Sending socket data ' + data);
};


// Cell coloring and animations 

window.onload = function () {
    console.log("Document loaded");
    document.querySelectorAll('.cell').forEach(cell => {
        cell.addEventListener('mouseenter', function () {
            enterCell(this);
        });
        cell.addEventListener('mouseleave', function () {
            leaveCell(this);
        });
    });
}

function enterCell(hoverCell) {
    prevColor = hoverCell.style.backgroundColor;
    prevColorHex = rgbToHex(hoverCell.style.backgroundColor);
    // avoid ghost color on quick mouse movements where prevColor would be set to a fading color
    //if (!allColors.includes(prevColorHex)) { 
    //    prevColor = "rgb(255, 255, 255)";
    //}
    hoverCell.style.backgroundColor = highColor;
}

function leaveCell(hoverCell) {
    hoverCell.style.backgroundColor = prevColor;
    //transitionColor(hoverCell, highColor, rgbToHex(prevColor), 500);
}

function transitionColor(element, startColor, endColor, duration) {
    var startTime = new Date().getTime();

    var endColorRGB = hexToRgb(endColor);
    var startColorRGB = hexToRgb(startColor);

    var timer = setInterval(function () {
        var now = new Date().getTime();
        var elapsedTime = now - startTime;
        var fraction = elapsedTime / duration;

        if (fraction >= 1) {
            clearInterval(timer);
            fraction = 1;
        }

        var currentColor = 'rgb(' +
            (Math.round(startColorRGB.r + (endColorRGB.r - startColorRGB.r) * fraction)) + ',' +
            (Math.round(startColorRGB.g + (endColorRGB.g - startColorRGB.g) * fraction)) + ',' +
            (Math.round(startColorRGB.b + (endColorRGB.b - startColorRGB.b) * fraction)) + ')';
        element.style.backgroundColor = currentColor;
    }, 25);
}

function hexToRgb(hex) {
    var r = 0, g = 0, b = 0;
    // 3 digits
    if (hex.length == 4) {
        r = parseInt(hex[1] + hex[1], 16);
        g = parseInt(hex[2] + hex[2], 16);
        b = parseInt(hex[3] + hex[3], 16);
    }
    // 6 digits
    else if (hex.length == 7) {
        r = parseInt(hex[1] + hex[2], 16);
        g = parseInt(hex[3] + hex[4], 16);
        b = parseInt(hex[5] + hex[6], 16);
    }
    return { r: r, g: g, b: b };
}

function rgbToHex(rgb) {
    var parts = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    delete (parts[0]);
    for (var i = 1; i <= 3; ++i) {
        parts[i] = parseInt(parts[i]).toString(16);
        if (parts[i].length == 1) parts[i] = '0' + parts[i];
    }
    return '#' + parts.join('');
}