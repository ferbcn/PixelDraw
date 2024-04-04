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
            // Count neighbors
            for (let x = -1; x < 2; x++) {
                for (let y = -1; y < 2; y++) {
                    if (i + x >= 0 && i + x < grid.length && j + y >= 0 && j + y < grid[i].length) {
                        if (grid[i + x][j + y] === mainColor) {
                            neighbors++;
                        }
                    }
                }
            }
            // Apply rules
            if (currentColor === mainColor) {
                if (neighbors < 2 || neighbors > 3) {
                    grid[i][j] = baseColor;
                }
            } else {
                if (neighbors === 3) {
                    grid[i][j] = mainColor;
                }
            }
            // Update cell color
            if (currentColor !== grid[i][j]) {
                cell.style.backgroundColor = grid[i][j];
            }
        }
    }
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