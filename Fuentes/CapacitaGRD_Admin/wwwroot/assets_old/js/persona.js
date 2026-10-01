var Personas = {
    BaseUrl: "/personas/",
    DataTable: null,
    Paises: null,
    Distritos: null,
    Profesiones: null,
    Cargos: null,
    Entidades: null,
    esLocal : 0,
    Table: '#Personas-Tabla',


    Init: () => {
        Personas.esLocal = Personas.Paises.find(p => p.eslocal == 1).iD_PAIS;

        setTimeout(() => {
            $('#Personas-Buscar-Tipo').selectpicker('destroy');
            $('#Personas-Buscar-Tipo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Nacionalidad').selectpicker('destroy');
            $('#Modal-Participantes-Nacionalidad').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-NacionalidadLabora').selectpicker('destroy');
            $('#Modal-Participantes-NacionalidadLabora').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Departamento').selectpicker('destroy');
            $('#Modal-Participantes-Departamento').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Provincia').selectpicker('destroy');
            $('#Modal-Participantes-Provincia').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Distrito').selectpicker('destroy');
            $('#Modal-Participantes-Distrito').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });


            $('#Modal-Participantes-Tipo').selectpicker('destroy');
            $('#Modal-Participantes-Tipo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-CargoDesenpena').selectpicker('destroy');
            $('#Modal-Participantes-CargoDesenpena').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Profesion').selectpicker('destroy');
            $('#Modal-Participantes-Profesion').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Sexo').selectpicker('destroy');
            $('#Modal-Participantes-Sexo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Eventos').selectpicker('destroy');
            $('#Modal-Participantes-Eventos').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                let endpoint = Personas.BaseUrl + "ObtenerData/" + e.target.value;

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

                        

                        document.querySelector("#Modal-Participantes-NacionalidadLabora").value = data.iD_PAIS_LABORA;
                        document.querySelector("#Modal-Participantes-NacionalidadLabora").dispatchEvent(new Event("change"));

                        if (Personas.esLocal == data.iD_PAIS_LABORA) {
                            document.querySelector("#Modal-Participantes-Tipo").value = data.iD_ENTIDAD;
                            document.querySelector("#Modal-Participantes-Tipo").dispatchEvent(new Event("change"));   
                        } else {
                            document.querySelector("#Modal-Participantes-Tipo").value = data.iD_ENTIDAD;
                            document.querySelector("#Modal-Participantes-Tipo").dispatchEvent(new Event("change"));   
                        }

                        document.querySelector("#Modal-Participantes-Area").value = data.areA_LABORA;

                        document.querySelector("#Modal-Participantes-CargoDesenpena").value = data.iD_CARGO;
                        document.querySelector("#Modal-Participantes-CargoDesenpena").dispatchEvent(new Event("change"));   

                        document.querySelector("#Modal-Participantes-Cargo").value = data.nombrE_CARGO_OTRA;

                        console.log(response);                        
                            
                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    });
            });


            Personas.Events(); 

            Personas.List();
        }, 100);


 
    },

    Events: () => {
 

        let Personas_Guardar = document.querySelector("#Personas-Guardar");
        if (Personas_Guardar) {
            Personas_Guardar.addEventListener("click", e => {
                e.preventDefault();                 
                
                let params = {
                    id: document.querySelector("#Modal-Participantes").dataset.codigo,
                    
                }

                let endpoint = Personas.BaseUrl + "Actualizar?id=" + params.id;
                
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
                            method: "PUT",
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
                                        $("#Modal-Personas").modal('hide');
                                        Personas.List();
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

        let Personas_Buscar = document.querySelector("#Personas-Buscar");
        if (Personas_Buscar) {
            Personas_Buscar.addEventListener("click", e => {
                e.preventDefault();
                Personas.DataTable.ajax.reload();  
            });
        }
 
       
    },

    List: function () {
        let endpoint = Personas.BaseUrl + "Paginar";

        Personas.DataTable = $(Personas.Table)
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
                            tipoDocumento: document.querySelector("#Personas-Buscar-Tipo").value,
                            numeroDocumento: document.querySelector("#Personas-Buscar-Documento").value,
                            paterno: document.querySelector("#Personas-Buscar-Paterno").value,
                            materno: document.querySelector("#Personas-Buscar-Materno").value
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
                    {
                        targets: [1],
                        class: "text-center",
                        width: "5%",
                    },
                ],

                columns: [
                    { data: "iD_PERSONA", name: "iD_PERSONA", width: "10%" },
                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: function (data, type, row) {
                            return (row.validadO_PIDE == "S" ? '<i class="material-icons text-success">check_circle</i>' : '') ;
                        },
                    },
                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: (data, type, row) => row.tipO_DOCUMENTO.nombre,
                    },
 
                    { data: "nuM_DOCUMENTO", name: "nuM_DOCUMENTO", width: "20%" },
                    { data: "nombres", name: "nombres", width: "20%" },
                    { data: "apellidO_PATERNO", name: "apellidO_PATERNO", width: "20%" },
                    { data: "apellidO_MATERNO", name: "apellidO_MATERNO", width: "20%" },

                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Personas.Edit(' + row.iD_PERSONA + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

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
        let endpoint = Personas.BaseUrl + "Obtener/" + codigo; 
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
                document.querySelector("#Modal-Participantes-Naci").value = new Date(data.fechA_NACIMIENTO).toISOString().split('T')[0];
                document.querySelector("#Modal-Participantes-Nombres").value = data.nombres;
                document.querySelector("#Modal-Participantes-Paterno").value = data.apellidO_PATERNO;
                document.querySelector("#Modal-Participantes-Materno").value = data.apellidO_MATERNO;
                document.querySelector("#Modal-Participantes-Nacionalidad").value = data.iD_PAIS_NACIMIENTO;
                document.querySelector("#Modal-Participantes-Nacionalidad").dispatchEvent(new Event("change"));   

                if (data.validadO_PIDE == "S") {
                    document.querySelector("#Modal-Participantes-Naci").disabled = true;
                    document.querySelector("#Modal-Participantes-Nombres").disabled = true;
                    document.querySelector("#Modal-Participantes-Paterno").disabled = true;
                    document.querySelector("#Modal-Participantes-Materno").disabled = true;
                    document.querySelector("#Modal-Participantes-Nacionalidad").disabled = true;
                }

                if (data.validadO_PIDE == "N") {
                    document.querySelector("#Modal-Participantes-Naci").disabled = false;
                    document.querySelector("#Modal-Participantes-Nombres").disabled = false;
                    document.querySelector("#Modal-Participantes-Paterno").disabled = false;
                    document.querySelector("#Modal-Participantes-Materno").disabled = false;
                    document.querySelector("#Modal-Participantes-Nacionalidad").disabled = false;
                }
                  
                document.querySelector("#Modal-Participantes-Sexo").value = data.sexo;
                document.querySelector("#Modal-Participantes-Sexo").dispatchEvent(new Event("change"));   

                document.querySelector("#Modal-Participantes-Profesion").value = data.iD_PROFESION;
                document.querySelector("#Modal-Participantes-Profesion").dispatchEvent(new Event("change"));   

                document.querySelector("#Modal-Participantes-Celular").value = data.celular;
                document.querySelector("#Modal-Participantes-Correo").value = data.email;

                var esLocal = Personas.Paises.find(p => p.eslocal == 1);

                document.querySelector("#Modal-Participantes-Profesion").value = data.iD_PROFESION;
                document.querySelector("#Modal-Participantes-Profesion").dispatchEvent(new Event("change"));   
                
                document.querySelectorAll('#Modal-Participantes-Eventos option').forEach(option => option.remove());
                var dataAll = new Option("Seleccione", "0", false, false);
                document.querySelector("#Modal-Participantes-Eventos").add(dataAll);

                data.eventos.forEach((evento, i) => {
                    var item = new Option(evento.evento.nombre, evento.iD_PERSONA_DATA, false, false);
                    document.querySelector("#Modal-Participantes-Eventos").add(item);
                });

                document.querySelector("#Modal-Participantes").dataset.codigo = response.iD_PERSONA;

                document.querySelectorAll(".clsOculta").forEach(div => div.style.display = 'none');

                $("#Modal-Participantes").modal({
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
        document.querySelector("#Modal-Participantes-Nombres").value = "";
        document.querySelector("#Modal-Participantes-Paterno").value = "";
        document.querySelector("#Modal-Participantes-Materno").value = "";

        document.querySelector("#Modal-Participantes-Celular").value = "";
        document.querySelector("#Modal-Participantes-Entidad").value = "";
        document.querySelector("#Modal-Participantes-Area").value = "";
        document.querySelector("#Modal-Participantes-Cargo").value = "";
        document.querySelector("#Modal-Participantes-OtroNacionalidad").value = "";
        document.querySelector("#Modal-Participantes-Profesion").value = "";

        document.querySelector("#Modal-Participantes-Nacionalidad").value = "";
        document.querySelector("#Modal-Participantes-Nacionalidad").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Participantes-Sexo").value = "";
        document.querySelector("#Modal-Participantes-Sexo").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Participantes-NacionalidadLabora").value = "";
        document.querySelector("#Modal-Participantes-NacionalidadLabora").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Participantes-Tipo").value = "";
        document.querySelector("#Modal-Participantes-Tipo").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Participantes-CargoDesenpena").value = "";
        document.querySelector("#Modal-Participantes-CargoDesenpena").dispatchEvent(new Event("change"));

        document.querySelectorAll("label.error").forEach((item, i) => console.log(item.style.display = "none"))
        $('form').find('input[type=text], input[type=password], input[type=number], input[type=email], textarea').val('');
    }, 

    Delete: function (codigo) {
       

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