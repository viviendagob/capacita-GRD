var Paises = {
    BaseUrl: "/paises/",
    DataTable: null,

    Table: '#Paises-Tabla',


    Init: () => {
        setTimeout(() => {
            Paises.List();
        }, 100);

        Paises.Events(); 
 
    },

    Events: () => {
        let Paises_Agregar = document.querySelector("#Paises-Agregar");
        if (Paises_Agregar) {
            Paises_Agregar.addEventListener("click", e => {
                e.preventDefault();
                
                document.querySelector("#Modal-Paises-Id").value = "0000";
                document.querySelector("#Modal-Paises-Codigo").value = "";
                document.querySelector("#Modal-Paises-Nombre").value = "";
                document.querySelector("#Modal-Paises-Local").checked = false;
               
                $("#Modal-Paises").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Paises_Guardar = document.querySelector("#Paises-Guardar");
        if (Paises_Guardar) {
            Paises_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    id: document.querySelector("#Modal-Paises-Id").value,
                    coD_PAIS: document.querySelector("#Modal-Paises-Codigo").value,
                    nombre: document.querySelector("#Modal-Paises-Nombre").value,
                    eslocal: (document.querySelector("#Modal-Paises-Local").checked ? "1" : "0"),                    
                }

                let endpoint = Paises.BaseUrl + (params.id == "0000" ? "Agregar" : "Actualizar?id=" + params.id);
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
                                        $("#Modal-Paises").modal('hide');
                                        Paises.DataTable.ajax.reload();
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

        let Paises_Buscar = document.querySelector("#Paises-Buscar");
        if (Paises_Buscar) {
            Paises_Buscar.addEventListener("click", e => {
                e.preventDefault();

                let dato = document.querySelector("#Paises-Buscar-Nombre").value;
                if (dato.trim() == '') {
                    Paises.DataTable.column(2).search(dato).draw();
                } else {
                    Paises.DataTable.column(2).search(dato + '+', true, false).draw();
                }
                

            });
        }

        let Paises_Buscar_Nombre = document.querySelector("#Paises-Buscar-Nombre")
        Paises_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Paises_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Paises.BaseUrl + "Paginar";
        Paises.DataTable = $(Paises.Table)
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
                            dato: document.querySelector("#Paises-Buscar-Nombre").value
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
                    { data: "iD_PAIS", name: "iD_PAIS", width: "10%" },
                    { data: "coD_PAIS", name: "coD_PAIS", width: "10%" },
                    { data: "nombre", name: "nombre", width: "70%" },
                    {
                        data: "nombre",
                        name: "nombre",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Paises.Edit(' + row.iD_PAIS + ');" title="Editar">' +
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
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Paises.Delete(' + row.iD_PAIS + ');" title="Eliminar">' +
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
        let endpoint = Paises.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Paises-Id").value = response.iD_PAIS;
                document.querySelector("#Modal-Paises-Codigo").value = response.coD_PAIS;
                document.querySelector("#Modal-Paises-Nombre").value = response.nombre;
                document.querySelector("#Modal-Paises-Local").checked = (response.eslocal == "1" ? true : false); 
             
                $("#Modal-Paises").modal({
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

                let endpoint = Paises.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Paises").modal('hide');
                                Paises.DataTable.ajax.reload();
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