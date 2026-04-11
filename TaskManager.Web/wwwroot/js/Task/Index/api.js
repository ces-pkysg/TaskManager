(function (window, $) {

    window.Tasks = window.Tasks || {};
    const ns = window.Tasks.index = window.Tasks.index || {};
    ns.api = ns.api || {};


    //aqui solo van endpoints

    ns.api = {
        //obtener formulario para crear
        createPartial: function () {
            return $.ajax({
                url: ns.core.urls.createPartial,
                type: 'GET'
            });
        },

        //obtener formulario para editar
        editPartial: function (id) {
            return $.ajax({
                url: ns.core.urls.editPartial + id,
                type: 'GET'
            });
        },

        //guardar tarea

        //saveTask: function (data) {
        //    return $.ajax({
        //        url: ns.core.urls.create,
        //        type: 'POST',
        //        data: JSON.stringify(data),
        //        contentType: 'application/json'
        //    }).then(async data => {
        //        let result = data;
               

        //        return false;
        //    }).catch(async error => {
        //        let errorLocal = error;
        //        return false;
        //    });
        //},

        saveTask: function (data) {
            return $.ajax({
                url: ns.core.urls.create,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json'
            });
        },


        saveEditTask: function (data, id) {
            return $.ajax({
                url: ns.core.urls.edit + id,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json'
            });
        },


        deleteTask: function (id) {
            return $.ajax({
                url: ns.core.urls.delete + id,
                type: 'POST',
                
            });
        },


        loadTable: function (query) {
            return $.ajax({
                url: ns.core.urls.loadTablePartial + query.toString(),
                type: 'GET',

            });
        },


        //obtener categorias
        getCategories: function () {
            return $.ajax({
                url: ns.core.urls.getCategories,
                type: 'GET',

            });
        }
    };


})(window, jQuery);

//////////  Ejemplo guia
//guardarUsuario: function (payload) {
//    return $.ajax({
//        url: ns.urls.guardar,
//        type: 'POST',
//        data: payload
//    });
//}




//ns.api.validaEstatusTV429 = async function () {
//    loading.show();

//    ns.datosValidaEstatusTV4 = null;

//    model = {

//    };

//    return $.ajax({
//        url: ns.urls.validaEstatusTV429Evento,
//        type: 'POST',
//        data: model
//    }).then(async data => {
//        if (data.success) {
//            ns.datosValidaEstatusTV4 = data.data;
//            loading.hide();
//            return true;
//        } else {
//            notificacion.warning("No se pudo validar el estatus de la TV.");
//        }

//        loading.hide();
//        return false;
//    }).catch(async error => {
//        notificacion.error("Ocurrió un error al verificar el estatus de la TV.");
//        loading.hide();
//        return false;
//    });
//};