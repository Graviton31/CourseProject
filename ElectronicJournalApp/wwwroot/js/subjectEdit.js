document.addEventListener('DOMContentLoaded', async function () {
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
            Groups: [] // Массив для групп
        };

        // Собираем данные о группах
        const groupElements = document.querySelectorAll('.group');
        groupElements.forEach(groupElement => {
            const groupName = groupElement.querySelector('.groupName').value;
            const studentCount = parseInt(groupElement.querySelector('.studentCount').value);
            const classroom = groupElement.querySelector('.classroom').value;
            const userId = groupElement.querySelector('.userSelect').value;
            console.log("сбор 1", groupElement.querySelector('.idGroup').value)
            const idGroup = groupElement.querySelector('.idGroup').value; // Получаем IdGroup
            console.log("сбор 2", groupElement.querySelector('.idGroup').value)
            // Создаем объект группы
            const groupData = {
                Name: groupName,
                StudentCount: studentCount,
                Classroom: classroom,
                IdUsers: userId,
                IdGroup: idGroup !== 0 ? idGroup : null // Устанавливаем IdGroup в null, если оно равно 0
            };
            console.log(groupData)
            formData.Groups.push(groupData);
        });


        console.log(formData)
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

function addGroupField(groupName = '', studentCount = '', classroom = '', userId = '', idGroup = 0) {
    console.log(idGroup)
    const groupsContainer = document.getElementById('groupsContainer');
    const groupDiv = document.createElement('div');
    groupDiv.className = 'group mb-2';
    groupDiv.innerHTML = `
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Название группы <span class="required">*</span></label>
            <input type="text" class="groupName appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${groupName}" required>
        </div>
        <div class="mb-2">
                        <label class="block text-secondary font-semibold mb-1">Количество студентов<span class="required">*</span></label>
            <input type="number" class="studentCount appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${studentCount}" required>
        </div>
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Класс<span class="required">*</span></label>
            <input type="text" class="classroom appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none" value="${classroom}" required>
        </div>
        <div class="mb-2">
            <label class="block text-secondary font-semibold mb-1">Учитель<span class="required">*</span></label>
            <select class="userSelect appearance-none border rounded w-full py-2 px-3 text-secondary leading-tight focus:outline-none">
                <option value="">Выберите учителя</option>
                <!-- Здесь будут динамически загружены пользователи -->
            </select>
        </div>
        <input type="hidden" class="idGroup" value="${idGroup}"> <!-- Скрытое поле для IdGroup -->
    `;

    groupsContainer.appendChild(groupDiv);
    loadUsers(groupDiv.querySelector('.userSelect'), userId);

    // Показываем заголовок, если он скрыт
    if (document.getElementById('groupsHeader').style.display === 'none') {
        document.getElementById('groupsHeader').style.display = 'block';
    }
}

// Функция для загрузки пользователей
async function loadUsers(selectElement, selectedUserId = '') {
    try {
        const response = await fetch('https://localhost:7022/api/Users/teachers'); // Предполагается, что у вас есть API для получения пользователей
        const users = await response.json();
        users.forEach(user => {
            const option = document.createElement('option');
            option.value = user.idUsers;
            if ((user.surname === null && user.name === null) || (user.name === "" && user.surname === "")) {
                option.textContent = user.login; // Выводим логин пользователя, если имя и фамилия равны null или ""
            } else {
                option.textContent = `${user.surname} ${user.name} ${user.patronymic}`;
            }
            if (user.idUsers === selectedUserId) {
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

    // Проверка на наличие группы
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
            if (!group.IdUsers) {
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
        const response = await fetch(`https://localhost:7022/api/Subjects/${subjectId}/Groups`);
        const groups = await response.json();

        // Проверяем, есть ли группы
        if (groups.length > 0) {
            document.getElementById('groupsHeader').style.display = 'block'; // Показываем заголовок
            groups.forEach(group => {
                addGroupField(group.name, group.studentCount, group.classroom, group.idUsers, group.idGroup);
                console.log(group.idGroup)
            });
        } else {
            document.getElementById('groupsHeader').style.display = 'none'; // Скрываем заголовок, если групп нет
        }
    } catch (error) {
        console.error('Ошибка при загрузке групп:', error);
        showAlert('Не удалось загрузить группы. Пожалуйста, попробуйте позже.', 'error');
    }
}


