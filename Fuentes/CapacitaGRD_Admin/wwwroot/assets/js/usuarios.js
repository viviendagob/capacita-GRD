var Usuarios = {
    BaseUrl: "/usuarios/",
    DataTable: null,

    Table: '#Usuarios-Tabla',


    Init: () => {
        setTimeout(() => {
            Usuarios.List();
        }, 100);

        Usuarios.Events(); 
 
    },

    Events: () => {
        let Usuarios_Agregar = document.querySelector("#Usuarios-Agregar");
        if (Usuarios_Agregar) {
            Usuarios_Agregar.addEventListener("click", e => {
                e.preventDefault();
                
                document.querySelector("#Modal-Usuarios-Id").value = "0000";
                document.querySelector("#Modal-Usuarios-Usuario").value = "";

                let claveElement = document.querySelector("#Modal-Usuarios-Clave");

                if (claveElement) {
                    claveElement.value = "";
                } else {
                    console.error("El elemento con ID 'Modal-Usuarios-Clave' no existe.");
                }

                document.querySelector("#Modal-Usuarios-Estado").checked = true;
               
                $("#Modal-Usuarios").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Usuarios_Guardar = document.querySelector("#Usuarios-Guardar");
        if (Usuarios_Guardar) {
            Usuarios_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    id: document.querySelector("#Modal-Usuarios-Id").value,
                    usuario: document.querySelector("#Modal-Usuarios-Usuario").value,
                    clave: document.querySelector("#Modal-Usuarios-Clave").value,
                    estado: (document.querySelector("#Modal-Usuarios-Estado").checked ? 1 : 0),                    
                }

                let endpoint = Usuarios.BaseUrl + (params.id == "0000" ? "Agregar" : "Actualizar?id=" + params.id);
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
                                        $("#Modal-Usuarios").modal('hide');
                                        Usuarios.DataTable.ajax.reload();
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

        let Usuarios_Buscar = document.querySelector("#Usuarios-Buscar");
        if (Usuarios_Buscar) {
            Usuarios_Buscar.addEventListener("click", e => {
                e.preventDefault();

                let dato = document.querySelector("#Usuarios-Buscar-Nombre").value;
                if (dato.trim() == '') {
                    Usuarios.DataTable.column(2).search(dato).draw();
                } else {
                    Usuarios.DataTable.column(2).search(dato + '+', true, false).draw();
                }
                

            });
        }

        let Usuarios_Buscar_Nombre = document.querySelector("#Usuarios-Buscar-Nombre")
        Usuarios_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Usuarios_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Usuarios.BaseUrl + "Paginar";
        Usuarios.DataTable = $(Usuarios.Table)
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
                            dato: document.querySelector("#Usuarios-Buscar-Nombre").value
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
                    { data: "iD_USUARIO", name: "iD_USUARIO", width: "10%" },
                    { data: "usuario", name: "usuario", width: "70%" },
                    {
                        data: "estado",
                        name: "estado",
                        width: "10%",
                        render: function (data, type, row) {
                            if (row.estado === 1) {
                                return '<h5><span class="badge badge-pill badge-success">Activo</span></h5>';
                            } else {
                                return '<h5><span class="badge badge-pill badge-danger">Inactivo</span></h5>';
                            }
                        }
                    },
                    {
                        data: "usuario",
                        name: "usuario",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Usuarios.Edit(' + row.iD_USUARIO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "usuario",
                        width: "5%",
                        name: "usuario",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Usuarios.Delete(' + row.iD_USUARIO + ');" title="Eliminar">' +
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
        let endpoint = Usuarios.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Usuarios-Id").value = response.iD_USUARIO;
                document.querySelector("#Modal-Usuarios-Usuario").value = response.usuario;
                document.querySelector("#Modal-Usuarios-Clave").value = "";
                document.querySelector("#Modal-Usuarios-Estado").checked = (response.estado == "1" ? true : false); 
             
                $("#Modal-Usuarios").modal({
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

                let endpoint = Usuarios.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Usuarios").modal('hide');
                                Usuarios.DataTable.ajax.reload();
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