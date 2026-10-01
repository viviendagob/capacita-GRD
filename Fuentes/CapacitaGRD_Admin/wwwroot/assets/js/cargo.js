var Cargos = {
    BaseUrl: "/cargos/",
    DataTable: null,

    Table: '#Cargos-Tabla',


    Init: () => {
         
        setTimeout(() => {
                Cargos.List();            
        }, 100);

        Cargos.Events(); 
 
    },

    Events: () => {
        let Cargos_Agregar = document.querySelector("#Cargos-Agregar");
        if (Cargos_Agregar) {
            Cargos_Agregar.addEventListener("click", e => {
                e.preventDefault();
                
                document.querySelector("#Modal-Cargos-Id").value = "0000";
                document.querySelector("#Modal-Cargos-Nombre").value = "";
               
                $("#Modal-Cargos").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Cargos_Guardar = document.querySelector("#Cargos-Guardar");
        if (Cargos_Guardar) {
            Cargos_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    id: document.querySelector("#Modal-Cargos-Id").value,
                    nombre: document.querySelector("#Modal-Cargos-Nombre").value,
                }

                let endpoint = Cargos.BaseUrl + (params.id == "0000" ? "Agregar" : "Actualizar?id=" + params.id);
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
                                        $("#Modal-Cargos").modal('hide');
                                        Cargos.DataTable.ajax.reload();
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

        let Cargos_Buscar = document.querySelector("#Cargos-Buscar");
        if (Cargos_Buscar) {
            Cargos_Buscar.addEventListener("click", e => {
                e.preventDefault();

                Cargos.DataTable.ajax.reload(); 
            });
        }

        let Cargos_Buscar_Nombre = document.querySelector("#Cargos-Buscar-Nombre")
        Cargos_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Cargos_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Cargos.BaseUrl + "Paginar";
        Cargos.DataTable = $(Cargos.Table)
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
                            dato: document.querySelector("#Cargos-Buscar-Nombre").value
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
                    { data: "iD_CARGO", name: "iD_CARGO", width: "10%" },
                    { data: "nombre", name: "nombre", width: "80%" },
                    {
                        data: "iD_CARGO",
                        name : "iD_CARGO",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Cargos.Edit(' + row.iD_CARGO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "iD_CARGO",
                        width: "5%",
                        name: "iD_CARGO",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Cargos.Delete(' + row.iD_CARGO + ');" title="Eliminar">' +
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
        let endpoint = Cargos.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Cargos-Id").value = data.iD_CARGO;
                document.querySelector("#Modal-Cargos-Nombre").value = data.nombre;                 
             
                $("#Modal-Cargos").modal({
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

                let endpoint = Cargos.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Cargos").modal('hide');
                                Cargos.DataTable.ajax.reload();
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