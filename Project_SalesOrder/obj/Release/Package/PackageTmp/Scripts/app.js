
document.addEventListener("DOMContentLoaded", function () {
    const rowsPerPage = 10; // Jumlah baris per halaman
    const table = document.querySelector("table");
    const tbody = table.querySelector("tbody");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const paginationInfo = document.getElementById("pagination-info");
    const paginationElement = document.getElementById("pagination");
    const totalItems = rows.length;

    function renderTable(page = 1) {
        const startIdx = (page - 1) * rowsPerPage;
        const endIdx = Math.min(page * rowsPerPage, totalItems);

        rows.forEach((row, index) => {
            row.style.display = index >= startIdx && index < endIdx ? "" : "none";
        });

        // Perbarui informasi pagination
        paginationInfo.textContent = `${startIdx + 1} - ${endIdx} of ${totalItems} items`;

        // Perbarui tombol pagination
        renderPagination(page);
    }

    function renderPagination(currentPage) {
        const totalPages = Math.ceil(totalItems / rowsPerPage);
        let paginationHTML = "";

        // Tombol pertama dan sebelumnya
        paginationHTML += `
            <button class="pagination-btn" data-page="1" ${currentPage === 1 ? "disabled" : ""}>&laquo;</button>
            <button class="pagination-btn" data-page="${currentPage - 1}" ${currentPage === 1 ? "disabled" : ""}>&lsaquo;</button>
        `;

        // Tombol halaman
        for (let i = 1; i <= totalPages; i++) {
            paginationHTML += `
                <button class="pagination-btn ${i === currentPage ? "active" : ""}" data-page="${i}">${i}</button>
            `;
        }

        // Tombol berikutnya dan terakhir
        paginationHTML += `
            <button class="pagination-btn" data-page="${currentPage + 1}" ${currentPage === totalPages ? "disabled" : ""}>&rsaquo;</button>
            <button class="pagination-btn" data-page="${totalPages}" ${currentPage === totalPages ? "disabled" : ""}>&raquo;</button>
        `;

        paginationElement.innerHTML = paginationHTML;

        // Tambahkan event listener untuk tombol pagination
        paginationElement.querySelectorAll(".pagination-btn").forEach(button => {
            button.addEventListener("click", function () {
                const page = parseInt(this.getAttribute("data-page"));
                renderTable(page);
            });
        });
    }

    // Inisialisasi tabel di halaman pertama
    renderTable(1);
});


// Section toggle
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".section h3").forEach(function (header) {
        header.addEventListener("click", function () {
            const content = this.nextElementSibling;
            content.style.display = content.style.display === "grid" || !content.style.display ? "none" : "grid";
            const icon = this.querySelector(".toggle-icon");
            icon.classList.toggle("fa-chevron-down");
            icon.classList.toggle("fa-chevron-up");
        });
    });
});
