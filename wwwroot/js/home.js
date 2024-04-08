var stateLabel = document.getElementById("stateLabel");
var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";
var connectionUrl = scheme + "://" + document.location.hostname + port + "/wsclick";

var mainColor = '#000000'; // red
var prevColor; // to store the previous color of the cell when hovering over it
var baseColor = '#ffffff'; // white
var selectedColor = '#000000'; // color selected in the color picker

// Cells in the drawing board
var allCells = document.querySelectorAll('.cell');

// Mouse and touch control variables
var mouseDown = 0;
var touchActive = false;

// Controls and Buttons
var eraserOn = false;
var eraserBtn = document.getElementById('eraser-btn');
var nukeBtn = document.getElementById('nuke-btn');
var colorPicker = document.getElementById('html5colorpicker');

// WebSockets connection and event handling
// stateLabel.innerHTML = "Connecting...";
console.log("Connecting to " + connectionUrl);
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
    if (receivedColor === "nuke") {
        allCells.forEach(cell => {
            cell.style.backgroundColor = baseColor;
        });
        return;
    }
    var cell = document.getElementById('cell_' + i + "/" + j);
    cell.style.backgroundColor = receivedColor;
    prevColor = receivedColor; // avoid overwriting the new color on mouseleave
};


/////////////////////////////////
///// Buttons and Controls  /////
/////////////////////////////////

colorPicker.addEventListener('change', function() {
    selectedColor = colorPicker.value;
    mainColor = selectedColor;
    console.log("Color changed to " + selectedColor);
});


eraserBtn.addEventListener('click', function() {
    eraserOn = !eraserOn;
    if (eraserOn) {
        mainColor = baseColor;
        eraserBtn.classList.remove('btn-light');
        eraserBtn.classList.add('btn-danger');
    }
    else {
        mainColor = selectedColor;
        eraserBtn.classList.remove('btn-danger');
        eraserBtn.classList.add('btn-light');
    }
});

// Erase the whole board
nukeBtn.addEventListener('click', function() {
    var data = boardId + ":-1:-1:" + "nuke";
    socket.send(data);
    console.log('Sending socket data ' + data);
});


function clickCell (i, j) {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("socket not connected");
    }
    var data = boardId + ":" + i + ":" + j + ":" + mainColor;
    prevColor = mainColor;
    socket.send(data);
    console.log('Sending socket data ' + data);
};


// Cell Enter / Cell leave
// Cell coloring and animations
window.onload = function () {
    console.log("Document loaded");
    allCells.forEach(cell => {
        cell.addEventListener('mouseenter', function () {
            enterCell(this);
        });
        cell.addEventListener('mouseleave', function () {
            leaveCell(this);
        });
    });
}

////////////////////////////////
// Mouse and Touch Handlers  //
////////////////////////////////

document.body.addEventListener('mousedown', function(){
    mouseDown = 1;
});

document.body.addEventListener('mouseup', function(){
    mouseDown = 0;
});

document.body.addEventListener('touchstart', function(event){
    touchActive = true;
    console.log("Touch start");
    //touchHandler(event);
});

document.body.addEventListener('touchend', function(){
    touchActive = false;
    console.log("Touch end");
});

document.body.addEventListener('touchmove', function(event){
    if (!touchActive) return;
    event.preventDefault();  // prevent scrolling page when touch is active
    touchHandler(event);
});

function touchHandler(event) {
    var touches = event.changedTouches;

    for(var i=0; i < event.changedTouches.length; i++) {
        var touchId = event.changedTouches[i].identifier;
        var touchPosX       = event.changedTouches[i].pageX;
        var touchPosY       = event.changedTouches[i].pageY;
    }
    console.log("Touch start: " + touchPosX + " / " +  touchPosY);

    let cellElement = document.querySelector('.cell'); // change .cellClassName to your actual class or id 
    let cellWidth = cellElement.offsetWidth - 2.4;
    let cellHeight = cellElement.offsetHeight - 2.4;
    let xOffset = cellElement.getBoundingClientRect().x;
    let yOffset = cellElement.getBoundingClientRect().y;
    let cellX = Math.floor((touchPosX - xOffset) / cellWidth);
    let cellY = Math.floor((touchPosY - yOffset) / cellHeight);

    // Get the Cell ID associated with touch x/y position
    console.log("Cell ID: " + cellX + " / " +  cellY);
    
    if (cellX >= 0 && cellX < boardSize && cellY >= 0 && cellY < boardSize){
        clickCell(cellY, cellX);
    }
}

function enterCell(hoverCell) {
    prevColor = hoverCell.style.backgroundColor;

    // if the mouse is down and the color is not the same as the current color trigger click cell event for the current cell
    if (mouseDown) {
        var cellId = hoverCell.id.split('_')[1];
        var i = cellId.split('/')[0];
        var j = cellId.split('/')[1];
        clickCell(i, j);
    }
    else{
        hoverCell.style.backgroundColor = mainColor;
    }
}

function leaveCell(hoverCell) {
    if (mouseDown) {
        var cellId = hoverCell.id.split('_')[1];
        var i = cellId.split('/')[0];
        var j = cellId.split('/')[1];
        clickCell(i, j);
    }
    else {
        hoverCell.style.backgroundColor = prevColor;
    }
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
        formData.append('Size', boardSize);

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

