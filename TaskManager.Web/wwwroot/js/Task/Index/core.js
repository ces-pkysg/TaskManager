(function (window, $) {

    //namespace
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
    ns.core.$btnEditTask = $(".btnEdit"); //??????
    ns.core.$btnDeleteTask = $(".btnDelete");//??????


    //URLs (rutas del modulo)
    ns.core.urls = {
        createPartial: '/Task/CreatePartial',
        editPartial: '/Tasks/EditPartial/' + id,
        delete: '/Tasks/DeleteAjax/' + id,
        edit: '/Tasks/Edit/' + data.Id,
        create: '/Tasks/Create',
        loadTablePartial: '/Tasks/LoadTablePartial?' + query.toString(),
        getCategories: "/Categories/GetCategories"
    };




    
})(window, jQuery);