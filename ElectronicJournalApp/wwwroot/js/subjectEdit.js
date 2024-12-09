﻿document.addEventListener('DOMContentLoaded', async function () {
    const subjectId = document.getElementById('subjectId').value;

    // Загрузка групп и пользователей при загрузке страницы
    await loadGroups(subjectId);

    document.getElementById('editSubjectForm').addEventListener('submit', async function (event) {
        event.preventDefault(); // Предотвращаем стандартное поведение формы

        const formData = {
            IdSubject: subjectId,
            Name: document.getElementById('name').value,
            FullName: document.getElementById('fullName').value,
            Description: document.getElementById('description').value,
            Duration: parseInt(document.getElementById('duration').value),
            LessonLenght: parseInt(document.getElementById('lessonLength').value),
            LessonsCount: parseInt(document.getElementById('lessonsCount').value),
            IsDelete: false,
            Groups: [] // Массив для групп
        };

        // Собираем данные о группах
        const groupElements = document.querySelectorAll('.group');
        groupElements.forEach(groupElement => {
            const groupName = groupElement.querySelector('.groupName').value;
            const studentCount = parseInt(groupElement.querySelector('.studentCount').value);
            const classroom = groupElement.querySelector('.classroom').value;
            const userId = groupElement.querySelector('.userSelect').value;
            formData.Groups.push({ Name: groupName, StudentCount: studentCount, Classroom: classroom, UserId: userId });
        });

        // Дополнительные проверки данных
        const validationErrors = validateForm(formData);
        if (validationErrors.length > 0) {
            showAlert(validationErrors.join(' '), 'error');
            return;
        }

        try {
            const response = await fetch(`https://localhost:7022/api/Subjects/UpdateSubject`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const result = await response.json();
                showAlert(result.Message || 'Предмет успешно обновлен.', 'success');
                document.getElementById('editSubjectForm').reset();
            } else {
                const errorResponse = await response.json();
                showAlert(errorResponse.Message || errorResponse.message, 'error');
            }
        } catch (error) {
            showAlert('При обновлении предмета произошла ошибка.', 'error');
        }
    });

    document.getElementById('addGroupButton').addEventListener('click', function () {
        addGroupField();
    });
});

// Функция для добавления поля группы
function addGroupField(groupName = '', studentCount = '', classroom = '', userId = '') {
    const groupsContainer = document.getElementById('groupsContainer');
    const groupDiv = document.createElement('div');
    groupDiv.className = 'group mb-2';

    groupDiv.innerHTML = `
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Название группы <span class="required">*</span></label>
            <input type="text" class="groupName appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${groupName}" required>
        </div>
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Количество студентов</label>
            <input type="number" class="studentCount appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${studentCount}">
        </div>
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Класс</label>
            <input type="text" class="classroom appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${classroom}">
        </div>
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Учитель</label>
            <select class="userSelect appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none">
                <option value="">Выберите учителя</option>
                <!-- Здесь будут динамически загружены пользователи -->
            </select>
        </div>
    `;

    groupsContainer.appendChild(groupDiv);
    loadUsers(groupDiv.querySelector('.userSelect'), userId);
}

// Функция для загрузки пользователей
async function loadUsers(selectElement, selectedUserId = '') {
    try {
        const response = await fetch('https://localhost:7022/api/Users'); // Предполагается, что у вас есть API для получения пользователей
        const users = await response.json();
        users.forEach(user => {
            const option = document.createElement('option');
            option.value = user.IdUsers;
            option.textContent = `${user.Surname} ${user.Name}`;
            if (user.IdUsers === selectedUserId) {
                option.selected = true; // Устанавливаем выбранного пользователя
            }
            selectElement.appendChild(option);
        });
    } catch (error) {
        console.error('Ошибка при загрузке пользователей:', error);
    }
}

// Функция для валидации формы
function validateForm(formData) {
    const errors = [];

    // Проверка на наличие названия предмета
    if (!formData.Name) {
        errors.push('Название предмета обязательно для заполнения.');
    }

    // Проверка на положительные значения для длительности, академических часов и количества уроков
    if (formData.Duration <= 0) {
        errors.push('Длительность должна быть положительным числом.');
    }
    if (formData.LessonLenght <= 0) {
        errors.push('Количество академических часов должно быть положительным числом.');
    }
    if (formData.LessonsCount <= 0) {
        errors.push('Количество уроков должно быть положительным числом.');
    }

    // Проверка на наличие хотя бы одной группы
    if (formData.Groups.length === 0) {
        errors.push('Необходимо добавить хотя бы одну группу.');
    } else {
        formData.Groups.forEach((group, index) => {
            if (!group.Name) {
                errors.push(`Название группы ${index + 1} обязательно для заполнения.`);
            }
            if (group.StudentCount < 0) {
                errors.push(`Количество студентов для группы ${index + 1} должно быть неотрицательным.`);
            }
            if (!group.Classroom) {
                errors.push(`Класс для группы ${index + 1} обязателен для заполнения.`);
            }
            if (!group.UserId) {
                errors.push(`Учитель для группы ${index + 1} не выбран.`);
            }
        });
    }

    return errors;
}

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

// Функция для загрузки групп при загрузке страницы
async function loadGroups(subjectId) {
    try {
        const response = await fetch(`https://localhost:7022/api/Subjects/${subjectId}/Groups`); // Предполагается, что у вас есть API для получения групп
        const groups = await response.json();

        // Проверяем, есть ли группы
        if (groups.length > 0) {
            groups.forEach(group => {
                addGroupField(group.Name, group.StudentCount, group.Classroom, group.IdUsers);
            });
        } else {
            // Если групп нет, можно добавить одно поле для добавления новой группы
            addGroupField();
        }
    } catch (error) {
        console.error('Ошибка при загрузке групп:', error);
    }
}