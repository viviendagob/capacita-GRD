var Encuestas = {
    BaseUrl: "/encuestas/",
    DataTable: null,

    Table: '#Encuestas-Tabla',


    Init: () => {
        setTimeout(() => {
            Encuestas.List();
        }, 100);

        Encuestas.Events(); 
 
    },

    Events: () => {
        let Encuestas_Agregar = document.querySelector("#Encuestas-Agregar");
        if (Encuestas_Agregar) {
            Encuestas_Agregar.addEventListener("click", e => {
                e.preventDefault();

                document.querySelector("#Modal-Encuestas").dataset.codigo = 0;
                document.querySelector("#Modal-Encuestas-Codigo").value = "0000";
                document.querySelector("#Modal-Encuestas-Nombre").value = "";
                document.querySelector("#Modal-Encuestas-Preguntas tbody").innerHTML = '';

                $("#Modal-Encuestas").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Encuestas_Guardar = document.querySelector("#Encuestas-Guardar");
        if (Encuestas_Guardar) {
            Encuestas_Guardar.addEventListener("click", e => {
                e.preventDefault();                 

                let preguntas = [];
                Array.from(document.querySelectorAll("#Modal-Encuestas-Preguntas tbody tr")).forEach((item, i) => {
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
                    id: parseInt(document.querySelector("#Modal-Encuestas").dataset.codigo),
                    "nombre": document.querySelector("#Modal-Encuestas-Nombre").value,
                    "respuestas": preguntas
                }
                 

                let endpoint = Encuestas.BaseUrl + (params.id == 0 ? "Agregar" : "Actualizar?id=" + params.id);
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
                                        $("#Modal-Encuestas").modal('hide');
                                        Encuestas.DataTable.ajax.reload();
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

        let Encuestas_Buscar = document.querySelector("#Encuestas-Buscar");
        if (Encuestas_Buscar) {
            Encuestas_Buscar.addEventListener("click", e => {
                e.preventDefault();

                let dato = document.querySelector("#Encuestas-Buscar-Nombre").value;
                if (dato.trim() == '') {
                    Encuestas.DataTable.column(2).search(dato).draw();
                } else {
                    Encuestas.DataTable.column(2).search(dato + '+', true, false).draw();
                }
                

            });
        }

        let Encuestas_Buscar_Nombre = document.querySelector("#Encuestas-Buscar-Nombre")
        Encuestas_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Encuestas_Buscar.click();
            }
        });

        let Modal_Encuestas_Preguntas_Agregar = document.querySelector("#Modal-Encuestas-Preguntas-Agregar");
        if (Modal_Encuestas_Preguntas_Agregar) {
            Modal_Encuestas_Preguntas_Agregar.addEventListener("click", e => {
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
                html += '<td><button class="btn btn-primary btn-icon  btn-icon-mini btn-round" onclick="Encuestas.Remove(this);"><i class="material-icons">remove</i></button></td>'

                html += '</tr>';

                document.querySelector("#Modal-Encuestas-Preguntas tbody").insertAdjacentHTML('beforeend', html);

            });
        }
    },

    List: function () {
        let endpoint = Encuestas.BaseUrl + "Paginar";
        Encuestas.DataTable = $(Encuestas.Table)
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
                            dato: document.querySelector("#Encuestas-Buscar-Nombre").value
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
                    { data: "iD_ENCUESTA", name: "iD_ENCUESTA", width: "10%" },
                     { data: "nombre", name: "nombre", width: "80%" },
                    {
                        data: "nombre",
                        name: "nombre",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Encuestas.Edit(' + row.iD_ENCUESTA + ');" title="Editar">' +
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
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Encuestas.Delete(' + row.iD_ENCUESTA + ');" title="Eliminar">' +
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
        let endpoint = Encuestas.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Encuestas-Codigo").value = response.iD_ENCUESTA;
                document.querySelector("#Modal-Encuestas-Nombre").value = response.nombre;
                document.querySelector("#Modal-Encuestas").dataset.codigo = response.iD_ENCUESTA;

                document.querySelector(
                    "#Modal-Encuestas-Preguntas tbody"
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
                        '<td><button class="btn btn-primary btn-icon  btn-icon-mini btn-round" onclick="Encuestas.Remove(this);"><i class="material-icons">remove</i></button></td>';

                    html += "</tr>";
                });

                document.querySelector(
                    "#Modal-Encuestas-Preguntas tbody"
                ).innerHTML = html;

                $("#Modal-Encuestas").modal({
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

                let endpoint = Encuestas.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Encuestas").modal('hide');
                                Encuestas.DataTable.ajax.reload();
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