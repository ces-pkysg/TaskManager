(function (window, $) {

    window.Tasks = window.Tasks || {};
    const ns = window.Tasks.index = window.Tasks.index || {};

    //se crea el espacio para modulo service
    ns.service = ns.service || {};

    //actualizar tabla
    ns.service.cargarTareas = async function () {
        //llamar al endpoint (api) y regresa json o html
        //se almacena en una variable
        let result = await ns.api.cargarTareas();

        //validacion
        if (result != null) {
            //actualizo UI, se usa lo que se guardo en core 
            ns.core.$contenedor.html(result);
        }
    };


    //cargar tabla filtrada
    ns.service.cargarTareasFiltradas = async function (page = 1) {

        const form = ns.core.$formulario[0];
        const contenedor = ns.core.$contenedor;

        if (!form || !contenedor) return;

        const formData = new FormData(form);
        let query = new URLSearchParams();

        formData.forEach((value, key) => {
            if (value !== null && value !== "") {
                query.append(key, value);
            }
        });


        query.append("page", page);

        contenedor.css("opacity", "0.5");

        try {
            const result = await ns.api.loadTable(query.toString());

            if (!result) {
                contenedor.html("<p>Error al cargar el resultado.</p>");
                return;
            }

            contenedor.html(result);

        } catch (err) {
            console.error(err);
        } finally {
            contenedor.css("opacity", "1");
        }
    };



    //refresh Table
    ns.service.refreshTable = async function () {
        const PageBtn = document.querySelector(".page-item.active .page-link-btn");
        const currentPage = PageBtn ? PageBtn.dataset.page : 1;

        await ns.service.cargarTareasFiltradas(currentPage);
    };



    //guardar tarea
    ns.service.saveTask = async function (data) {

        const isEdit = data.Id && data.Id !== 0;

        try {

            let response;

            if (isEdit) {
                //editar
                response = await ns.api.saveEditTask(data, data.Id);
            } else {
                //crear
                response = await ns.api.saveTask(data);
            }

            console.log("response: ", response);

            if (!response || response.success === false) {
                ns.core.showError(response?.message || "Error al guardar");
                return;
            }

            ns.core.$modalElement.modal("hide");
            ns.core.showSuccess("Tarea guardada exitosamente");

            await ns.service.refreshTable();

        } catch (err) {
            console.error(err);
            ns.core.showError("Error de comunicación");
        }
    };



    //eliminar tarea
    ns.service.deleteTask = async function (id) {

        try {
            const result = await ns.api.deleteTask(id);

            //console.log("delete response: ", result);

            //console.log("URL:", ns.core.urls.delete + id);
            //console.log("RESULT:", result);

            if (!result || result.success === false) {
                ns.core.showError(result?.message || "No se pudo eliminar");
                return;
            }

            ns.core.showSuccess(result.message || "Eliminado correctamente");

            await ns.service.refreshTable();

        } catch (err) {
            console.error(err);
            ns.core.showError("Error de conexión");
        }
    };



    //cargar categorias
    ns.service.loadCategories = async function (modalContent) {

        //busca select dentro del modal
        const select = modalContent.querySelector("#categorySelect");

        //si no existe se sale para evitar errores
        if (!select) return;

        //obtiene el id que viene para preseleccion  (cuando se edita)
        const selectedId = select.getAttribute("data-selected-category-id");
        
        try {
            //llama al api (backend) para obtener categorias
            const categories = await ns.api.getCategories();

            //limpia todas la opciones actuales del select 
            select.innerHTML = "";


            //agrega opcion por defecto 
            const defaultOpt = document.createElement("option");
            defaultOpt.value = "";
            defaultOpt.textContent = "-- Seleccione una categoría --";
            select.appendChild(defaultOpt);

            //recoge las categorias y crea opciones 
            categories.forEach(cat => {
                //crea opcion nueva
                const opt = document.createElement("option");
                //asigna un valor (id)
                opt.value = cat.id;
                //asigna texto visible
                opt.textContent = cat.name;

                //si es edicion, selecciona automaticamente la categoria correcta
                if (selectedId && String(selectedId) === String(cat.id)) {
                    opt.selected = true;
                }

                //agrega la opcion al select
                select.appendChild(opt);
            });

        } catch (err) {
            //manejo d eerror en consola
            console.error(err);
        }
    };


})(window, jQuery);