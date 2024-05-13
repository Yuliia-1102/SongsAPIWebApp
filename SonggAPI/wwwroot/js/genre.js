document.addEventListener('DOMContentLoaded', function () {
    fetchGenres();

    const form = document.getElementById('addGenreForm');
    form.onsubmit = async function (event) {
        event.preventDefault();

        const genreName = document.getElementById('newGenreName').value;
        addGenre(genreName);
    };
});

async function fetchGenres() {
    try {
        const response = await fetch('/api/genres');
        const genres = await response.json();

        const genreList = document.getElementById('genreList');
        genreList.innerHTML = '';

        genres.forEach(genre => {
            const li = document.createElement('li'); // пункт списку
            li.textContent = genre.name;
            li.appendChild(createEditButton(genre));
            li.appendChild(createDeleteButton(genre.id));
            genreList.appendChild(li);
        });
    }
    catch (error) {
        console.error('Failed to fetch genres:', error);
    }
}

async function addGenre(name) {
    try {
        const response = await fetch('/api/genres', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: name })
        });
        if (response.ok) {
            fetchGenres();
        } else {
            alert('Такий жанр вже існує.');
        }
    }
    catch (error) {
        console.error('Error adding genre:', error);
    }
}

function createEditButton(genre) {
    const button = document.createElement('button');
    button.textContent = 'Редагувати';
    button.onclick = function () { editGenre(genre); };
    return button;
}

function createDeleteButton(genreId) {
    const button = document.createElement('button');
    button.textContent = 'Видалити';
    button.onclick = function () { deleteGenre(genreId); };
    return button;
}

function editGenre(genre) {
    const newName = prompt('Введіть нову назву жанру:', genre.name);
    if (newName && newName !== genre.name) {
        updateGenre(genre.id, { id: genre.id, name: newName }); 
    }
}

async function updateGenre(genreId, genre) {

    try {
        const response = await fetch(`/api/genres/${genreId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(genre)
        });

        if (response.ok) {
            fetchGenres();
        } else {
            const errorText = await response.text();
            alert('Не вдалося оновити жанр.');
        }
    } catch (error) {
        console.error('Error updating genre:', error);
    }
}

async function deleteGenre(genreId) {
    try {
        const response = await fetch(`/api/genres/${genreId}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            fetchGenres();
        }
        else {
            alert('Не вдалося видалити жанр.');
        }
    } catch (error) {
        console.error('Error deleting genre:', error);
    }
}
