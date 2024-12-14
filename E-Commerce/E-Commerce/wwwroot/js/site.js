// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
 
const carousel = document.querySelector('#carouselExample');
let interval = setInterval(() => {
    $('#carouselExample').carousel('next');
}, 3000);

// Pause on hover
carousel.addEventListener('mouseenter', () => clearInterval(interval));
carousel.addEventListener('mouseleave', () => {
    interval = setInterval(() => {
        $('#carouselExample').carousel('next');
    }, 3000);
});

