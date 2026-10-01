var Cuestionarios = {
    BaseUrl: "/cuestionarios/",
    DataTable: null,

    Table: '#Cuestionarios-Tabla',


    Init: () => {
        setTimeout(() => {
            Cuestionarios.List();
        }, 100);

        Cuestionarios.Events();

    },

    Events: () => {
        let Cuestionarios_Agregar = document.querySelector("#Cuestionarios-Agregar");
        if (Cuestionarios_Agregar) {
            Cuestionarios_Agregar.addEventListener("click", e => {
                e.preventDefault();

                document.querySelector("#Modal-Cuestionarios").dataset.codigo = 0;
                document.querySelector("#Modal-Cuestionarios-Codigo").value = "0000";
                document.querySelector("#Modal-Cuestionarios-Nombre").value = "";
                document.querySelector("#Modal-Cuestionarios-Preguntas").innerHTML = '';

                $("#Modal-Cuestionarios").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Cuestionarios_Guardar = document.querySelector("#Cuestionarios-Guardar");
        if (Cuestionarios_Guardar) {
            Cuestionarios_Guardar.addEventListener("click", e => {
                e.preventDefault();

                let preguntas = [];
                Array.from(document.querySelectorAll("#Modal-Cuestionarios-Preguntas .pregunta-block")).forEach((bloque) => {
                    let respuestas = [];
                    Array.from(bloque.querySelectorAll(".opciones-table tbody tr")).forEach((fila) => {
                        respuestas.push({
                            nombre: fila.querySelector(".opcion-nombre").value
                        });
                    });

                    preguntas.push({
                        nombre: bloque.querySelector(".pregunta-nombre").value,
                        peso: parseInt(bloque.querySelector(".pregunta-peso").value) || 0,
                        respuestas: respuestas
                    });
                });

                let params =
                {
                    id: parseInt(document.querySelector("#Modal-Cuestionarios").dataset.codigo),
                    "nombre": document.querySelector("#Modal-Cuestionarios-Nombre").value,
                    "preguntas": preguntas
                }


                let endpoint = Cuestionarios.BaseUrl + (params.id == 0 ? "Agregar" : "Actualizar?id=" + params.id);
                let metodo = (params.id == 0 ? "POST" : "PUT");

                swal({
                    title: "Confirme?",
                    text: "Esta seguro que desea guardar los datos",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#FF3636",
                    confirmButtonText: "Si, guardar",
                    closeOnConfirm: false
                }, function (isConfirm) {
                    if (isConfirm) {

                        fetch(endpoint, {
                            method: metodo,
                            body: JSON.stringify(params),
                            headers: {
                                "Content-Type": "application/json",
                            }
                        })
                            .then((response) => {
                                return response.json();
                            })
                            .then((response) => {

                                if (response.success) {
                                    swal({
                                        title: response.titulo,
                                        text: response.mensaje,
                                        type: "success"
                                    }, () => {
                                        $("#Modal-Cuestionarios").modal('hide');
                                        Cuestionarios.DataTable.ajax.reload();
                                    });
                                } else {
                                    swal(response.titulo, response.mensaje, "warning");
                                }

                            }).catch((error) => {

                                swal("Upss, algo salio mal", error, "warning");
                            });
                    }
                });

            });
        }

        let Cuestionarios_Buscar = document.querySelector("#Cuestionarios-Buscar");
        if (Cuestionarios_Buscar) {
            Cuestionarios_Buscar.addEventListener("click", e => {
                e.preventDefault();

                let dato = document.querySelector("#Cuestionarios-Buscar-Nombre").value;
                if (dato.trim() == '') {
                    Cuestionarios.DataTable.column(2).search(dato).draw();
                } else {
                    Cuestionarios.DataTable.column(2).search(dato + '+', true, false).draw();
                }


            });
        }

        let Cuestionarios_Buscar_Nombre = document.querySelector("#Cuestionarios-Buscar-Nombre")
        Cuestionarios_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Cuestionarios_Buscar.click();
            }
        });

        let Modal_Cuestionarios_Preguntas_Agregar = document.querySelector("#Modal-Cuestionarios-Preguntas-Agregar");
        if (Modal_Cuestionarios_Preguntas_Agregar) {
            Modal_Cuestionarios_Preguntas_Agregar.addEventListener("click", e => {
                e.preventDefault();

                document.querySelector("#Modal-Cuestionarios-Preguntas")
                    .insertAdjacentHTML('beforeend', Cuestionarios.PreguntaHtml());
            });
        }
    },

    PreguntaHtml: function (pregunta) {
        pregunta = pregunta || { nombre: "", peso: 0, respuestas: [] };

        let html = '';
        html += '<div class="card pregunta-block">';
        html += '  <div class="body">';
        html += '    <div class="row">';
        html += '      <div class="col-sm-8">';
        html += '        <label>Nombre de la pregunta</label>';
        html += '        <input class="form-control pregunta-nombre" type="text" value="' + (pregunta.nombre || '') + '" autocomplete="off" />';
        html += '      </div>';
        html += '      <div class="col-sm-2">';
        html += '        <label>Peso</label>';
        html += '        <input class="form-control pregunta-peso" type="number" min="0" value="' + (pregunta.peso || 0) + '" />';
        html += '      </div>';
        html += '      <div class="col-sm-2 text-right" style="padding-top: 30px;">';
        html += '        <button type="button" class="btn btn-danger btn-icon btn-icon-mini btn-round" title="Eliminar pregunta" onclick="Cuestionarios.RemovePregunta(this);"><i class="material-icons">delete</i></button>';
        html += '      </div>';
        html += '    </div>';
        html += '    <div class="row mt-2">';
        html += '      <div class="col-sm-12 table-responsive">';
        html += '        <table class="table table-sm m-b-0 opciones-table">';
        html += '          <thead>';
        html += '            <tr>';
        html += '              <th>Opciones de respuesta</th>';
        html += '              <th style="width: 5%;">';
        html += '                <button type="button" class="btn btn-default btn-icon btn-icon-mini btn-round" title="Agregar opción de respuesta" onclick="Cuestionarios.AddOpcion(this);"><i class="material-icons">add</i></button>';
        html += '              </th>';
        html += '            </tr>';
        html += '          </thead>';
        html += '          <tbody>';
        (pregunta.respuestas || []).forEach((opcion) => {
            html += Cuestionarios.OpcionHtml(opcion);
        });
        html += '          </tbody>';
        html += '        </table>';
        html += '      </div>';
        html += '    </div>';
        html += '  </div>';
        html += '</div>';

        return html;
    },

    OpcionHtml: function (opcion) {
        opcion = opcion || { nombre: "" };

        let html = '';
        html += '<tr>';
        html += '  <td><input class="form-control opcion-nombre" type="text" value="' + (opcion.nombre || '') + '" autocomplete="off" /></td>';
        html += '  <td><button type="button" class="btn btn-primary btn-icon btn-icon-mini btn-round" title="Eliminar opción" onclick="Cuestionarios.RemoveOpcion(this);"><i class="material-icons">remove</i></button></td>';
        html += '</tr>';

        return html;
    },

    AddOpcion: function (obj) {
        let tbody = obj.closest(".pregunta-block").querySelector(".opciones-table tbody");
        tbody.insertAdjacentHTML('beforeend', Cuestionarios.OpcionHtml());
    },

    RemoveOpcion: function (obj) {
        obj.closest("tr").remove();
    },

    RemovePregunta: function (obj) {
        obj.closest(".pregunta-block").remove();
    },

    List: function () {
        let endpoint = Cuestionarios.BaseUrl + "Paginar";
        Cuestionarios.DataTable = $(Cuestionarios.Table)
            .on("draw.dt", function () {
                document.querySelector(".page-loader-wrapper").style.display = "none";
            })
            .DataTable({
                processing: true,
                serverSide: true,
                stateSave: false,
                lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                select: true,
                searching: false,

                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, "Todos"],
                ],
                ajax: {
                    url: endpoint,
                    type: 'POST',
                    contentType: 'application/json',
                    dataType: "json",
                    data: function (data) {
                        var params = {
                            filtro: data,
                            dato: document.querySelector("#Cuestionarios-Buscar-Nombre").value
                        }
                        return JSON.stringify(params);
                    },
                    beforeSend: function () {
                        document.querySelector(".page-loader-wrapper").style.display =
                            "block";
                    },
                },
                columnDefs: [
                    {
                        targets: [0],
                        visible: false,
                    },
                ],

                columns: [
                    { data: "iD_CUESTIONARIO", name: "iD_CUESTIONARIO", width: "10%" },
                     { data: "nombre", name: "nombre", width: "80%" },
                    {
                        data: "nombre",
                        name: "nombre",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Cuestionarios.Edit(' + row.iD_CUESTIONARIO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "nombre",
                        width: "5%",
                        name: "nombre",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Cuestionarios.Delete(' + row.iD_CUESTIONARIO + ');" title="Eliminar">' +
                                '<i class= "material-icons"> delete_forever</i>' +
                                '</button> ';

                            return html;
                        },
                    },

                    {
                        data: "nombre",
                        width: "5%",
                        name: "nombre",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-default btn-icon  btn-icon-mini btn-round" onclick="javascript:Cuestionarios.Previo(' + row.iD_CUESTIONARIO + ');" title="Vista previa">' +
                                '<i class= "material-icons"> visibility</i>' +
                                '</button> ';

                            return html;
                        },
                    },
                ],
                select: true,
                buttons: [],

                language: {
                    url: "../assets/plugins/datatables/Spanish.json",
                },
            });

    },

    Remove: function(obj){
        if (obj.parentElement.parentElement.tagName == "TR") {
            obj.parentElement.parentElement.remove();
        }
    },

    Edit: function (codigo) {
        let endpoint = Cuestionarios.BaseUrl + "Obtener/" + codigo;
        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(endpoint, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            }
        })
            .then((response) => {
                return response.json();
            })
            .then((response) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";

                document.querySelector("#Modal-Cuestionarios-Codigo").value = response.iD_CUESTIONARIO;
                document.querySelector("#Modal-Cuestionarios-Nombre").value = response.nombre;
                document.querySelector("#Modal-Cuestionarios").dataset.codigo = response.iD_CUESTIONARIO;

                let html = "";
                (response.preguntas || []).forEach((pregunta) => {
                    html += Cuestionarios.PreguntaHtml(pregunta);
                });

                document.querySelector("#Modal-Cuestionarios-Preguntas").innerHTML = html;

                $("#Modal-Cuestionarios").modal({
                    keyboard: true,
                    backdrop: "static",
                });

            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

             });

    },

    Previo: function (codigo) {
        let endpoint = Cuestionarios.BaseUrl + "Obtener/" + codigo;
        document.querySelector(".page-loader-wrapper").style.display = "block";
        fetch(endpoint, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            }
        })
            .then((response) => {
                return response.json();
            })
            .then((response) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";

                let html = '<h5 class="mb-3"><strong>' + response.nombre + '</strong></h5>';

                (response.preguntas || []).forEach((pregunta, i) => {
                    html += '<div class="form-group mb-3">';
                    html += '<label><strong>' + (i + 1) + '. ' + pregunta.nombre + '</strong> <span class="badge badge-info">Peso: ' + pregunta.peso + '</span></label>';

                    (pregunta.respuestas || []).forEach((opcion, j) => {
                        html += '<div class="form-check">';
                        html += '<input class="form-check-input" type="radio" name="Previo-Pregunta-' + i + '" disabled />';
                        html += '<label class="form-check-label">' + opcion.nombre + '</label>';
                        html += '</div>';
                    });

                    html += '</div>';
                });

                document.querySelector("#Modal-Cuestionarios-Previo-Contenido").innerHTML = html;

                $("#Modal-Cuestionarios-Previo").modal({
                    keyboard: true,
                    backdrop: 'static'
                });

            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

            });
    },

    Update : () =>{

    },

    Clear: () => {

    },

    Delete: function (codigo) {
        swal({
            title: "Confirme?",
            text: "Esta seguro de eliminar el item",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#FF3636",
            confirmButtonText: "Si, eliminar",
            closeOnConfirm: false
        }, function (isConfirm) {
            if (isConfirm) {

                let endpoint = Cuestionarios.BaseUrl + "Eliminar/" + codigo;
                document.querySelector(".page-loader-wrapper").style.display = "block";
                fetch(endpoint, {
                    method: "DELETE",
                    headers: {
                        "Content-Type": "application/json",
                    }
                })
                    .then((response) => {
                        return response.json();
                    })
                    .then((response) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";

                        if (response.success) {
                            swal({
                                title: response.titulo,
                                text: response.mensaje,
                                type: "success"
                            }, () => {
                                $("#Modal-Cuestionarios").modal('hide');
                                Cuestionarios.DataTable.ajax.reload();
                            });
                        } else {
                            swal(response.titulo, response.mensaje, "warning");
                        }

                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    });

            }
        });

    },

    getFormData: function (object) {
        const formData = new FormData();
        Object.keys(object).forEach(key => formData.append(key, object[key]));
        return formData;
    },

}

function groupBy(objectArray, property) {
    return objectArray.reduce(function (acc, obj) {
      var key = obj[property];
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push(obj);
      return acc;
    }, {});
}
