document.getElementById('createSubjectForm').addEventListener('submit', async function (event) {
    event.preventDefault(); // Предотвращаем стандартное поведение формы

    const formData = {
        Name: document.getElementById('name').value,
        FullName: document.getElementById('fullName').value,
        Description: document.getElementById('description').value,
        Duration: parseInt(document.getElementById('duration').value),
        LessonLenght: parseInt(document.getElementById('lessonLength').value),
        LessonsCount: parseInt(document.getElementById('lessonsCount').value),
        IsDelete: false // По умолчанию устанавливаем значение IsDelete в false
    };

    console.log(formData);

    // Дополнительные проверки данных
    const validationErrors = [];

    // Проверка на наличие названия предмета
    if (!formData.Name) {
        validationErrors.push('Название предмета обязательно для заполнения.');
    }

    // Проверка на положительные значения для длительности, академических часов и количества уроков
    if (formData.Duration <= 0) {
        validationErrors.push('Длительность должна быть положительным числом.');
    }
    if (formData.LessonLenght <= 0) {
        validationErrors.push('Количество академических часов должно быть положительным числом.');
    }
    if (formData.LessonsCount <= 0) {
        validationErrors.push('Количество уроков должно быть положительным числом.');
    }

    // Если есть ошибки валидации, выводим их и прекращаем отправку формы
    if (validationErrors.length > 0) {
        showAlert(validationErrors.join(' '), 'error');
        return;
    }

    console.log('Отправляемые данные формы:', JSON.stringify(formData));

    try {
        const response = await fetch('https://localhost:7022/api/Subjects/PostSubject', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        // Логируем ответ
        console.log('Response status:', response.status);

        if (response.ok) {
            const result = await response.json();
            showAlert(result.Message || 'Предмет успешно создан.', 'success');
            document.getElementById('createSubjectForm').reset();
        } else {
            const errorResponse = await response.json();
            console.error('Ошибка от сервера:', errorResponse);
            showAlert(errorResponse.Message || errorResponse.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('При создании предмета произошла ошибка.', 'error');
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
