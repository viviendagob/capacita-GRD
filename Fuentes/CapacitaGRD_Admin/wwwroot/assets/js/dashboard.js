var Dashboard = {
    BaseUrl: "/Dashboard/",

    // Paleta institucional real del sistema (tomada de la UI: menú, botones primarios
    // y barra de progreso pjax), NO colores genéricos de librería de gráficos.
    Color: {
        primary: "#EF1A1E",   // rojo institucional (menú, botones primarios, loader)
        ink: "#181B20",       // texto / neutro oscuro
        gold: "#E8A020",      // acento secundario (barra de progreso pjax)
        success: "#1E8E5A",   // verde semántico (tasas positivas)
        danger: "#C0392B",    // rojo semántico distinto del rojo de marca (para "anulado/cancelado")
        muted: "#5A6472",
    },

    EstadoColor: (nombre) => {
        var n = (nombre || "").toUpperCase();
        if (n === "ACTIVO") return Dashboard.Color.success;
        if (n === "CANCELADO") return Dashboard.Color.danger;
        if (n === "REALIZADO") return Dashboard.Color.ink;
        return Dashboard.Color.muted;
    },

    ModalidadColor: (nombre) => {
        var n = (nombre || "").toUpperCase();
        if (n === "PRESENCIAL") return Dashboard.Color.primary;
        if (n === "VIRTUAL") return Dashboard.Color.gold;
        if (n === "HIBRIDA" || n === "HÍBRIDA") return Dashboard.Color.ink;
        return Dashboard.Color.muted;
    },

    Init: () => {
        setTimeout(() => {
            $('#Dashboard-Evento, #Dashboard-Modalidad, #Dashboard-Estado, #Dashboard-Departamento, #Dashboard-Entidad')
                .each(function () { $(this).selectpicker && $(this).selectpicker('destroy'); })
                .select2();

            $('#Dashboard-FechaDesde, #Dashboard-FechaHasta').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true,
                language: 'es',
            });
        }, 300);

        document.querySelector("#Dashboard-Filtrar").addEventListener("click", Dashboard.Cargar);
        document.querySelector("#Dashboard-Limpiar").addEventListener("click", Dashboard.Limpiar);

        Dashboard.Cargar();
    },

    ParseFecha: (texto) => {
        if (!texto) return null;
        var partes = texto.split("/");
        if (partes.length !== 3) return null;
        return partes[2] + "-" + partes[1] + "-" + partes[0];
    },

    ObtenerFiltro: () => ({
        ID_EVENTO: parseInt(document.querySelector("#Dashboard-Evento").value) || null,
        FECHA_DESDE: Dashboard.ParseFecha(document.querySelector("#Dashboard-FechaDesde").value),
        FECHA_HASTA: Dashboard.ParseFecha(document.querySelector("#Dashboard-FechaHasta").value),
        ID_MODALIDAD: parseInt(document.querySelector("#Dashboard-Modalidad").value) || null,
        DEPARTAMENTO: document.querySelector("#Dashboard-Departamento").value || null,
        ID_ENTIDAD: parseInt(document.querySelector("#Dashboard-Entidad").value) || null,
        ID_ESTADO: parseInt(document.querySelector("#Dashboard-Estado").value) || null,
    }),

    Limpiar: () => {
        $('#Dashboard-Evento, #Dashboard-Modalidad, #Dashboard-Estado, #Dashboard-Departamento, #Dashboard-Entidad').val(null).trigger('change');
        document.querySelector("#Dashboard-FechaDesde").value = "";
        document.querySelector("#Dashboard-FechaHasta").value = "";
        Dashboard.Cargar();
    },

    Cargar: () => {
        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(Dashboard.BaseUrl + "Kpis", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(Dashboard.ObtenerFiltro()),
        })
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                if (data.success === false) {
                    swal("Upss", data.mensaje, "warning");
                    return;
                }
                Dashboard.Render(data);
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Kpi: (color, icono, etiqueta, valor) => {
        return '<div class="dash-kpi" style="--kpi-color: ' + color + ';">' +
            '<span class="material-icons">' + icono + '</span>' +
            '<div class="dash-kpi-value">' + valor + '</div>' +
            '<div class="dash-kpi-label">' + etiqueta + '</div>' +
            '</div>';
    },

    Render: (d) => {
        var C = Dashboard.Color;
        var html = "";
        html += Dashboard.Kpi(C.primary, "event", "Eventos", d.totaL_EVENTOS);
        html += Dashboard.Kpi(C.ink, "groups", "Participantes inscritos", d.totaL_PARTICIPANTES);
        html += Dashboard.Kpi(C.gold, "how_to_reg", "Asistentes únicos", d.totaL_ASISTENTES);
        html += Dashboard.Kpi(C.success, "percent", "% Asistencia", d.pcT_ASISTENCIA + "%");
        html += Dashboard.Kpi(C.primary, "verified", "Constancias generadas", d.totaL_CONSTANCIAS_GENERADAS);
        html += Dashboard.Kpi(C.danger, "block", "Constancias anuladas", d.totaL_CONSTANCIAS_ANULADAS);
        html += Dashboard.Kpi(C.success, "task_alt", "% Aptos para constancia", d.pcT_APTO_CONSTANCIA + "%");
        html += Dashboard.Kpi(C.gold, "assignment_turned_in", "Respondieron encuesta", d.totaL_RESPONDIERON_ENCUESTA);
        html += Dashboard.Kpi(C.success, "quiz", "% Respuesta encuesta", d.pcT_RESPUESTA_ENCUESTA + "%");
        html += Dashboard.Kpi(C.success, "sentiment_satisfied", "% Satisfacción", d.pcT_SATISFACCION + "%");
        html += Dashboard.Kpi(C.ink, "apartment", "Instituciones distintas", d.totaL_INSTITUCIONES);
        html += Dashboard.Kpi(C.primary, "public", "Departamentos con cobertura", d.totaL_DISTRITOS);
        document.querySelector("#Dashboard-Kpis").innerHTML = html;

        Dashboard.RenderBarList("Dashboard-BarModalidad", d.poR_MODALIDAD, (s) => Dashboard.ModalidadColor(s.etiqueta));
        Dashboard.RenderBarList("Dashboard-BarDepartamento", d.poR_DEPARTAMENTO, () => Dashboard.Color.primary);
        Dashboard.RenderBarList("Dashboard-BarEstado", d.poR_ESTADO_EVENTO, (s) => Dashboard.EstadoColor(s.etiqueta));
    },

    // Barras horizontales estilo ejecutivo: una fila por categoría, etiqueta a la
    // izquierda, track gris con relleno de color proporcional, cifra a la derecha.
    RenderBarList: (containerId, serie, colorFn) => {
        var contenedor = document.getElementById(containerId);
        if (!contenedor) return;

        if (!serie || !serie.length) {
            contenedor.innerHTML = '<p class="dash-empty">Sin datos para los filtros seleccionados.</p>';
            return;
        }

        var max = Math.max.apply(null, serie.map((s) => s.cantidad)) || 1;

        contenedor.innerHTML = serie.map((s) => {
            var pct = Math.round((s.cantidad * 100) / max);
            var color = colorFn(s);
            return '' +
                '<div class="dash-bar-row">' +
                '  <div class="dash-bar-lbl" title="' + s.etiqueta + '">' + s.etiqueta + '</div>' +
                '  <div class="dash-bar-track">' +
                '    <div class="dash-bar-fill" style="width:' + pct + '%; background:' + color + ';"></div>' +
                '  </div>' +
                '  <div class="dash-bar-count">' + s.cantidad + '</div>' +
                '</div>';
        }).join("");
    },
};

// Se llama directo (no en DOMContentLoaded) porque con pjax.js este script se
// re-ejecuta en cada navegación sin que haya un nuevo DOMContentLoaded.
Dashboard.Init();
