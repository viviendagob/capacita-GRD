var Documentos = {
    BaseUrl: "/documentos/",
    DataTable: null,

    Table: '#Documentos-Tabla',


    Init: () => {
         
        setTimeout(() => {
                Documentos.List();            
        }, 100);

        Documentos.Events(); 
 
    },

    Events: () => {
        let Documentos_Agregar = document.querySelector("#Documentos-Agregar");
        if (Documentos_Agregar) {
            Documentos_Agregar.addEventListener("click", e => {
                e.preventDefault();
                
                document.querySelector("#Modal-Documentos-Id").value = "0000";
                document.querySelector("#Modal-Documentos-Nombre").value = "";
               
                $("#Modal-Documentos").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Documentos_Guardar = document.querySelector("#Documentos-Guardar");
        if (Documentos_Guardar) {
            Documentos_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    id: document.querySelector("#Modal-Documentos-Id").value,
                    nombre: document.querySelector("#Modal-Documentos-Nombre").value,
                }

                let endpoint = Documentos.BaseUrl + (params.id == "0000" ? "Agregar" : "Actualizar?id=" + params.id);
                let metodo = (params.id == "0000" ? "POST" : "PUT");

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
                                        $("#Modal-Documentos").modal('hide');
                                        Documentos.DataTable.ajax.reload();
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

        let Documentos_Buscar = document.querySelector("#Documentos-Buscar");
        if (Documentos_Buscar) {
            Documentos_Buscar.addEventListener("click", e => {
                e.preventDefault();

                Documentos.DataTable.ajax.reload(); 
            });
        }

        let Documentos_Buscar_Nombre = document.querySelector("#Documentos-Buscar-Nombre")
        Documentos_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Documentos_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Documentos.BaseUrl + "Paginar";
        Documentos.DataTable = $(Documentos.Table)
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
                            dato: document.querySelector("#Documentos-Buscar-Nombre").value
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
                    { data: "iD_TIPO_DOCUMENTO_REQUERIDO", name: "iD_TIPO_DOCUMENTO_REQUERIDO", width: "10%" },
                    { data: "nombre", name: "nombre", width: "80%" },
                    {
                        data: "iD_TIPO_DOCUMENTO_REQUERIDO",
                        name: "iD_TIPO_DOCUMENTO_REQUERIDO",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Documentos.Edit(' + row.iD_TIPO_DOCUMENTO_REQUERIDO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "iD_TIPO_DOCUMENTO_REQUERIDO",
                        width: "5%",
                        name: "iD_TIPO_DOCUMENTO_REQUERIDO",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Documentos.Delete(' + row.iD_TIPO_DOCUMENTO_REQUERIDO + ');" title="Eliminar">' +
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
         
    },

    Edit: function (codigo) {
        let endpoint = Documentos.BaseUrl + "Obtener/" + codigo; 
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

                let data = response;

                document.querySelector("#Modal-Documentos-Id").value = data.iD_TIPO_DOCUMENTO_REQUERIDO;
                document.querySelector("#Modal-Documentos-Nombre").value = data.nombre;                 
             
                $("#Modal-Documentos").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

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

                let endpoint = Documentos.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Documentos").modal('hide');
                                Documentos.DataTable.ajax.reload();
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