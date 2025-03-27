const initPopovers = () => {
    document.querySelectorAll('[data-bs-toggle="popover"]').forEach(el => {
        new bootstrap.Popover(el);
    });
};

const initDataTables = () => {
    document.querySelectorAll("table.datatable").forEach(table => {
        $(table).DataTable({
            paging: true,
            searching: true,
            ordering: true,
            responsive: true,
            lengthMenu: [5, 10, 25, 50, 100]
        });
    });
};

document.addEventListener('DOMContentLoaded', () => {
    initPopovers();
    initDataTables();
});