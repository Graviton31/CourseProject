// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        const cookieValue = decodeURIComponent(parts.pop().split(';').shift());
        console.log(`Cookie ${name}: ${cookieValue}`);
        return cookieValue;
    }
    console.log(`Cookie ${name} не найден`);
    return null;
}
