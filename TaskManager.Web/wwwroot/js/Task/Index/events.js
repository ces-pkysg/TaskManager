(function (window, $) {

    window.Tasks = window.Tasks || {};

    //es un seguro en caso de que la referencia no este creada, la cree
    const ns = window.Tasks.index = window.Tasks.index || {};
    ns.events = ns.events || {};

    //inicia todos los eventos 
    ns.events.init = function () {

        //click buscar
        ns.core.$btnBuscar.on("click", async function () {

            //llamada al service para cargar con filtros  (siempre pagina 1)
            await ns.service.cargarTareasFiltradas(1);
        });

        //paginacion (evento delegado)
        $(document).on("click", ".page-link-btn", async function (e) {

            e.preventDefault();

            //obtener numero d epagina desde atributo data-page
            const page = $(this).data("page");

            if (page) {
                await ns.service.cargarTareasFiltradas(page);
            }
        });


        //crear tarea
        ns.core.$btnCreateTask.on("click", async function () {

            //muestra spinner mientras carga
            ns.core.$modalContent.html(ns.core.spinnerHtml());

            //llamada al api para obtener el partial
            const html = await ns.api.createPartial();

            //inserta html en el modal
            ns.core.$modalContent.html(html);

            //caraga categoriasen el select
            await ns.service.loadCategories(ns.core.$modalContent[0]);

            //muestra el modal
            ns.core.$modalElement.modal("show");
        });


        //editar tarea
        $(document).on("click", ".btnEdit", async function () {

            const id = $(this).data("id");

            //spinner
            ns.core.$modalContent.html(ns.core.spinnerHtml());

            //se obtiene partial de edit
            const html = await ns.api.editPartial(id);

            ns.core.$modalContent.html(html);

            //carga de categorias con preseleccion
            await ns.service.loadCategories(ns.core.$modalContent[0]);

            ns.core.$modalElement.modal("show");
        });


        //guardar tarea
        $(document).on("click", "#btnSaveTask", async function () {

            const form = document.getElementById("taskForm");

            if (!form) return;

            //crea objeto data desde form
            const data = {
                Id: parseInt(form.querySelector("[name='Id']").value) || 0,
                Title: form.querySelector("[name='Title']").value,
                CategoryId: parseInt(form.querySelector("[name='CategoryId']").value) || 0,
                Step: parseInt(form.querySelector("[name='Step']").value) || 0,
                IsCompleted: form.querySelector("[name='IsCompleted']").checked
            };

            //llama al service 
            await ns.service.saveTask(data);
        });


        //eliminar tarea
        $(document).on("click", "btnDeleted", async function () {

            const id = $(this).data("id");

            if (!id) return;

            //confirmacion de  usuario
            const confirmado = confirm("¿Seguro que deseas eliminar esta tarea?");
            if (!confirmado) return;

            //llama al service
            await ns.service.deleteTask(id);
        });

    };

})(window, jQuery);