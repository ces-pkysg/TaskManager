(function (window, $) {

    //namespace
    window.Tasks = window.Tasks || {};
    const ns = window.Tasks.index = window.Tasks.index || {};
    ns.core = ns.core || {};

    //Referencias DOM
    ns.core.$alert = $("#alertContainer");
    ns.core.$formulario = $("#filterForm");
    ns.core.$modalElement = $("#taskModal");
    ns.core.$modalContent = $("#taskModalContent");
    ns.core.$contenedor = $("#taskTableContainer");

    ns.core.$btnBuscar = $("#btnBuscarAjax");
    ns.core.$btnCreateTask = $("#btnCrearTask");
    ns.core.$btnSaveTask = $("#btnSaveTask");
    ns.core.$btnEditTask = $(".btnEdit"); //se quita # porque esta como una clase (.)
    ns.core.$btnDeleteTask = $(".btnDelete");//se quita # porque esta como una clase (.)


    //URLs (rutas del modulo)
    ns.core.urls = {
        createPartial: '/Tasks/CreatePartial',
        editPartial: '/Tasks/EditPartial/',//el + id se concatena cuando se use,
        delete: '/Tasks/DeleteAjax/',// + id se concatena cuando se use,
        edit: '/Tasks/Edit/', // el + data.Id se concatena cuando se use,,
        create: '/Tasks/Create',
        loadTablePartial: '/Tasks/LoadTablePartial?', // el + query.toString() se concatena cuando se use,
        getCategories: "/Categories/GetCategories"
    };


    //spinner 090426
    ns.core.spinnerHtml = function () {
        return `
        <div class="modal-body text-center">
            <div class="spinner-border text-primary"></div>
            <p>Cargando...</p>
        </div>`;
    };


    //msje éxito 090426
    ns.core.showSuccess = function (message) {
        ns.core.$alert.html(`
       <div class="alert alert-success alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`);

    };


    //msje error 090426
    ns.core.showError = function (message) {
        ns.core.$alert.html(`
       <div class="alert alert-danger alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`);
    };


    
})(window, jQuery);

//// Terminar de construir loadcategoriesinmodal en service.js