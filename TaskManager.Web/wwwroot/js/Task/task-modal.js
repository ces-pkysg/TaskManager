document.addEventListener("DOMContentLoaded", () => {

    const btnBuscar = document.getElementById("btnBuscarAjax");//boton buscar Ajax           ////REFERENCIA DOM 
    const formulario = document.getElementById("filterForm");//formulario de filtros         ////REFERENCIA DOM Task.index.core.$formulario
    const modal = new bootstrap.Modal(document.getElementById("taskModal"));                 ////REFERENCIA DOM 
    const modalContent = document.getElementById("taskModalContent");                        ////REFERENCIA DOM
    const contenedor = document.getElementById("taskTableContainer");//donde está la tabla   ////REFERECNIA DOM   

    //boton buscar va a llamar a la funcion inificada
    btnBuscar.addEventListener("click", async () => {
        //Al buscar manualmente hace que inicie en la pagina 1
        await fetchFilteredTask(1);


        //// Convertimos el formulario en QueryString automático
        //const formData = new FormData(formulario);
        //const query = new URLSearchParams();

        //formData.forEach((value, key) => {
        //    if (value !== null && value !== "") {
        //        query.append(key, value);
        //    }
        //});

        //// Construimos la URL para el GET parcial
        //let url = '/Tasks/LoadTablePartial?' + query.toString();

        //// Llamada AJAX
        //const response = await fetch(url);

        //if (!response.ok) {
        //    contenedor.innerHTML = "<p>Error al cargar resultados.</p>";
        //    return;
        //}

        //const html = await response.text();

        //// Reemplazamos la tabla
        //contenedor.innerHTML = html;
    });


    //Nuevo listener global para los clics en los numeros de pagina
    document.addEventListener("click", async (e) => {
        //si el elemento clicado tiene la clase del boton de pagina
        if (e.target.matches(".page-link-btn")) {
            e.preventDefault();// Evita saltos de pagina o recarga

            const page = e.target.dataset.page; // Obtiene el numero del atributo data-page
            if (page) {
                await fetchFilteredTask(page); // Llama a la busqueda manteniendo filtros
            }

        }
});

    // CREAR  ORIGINAL
    document.getElementById("btnCrearTask")//boton crear va acore.js
        .addEventListener("click", async () => {

            modalContent.innerHTML = spinnerHtml();

            const response = await fetch("/Tasks/CreatePartial");//url que va en core.js
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

            const response = await fetch('/Tasks/EditPartial/'+id);//url que va en core.js
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
                Id: parseInt(form.querySelector("[name='Id']").value) || 0,
                Title: form.querySelector("[name='Title']").value,
                CategoryId: parseInt(form.querySelector("[name='CategoryId']").value) || 0,
                Step: parseInt (form.querySelector("[name='Step']").value) || 0,
                IsCompleted: form.querySelector("[name='IsCompleted']").checked
            };

            //prueba console.log("Datos enviados al Controller: ", data);//revisa consola del navegador

            const isEdit = data.Id && data.Id !== "0";

            const url = isEdit
                ? '/Tasks/Edit/'+data.Id //url que va en core.js
                : '/Tasks/Create'; //url que va en core.js
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


    //eliminar
    document.addEventListener("click", async (e) => {
        if (e.target.matches(".btnDelete")) {

            const id = e.target.dataset.id;
            if (!id) return;

            const confirmado = confirm("¿Seguro que deseas eliminar esta tarea?");
            if (!confirmado) return;

            try {
                const response = await fetch('/Tasks/DeleteAjax/' + id, //url que va en core.js
                    {
                    method: "POST"
                });

                const result = await response.json();

                if (!response.ok || !result.success) {
                    showError(result.message || "No se pudo eliminar la tarea.");
                    return;
                }

                showSuccess(result.message || "La tarea fue eliminada correctamente.");

                await refreshTable();

            } catch (err) {
                console.error(err);
                showError("Error de comunicación con el servidor al eliminar la tarea.");
            }
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


//Maneja filtros y paginacion al mismo tiempo en una sola función
async function fetchFilteredTask(page = 1) {
    const formulario = document.getElementById("filterForm");
    const contenedor = document.getElementById("taskTableContainer");

    if (!formulario || !contenedor) return;

    // Convertimos el formulario en QueryString automático
    const formData = new FormData(formulario);
    const query = new URLSearchParams();

    formData.forEach((value, key) => {
        if (value !== null && value !== "") {
            query.append(key, value);
        }
    });


//Agrega la pagina a la url
    query.append("page", page);

    //Feedback visual para usuario
    contenedor.style.opacity = "0.5";

    try {
        const url = '/Tasks/LoadTablePartial?' + query.toString();
        const response = await fetch(url);

        if (!response.ok) {
            contenedor.innerHTML = "<p>Error al cargar resultados.</p>";
            return;
        }

        const html = await response.text();

        //Se reemplaza tabla con contenido nuevo
        contenedor.innerHTML = html;
    } catch (err) {
        console.error("Error en la petición AJAX", err);
    } finally {
        contenedor.style.opacity = "1";
    }
}

//Actualiza la tabla después de crear, editar o eliminar para mostrar los cambios sin recargar toda la página
//respetando filtros actuales
async function refreshTable() {
    //Encuentra la pagina activa actualmente en el HTML 
    const activePageBtn = document.querySelector(".page-item.active .page-link-btn");
    const currentPage = activePageBtn ? activePageBtn.dataset.page : 1;

    //Llama a la función maestra con la página que ya teníamos
    await fetchFilteredTask(currentPage);


    //const response = await fetch("/Tasks/LoadTablePartial");
    //const html = await response.text();
    //document.getElementById("taskTableContainer").innerHTML = html;
}



// Función para cargar las categorías en el select del modal
async function loadCategoriesInModal(modalContent) {

    const select = modalContent.querySelector("#categorySelect");
    if (!select) return;

    // Valor actual (cuando edito)
    const selectedId = select.getAttribute("data-selected-category-id");
    //prueba para revisar console.log("ID recuperado para preselección:", selectedId);

    try {
        // Antes: const response = await fetch("https://127.0.0.1:7074/api/Categories");

        // Ahora :Llama a CategoriesController.cs
        const response = await fetch("/Categories/GetCategories");//url que va en core.js
        const categories = await response.json();

        if (!response.ok) {
            console.error("Error al cargar categorías");
            return;
        }


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

        // Agregamos las categorías dinamicamente
        categories.forEach(cat => {
            const opt = document.createElement("option");
            opt.value = cat.id;
            opt.textContent = cat.name;


            // Agregado: Comparación forzada como String para evitar fallos de tipos
            if (selectedId && String(selectedId) === String(cat.id)) {
                opt.selected = true;

                // Opcional: Tambien se puede usar el atributo:
                opt.setAttribute("selected","selected");
            }

            select.appendChild(opt);
        });

        //Retraso de seguridad para asegurar la preseleccion
        setTimeout(() => {
            if (selectedId && selectedId !== "0") {
                select.value = selectedId;
            }
        }, 100);



        // Agregado: Refuerzo manual. Forza al select a marcar el valor después de cargar el DOM
        //if (selectedId) {
        //    select.value = selectedId;
        //}

    } catch (err) {
        console.error("Error de red al cargar categorías", err);
    }
}


// ✅ Funciones auxiliares para mostrar mensajes
function showSuccess(message) {
    const container = document.getElementById("alertContainer");
    if (!container) return;

    container.innerHTML = `
       <div class="alert alert-success alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`;
}
//ns.core.$alert = $("#alertContainer");

/*Ejemplo d emigracion a core.js
function showSuccesstarea(message) {
    if (!ns.core.$alert) return;
    ns.core.$alert.innerHTML = `
       <div class="alert alert-success alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`;
}*/

function showError(message) {
    const container = document.getElementById("alertContainer");
    if (!container) return;

    container.innerHTML = `
       <div class="alert alert-danger alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`;
}