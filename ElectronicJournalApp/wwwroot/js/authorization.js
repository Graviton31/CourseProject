$(document).ready(function () {
    // Проверка, если пользователь уже авторизован
    if (document.cookie.includes("UserPassword")) {
        console.log(getCookie("UserRole"))
        // Переход в зависимости от роли
        const role = getCookie("UserRole");
        if (role === "администратор") {
            window.location.href = '/Users/Index';
        } else if (role === "руководитель") {
            window.location.href = 'Home/Index';
        } else {
            window.location.href = 'AttendanceJournal/Index';
        }
    }

    $('#togglePassword').click(function () {
        const passwordField = $('#Password');
        const passwordFieldType = passwordField.attr('type');

        if (passwordFieldType === 'password') {
            passwordField.attr('type', 'text');
            $(this).text('Скрыть');
        } else {
            passwordField.attr('type', 'password');
            $(this).text('Показать');
        }
    });

    $('#Authorization').submit(function (event) {
        event.preventDefault(); // Предотвращаем стандартное поведение формы

        var user = {
            Login: $('#Login').val(),
            Password: $('#Password').val()
        };

        // Проверка, если пользователь уже авторизован
        if (document.cookie.includes("UserPassword")) {
            alert("Вы уже авторизованы.");
            return; // Прекращаем выполнение, если пользователь уже авторизован

        }

        $.ajax({
            url: 'https://localhost:7022/api/Users/Authorization', // URL вашего API
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(user),
            success: function (response) {

                // Переход в зависимости от роли
                if (response.role === "администратор") {
                    window.location.href = '/Users/Index'; // Переход для администраторов
                } else if (response.role === "руководитель") {
                    window.location.href = 'Home/Index'; // Переход для обычных пользователей
                } else {
                    window.location.href = 'AttendanceJournal/Index'; // Переход по умолчанию
                }

                // Создание cookie, если пользователь выбрал "Запомнить меня"
                if ($('#rememberMe').is(':checked')) {
                    // Устанавливаем Cookie на 7 дней
                    // Кодирование значений cookie
                    document.cookie = "UserLogin=" + encodeURIComponent(user.Login) + "; path=/; max-age=" + (7 * 24 * 60 * 60) + "; HttpOnly; Secure"; // 7 дней
                    document.cookie = "UserPassword=" + encodeURIComponent(user.Password) + "; path=/; max-age=" + (7 * 24 * 60 * 60) + "; HttpOnly; Secure";
                    document.cookie = "UserRole=" + encodeURIComponent(response.role) + "; path=/; max-age=" + (7 * 24 * 60 * 60) + "; HttpOnly; Secure";

                } else {
                    // Устанавливаем сессионные Cookie (удаляются при закрытии браузера)
                    document.cookie = "UserLogin=" + encodeURIComponent(user.Login) + "; path=/"; // Сессионный Cookie
                    document.cookie = "UserRole=" + encodeURIComponent(response.role) + "; path=/"; // Сессионный Cookie
                }
            },
            error: function (xhr) {
                var message = xhr.responseJSON ? xhr.responseJSON.message : "Произошла ошибка на сервере";
                alert(message); // Показываем сообщение об ошибке
            }
        });
    });
});