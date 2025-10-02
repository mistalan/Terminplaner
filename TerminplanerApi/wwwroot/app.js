const API_URL = '/api/appointments';
let appointments = [];
let draggedItem = null;
let editingId = null;

// Load appointments on page load
document.addEventListener('DOMContentLoaded', () => {
    loadAppointments();
    
    // Add enter key support for input fields
    document.getElementById('appointmentText').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') addAppointment();
    });
    document.getElementById('appointmentCategory').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') addAppointment();
    });
});

async function loadAppointments() {
    try {
        const response = await fetch(API_URL);
        appointments = await response.json();
        renderAppointments();
    } catch (error) {
        console.error('Fehler beim Laden der Termine:', error);
        showError('Termine konnten nicht geladen werden.');
    }
}

function renderAppointments() {
    const list = document.getElementById('appointmentsList');
    
    if (appointments.length === 0) {
        list.innerHTML = `
            <div class="empty-state">
                <div class="emoji">📝</div>
                <p><strong>Noch keine Termine vorhanden!</strong><br>
                Füge deinen ersten Termin hinzu, um loszulegen.</p>
            </div>
        `;
        return;
    }
    
    list.innerHTML = appointments.map((appointment, index) => `
        <div class="appointment-item" 
             data-id="${appointment.id}"
             draggable="true"
             ondragstart="handleDragStart(event)"
             ondragover="handleDragOver(event)"
             ondrop="handleDrop(event)"
             ondragend="handleDragEnd(event)">
            <div class="drag-handle">⋮⋮</div>
            <div class="color-indicator" style="background-color: ${appointment.color}"></div>
            <div class="appointment-content">
                <div class="appointment-text">${escapeHtml(appointment.text)}</div>
                <div class="appointment-category">🏷️ ${escapeHtml(appointment.category)}</div>
                <div class="appointment-priority">Priorität: ${appointment.priority}</div>
            </div>
            <div class="appointment-actions">
                <button class="edit-btn" onclick="editAppointment(${appointment.id})">✏️ Bearbeiten</button>
                <button class="delete-btn" onclick="deleteAppointment(${appointment.id})">🗑️ Löschen</button>
            </div>
        </div>
    `).join('');
}

async function addAppointment() {
    const text = document.getElementById('appointmentText').value.trim();
    const category = document.getElementById('appointmentCategory').value.trim() || 'Standard';
    const color = document.getElementById('appointmentColor').value;
    
    if (!text) {
        alert('Bitte gib einen Termin ein!');
        return;
    }
    
    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ text, category, color, priority: 0 })
        });
        
        if (response.ok) {
            document.getElementById('appointmentText').value = '';
            document.getElementById('appointmentCategory').value = '';
            document.getElementById('appointmentColor').value = '#808080';
            await loadAppointments();
        }
    } catch (error) {
        console.error('Fehler beim Hinzufügen:', error);
        showError('Termin konnte nicht hinzugefügt werden.');
    }
}

function editAppointment(id) {
    const appointment = appointments.find(a => a.id === id);
    if (!appointment) return;
    
    editingId = id;
    
    // Create modal
    const modal = document.createElement('div');
    modal.className = 'modal active';
    modal.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">Termin bearbeiten</div>
            <div class="modal-form">
                <input type="text" id="editText" value="${escapeHtml(appointment.text)}" placeholder="Termin">
                <input type="text" id="editCategory" value="${escapeHtml(appointment.category)}" placeholder="Kategorie">
                <input type="color" id="editColor" value="${appointment.color}">
            </div>
            <div class="modal-actions">
                <button class="save-btn" onclick="saveEdit()">💾 Speichern</button>
                <button class="cancel-btn" onclick="closeModal()">❌ Abbrechen</button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    document.getElementById('editText').focus();
}

async function saveEdit() {
    const text = document.getElementById('editText').value.trim();
    const category = document.getElementById('editCategory').value.trim() || 'Standard';
    const color = document.getElementById('editColor').value;
    
    if (!text) {
        alert('Bitte gib einen Termin ein!');
        return;
    }
    
    const appointment = appointments.find(a => a.id === editingId);
    
    try {
        const response = await fetch(`${API_URL}/${editingId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                text, 
                category, 
                color, 
                priority: appointment.priority 
            })
        });
        
        if (response.ok) {
            closeModal();
            await loadAppointments();
        }
    } catch (error) {
        console.error('Fehler beim Bearbeiten:', error);
        showError('Termin konnte nicht bearbeitet werden.');
    }
}

function closeModal() {
    const modal = document.querySelector('.modal');
    if (modal) modal.remove();
    editingId = null;
}

async function deleteAppointment(id) {
    if (!confirm('Möchtest du diesen Termin wirklich löschen?')) return;
    
    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            await loadAppointments();
        }
    } catch (error) {
        console.error('Fehler beim Löschen:', error);
        showError('Termin konnte nicht gelöscht werden.');
    }
}

// Drag and Drop handlers
function handleDragStart(e) {
    draggedItem = e.target;
    e.target.classList.add('dragging');
    e.dataTransfer.effectAllowed = 'move';
}

function handleDragOver(e) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    
    const target = e.target.closest('.appointment-item');
    if (!target || target === draggedItem) return;
    
    const list = document.getElementById('appointmentsList');
    const items = [...list.querySelectorAll('.appointment-item')];
    const draggedIndex = items.indexOf(draggedItem);
    const targetIndex = items.indexOf(target);
    
    if (draggedIndex < targetIndex) {
        target.parentNode.insertBefore(draggedItem, target.nextSibling);
    } else {
        target.parentNode.insertBefore(draggedItem, target);
    }
}

function handleDrop(e) {
    e.preventDefault();
}

async function handleDragEnd(e) {
    e.target.classList.remove('dragging');
    
    // Update priorities based on new order
    const items = document.querySelectorAll('.appointment-item');
    const priorities = {};
    
    items.forEach((item, index) => {
        const id = parseInt(item.dataset.id);
        priorities[id] = index + 1;
    });
    
    try {
        await fetch(`${API_URL}/priorities`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(priorities)
        });
        
        await loadAppointments();
    } catch (error) {
        console.error('Fehler beim Aktualisieren der Prioritäten:', error);
        showError('Prioritäten konnten nicht aktualisiert werden.');
    }
    
    draggedItem = null;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function showError(message) {
    alert('❌ ' + message);
}
