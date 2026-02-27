document.addEventListener("DOMContentLoaded", async () => {

    const btnBuscar = document.getElementById("btnBuscarAjax");//boton buscar Ajax
    const contenedor = document.getElementById("taskTableContainer");//donde está la tabla
    const formulario = document.getElementById("filterForm");//formulario de filtros
    let url = "";

    btnBuscar.addEventListener("click", async () => {

        // Convertimos el formulario en QueryString automático
        const formData = new FormData(formulario);
        const query = new URLSearchParams();

        formData.forEach((value, key) => {
            if (value !== null && value !== "") {
                query.append(key, value);
            }
        });

        // Construimos la URL para el GET parcial
        url = '/Tasks/LoadTablePartial?' + query.toString();

        // Llamada AJAX
        const response = await fetch(url);

        if (!response.ok) {
            contenedor.innerHTML = "<p>Error al cargar resultados.</p>";
            return;
        }

        const html = await response.text();

        // Reemplazamos la tabla
        contenedor.innerHTML = html;
    });

    
});
