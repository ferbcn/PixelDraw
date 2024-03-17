// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

let lastColor;
let highColor = '#AAAAAA'; // lightgrey

let allColors = ["#b22222", "#006400", "#00008b"];  // red, green, blue

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
    lastColor = hoverCell.style.backgroundColor;
    lastColorHex = rgbToHex(hoverCell.style.backgroundColor);
    if (!allColors.includes(lastColorHex)) {
        lastColor = "rgb(255, 255, 255)";
    }
    hoverCell.style.backgroundColor = highColor;
}

function leaveCell(hoverCell) {
    //hoverCell.style.backgroundColor = lastColor;
    transitionColor(hoverCell, highColor, rgbToHex(lastColor), 500);
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