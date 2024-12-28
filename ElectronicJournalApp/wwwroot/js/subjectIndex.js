const userRole = getCookie("role");
console.log("userRole", userRole);

const editButtons = document.querySelectorAll('.editButton');
const deleteButtons = document.querySelectorAll('.deleteButton');

if (userRole === 'руководитель' || userRole === 'администратор') {
    document.getElementById('SubjectAddButton').style.display = 'block';
    document.getElementById('SchedulesEditButton').style.display = 'block';
    editButtons.forEach(button => button.style.display = 'block');
    deleteButtons.forEach(button => button.style.display = 'block');
} else {
    document.getElementById('SubjectAddButton').style.display = 'none';
    document.getElementById('SchedulesEditButton').style.display = 'none';
    editButtons.forEach(button => button.style.display = 'none');
    deleteButtons.forEach(button => button.style.display = 'none');
}


function deleteSubject(subjectId) {
    console.log(`Deleting subject with ID: ${subjectId}`);
    fetch(`https://localhost:7022/api/Subjects/DeleteSubject/${subjectId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                const subjectRow = document.getElementById(`subject-row-${subjectId}`);
                if (subjectRow) {
                    subjectRow.remove(); 
                }
            } else if (response.status === 404) {
                alert('Предмет не найден');
            } else {
                alert('Ошибка при удалении предмета');
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при удалении предмета');
        });
}