var mainColor = '#DD3333'; // red
var baseColor = '#FFFFFF'; // white
var highColor = '#AAAAAA'; // lightgrey
var prevColor; // to store the previous color of the cell when hovering over it

var grid = Array(50).fill().map(() => Array(50).fill('#FFFFFF'));

var animationIsRunning = false;
var timeout = 500;
var animationInterval;

var genCount = 0;
var genCountElement = document.getElementById("genCount");

// Mouse and touch control variables
var mouseDown = false;
var touchActive = false;

// Cells in the drawing board
var allCells = document.querySelectorAll('.cell');

// disable touch scrolling (for drawing on mobile)
document.body.setAttribute("style","touch-action: none;");

function clickCell (i, j) {
    // get color of the cell
    let currentColor = grid[i][j];
    // console.log("Cell color: " + currentColor);
    
    // color the cell on DOM
    var cell = document.getElementById('cell_' + i + "/" + j);
    
    if (currentColor !== mainColor) {
        currentColor = mainColor;
        // cell.innerHTML = "&#128126";
    } else {
        currentColor = baseColor;
        // cell.innerHTML = "";
    }
    cell.style.backgroundColor = currentColor;
    
    // set prev color to the current color
    prevColor = currentColor;
    
    // keep track of the color of the cell in the server
    grid[i][j] = currentColor;
}

// Cell coloring and animations

// Cell Enter / Cell leave
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

document.body.addEventListener('mousedown', function(){
    mouseDown = true;
    console.log("Mouse Down");
});

document.body.addEventListener('mouseup', function(){
    mouseDown = false;
    console.log("Mouse Up");
});

