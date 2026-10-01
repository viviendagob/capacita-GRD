var ConsultaParticipants = {
    BaseUrl: "/consultaparticipants/",
    DataTable: null,
    Paises: null,
    Distritos: null,
    Profesiones: null,
    Cargos: null,
    Entidades: null,
    esLocal : 0,
    Table: '#ConsultaParticipants-Tabla',


    Init: () => {
        ConsultaParticipants.esLocal = ConsultaParticipants.Paises.find(p => p.eslocal == 1).iD_PAIS;

        setTimeout(() => {
            $('#ConsultaParticipants-Buscar-Evento').selectpicker('destroy');
            $('#ConsultaParticipants-Buscar-Evento').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#ConsultaParticipants-Buscar-Tipo').selectpicker('destroy');
            $('#ConsultaParticipants-Buscar-Tipo').select2().on('select2:selecting', function (e) {
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
                ConsultaParticipants.LoadDatosLaborales(e.target.value);
            });

            ConsultaParticipants.PopulateDepartamentos();

            document.querySelector("#Modal-Participantes-Departamento").addEventListener("change", function () {
                ConsultaParticipants.PopulateProvincias(this.value, "0");
                ConsultaParticipants.PopulateDistritos(this.value, "0", "0");
            });

            document.querySelector("#Modal-Participantes-Provincia").addEventListener("change", function () {
                let departamento = document.querySelector("#Modal-Participantes-Departamento").value;
                ConsultaParticipants.PopulateDistritos(departamento, this.value, "0");
            });

            ConsultaParticipants.Events();

            ConsultaParticipants.List();
        }, 100);


 
    },

    Events: () => {
 

        let ConsultaParticipants_Guardar = document.querySelector("#ConsultaParticipants-Guardar");
        if (ConsultaParticipants_Guardar) {
            ConsultaParticipants_Guardar.addEventListener("click", e => {
                e.preventDefault();

                let id = document.querySelector("#Modal-Participantes").dataset.codigo;
                let actual = ConsultaParticipants.CurrentPersona || {};

                let nacionalidadLabora = document.querySelector("#Modal-Participantes-NacionalidadLabora").value;
                let crearPersonaDataDTO = null;
                if (nacionalidadLabora && nacionalidadLabora != "0") {
                    crearPersonaDataDTO = {
                        iD_PAIS_LABORA: parseInt(nacionalidadLabora) || 0,
                        iD_ENTIDAD: parseInt(document.querySelector("#Modal-Participantes-Tipo").value) || 0,
                        nombrE_ENTIDAD_OTRA: document.querySelector("#Modal-Participantes-Entidad").value,
                        areA_LABORA: document.querySelector("#Modal-Participantes-Area").value,
                        iD_CARGO: parseInt(document.querySelector("#Modal-Participantes-CargoDesenpena").value) || 0,
                        nombrE_CARGO_OTRA: document.querySelector("#Modal-Participantes-Cargo").value,
                        iD_DISTRITO: parseInt(document.querySelector("#Modal-Participantes-Distrito").value) || 0
                    };
                }

                let params = {
                    coD_PERSONA: actual.coD_PERSONA,
                    iD_TIPO_DOCUMENTO: actual.iD_TIPO_DOCUMENTO,
                    nuM_DOCUMENTO: actual.nuM_DOCUMENTO,
                    validadO_PIDE: actual.validadO_PIDE,
                    nombres: document.querySelector("#Modal-Participantes-Nombres").value,
                    apellidO_PATERNO: document.querySelector("#Modal-Participantes-Paterno").value,
                    apellidO_MATERNO: document.querySelector("#Modal-Participantes-Materno").value,
                    sexo: document.querySelector("#Modal-Participantes-Sexo").value,
                    iD_PAIS_NACIMIENTO: parseInt(document.querySelector("#Modal-Participantes-Nacionalidad").value) || 0,
                    fechA_NACIMIENTO: document.querySelector("#Modal-Participantes-Naci").value,
                    email: document.querySelector("#Modal-Participantes-Correo").value,
                    celular: document.querySelector("#Modal-Participantes-Celular").value,
                    iD_PROFESION: parseInt(document.querySelector("#Modal-Participantes-Profesion").value) || null,
                    crearPersonaDataDTO: crearPersonaDataDTO
                };

                let endpoint = ConsultaParticipants.BaseUrl + "Actualizar?id=" + id;

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
                                        $("#Modal-Participantes").modal('hide');
                                        ConsultaParticipants.List();
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

        let ConsultaParticipants_Buscar = document.querySelector("#ConsultaParticipants-Buscar");
        if (ConsultaParticipants_Buscar) {
            ConsultaParticipants_Buscar.addEventListener("click", e => {
                e.preventDefault();
                ConsultaParticipants.DataTable.ajax.reload();  
            });
        }
 
       
    },

    PopulateDepartamentos: function () {
        let select = document.querySelector("#Modal-Participantes-Departamento");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let departamentos = [...new Set(ConsultaParticipants.Distritos.map(d => d.departamento))].sort();
        departamentos.forEach((departamento) => {
            select.add(new Option(departamento, departamento, false, false));
        });
    },

    PopulateProvincias: function (departamento, selected) {
        let select = document.querySelector("#Modal-Participantes-Provincia");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let provincias = [...new Set(ConsultaParticipants.Distritos.filter(d => d.departamento == departamento).map(d => d.provincia))].sort();
        provincias.forEach((provincia) => {
            select.add(new Option(provincia, provincia, provincia == selected, provincia == selected));
        });
    },

    PopulateDistritos: function (departamento, provincia, selected) {
        let select = document.querySelector("#Modal-Participantes-Distrito");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let distritos = ConsultaParticipants.Distritos.filter(d => d.departamento == departamento && d.provincia == provincia);
        distritos.forEach((distrito) => {
            let seleccionado = (distrito.iD_DISTRITO == selected);
            select.add(new Option(distrito.distrito, distrito.iD_DISTRITO, seleccionado, seleccionado));
        });
    },

    LoadDatosLaborales: function (idPersonaData) {
        let endpoint = ConsultaParticipants.BaseUrl + "ObtenerData/" + idPersonaData;
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
            .then((data) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";

                document.querySelector("#Modal-Participantes-NacionalidadLabora").value = data.iD_PAIS_LABORA;
                document.querySelector("#Modal-Participantes-NacionalidadLabora").dispatchEvent(new Event("change"));

                document.querySelector("#Modal-Participantes-Tipo").value = data.iD_ENTIDAD;
                document.querySelector("#Modal-Participantes-Tipo").dispatchEvent(new Event("change"));

                document.querySelector("#Modal-Participantes-Entidad").value = data.nombrE_ENTIDAD_OTRA || "";
                document.querySelector("#Modal-Participantes-Area").value = data.areA_LABORA || "";

                document.querySelector("#Modal-Participantes-CargoDesenpena").value = data.iD_CARGO;
                document.querySelector("#Modal-Participantes-CargoDesenpena").dispatchEvent(new Event("change"));

                document.querySelector("#Modal-Participantes-Cargo").value = data.nombrE_CARGO_OTRA || "";

                let distrito = ConsultaParticipants.Distritos.find(d => d.iD_DISTRITO == data.iD_DISTRITO);
                if (distrito) {
                    document.querySelector("#Modal-Participantes-Departamento").value = distrito.departamento;
                    ConsultaParticipants.PopulateProvincias(distrito.departamento, distrito.provincia);
                    ConsultaParticipants.PopulateDistritos(distrito.departamento, distrito.provincia, distrito.iD_DISTRITO);
                }

            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

            });
    },

    List: function () {
        let endpoint = ConsultaParticipants.BaseUrl + "Paginar";

        ConsultaParticipants.DataTable = $(ConsultaParticipants.Table)
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
                            tipoDocumento: document.querySelector("#ConsultaParticipants-Buscar-Tipo").value,
                            numeroDocumento: document.querySelector("#ConsultaParticipants-Buscar-Documento").value,
                            paterno: document.querySelector("#ConsultaParticipants-Buscar-Paterno").value,
                            materno: document.querySelector("#ConsultaParticipants-Buscar-Materno").value,
                            idEvento: parseInt(document.querySelector("#ConsultaParticipants-Buscar-Evento").value)
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
                        width: "15%",
                        render: function (data, type, row) {
                            if (!row.asistencias || row.asistencias.length == 0) {
                                return '<span class="badge badge-secondary">Inactivo</span> <span class="text-muted d-block">Sin asistencia registrada</span>';
                            }

                            // FECHA es tipo "date" en la BD (sin hora); la hora real de registro
                            // viene en FECHA_REG.
                            var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
                            var fechas = row.asistencias.map(a => formatter.format(new Date(a.fechA_REG))).join("<br>");
                            return '<span class="badge badge-success">Activo</span><br>' + fechas;
                        },
                    },

                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:ConsultaParticipants.Edit(' + row.iD_PERSONA + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-warning btn-icon  btn-icon-mini btn-round" onclick="javascript:ConsultaParticipants.Edit(' + row.iD_PERSONA + ');" title="Editar">' +
                                '<i class="material-icons">attach_file</i>' +
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
        let endpoint = ConsultaParticipants.BaseUrl + "Obtener/" + codigo; 
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
                ConsultaParticipants.CurrentPersona = data;
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

                var esLocal = ConsultaParticipants.Paises.find(p => p.eslocal == 1);

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

                if (data.eventos.length > 0) {
                    let ultimoEvento = data.eventos[data.eventos.length - 1];
                    $('#Modal-Participantes-Eventos').val(ultimoEvento.iD_PERSONA_DATA).trigger('change');
                    ConsultaParticipants.LoadDatosLaborales(ultimoEvento.iD_PERSONA_DATA);
                }

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