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

// stateLabel.innerHTML = "Connecting...";
socket = new WebSocket(connectionUrl);

socket.onopen = function (event) {
    // updateState();
    console.log("Connection opened");
};

socket.onclose = function (event) {
    // updateState();
    console.log('Connection closed. Code: ' + event.code + '. Reason: ' + event.reason);
};

socket.onerror = function (event) {
    // updateState();
    console.log('Connection error. Code: ' + event.code + '. Reason: ' + event.reason);
};

socket.onmessage = function (event) {
    console.log('Socket data received ' + event.data);
    // update the revieved cell
    var cell = event.data.split(':');
    var rxBoardId = cell[0];
    if (boardId != rxBoardId)
        return;
    var i = cell[1];
    var j = cell[2];
    var receivedColor = cell[3];
    var cell = document.getElementById('cell_' + i + "/" + j);
    cell.style.backgroundColor = receivedColor;
    prevColor = receivedColor; // avoid overwriting the new color on mouseleave
};


function pickColor(element) { 
    mainColor = element.value;
}


/////////////////////////////////
// EDIT Board Name dynamically //
/////////////////////////////////

document.getElementById('boardName').addEventListener('click', function() {
    this.style.display = 'none';
    var input = document.getElementById('editInput');
    input.style.display = 'block';
    input.value = this.textContent;
    input.focus();
});

document.getElementById('editInput').addEventListener('keydown', function(e) {
    if (e.key === 'Enter') {
        this.style.display = 'none';
        var label = document.getElementById('boardName');
        label.style.display = 'block';
        label.textContent = this.value;

        // Send a POST request
        // var boardId = /* the ID of the board you want to edit */;
        var formData = new FormData();
        formData.append('Id', boardId);
        formData.append('Name', this.value);
        
        fetch(`Edit?id=${boardId}`, {
            method: 'POST',
            headers: {
                
            },
            body: formData,
        })
        //.then(response => response.json())
        //.then(data => console.log(data))
        //.catch((error) => console.error('Error:', error));
    }
    else if (e.key === 'Escape') {
        this.style.display = 'none';
        var label = document.getElementById('boardName');
        label.style.display = 'block';
    }
});

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
    var data = boardId + ":" + i + ":" + j + ":" + mainColor;
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

/*
function enterCell(hoverCell) {
    prevColor = hoverCell.style.backgroundColor;
    prevColorHex = rgbToHex(hoverCell.style.backgroundColor);
    // avoid ghost color on quick mouse movements where prevColor would be set to a fading color
    //if (!allColors.includes(prevColorHex)) { 
    //    prevColor = "rgb(255, 255, 255)";
    //}
    hoverCell.style.backgroundColor = highColor;
}
*/

var mouseDown = 0;
document.body.onmousedown = function() {
    mouseDown = 1;
    
}
document.body.onmouseup = function() {
    mouseDown = 0;
}

function enterCell(hoverCell) {
    prevColor = hoverCell.style.backgroundColor;
    //prevColorHex = rgbToHex(hoverCell.style.backgroundColor);

    // if the mouse is down and the color is not the same as the current color trigger click cell event for the current cell
    if (mouseDown && prevColor !== mainColor) {
        var cellId = hoverCell.id.split('_')[1];
        var i = cellId.split('/')[0];
        var j = cellId.split('/')[1];
        clickCell(i, j);
    }
    else{
        hoverCell.style.backgroundColor = highColor;
    }
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