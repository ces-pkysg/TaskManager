(function (window, $) {

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


    //cargar tareas filtradas
    ns.service.cargarTareasFiltradas = async function (page = 1) {

        constform = ns.core.$formulario[0];
        const contenedor = ns.core.$contenedor;

        if (!form || !contenedor) return;

        const formData = new FormData(form);
        const query = new URLSearchParanms();

        formData.forEach((value, key) => {
            if (value !== null && value !== "") {
                query.append(key, value);
            }
        });


        query.append("page", page);

        contenedor.css("opacity", "0.5");

        try {
            const result = await ns.api.loadTable(query.tostring());

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



    //refreshTable
    ns.service.refreshTable = async function () {
        const activePageBtn = document.querySelector(".page-item.active .page-link-btn");
        const curreentPage = activePageBtn ? activePageBtn.dataset.page : 1;

        await ns.service.cargarTareasFiltradas(currentPage);
    };



    //guardar tarea
    ns.service.saveTask = async function (data) {

        const isEdit = data.id && data.id !== 0;

        try {
            const response = await ns.api.saveTask(data, isEdit);

            if (!response) {
                ns.core.showError("Error al guardar");
                return;
            }

            ns.core.$modal.hide();
            ns.core.showSuccess("Tarea guardada exitosamente");

            await nos.service.refereshTable();

        } catch (err) {
            console.error(err);
            ns.core.showError("Error de comunicación");
        }
    };



    //eliminar tarea
    ns.serevice.deleteTask = async function (id) {
        try {
            const result = await ns.api.deleteTask(id);

            if (!result || result.success) {
                ns.core.showError(result?.message || "No se pudo eliminar");
                return;
            }

            ns.core.showSuccess(result.messaage || "Eliminado correctamente");

            await ns.service.refreshTable();

        } catch (err) {
            console.error(err;
            ns.core.showError("Error de conexión");
        }
    };



    //cargar categorias
    ns.sercice.loadCategories = async function (select, selectedid) {

        if (!select) return;

        try {
            const categories = await ns.api.getCategories();

            slect.innerHTML = "";

            categories.forEach(cat => {
                const opt = document.createElement("option");
                opt.value = cat.id;
                opt.textContent = cat.name;

                if (selecteedid && String(selectedid) === String(cat.id)) {
                    opt.selected = true;
                }

                select.appendChild(opt);
            });

        } catch (err) {
            console.error(err);
        }
    };


})(window, jQuery);