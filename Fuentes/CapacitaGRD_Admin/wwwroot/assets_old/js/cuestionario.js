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
                document.querySelector("#Modal-Cuestionarios-Preguntas tbody").innerHTML = '';

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
                Array.from(document.querySelectorAll("#Modal-Cuestionarios-Preguntas tbody tr")).forEach((item, i) => {
                    let pregunta = {};
                    $(item).find('td').each(function (iTD, itemTD) {

                        if (iTD == 0) {
                            pregunta.codigo = $(itemTD).data("codigo")
                        }

                        if (iTD == 1) {
                            pregunta.nombre = $(itemTD).find("input").val()
                        }

                        if (iTD == 2) {
                            pregunta.iD_TIPO_ENCUESTA_PREGUNTA = $(itemTD).find("select").val()
                        }

                        if (iTD == 3) {
                            pregunta.respuestas = $(itemTD).find("input").val()
                        }

                    })
                    preguntas.push(pregunta);
                })

                let params =
                {
                    id: parseInt(document.querySelector("#Modal-Cuestionarios").dataset.codigo),
                    "nombre": document.querySelector("#Modal-Cuestionarios-Nombre").value,
                    "respuestas": preguntas
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
                let html = '';
                html += '<tr>';
                html += '<td data-codigo="00000"  style="display: none;">00000</td>';
                html += '<td><input class="form-control" type="text" value="" /></td>';
                html += '<td><select class="form-control">';
                html += '<option value="1" >Rango</option>';
                html += '<option value="2" >Marcar</option>';
                html += '<option value="3" >Texto</option>';                
                html += '</select></td>';
                html += '<td><input class="form-control" type="text" value="" /></td>';
                html += '<td><button class="btn btn-primary btn-icon  btn-icon-mini btn-round" onclick="Cuestionarios.Remove(this);"><i class="material-icons">remove</i></button></td>'

                html += '</tr>';

                document.querySelector("#Modal-Cuestionarios-Preguntas tbody").insertAdjacentHTML('beforeend', html);

            });
        }
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

                document.querySelector("#Modal-Cuestionarios-Codigo").value = response.iD_ENCUESTA;
                document.querySelector("#Modal-Cuestionarios-Nombre").value = response.nombre;
                document.querySelector("#Modal-Cuestionarios").dataset.codigo = response.iD_ENCUESTA;

                document.querySelector(
                    "#Modal-Cuestionarios-Preguntas tbody"
                ).innerHTML = "";
                let html = "";
                response.respuestas.forEach((item, i) => {
                    html += "<tr>";
                    html +=
                        '<td data-codigo="' +
                    item.iD_RESPUESTA +
                        '"  style="display: none;">' +
                    item.iD_RESPUESTA +
                        "</td>";
                    html +=
                        '<td><input class="form-control" type="text" value="' +
                    item.nombre +
                        '" /></td>';
                    html += '<td><select class="form-control">';
                    html +=
                        '<option value="1" ' +
                    (item.iD_TIPO_ENCUESTA_PREGUNTA == "1" ? " selected " : " ") +
                        ">Rango</option>";
                    html +=
                        '<option value="3" ' +
                    (item.iD_TIPO_ENCUESTA_PREGUNTA == "3" ? " selected " : " ") +
                        ">Texto</option>";
                    html +=
                        '<option value="2" ' +
                    (item.iD_TIPO_ENCUESTA_PREGUNTA == "2" ? " selected " : " ") +
                        ">Marcar</option>";
                    html += "</select></td>";

                    html +=
                        '<td><input class="form-control" type="text" value="' +
                    (item.respuestas == null ? " " : item.respuestas) +
                        '" /></td>';
                    html +=
                        '<td><button class="btn btn-primary btn-icon  btn-icon-mini btn-round" onclick="Cuestionarios.Remove(this);"><i class="material-icons">remove</i></button></td>';

                    html += "</tr>";
                });

                document.querySelector(
                    "#Modal-Cuestionarios-Preguntas tbody"
                ).innerHTML = html;

                $("#Modal-Cuestionarios").modal({
                    keyboard: true,
                    backdrop: "static",
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