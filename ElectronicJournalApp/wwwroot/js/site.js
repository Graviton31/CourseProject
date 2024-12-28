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

function checkToken() {
    // Проверка наличия токена
    if (!localStorage.getItem('token')) {
        window.location.href = '/Authorization'; // Перенаправляем на страницу входа
    } else {
        // Дополнительно: можно проверить срок действия токена
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split('.')[1])); // Декодируем токен для получения полезной нагрузки
        const exp = payload.exp * 1000; // Время истечения в миллисекундах

        if (Date.now() >= exp) {
            alert("Ваш токен истек. Пожалуйста, войдите снова.");
            localStorage.removeItem('token'); // Удаляем истекший токен
            window.location.href = '/Authorization'; // Перенаправляем на страницу входа
        }
    }
}
