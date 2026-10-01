var Reportes = {
    BaseUrl: "/reportes/",
    DataTable: null,
    Table: '#Reportes-Tabla',

    Init: () => {
        setTimeout(() => {
            $('#Reportes-Evento').selectpicker('destroy');
            $('#Reportes-Evento').select2();
        }, 300);

        Reportes.DataTable = $(Reportes.Table).DataTable({
            data: [],
            columns: [
                { data: "nuM_DOCUMENTO" },
                { data: "nombres" },
                { data: "apellidO_PATERNO" },
                { data: "apellidO_MATERNO" },
                { data: "departamento" },
                { data: "provincia" },
                { data: "distrito" },
                { data: "entidad" },
                { data: "cargo" },
                { data: "modalidad" },
                { data: "sesioneS_OBLIGATORIAS" },
                { data: "sesioneS_ASISTIDAS" },
                {
                    data: "encuestA_RESPONDIDA",
                    render: (data) => data
                        ? '<span class="badge badge-success">Sí</span>'
                        : '<span class="badge badge-default">No</span>',
                },
                {
                    data: "aptO_CONSTANCIA",
                    render: (data) => data
                        ? '<span class="badge badge-success">Apto</span>'
                        : '<span class="badge badge-danger">No apto</span>',
                },
                {
                    data: null,
                    render: (data, type, row) => {
                        if (row.estadO_CONSTANCIA === "GENERADA") {
                            return '<span class="badge badge-info">' + row.codigO_CONSTANCIA + '</span>';
                        }
                        if (row.estadO_CONSTANCIA === "ANULADA") {
                            return '<span class="badge badge-danger">ANULADA</span>';
                        }
                        return '<span class="text-muted">No generada</span>';
                    },
                },
            ],
            language: { url: "../assets/plugins/datatables/Spanish.json" },
        });

        document.querySelector("#Reportes-Buscar").addEventListener("click", Reportes.Buscar);
        document.querySelector("#Reportes-Descargar").addEventListener("click", Reportes.Descargar);
    },

    Buscar: () => {
        var idEvento = parseInt(document.querySelector("#Reportes-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(Reportes.BaseUrl + "Participantes?idEvento=" + idEvento)
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                Reportes.DataTable.clear();
                Reportes.DataTable.rows.add(Array.isArray(data) ? data : []);
                Reportes.DataTable.draw();
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Descargar: () => {
        var idEvento = parseInt(document.querySelector("#Reportes-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }
        window.location.href = Reportes.BaseUrl + "Excel?idEvento=" + idEvento;
    },
};

// Se llama directo (no en DOMContentLoaded) porque con pjax.js este script se
// re-ejecuta en cada navegación sin que haya un nuevo DOMContentLoaded.
Reportes.Init();
