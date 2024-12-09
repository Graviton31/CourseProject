function toggleCollapse(id) {
    var element = document.getElementById(id);
    var icon = document.getElementById(id + 'Icon');
    if (element.classList.contains('collapse')) {
        element.classList.remove('collapse');
        icon.classList.remove('fa-chevron-down');
        icon.classList.add('fa-chevron-up');
    } else {
        element.classList.add('collapse');
        icon.classList.remove('fa-chevron-up');
        icon.classList.add('fa-chevron-down');
    }
}

function deleteUser(userId) {
    fetch(`https://localhost:7022/api/Users/DeleteUser/${userId}`, {
        method: 'DELETE'
    })
        .then(response => {
            console.log(userId);
            if (response.ok) {
                // Удаляем строку из таблицы
                const userRow = document.getElementById(`user-row-${userId}`);
                if (userRow) {
                    userRow.remove(); // Удаляем строку из DOM
                }
            } else if (response.status === 404) {
                console.error('Пользователь не найден');
                alert('Пользователь не найден'); // Уведомление для пользователя
            } else {
                console.error('Ошибка при удалении пользователя');
                alert('Ошибка при удалении пользователя'); // Уведомление для пользователя
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при удалении пользователя'); // Уведомление для пользователя
        });
}