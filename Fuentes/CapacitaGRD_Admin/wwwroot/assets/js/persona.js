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
                Personas.PopulateProvincias(this.value, "0");
                Personas.PopulateDistritos(this.value, "0", "0");
            });

            $('#Modal-Participantes-Provincia').selectpicker('destroy');
            $('#Modal-Participantes-Provincia').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                let departamento = document.querySelector("#Modal-Participantes-Departamento").value;
                Personas.PopulateDistritos(departamento, this.value, "0");
            });

            $('#Modal-Participantes-Distrito').selectpicker('destroy');
            $('#Modal-Participantes-Distrito').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });


            $('#Modal-Participantes-Tipo').selectpicker('destroy');
            $('#Modal-Participantes-Tipo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            // tags: true habilita escribir un valor que no está en la lista; select2:select
            // detecta cuando ese valor es nuevo (su "id" es el mismo texto, no un ID numérico)
            // y lo crea en el maestro correspondiente antes de continuar.
            $('#Modal-Participantes-CargoDesenpena').selectpicker('destroy');
            $('#Modal-Participantes-CargoDesenpena').select2({ tags: true }).on("select2:select", function (e) {
                Personas.CrearSiEsNuevo('#Modal-Participantes-CargoDesenpena', '/cargos/Agregar', 'iD_CARGO');
            });

            $('#Modal-Participantes-Profesion').selectpicker('destroy');
            $('#Modal-Participantes-Profesion').select2({ tags: true }).on("select2:select", function (e) {
                Personas.CrearSiEsNuevo('#Modal-Participantes-Profesion', '/profesiones/Agregar', 'iD_PROFESION');
            });

            $('#Modal-Participantes-Sexo').selectpicker('destroy');
            $('#Modal-Participantes-Sexo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Participantes-Eventos').selectpicker('destroy');
            $('#Modal-Participantes-Eventos').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                Personas.LoadDatosLaborales(e.target.value);
            });

            // Reemplaza el <input type="date"> (limitado, sin poder tipear "/" ni navegar
            // libremente entre años) por bootstrap-datepicker, igual que en Eventos.
            $('#Modal-Participantes-Naci').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true,
                orientation: 'bottom auto',
                startDate: new Date(new Date().getFullYear() - 100, 0, 1),
                endDate: new Date(),
            });

            Personas.PopulateDepartamentos();

            document.querySelector("#Modal-Participantes-Departamento").addEventListener("change", function () {
                Personas.PopulateProvincias(this.value, "0");
                Personas.PopulateDistritos(this.value, "0", "0");
            });

            document.querySelector("#Modal-Participantes-Provincia").addEventListener("change", function () {
                let departamento = document.querySelector("#Modal-Participantes-Departamento").value;
                Personas.PopulateDistritos(departamento, this.value, "0");
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

                let id = document.querySelector("#Modal-Participantes").dataset.codigo;
                let actual = Personas.CurrentPersona || {};

                let nacionalidadLabora = document.querySelector("#Modal-Participantes-NacionalidadLabora").value;
                let crearPersonaDataDTO = null;
                if (nacionalidadLabora && nacionalidadLabora != "0") {
                    crearPersonaDataDTO = {
                        iD_PAIS_LABORA: parseInt(nacionalidadLabora) || 0,
                        iD_ENTIDAD: parseInt(document.querySelector("#Modal-Participantes-Tipo").value) || 0,
                        nombrE_ENTIDAD_OTRA: document.querySelector("#Modal-Participantes-Entidad").value,
                        areA_LABORA: document.querySelector("#Modal-Participantes-Area").value,
                        iD_CARGO: parseInt(document.querySelector("#Modal-Participantes-CargoDesenpena").value) || 0,
                        nombrE_CARGO_OTRA: "",
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
                    fechA_NACIMIENTO: Personas.FechaDDMMYYYYaISO(document.querySelector("#Modal-Participantes-Naci").value),
                    email: document.querySelector("#Modal-Participantes-Correo").value,
                    celular: document.querySelector("#Modal-Participantes-Celular").value,
                    iD_PROFESION: parseInt(document.querySelector("#Modal-Participantes-Profesion").value) || null,
                    crearPersonaDataDTO: crearPersonaDataDTO
                };

                let endpoint = Personas.BaseUrl + "Actualizar?id=" + id;

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

        let Personas_Descargar = document.querySelector("#Personas-Descargar");
        if (Personas_Descargar) {
            Personas_Descargar.addEventListener("click", e => {
                e.preventDefault();

                let params = new URLSearchParams({
                    tipoDocumento: document.querySelector("#Personas-Buscar-Tipo").value,
                    numeroDocumento: document.querySelector("#Personas-Buscar-Documento").value,
                    paterno: document.querySelector("#Personas-Buscar-Paterno").value,
                    materno: document.querySelector("#Personas-Buscar-Materno").value
                });

                window.location = Personas.BaseUrl + "Descargar?" + params.toString();
            });
        }
 
       
    },

    PopulateDepartamentos: function () {
        let select = document.querySelector("#Modal-Participantes-Departamento");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let departamentos = [...new Set(Personas.Distritos.map(d => d.departamento))].sort();
        departamentos.forEach((departamento) => {
            select.add(new Option(departamento, departamento, false, false));
        });
    },

    PopulateProvincias: function (departamento, selected) {
        let select = document.querySelector("#Modal-Participantes-Provincia");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let provincias = [...new Set(Personas.Distritos.filter(d => d.departamento == departamento).map(d => d.provincia))].sort();
        provincias.forEach((provincia) => {
            select.add(new Option(provincia, provincia, provincia == selected, provincia == selected));
        });
        $('#Modal-Participantes-Provincia').trigger('change.select2');
    },

    PopulateDistritos: function (departamento, provincia, selected) {
        let select = document.querySelector("#Modal-Participantes-Distrito");
        select.innerHTML = '';
        select.add(new Option("Seleccione", "0", false, false));

        let distritos = Personas.Distritos.filter(d => d.departamento == departamento && d.provincia == provincia);
        distritos.forEach((distrito) => {
            let seleccionado = (distrito.iD_DISTRITO == selected);
            select.add(new Option(distrito.distrito, distrito.iD_DISTRITO, seleccionado, seleccionado));
        });
        $('#Modal-Participantes-Distrito').trigger('change.select2');
    },

    LoadDatosLaborales: function (idPersonaData) {
        let endpoint = Personas.BaseUrl + "ObtenerData/" + idPersonaData;
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

                let distrito = Personas.Distritos.find(d => d.iD_DISTRITO == data.iD_DISTRITO);
                if (distrito) {
                    document.querySelector("#Modal-Participantes-Departamento").value = distrito.departamento;
                    $('#Modal-Participantes-Departamento').trigger('change.select2');
                    Personas.PopulateProvincias(distrito.departamento, distrito.provincia);
                    Personas.PopulateDistritos(distrito.departamento, distrito.provincia, distrito.iD_DISTRITO);
                }

            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

            });
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

                    {
                        data: "iD_PERSONA",
                        name: "iD_PERSONA",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-warning btn-icon  btn-icon-mini btn-round" onclick="javascript:Personas.Reiniciar(' + row.iD_PERSONA + ');" title="Reiniciar Participante">' +
                                '<i class= "material-icons"> restart_alt</i>' +
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

    Reiniciar: function (codigo) {
        swal({
            title: "Confirme?",
            text: "Esta seguro de reiniciar la fecha de nacimiento de este participante",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#FF3636",
            confirmButtonText: "Si, reiniciar",
            closeOnConfirm: false
        }, function (isConfirm) {
            if (isConfirm) {
                let endpoint = Personas.BaseUrl + "Reiniciar?id=" + codigo;
                document.querySelector(".page-loader-wrapper").style.display = "block";

                fetch(endpoint, {
                    method: "PUT",
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
                                Personas.List();
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
                Personas.CurrentPersona = data;
                Personas.SetFechaNacimiento(data.fechA_NACIMIENTO);
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

                if (data.eventos.length > 0) {
                    let ultimoEvento = data.eventos[data.eventos.length - 1];
                    $('#Modal-Participantes-Eventos').val(ultimoEvento.iD_PERSONA_DATA).trigger('change');
                    Personas.LoadDatosLaborales(ultimoEvento.iD_PERSONA_DATA);
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

    // "dd/mm/yyyy" (formato del datepicker) -> "yyyy-mm-dd" (lo que espera la API).
    FechaDDMMYYYYaISO: function (valor) {
        if (!valor) return "";
        const partes = valor.split("/");
        if (partes.length !== 3) return valor;
        const [dd, mm, yyyy] = partes;
        return `${yyyy}-${mm.padStart(2, "0")}-${dd.padStart(2, "0")}`;
    },

    SetFechaNacimiento: function (fechaISO) {
        const input = document.querySelector("#Modal-Participantes-Naci");
        if (!fechaISO) {
            input.value = "";
            return;
        }
        const formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric', timeZone: 'UTC' });
        input.value = formatter.format(new Date(fechaISO)).replace(/-/g, "/");
        $(input).datepicker('update');
    },

    // Cargo/Profesión usan select2 con tags: true. Cuando el usuario escribe un valor que no
    // está en la lista, select2 crea una opción temporal cuyo value es el mismo texto (no un
    // ID numérico) — acá se detecta eso, se crea el registro real en el maestro vía la API, y
    // se reemplaza la opción temporal por una con el ID definitivo.
    CrearSiEsNuevo: function (selector, endpointAgregar, idField) {
        const el = document.querySelector(selector);
        const opt = el.options[el.selectedIndex];
        if (!opt || !isNaN(parseInt(opt.value))) return;

        const nombre = opt.text.trim().toUpperCase();
        if (!nombre) return;

        fetch(endpointAgregar, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ NOMBRE: nombre }),
        })
            .then((response) => response.json())
            .then((response) => {
                if (!response.success || !response.data) {
                    swal("Upss", response.mensaje || "No se pudo agregar", "warning");
                    $(opt).remove();
                    return;
                }
                const nuevoId = response.data[idField];
                $(opt).remove();
                const nuevaOpcion = new Option(nombre, nuevoId, true, true);
                $(selector).append(nuevaOpcion).trigger("change");
            })
            .catch(() => {
                swal("Upss", "No se pudo agregar", "warning");
                $(opt).remove();
            });
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