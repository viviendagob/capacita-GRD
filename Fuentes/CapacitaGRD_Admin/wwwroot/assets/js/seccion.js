var Secciones = {
    BaseUrl: "/secciones/",
    DataTable: null,

    Table: '#Secciones-Tabla',


    Init: () => {
        setTimeout(() => {
            if (!Secciones.DataTable) {
                Secciones.DataTable = $(Secciones.Table).DataTable({
                    select: true,
                    bFilter: true,
                    bInfo: false,
                    language: {
                        url: "../assets/plugins/datatables/Spanish.json",
                    }, columnDefs: [
                        {
                            targets: [0],
                            visible: false,
                            width: "10%"
                        },
                        {
                            targets: [1],
                            width: "80%"
                        },
                        {
                            targets: [2, 3],
                            width: "5%",
                            className: 'text-center'
                        }
                    ]
                });

                setTimeout(() => {
                    document.querySelector("#Secciones-Tabla_filter").style.display = "none";
                }, 100)
            };           
        }, 100);

        Secciones.Events(); 
 
    },

    Events: () => {
        let Secciones_Agregar = document.querySelector("#Secciones-Agregar");
        if (Secciones_Agregar) {
            Secciones_Agregar.addEventListener("click", e => {
                e.preventDefault();
                
                document.querySelector("#Modal-Secciones-Codigo").value = "0000";
                document.querySelector("#Modal-Secciones-Nombre").value = "";
               
                $("#Modal-Secciones").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Secciones_Guardar = document.querySelector("#Secciones-Guardar");
        if (Secciones_Guardar) {
            Secciones_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    codigo: document.querySelector("#Modal-Secciones-Codigo").value,
                    nombre: document.querySelector("#Modal-Secciones-Nombre").value,
                }

                let endpoint = Secciones.BaseUrl + (params.codigo == "0000" ? "Agregar" : "Actualizar?id=" + params.codigo);
                let metodo =  (params.codigo == "0000" ? "POST" : "PUT");

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
                                        $("#Modal-Secciones").modal('hide');
                                        Secciones.List();
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

        let Secciones_Buscar = document.querySelector("#Secciones-Buscar");
        if (Secciones_Buscar) {
            Secciones_Buscar.addEventListener("click", e => {
                e.preventDefault();

                Secciones.DataTable.column(1).search(
                    document.querySelector("#Secciones-Buscar-Nombre").value
                ).draw();
  
            });
        }

        let Secciones_Buscar_Nombre = document.querySelector("#Secciones-Buscar-Nombre")
        Secciones_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Secciones_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Secciones.BaseUrl + "Listar";
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
                if ($.fn.DataTable.isDataTable(Secciones.Table)) {
                    Secciones.DataTable.clear().draw();
                }

                response.forEach((item, i) => {
                    var data = [
                        item.iD_SECCION,
                        item.nombre,
                        (item.iD_SECCION == 1 ? "" : '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Secciones.Edit(' + item.iD_SECCION + ');" title="Editar">' +
                            '<i class= "material-icons"> border_color</i>' +
                            '</button>')
                        ,
                        (item.iD_SECCION == 1 ? "" : '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Secciones.Delete(' + item.iD_SECCION + ');" title="Eliminar">' +
                        '<i class= "material-icons"> delete_forever</i>' +
                        '</button> ')                         
                    ];
                    Secciones.DataTable.row.add(data).draw(false);
                });
                $(Secciones.DataTable).DataTable().columns.adjust().draw();

            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

            });
 
    },

    Remove: function(obj){
         
    },

    Edit: function (codigo) {
        let endpoint = Secciones.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Secciones-Codigo").value = data.iD_SECCION;
                document.querySelector("#Modal-Secciones-Nombre").value = data.nombre;                 
             
                $("#Modal-Secciones").modal({
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

                let endpoint = Secciones.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Secciones").modal('hide');
                                Secciones.List();
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