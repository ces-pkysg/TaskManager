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
    ns.core.$btnEditTask = $(".btnEdit"); //se quita # porque esta como una clase (.)
    ns.core.$btnDeleteTask = $(".btnDelete");//se quita # porque esta como una clase (.)


    //URLs (rutas del modulo)
    ns.core.urls = {
        createPartial: '/Task/CreatePartial',
        editPartial: '/Tasks/EditPartial/',//el + id se concatena cuando se use,
        delete: '/Tasks/DeleteAjax/',// + id se concatena cuando se use,
        edit: '/Tasks/Edit/', // el + data.Id se concatena cuando se use,,
        create: '/Tasks/Create',
        loadTablePartial: '/Tasks/LoadTablePartial?', // el + query.toString() se concatena cuando se use,
        getCategories: "/Categories/GetCategories"
    };




    
})(window, jQuery);