document.body.addEventListener('touchstart', function(event){
    touchActive = true;
    console.log("Touch start");
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
    // console.log("Touch start: " + touchPosX + " / " +  touchPosY);

    let cellElement = document.querySelector('.cell'); // change .cellClassName to your actual class or id 
    let cellWidth = cellElement.offsetWidth - 2.4;
    let cellHeight = cellElement.offsetHeight - 2.4;
    let xOffset = cellElement.getBoundingClientRect().x;
    let yOffset = cellElement.getBoundingClientRect().y;
    let cellX = Math.floor((touchPosX - xOffset) / cellWidth);
    let cellY = Math.floor((touchPosY - yOffset) / cellHeight);

    // Get the Cell ID associated with touch x/y position
    // console.log("Cell ID: " + cellX + " / " +  cellY);

    if (cellX >= 0 && cellX < boardSize && cellY >= 0 && cellY < boardSize){
        clickCell(cellY, cellX);
    }
}


function enterCell(hoverCell) {
    prevColor = hoverCell.style.backgroundColor;
    prevColorHex = rgbToHex(hoverCell.style.backgroundColor);

    // if the mouse is down and the color is not the same as the current color trigger click cell event for the current cell
    if (mouseDown && prevColor !== mainColor) {
        var cellId = hoverCell.id.split('_')[1];
        var i = cellId.split('/')[0];
        var j = cellId.split('/')[1];
        clickCell(i, j);
    }
    else {
        hoverCell.style.backgroundColor = highColor;
    }
}


function leaveCell(hoverCell) {
    if (mouseDown && prevColor !== mainColor) {
        var cellId = hoverCell.id.split('_')[1];
        var i = cellId.split('/')[0];
        var j = cellId.split('/')[1];
        clickCell(i, j);
    }
    else {
        hoverCell.style.backgroundColor = prevColor;
    }
}


document.addEventListener('DOMContentLoaded', function (event) {

    // Cellular Automata animation
    document.getElementById("btnStart").addEventListener("click", function () {
        if (animationIsRunning) {
            console.log("Pausing Cellular Automata");
            animationIsRunning = false;
            StopAnimation();
        }
        else {
            console.log("Starting Cellular Automata");
            animationIsRunning = true;
            StartAnimation();
        }
    });

    // Buttons to control the speed of the animation
    document.getElementById("btnMinus").addEventListener("click", function () {
        if (timeout < 1000) timeout += 50;
        StopAnimation();
        StartAnimation();

    });

    document.getElementById("btnPlus").addEventListener("click", function () {
        if (timeout > 50) timeout -= 50;
        StopAnimation();
        StartAnimation();
    });

    document.getElementById("btnReset").addEventListener("click", function () {
        animationIsRunning = false;
        genCount = 0;
        genCountElement.innerHTML = genCount;
        var allCells = document.querySelectorAll('.cell');
        allCells.forEach(cell => {
            cell.style.backgroundColor = baseColor;
            grid = Array(50).fill().map(() => Array(50).fill('#FFFFFF'));
        });
        StopAnimation();
    });
    
    document.getElementById('colorAutomata').addEventListener('change', function () {
        mainColor = this.value;
    });
});

function StopAnimation(){
    clearInterval(animationInterval);
}

function StartAnimation(){
    // Local animation of cells in grid with JS
    animationInterval = setInterval(() => {
        // Set Counter
        genCount++;
        genCountElement.innerHTML = genCount;

        // run animation
        runCellularAutomata();
        
    }, timeout); // timeout between frames, defines the speed of the animation
}

function runCellularAutomata(){
    // Iterate through the grid
    for (let i = 0; i < grid.length; i++) {
        for (let j = 0; j < grid[i].length; j++) {
            let cell = document.getElementById('cell_' + i + "/" + j);
            let currentColor = grid[i][j];
            let neighbors = 0;
            let neighborsColor = [];
            // Count neighbors
            for (let x = -1; x < 2; x++) {
                for (let y = -1; y < 2; y++) {
                    if (i + x >= 0 && i + x < grid.length && j + y >= 0 && j + y < grid[i].length) {
                        if (grid[i + x][j + y] !== baseColor) {
                            neighbors++;
                            neighborsColor.push(grid[i + x][j + y]);
                        }
                    }
                }
            }
            // Select top color for growing new Cells
            let topColor = neighborsColor.length > 0
                ? neighborsColor.reduce((a, b) => neighborsColor.filter(v => v === a).length >= neighborsColor.filter(v => v === b).length ? a : b)
                : mainColor;  // Replace defaultColor with the color you want to use in case of no neighbors
            
            // Apply rules
            // If Cell is living and has 3 neighbours, kill it
            if (currentColor !== baseColor) {
                if (neighbors < 2 || neighbors > 3) {
                    grid[i][j] = baseColor;
                }
            // If Cell is dead and has correct number of neighbours, bring to life
            } else {
                if (neighbors === 3) {
                    grid[i][j] = topColor;
                }
            }
            // Update cell color
            if (currentColor !== grid[i][j]) {
                cell.style.backgroundColor = grid[i][j];
            }
            
        }
    }
    // Count all the colors present in the grid and calculate its percentage
    var colorCount = {};
    var filledCount = 0;
    for (let i = 0; i < grid.length; i++) {
        for (let j = 0; j < grid[i].length; j++) {
            if (grid[i][j] === baseColor) continue;
            if (grid[i][j] in colorCount) {
                colorCount[grid[i][j]]++;
                filledCount++;
            } else {
                colorCount[grid[i][j]] = 1;
                filledCount++;
            }
        }
    }
    // Convert each cell to a percentage base = 2500 cells
    for (let color in colorCount) {
        // trim the percentage to 1 decimal places
        colorCount[color] = (colorCount[color] / filledCount * 100).toFixed(1);
    }
    
    // display the values as bar graphs in the browser
    var colorBars = document.getElementById("colorBarContainer");
    // add the color bars to the container
    colorBars.innerHTML = "";
    let barWidth = 80 / Object.keys(colorCount).length;
    for (let color in colorCount) {
        let colorBar = document.createElement("div");
        colorBar.style.width = barWidth + "%";
        colorBar.style.height = colorCount[color] + "%"; // scale the height as the board will never fill above 50%
        colorBar.style.backgroundColor = color;
        colorBar.innerHTML = colorCount[color] + "%";
        colorBars.appendChild(colorBar);
    }
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
