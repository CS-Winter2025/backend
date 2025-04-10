function addDetail() {
    const container = document.getElementById('details-container');
    const div = document.createElement('div');
    div.className = 'form-group d-flex gap-2';
    div.innerHTML = `
        <input type="text" class="form-control detail-key" placeholder="Key" />
        <input type="text" class="form-control detail-value" placeholder="Value" />
    `;
    container.appendChild(div);
}

function serializeDetails() {
    const keys = document.querySelectorAll('.detail-key');
    const values = document.querySelectorAll('.detail-value');
    const data = {};
    for (let i = 0; i < keys.length; i++) {
        const key = keys[i].value.trim();
        const value = values[i].value.trim();
        if (key !== '') data[key] = value;
    }
    document.getElementById('DetailsJson').value = JSON.stringify(data);
    return true;
}
