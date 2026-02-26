document.addEventListener("DOMContentLoaded", () => {

    const modal = new bootstrap.Modal(document.getElementById("taskModal"));
    const modalContent = document.getElementById("taskModalContent");

    // CREAR
    document.getElementById("btnCrearTask")
        .addEventListener("click", async () => {

            modalContent.innerHTML = spinnerHtml();

            const response = await fetch("/Tasks/CreatePartial");
            console.log(response);
            const html = await response.text();

            modalContent.innerHTML = html;
            // 🔹 Cargar categorías después de insertar el HTML
            await loadCategoriesInModal(modalContent);
            modal.show();
        });

    // EDITAR
    document.addEventListener("click", async (e) => {
        if (e.target.matches(".btnEdit")) {

            const id = e.target.dataset.id;

            modalContent.innerHTML = spinnerHtml();

            const response = await fetch('/Tasks/EditPartial/'+id);
            const html = await response.text();

            modalContent.innerHTML = html;
            // 🔹 Cargar categorías después de insertar el HTML
            await loadCategoriesInModal(modalContent);
            modal.show();
        }
    });

    // GUARDAR
    document.addEventListener("click", async (e) => {
        if (e.target.id === "btnSaveTask") {

            const form = document.getElementById("taskForm");

            //const formData = new FormData(form);
            //const data = Object.fromEntries(formData.entries());
            const data = {
                Id: form.querySelector("[name='Id']").value,
                Title: form.querySelector("[name='Title']").value,
                CategoryId: form.querySelector("[name='CategoryId']").value,
                Step: form.querySelector("[name='Step']").value,
                IsCompleted: form.querySelector("[name='IsCompleted']").checked
            };

            const isEdit = data.Id && data.Id !== "0";

            const url = isEdit
                ? '/Tasks/Edit/'+data.Id
                : '/Tasks/Create';
            console.log(JSON.stringify(data));

            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const html = await response.text();
                modalContent.innerHTML = html;
                return;
            }

            modal.hide();

            await refreshTable();
        }
    });

});

function spinnerHtml() {
    return `
        <div class="modal-body text-center">
            <div class="spinner-border text-primary"></div>
            <p>Cargando...</p>
        </div>`;
}

async function refreshTable() {
    const response = await fetch("/Tasks/LoadTablePartial");
    const html = await response.text();
    document.getElementById("taskTableContainer").innerHTML = html;
}





// Función para cargar las categorías en el select del modal
async function loadCategoriesInModal(modalContent) {

    const select = modalContent.querySelector("#categorySelect");
    if (!select) return;

    // Valor actual (cuando edito)
    const selectedId = select.dataset.selectedCategoryId || "";

    try {
        const response = await fetch("https://127.0.0.1:7074/api/Categories");

        if (!response.ok) {
            console.error("Error al cargar categorías");
            return;
        }

        const categories = await response.json();

        // Limpiamos las opciones actuales excepto la primera
        const firstOption = select.querySelector("option[value='']");
        select.innerHTML = "";
        if (firstOption) {
            select.appendChild(firstOption);
        } else {
            const defaultOpt = document.createElement("option");
            defaultOpt.value = "";
            defaultOpt.textContent = "-- Seleccione una categoría --";
            select.appendChild(defaultOpt);
        }

        // Agregamos las categorías
        categories.forEach(cat => {
            const opt = document.createElement("option");
            opt.value = cat.id;
            opt.textContent = cat.name;

            if (selectedId && selectedId === String(cat.id)) {
                opt.selected = true;
            }

            select.appendChild(opt);
        });

    } catch (err) {
        console.error("Error de red al cargar categorías", err);
    }
}