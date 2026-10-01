var EventoAsistencia = {
    BaseUrl: "/eventoasistencia/",
    DataTable: null,
    Table: '#EventoAsistencia-Tabla',

    Init: () => {
        setTimeout(() => {
            $('#EventoAsistencia-Evento').selectpicker('destroy');
            $('#EventoAsistencia-Evento').select2();
        }, 300);

        EventoAsistencia.DataTable = $(EventoAsistencia.Table).DataTable({
            data: [],
            columns: [
                { data: "tipO_DOCUMENTO" },
                { data: "nuM_DOCUMENTO" },
                { data: "nombres" },
                { data: "apellidO_PATERNO" },
                { data: "apellidO_MATERNO" },
                { data: "modalidad" },
                {
                    data: "fechA_REG",
                    render: function (data) {
                        if (!data) return "";
                        var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
                        return formatter.format(new Date(data));
                    },
                },
            ],
            language: {
                url: "../assets/plugins/datatables/Spanish.json",
            },
        });

        document.querySelector("#EventoAsistencia-Buscar").addEventListener("click", EventoAsistencia.Buscar);
        document.querySelector("#EventoAsistencia-Descargar").addEventListener("click", EventoAsistencia.Descargar);
    },

    Buscar: () => {
        var idEvento = parseInt(document.querySelector("#EventoAsistencia-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(EventoAsistencia.BaseUrl + "Listar?idEvento=" + idEvento, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        })
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                EventoAsistencia.DataTable.clear();
                EventoAsistencia.DataTable.rows.add(Array.isArray(data) ? data : []);
                EventoAsistencia.DataTable.draw();
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Descargar: () => {
        var idEvento = parseInt(document.querySelector("#EventoAsistencia-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }
        window.location.href = EventoAsistencia.BaseUrl + "Reporte?idEvento=" + idEvento;
    },
};

// Se llama directo (no en DOMContentLoaded) porque con pjax.js este script se
// re-ejecuta en cada navegación sin que haya un nuevo DOMContentLoaded.
EventoAsistencia.Init();
