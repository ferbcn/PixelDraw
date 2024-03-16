// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let currentColor;

function enterCell(hoverCell) {
    currentColor = hoverCell.style.backgroundColor;
    hoverCell.style.backgroundColor = "lightgrey";
}

function leaveCell(hoverCell) {
    hoverCell.style.backgroundColor = currentColor;
}

document.onload = function () {
    document.querySelectorAll('.cell').forEach(cell => {
        cell.addEventListener('mouseenter', function () {
            enterCell(this);
        });
        cell.addEventListener('mouseleave', function () {
            leaveCell(this);
        });
    });
}