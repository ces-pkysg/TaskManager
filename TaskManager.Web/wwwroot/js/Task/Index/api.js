(function (window, $) {

    const ns = window.Tasks.index = window.Tasks.index || {};
    ns.api = ns.api || {};

    ns.api = {
        crearPartial: function () {
            return $.ajax({
                url: ns.core.urls.createPartial,
                type: 'GET'
            });
        },


         editarPartial: function (id) {
            return $.ajax({
                url: ns.core.urls.editPartial + id,
                type: 'GET'
            });
         },


        guardarTarea: function (data) {
            return $.ajax({
                url: ns.core.urls.create,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json'
            });
        },

        guardarEdicionTarea: function (data, id) {
            return $.ajax({
                url: ns.core.urls.edit + id,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json'
            });
        },

        eliminarTarea: function (id) {
            return $.ajax({
                url: ns.core.urls.delete + id,
                type: 'POST',
                
            });
        },

        cargarTareas: function (id) {
            return $.ajax({
                url: ns.core.urls.loadTablePartial + query.toString(),
                type: 'GET',

            });
        }
    };

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


})(window, jQuery);

//////////  Ejemplo guia
//guardarUsuario: function (payload) {
//    return $.ajax({
//        url: ns.urls.guardar,
//        type: 'POST',
//        data: payload
//    });
//}
