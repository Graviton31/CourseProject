// Функция для загрузки ролей из API
async function loadRoles() {
    try {
        const response = await fetch('https://localhost:7022/api/users/roles'); // Замените на ваш URL
        if (response.ok) {
            const roles = await response.json();
            const roleSelect = document.getElementById('role');
            roles.forEach(role => {
                const option = document.createElement('option');
                option.value = role;
                option.textContent = role.charAt(0).toUpperCase() + role.slice(1); // Форматируем текст
                roleSelect.appendChild(option);
            });
        } else {
            console.error('Error fetching roles:', response.statusText);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

// Загружаем роли при загрузке страницы
window.onload = loadRoles;

document.getElementById('createUserForm').addEventListener('submit', async function (event) {
    event.preventDefault(); // Предотвращаем стандартное поведение формы

    const formData = {
        UserDto: {
            Surname: document.getElementById('surname').value,
            Name: document.getElementById('name').value,
            Patronymic: document.getElementById('patronymic').value,
            Login: document.getElementById('login').value,
            PasswordString: document.getElementById('password').value,
            Phone: document.getElementById('phone').value,
            BirthDate: document.getElementById('birthdate').value || null,
            Role: document.getElementById('role').value
        }
    };

    console.log(formData)

    // Дополнительные проверки данных
    const validationErrors = [];

    // Проверка формата телефона (пример: только цифры, длина 10-15)
    const phoneRegex = /^\+?\d\s?\d{3}\s?\d{3}[-]?\d{2}[-]?\d{2}$/;
    if (formData.UserDto.Phone && !phoneRegex.test(formData.UserDto.Phone)) {
        validationErrors.push('Неверный формат номера телефона.');
    }

    // Проверка даты рождения (должна быть в прошлом)
    const currentDate = new Date();
    if (formData.UserDto.BirthDate) {
        const birthDate = new Date(formData.UserDto.BirthDate);
        if (birthDate > currentDate) {
            validationErrors.push('Дата рождения не может быть в будущем.');
        }
    }

    // Проверка длины пароля (например, минимум 6 символов)
    if (formData.UserDto.PasswordString && formData.UserDto.PasswordString.length < 6) {
        validationErrors.push('Пароль должен содержать минимум 6 символов.');
    }

    // Если есть ошибки валидации, выводим их и прекращаем отправку формы
    if (validationErrors.length > 0) {
        showAlert(validationErrors.join(' '), 'error');
        return;
    }

    console.log('Отправляемые данные формы:', JSON.stringify(formData.UserDto));

    try {
        const response = await fetch('https://localhost:7022/api/Users/PostUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData.UserDto)
        });

        // Логируем ответ
        console.log('Response status:', response.status);

        if (response.ok) {
            const result = await response.json();
            showAlert(result.Message || 'Пользователь успешно создан.', 'success');
            document.getElementById('createUserForm').reset();
        } else {
            const errorResponse = await response.json();
            console.error('Ошибка от сервера:', errorResponse);
            showAlert(errorResponse.Message || errorResponse.message, 'error');
        }
    }
    catch (error) {
        console.error('Error:', error);
        showAlert('При создании пользователя произошла ошибка.', 'error');
    }
});

// Функция для отображения уведомлений
function showAlert(message, type) {
    const alertBox = document.getElementById('alert');
    alertBox.textContent = message;
    alertBox.className = 'alert ' + (type === 'success' ? 'alert-success' : 'alert-error');
    alertBox.style.display = 'block'; // Показываем уведомление
    // Скрываем уведомление через 10 секунд
    setTimeout(() => {
        alertBox.style.display = 'none';
    }, 10000);
}