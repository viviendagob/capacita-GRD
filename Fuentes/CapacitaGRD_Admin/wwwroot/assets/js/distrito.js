var Distritos = {
    BaseUrl: "/distritos/",
    DataTable: null,
    Paises: null,
    Table: '#Distritos-Tabla',
    esLocal: 0,

    Init: () => {       
        Distritos.esLocal = Distritos.Paises.find(p => p.eslocal == 1).iD_PAIS;

        setTimeout(() => {                         
            $('#Distritos-Buscar-Pais').selectpicker('destroy');
            $('#Distritos-Buscar-Pais').select2().on('select2:selecting', function (e) {               
            }).on("select2:select", function (e) {
                Distritos.DataTable.ajax.reload();                
            });

            $('#Modal-Distritos-Pais').selectpicker('destroy');
            $('#Modal-Distritos-Pais').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {                
            });

            
            $('#Distritos-Buscar-Pais').val(Distritos.esLocal).trigger('change');

            Distritos.List();
        }, 100);


        Distritos.Events(); 
 
    },

    Events: () => {
        let Distritos_Agregar = document.querySelector("#Distritos-Agregar");
        if (Distritos_Agregar) {
            Distritos_Agregar.addEventListener("click", e => {
                e.preventDefault();

                let local = Distritos.Paises.find(p => p.eslocal == 1)

                document.querySelector("#Modal-Distritos-Id").value = "0000";
                document.querySelector("#Modal-Distritos-Ubigeo").value = "";
                document.querySelector("#Modal-Distritos-Capital").value = "";
                document.querySelector("#Modal-Distritos-Distrito").value = "";
                document.querySelector("#Modal-Distritos-Provincia").value = "";
                document.querySelector("#Modal-Distritos-Departamento").value = "";
                document.querySelector("#Modal-Distritos-Categoria").value = "";

                $('#Modal-Distritos-Pais').val(Distritos.esLocal).trigger('change');
                 
                $("#Modal-Distritos").modal({
                    keyboard: true,
                    backdrop: 'static'
                })

            });
        }

        let Distritos_Guardar = document.querySelector("#Distritos-Guardar");
        if (Distritos_Guardar) {
            Distritos_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
 
                let params = {
                    id: document.querySelector("#Modal-Distritos-Id").value,

                    codigO_DEPARTAMENTO: document.querySelector("#Modal-Distritos-Ubigeo").value.substring(0, 2),
                    departamento: document.querySelector("#Modal-Distritos-Departamento").value,

                    codigO_PROVINCIA: document.querySelector("#Modal-Distritos-Ubigeo").value.substring(0, 4),
                    provincia: document.querySelector("#Modal-Distritos-Provincia").value,

                    codigO_DISTRITO: document.querySelector("#Modal-Distritos-Ubigeo").value,
                    distrito: document.querySelector("#Modal-Distritos-Distrito").value,

                    capital: document.querySelector("#Modal-Distritos-Capital").value,
                    categoria: document.querySelector("#Modal-Distritos-Categoria").value,
                    iD_PAIS: document.querySelector("#Modal-Distritos-Pais").value
                }

                let endpoint = Distritos.BaseUrl + (params.id == "0000" ? "Agregar" : "Actualizar?id=" + params.id);
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
                                        $("#Modal-Distritos").modal('hide');
                                        Distritos.DataTable.ajax.reload();
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

        let Distritos_Buscar = document.querySelector("#Distritos-Buscar");
        if (Distritos_Buscar) {
            Distritos_Buscar.addEventListener("click", e => {
                e.preventDefault();

                Distritos.DataTable.ajax.reload();                

            });
        }

        let Distritos_Buscar_Nombre = document.querySelector("#Distritos-Buscar-Nombre")
        Distritos_Buscar_Nombre.addEventListener("keyup", function (event) {
            if (event.key === "Enter") {
                Distritos_Buscar.click();
            }
        });
    },

    List: function () {
        let endpoint = Distritos.BaseUrl + "Paginar";
        Distritos.DataTable = $(Distritos.Table)
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
                            idpais: parseInt(document.querySelector("#Distritos-Buscar-Pais").value),
                            dato: document.querySelector("#Distritos-Buscar-Nombre").value
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
                    { data: "iD_DISTRITO", name: "iD_DISTRITO", width: "10%" },
                    { data: "codigO_DISTRITO", name: "codigO_DISTRITO", width: "10%" },
                    { data: "capital", name: "capital", width: "20%" },
                    { data: "distrito", name: "distrito", width: "20%" },
                    { data: "provincia", name: "provincia", width: "20%" },
                    { data: "departamento", name: "departamento", width: "20%" },

                    {
                        data: "distrito",
                        name: "distrito",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Distritos.Edit(' + row.iD_DISTRITO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "distrito",
                        width: "5%",
                        name: "distrito",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Distritos.Delete(' + row.iD_DISTRITO + ');" title="Eliminar">' +
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
        let endpoint = Distritos.BaseUrl + "Obtener/" + codigo; 
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

                document.querySelector("#Modal-Distritos-Id").value = response.iD_DISTRITO;
                document.querySelector("#Modal-Distritos-Ubigeo").value = response.codigO_DISTRITO;
                document.querySelector("#Modal-Distritos-Capital").value = response.capital;
                document.querySelector("#Modal-Distritos-Distrito").value = response.distrito;
                document.querySelector("#Modal-Distritos-Provincia").value = response.provincia;
                document.querySelector("#Modal-Distritos-Departamento").value = response.departamento;
                document.querySelector("#Modal-Distritos-Categoria").value = response.categoria;

                $('#Modal-Distritos-Pais').val(response.iD_PAIS).change();

             
                $("#Modal-Distritos").modal({
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

                let endpoint = Distritos.BaseUrl + "Eliminar/" + codigo;
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
                                $("#Modal-Distritos").modal('hide');
                                Distritos.DataTable.ajax.reload();
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