var Constancias = {
    BaseUrl: "/constancias/",
    DataTable: null,
    Table: '#Constancias-Tabla',
    BuscarDataTable: null,
    BuscarTable: '#Constancias-Buscar-Tabla',

    Init: () => {
        setTimeout(() => {
            $('#Constancias-Evento').selectpicker('destroy');
            $('#Constancias-Evento').select2();
        }, 300);

        Constancias.DataTable = $(Constancias.Table).DataTable({
            data: [],
            columns: [
                { data: "nuM_DOCUMENTO" },
                { data: "nombres" },
                { data: "apellidO_PATERNO" },
                { data: "apellidO_MATERNO" },
                { data: "sesioneS_OBLIGATORIAS" },
                { data: "sesioneS_ASISTIDAS" },
                {
                    data: "encuestA_RESPONDIDA",
                    render: (data) => data
                        ? '<span class="badge badge-success">Sí</span>'
                        : '<span class="badge badge-default">No</span>',
                },
                {
                    data: "apto",
                    render: (data) => data
                        ? '<span class="badge badge-success">Apto</span>'
                        : '<span class="badge badge-danger">No apto</span>',
                },
                {
                    data: "constancia",
                    render: (data) => {
                        if (!data) return '<span class="text-muted">—</span>';
                        var estilo = data.estado === "ANULADA" ? "badge-danger" : "badge-info";
                        return '<span class="badge ' + estilo + '">' + data.codigo + '</span>';
                    },
                },
                {
                    data: null,
                    orderable: false,
                    render: (data, type, row) => {
                        if (row.constancia && row.constancia.estado === "GENERADA") {
                            return '<button class="btn btn-sm btn-success" onclick="Constancias.Descargar(' + row.constancia.iD_CONSTANCIA + ')">Descargar</button> ' +
                                   '<button class="btn btn-sm btn-danger" onclick="Constancias.Anular(' + row.constancia.iD_CONSTANCIA + ')">Anular</button>';
                        }
                        if (!row.apto) {
                            return '<span class="text-muted">No cumple requisitos</span>';
                        }
                        return '<button class="btn btn-sm btn-primary" onclick="Constancias.Generar(' + row.iD_PERSONA + ')">Generar</button>';
                    },
                },
            ],
            language: { url: "../assets/plugins/datatables/Spanish.json" },
        });

        Constancias.BuscarDataTable = $(Constancias.BuscarTable).DataTable({
            data: [],
            columns: [
                { data: "codigo" },
                {
                    data: "estado",
                    render: (data) => data === "ANULADA"
                        ? '<span class="badge badge-danger">ANULADA</span>'
                        : '<span class="badge badge-info">GENERADA</span>',
                },
                {
                    data: "fechA_GENERACION",
                    render: (data) => {
                        if (!data) return "";
                        var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
                        return formatter.format(new Date(data));
                    },
                },
                { data: "usER_GENERACION" },
                { data: "nuM_DESCARGAS" },
                {
                    data: null,
                    orderable: false,
                    render: (data, type, row) => row.estado === "GENERADA"
                        ? '<button class="btn btn-sm btn-success" onclick="Constancias.Descargar(' + row.iD_CONSTANCIA + ')">Descargar</button> ' +
                          '<button class="btn btn-sm btn-danger" onclick="Constancias.Anular(' + row.iD_CONSTANCIA + ')">Anular</button>'
                        : '',
                },
            ],
            language: { url: "../assets/plugins/datatables/Spanish.json" },
        });

        document.querySelector("#Constancias-Buscar").addEventListener("click", Constancias.CargarElegibles);
        document.querySelector("#Constancias-Buscar-Boton").addEventListener("click", Constancias.BuscarGeneradas);
    },

    CargarElegibles: () => {
        var idEvento = parseInt(document.querySelector("#Constancias-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }
        Constancias._idEventoActual = idEvento;

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(Constancias.BaseUrl + "Elegibles?idEvento=" + idEvento)
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                Constancias.DataTable.clear();
                Constancias.DataTable.rows.add(Array.isArray(data) ? data : []);
                Constancias.DataTable.draw();
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Generar: (idPersona) => {
        var idEvento = Constancias._idEventoActual;
        if (!idEvento) return;

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(Constancias.BaseUrl + "Generar?idEvento=" + idEvento + "&idPersona=" + idPersona, { method: "POST" })
            .then(async (response) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                var data = await response.json();
                if (data.success === false) {
                    swal("No se pudo generar", data.mensaje, "warning");
                    return;
                }
                swal("Listo", "Constancia generada correctamente", "success");
                Constancias.CargarElegibles();
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Descargar: (idConstancia) => {
        window.open(Constancias.BaseUrl + "Descargar?idConstancia=" + idConstancia, "_blank");
    },

    Anular: (idConstancia) => {
        swal({
            title: "Anular constancia",
            text: "Indica el motivo de la anulación:",
            content: { element: "input", attributes: { placeholder: "Motivo" } },
            buttons: ["Cancelar", "Anular"],
        }).then((motivo) => {
            if (!motivo) return;
            document.querySelector(".page-loader-wrapper").style.display = "block";
            fetch(Constancias.BaseUrl + "Anular?idConstancia=" + idConstancia + "&motivo=" + encodeURIComponent(motivo), { method: "POST" })
                .then(async (response) => {
                    document.querySelector(".page-loader-wrapper").style.display = "none";
                    var data = await response.json();
                    if (data.success === false) {
                        swal("Upss", data.mensaje, "warning");
                        return;
                    }
                    swal("Listo", "Constancia anulada", "success");
                    if (Constancias._idEventoActual) Constancias.CargarElegibles();
                    Constancias.BuscarGeneradas();
                })
                .catch((error) => {
                    document.querySelector(".page-loader-wrapper").style.display = "none";
                    swal("Upss, algo salió mal", String(error), "warning");
                });
        });
    },

    BuscarGeneradas: () => {
        var documento = document.querySelector("#Constancias-Buscar-Documento").value.trim();
        var codigo = document.querySelector("#Constancias-Buscar-Codigo").value.trim();
        var params = [];
        if (documento) params.push("documento=" + encodeURIComponent(documento));
        if (codigo) params.push("codigo=" + encodeURIComponent(codigo));

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(Constancias.BaseUrl + "Buscar?" + params.join("&"))
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                Constancias.BuscarDataTable.clear();
                Constancias.BuscarDataTable.rows.add(Array.isArray(data) ? data : []);
                Constancias.BuscarDataTable.draw();
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },
};

// Se llama directo (no en DOMContentLoaded) porque con pjax.js este script se
// re-ejecuta en cada navegación sin que haya un nuevo DOMContentLoaded.
Constancias.Init();
