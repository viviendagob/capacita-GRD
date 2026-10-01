var EstadisticasEncuesta = {
    BaseUrl: "/EstadisticasEncuesta/",

    Init: () => {
        setTimeout(() => {
            $('#EstadisticasEncuesta-Evento').selectpicker('destroy');
            $('#EstadisticasEncuesta-Evento').select2();
        }, 300);

        document.querySelector("#EstadisticasEncuesta-Buscar").addEventListener("click", EstadisticasEncuesta.Buscar);
        document.querySelector("#EstadisticasEncuesta-Descargar").addEventListener("click", EstadisticasEncuesta.Descargar);
    },

    Buscar: () => {
        var idEvento = parseInt(document.querySelector("#EstadisticasEncuesta-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }

        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(EstadisticasEncuesta.BaseUrl + "Estadisticas?idEvento=" + idEvento)
            .then((response) => response.json())
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                if (data.success === false) {
                    swal("Upss", data.mensaje, "warning");
                    return;
                }
                EstadisticasEncuesta.Render(data);
            })
            .catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salió mal", String(error), "warning");
            });
    },

    Render: (data) => {
        document.querySelector("#EstadisticasEncuesta-Resumen").style.display = "flex";
        document.querySelector("#EstadisticasEncuesta-TotalAsistentes").textContent = data.totaL_ASISTENTES;
        document.querySelector("#EstadisticasEncuesta-TotalRespondieron").textContent = data.totaL_RESPONDIERON;
        document.querySelector("#EstadisticasEncuesta-Meta").textContent = data.metA_RESPUESTA;
        document.querySelector("#EstadisticasEncuesta-PctRespuesta").textContent = data.porcentajE_RESPUESTA + "%";
        document.querySelector("#EstadisticasEncuesta-PctRespuesta").className =
            data.porcentajE_RESPUESTA >= data.metA_RESPUESTA ? "text-success" : "text-danger";
        document.querySelector("#EstadisticasEncuesta-PctSatisfechos").textContent = data.porcentajE_SATISFECHOS + "%";

        var contenedor = document.querySelector("#EstadisticasEncuesta-Preguntas");
        contenedor.innerHTML = "";

        if (!data.preguntas || !data.preguntas.length) {
            contenedor.innerHTML = '<p class="text-muted">Todavía no hay respuestas registradas para este evento.</p>';
            return;
        }

        data.preguntas.forEach((pregunta) => {
            var total = pregunta.alternativas.reduce((acc, a) => acc + a.cantidad, 0);

            var bloque = document.createElement("div");
            bloque.className = "mb-4";

            var titulo = document.createElement("h6");
            titulo.innerHTML = "<strong>" + pregunta.pregunta + "</strong>";
            bloque.appendChild(titulo);

            pregunta.alternativas.forEach((alt) => {
                var pct = total > 0 ? Math.round((alt.cantidad * 100) / total) : 0;

                var fila = document.createElement("div");
                fila.className = "mb-2";
                fila.innerHTML =
                    '<div class="d-flex justify-content-between"><span>' + alt.respuesta + '</span>' +
                    '<span>' + alt.cantidad + ' (' + pct + '%)</span></div>' +
                    '<div class="progress" style="height: 8px;">' +
                    '<div class="progress-bar bg-primary" role="progressbar" style="width: ' + pct + '%;"></div>' +
                    '</div>';
                bloque.appendChild(fila);
            });

            contenedor.appendChild(bloque);
        });
    },

    Descargar: () => {
        var idEvento = parseInt(document.querySelector("#EstadisticasEncuesta-Evento").value) || 0;
        if (!idEvento) {
            swal("Atención", "Selecciona un evento", "warning");
            return;
        }
        window.location.href = EstadisticasEncuesta.BaseUrl + "Excel?idEvento=" + idEvento;
    },
};

// Se llama directo (no en DOMContentLoaded) porque con pjax.js este script se
// re-ejecuta en cada navegación sin que haya un nuevo DOMContentLoaded.
EstadisticasEncuesta.Init();
