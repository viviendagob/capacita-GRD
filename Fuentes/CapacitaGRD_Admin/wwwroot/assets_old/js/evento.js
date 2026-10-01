
var Eventos = {
    BaseUrl: "/eventos/",
    DataTable: null,
    Paises: null,
    Distritos: null,
    Profesiones: null,
    Cargos: null,
    Entidades: null,
    Table: '#Eventos-Tabla',
    base64: null,
    base64Formato: null,
    esVirtual: 2,

    Init: () => {

        setTimeout(() => {
            $("#Modal-Eventos-HInicio").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });
            $("#Modal-Eventos-HFin").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });

            $('#Eventos-Buscar-Modalidad').selectpicker('destroy');
            $('#Eventos-Buscar-Modalidad').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Eventos-Buscar-Tipo').selectpicker('destroy');
            $('#Eventos-Buscar-Tipo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Eventos-Buscar-Estado').selectpicker('destroy');
            $('#Eventos-Buscar-Estado').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Eventos-Modalidad').selectpicker('destroy');
            $('#Modal-Eventos-Modalidad').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                if (e.target.value == 0) {
                    document.querySelector("#Modal-Eventos-Distrito").value = "0";
                    document.querySelector("#Modal-Eventos-Distrito").dispatchEvent(new Event("change"));
                    document.querySelector("#Modal-Eventos-Distrito").disabled = true;

                    document.querySelector("#Modal-Eventos-Lugar").value = "";
                    document.querySelector("#Modal-Eventos-Lugar").disabled = true;

                    document.querySelector("#Modal-Eventos-Enlace").value = "";
                    document.querySelector("#Modal-Eventos-Enlace").disabled = true;
                }
                else if (e.target.value == Eventos.esVirtual) {
                    document.querySelector("#Modal-Eventos-Distrito").value = "0";
                    document.querySelector("#Modal-Eventos-Distrito").dispatchEvent(new Event("change"));
                    document.querySelector("#Modal-Eventos-Distrito").disabled = true;

                    document.querySelector("#Modal-Eventos-Lugar").value = "";
                    document.querySelector("#Modal-Eventos-Lugar").disabled = true;

                    document.querySelector("#Modal-Eventos-Enlace").disabled = false;
                } else {

                    document.querySelector("#Modal-Eventos-Distrito").disabled = false;
                    document.querySelector("#Modal-Eventos-Lugar").disabled = false;

                    document.querySelector("#Modal-Eventos-Enlace").value = "";
                    document.querySelector("#Modal-Eventos-Enlace").disabled = true;

                }
            });

            $('#Modal-Eventos-Tipo').selectpicker('destroy');
            $('#Modal-Eventos-Tipo').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Eventos-Distrito').selectpicker('destroy');
            $('#Modal-Eventos-Distrito').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Eventos-Encuesta').selectpicker('destroy');
            $('#Modal-Eventos-Encuesta').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                let item = e.target.value;
                let modo = document.querySelector("#Modal-Eventos").dataset.codigo;
                if (modo == 0) {
                    document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = true;
                } else {
                    if (item == 0) {
                        document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = true;
                    } else {
                        document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = false;
                    }
                }

            });

            $('#Modal-Eventos-Cuestionario').selectpicker('destroy');
            $('#Modal-Eventos-Cuestionario').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
                let item = e.target.value;
                let modo = document.querySelector("#Modal-Eventos").dataset.codigo;
                if (modo == 0) {
                    document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = true;
                } else {
                    if (item == 0) {
                        document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = true;
                    } else {
                        document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = false;
                    }
                }
            });

            $('#Modal-Eventos-Estado').selectpicker('destroy');
            $('#Modal-Eventos-Estado').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $('#Modal-Eventos-Documentos').selectpicker('destroy');
            $('#Modal-Eventos-Documentos').select2().on('select2:selecting', function (e) {
            }).on("select2:select", function (e) {
            });

            $("#Modal-Eventos-Inicio").datepicker({
                multidate: true,
                autoclose: false,
                todayHighlight: true,
                container: "div#Modal-Eventos-Inicio",
                language: "es-PE",
                orientation: 'bottom',
                minDate: new Date(),

            }).on('changeDate', function (e) {
                var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
                var fechas = e.dates.map(d => [formatter.format((new Date(d)))]).join(', ');
                document.querySelector("#Modal-Eventos-Inicio input").value = fechas;
            }).on('hide', function (e) {
                var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
                var fechas = e.dates.map(d => [formatter.format((new Date(d)))]).join(', ');
                document.querySelector("#Modal-Eventos-Inicio input").value = fechas;
            }); 

            Eventos.Events();

            Eventos.List();
        }, 500);

    },

    Events: () => {

        let Modal_Eventos_Requiere = document.querySelector("#Modal-Eventos-Requiere");
        if (Modal_Eventos_Requiere) {
            Modal_Eventos_Requiere.addEventListener("click", (e) => {
                if (e.target.checked) {
                    document.querySelector("#Modal-Eventos-Documentos").parentElement.style.display = "block";
                } else {
                    document.querySelector("#Modal-Eventos-Documentos").parentElement.style.display = "none";
                }
            })
        }

        let Eventos_Tablero = document.querySelector("#Eventos-Tablero");
        if (Eventos_Tablero) {
            Eventos_Tablero.addEventListener("click", e => {
                e.preventDefault();


                $("#Modal-Eventos-Tablero").modal({
                    keyboard: true,
                    backdrop: 'static'
                })
            })
        }

        let Eventos_Guardar = document.querySelector("#Eventos-Guardar");
        if (Eventos_Guardar) {
            Eventos_Guardar.addEventListener("click", e => {
                e.preventDefault();

                let periodo = [];
                let fechasSeleccionadas = $('#Modal-Eventos-Inicio').data().datepicker.getFormattedDate('yyyy/mm/dd').split(',');
                fechasSeleccionadas.forEach((item, i) => {
                    if (item != "")
                        periodo.push(new Date(item));
                });
                let fechaInicio;
                let fechaFin;

                if (periodo.length > 0) {
                    let maxDate = new Date(Math.max.apply(null, periodo));
                    let minDate = new Date(Math.min.apply(null, periodo));
                    fechaInicio = new Date(minDate)
                    fechaFin = new Date(maxDate)
                }

                var fechas = [];
                fechasSeleccionadas.forEach((item, i) => {
                    if (item != "")
                        fechas.push({
                            fecha: new Date(item),
                            horA_INICIO: document.querySelector("#Modal-Eventos-HInicio").value + ":00",
                            horA_FIN: document.querySelector("#Modal-Eventos-HFin").value + ":00",
                            eS_ENCUESTA: (document.querySelector("#Modal-Eventos-Encuesta").value == 0 ? "N" : "S"),
                            iD_ENCUESTA: (document.querySelector("#Modal-Eventos-Encuesta").value == 0 ? null : document.querySelector("#Modal-Eventos-Encuesta").value),
                            eS_CUESTIONARIO: (document.querySelector("#Modal-Eventos-Cuestionario").value == 0 ? "N" : "S"),
                            iD_CUESTIONARIO: (document.querySelector("#Modal-Eventos-Cuestionario").value == 0 ? null : document.querySelector("#Modal-Eventos-Cuestionario").value),
                        });
                }); 

                let params = {
                    id: document.querySelector("#Modal-Eventos").dataset.codigo,
                    nombre: document.querySelector("#Modal-Eventos-Nombre").value,
                    fechA_INICIO: fechaInicio,
                    fechA_FIN: fechaFin,
                    nombrE_LUGAR: document.querySelector("#Modal-Eventos-Lugar").value,
                    iD_ESTADO: document.querySelector("#Modal-Eventos-Estado").value,
                    iD_UBIGEO: parseInt(document.querySelector("#Modal-Eventos-Distrito").value),
                    banner: Eventos.base64,
                    formato: Eventos.base64Formato,
                    nuM_PARTICIPANTES: parseInt(document.querySelector("#Modal-Eventos-Participantes").value),
                    descripcion: document.querySelector("#Modal-Eventos-Descripcion").value,
                    enlacE_WHATSAPP: document.querySelector("#Modal-Eventos-Wasape").value,
                    horA_INICIO: document.querySelector("#Modal-Eventos-HInicio").value + ":00",
                    horA_FIN: document.querySelector("#Modal-Eventos-HFin").value + ":00",
                    reD_SOCIAL: document.querySelector("#Modal-Eventos-Enlace").value,
                    iD_TIPO_EVENTO: parseInt(document.querySelector("#Modal-Eventos-Tipo").value),
                    iD_MODALIDAD: parseInt(document.querySelector("#Modal-Eventos-Modalidad").value),
                    fechas: fechas,
                    generA_TICKET: (document.querySelector("#Modal-Eventos-Ticket").checked ? "S" : "N"),
                    enviA_CORREO: (document.querySelector("#Modal-Eventos-Correo").checked ? "S" : "N"),
                    iD_ENCUESTA: (document.querySelector("#Modal-Eventos-Encuesta").value == 0 ? null : document.querySelector("#Modal-Eventos-Encuesta").value),
                    iD_CUESTIONARIO: (document.querySelector("#Modal-Eventos-Cuestionario").value == 0 ? null : document.querySelector("#Modal-Eventos-Cuestionario").value),
                    requierE_DOCUMENTO: (document.querySelector("#Modal-Eventos-Requiere").checked ? "S" : "N"),
                    documentos: (document.querySelector("#Modal-Eventos-Requiere").checked ? Array.from($("#Modal-Eventos-Documentos").val()).map(d => parseInt(d)) : null),
                }

                if (isNaN(params.iD_UBIGEO)) {
                    params.iD_UBIGEO = 0;
                }

                let errores = [];

                if (isNaN(params.nuM_PARTICIPANTES)) {
                    errores.push("* Ingrese el n\u00famero de participantes");
                }

                if (params.nuM_PARTICIPANTES == "0") {
                    errores.push("* Ingrese el número de participantes");
                }

                if (params.nombre == "") {
                    errores.push("* Ingrese el nombre");
                }

                if (params.id == 0) {
                    if (params.banner == "") {
                        errores.push("* Seleccione el banner");
                    }
                }

                if (params.iD_ESTADO == "0") {
                    errores.push("* Seleccione el estado");
                }

                if (params.iD_TIPO_EVENTO == "0") {
                    errores.push("* Seleccione el tipo");
                }

                if (params.iD_MODALIDAD == "0") {
                    errores.push("* Seleccione la modalidad");
                }

                if (fechas.length == 0) {
                    errores.push("* Seleccione el periodo del evento");
                }

                if (params.horA_INICIO == ":00") {
                    errores.push("* Ingrese la hora inicio");
                }

                if (params.horA_FIN == ":00") {
                    errores.push("* Ingrese la hora fin");
                }

                if (params.requierE_DOCUMENTO == "S") {
                    if (params.documentos.length == 0) {
                        errores.push("* Seleccione los documentos requeridos");
                    }
                }

                if (errores.length > 0) {
                    swal("Advertencia", errores.join('\n'), "warning");

                    return;
                }


                let endpoint = Eventos.BaseUrl + (params.id == "0" ? "Agregar" : "Actualizar?id=" + params.id);
                let metodo = (params.id == "0" ? "POST" : "PUT");

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
                        document.querySelector(".page-loader-wrapper").style.display = "block";
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
                                document.querySelector(".page-loader-wrapper").style.display = "none";
                                if (response.success) {
                                    swal({
                                        title: response.titulo,
                                        text: response.mensaje,
                                        type: "success"
                                    }, () => {
                                        $("#Modal-Eventos").modal('hide');
                                        Eventos.DataTable.ajax.reload();
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

            });
        }

        let Eventos_Agregar = document.querySelector("#Eventos-Agregar");
        if (Eventos_Agregar) {
            Eventos_Agregar.addEventListener("click", e => {
                e.preventDefault();
                Eventos.Clear();

                $('#Modal-Eventos-Inicio').datepicker('setStartDate', new Date());
                $("#Modal-Eventos-Inicio").data('datepicker').setDate(null);

                document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = true;
                document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = true;

                document.querySelector("#Modal-Eventos").dataset.codigo = "0";

                $("#Modal-Eventos").modal({
                    keyboard: true,
                    backdrop: 'static'
                })
            });
        }

        let Eventos_Buscar = document.querySelector("#Eventos-Buscar");
        if (Eventos_Buscar) {
            Eventos_Buscar.addEventListener("click", e => {
                e.preventDefault();
                Eventos.DataTable.ajax.reload();
            });
        }

        let Modal_Eventos_Eliminar_Guardar = document.querySelector("#Modal-Eventos-Eliminar-Guardar");
        if (Modal_Eventos_Eliminar_Guardar) {
            Modal_Eventos_Eliminar_Guardar.addEventListener("click", e => {
                e.preventDefault();
                let allowDismiss = true;
                if (grecaptcha.getResponse() == '') {
                    $.notify({
                        message: "Antes de continuar debe confirmar la acci\u00f3n"
                    },
                        {
                            type: "alert-warning",
                            allow_dismiss: allowDismiss,
                            newest_on_top: true,
                            timer: 1000,
                            placement: {
                                from: "bottom",
                                align: "right"
                            },
                            animate: {
                                enter: "",
                                exit: ""
                            },
                            template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} ' + (allowDismiss ? "p-r-35" : "") + '" role="alert">' +
                                '<span data-notify="icon"></span> ' +
                                '<span data-notify="title">{1}</span> ' +
                                '<span data-notify="message">{2}</span>' +
                                '<div class="progress" data-notify="progressbar">' +
                                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                                '</div>' +
                                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                                '</div>'
                        });

                    return;
                }

                let endpoint = Eventos.BaseUrl + "Eliminar/" + document.querySelector("#Modal-Eventos-Eliminar").dataset.codigo;
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
                                $("#Modal-Eventos-Eliminar").modal('hide');
                                Eventos.DataTable.ajax.reload();
                            });
                        } else {
                            swal(response.titulo, response.mensaje, "warning");
                        }


                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    });


            });
        }

        let Modal_Eventos_Encuesta_Horarios = document.querySelector("#Modal-Eventos-Encuesta-Horarios");
        if (Modal_Eventos_Encuesta_Horarios) {
            Modal_Eventos_Encuesta_Horarios.addEventListener("click", (e) => {
                e.preventDefault();

                let codigo = document.querySelector("#Modal-Eventos").dataset.codigo;
                let endpoint = Eventos.BaseUrl + "Obtener/" + codigo;
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
                    
                        document.querySelector("#Modal-Eventos-Horario-Tabla tbody").innerHTML = "";
                        let html = '';
                        response.fechas.forEach((item, i) => {
                            var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
                            var date = new Date(item.fecha);
                            var fecha = formatter.format(date);

                            html += '<tr>';
                            html += '<td>' + (i + 1) + '</td>';
                            html += '<td data-fecha="' + item.fecha + '">' + fecha + '</td>';
                            html += '<td><input class="form-control clsHoraInicio" type="text" value="' + item.horA_INICIO + '" /></td>';
                            html += '<td><input class="form-control clsHoraFin" type="text" value="' + item.horA_FIN + '" /></td>';
                            html += '<td><div class="checkbox"><input  id="Modal-Eventos-Horario-Tabla-' + (i + 1) + '" type="checkbox" ' + (item.eS_ENCUESTA == "S" ? ' checked="" ' : ' ') + '><label for="Modal-Eventos-Horario-Tabla-' + (i + 1) + '">Encuesta</label></div></td>';
                            html += '<td><div class="checkbox"><input  id="Eventos-Horario-Tabla-Cuestionario-' + (i + 1) + '" type="checkbox" ' + (item.eS_CUESTIONARIO == "S" ? ' checked="" ' : ' ') + '><label for="Eventos-Horario-Tabla-Cuestionario-' + (i + 1) + '">Cuestionario</label></div></td>';


                            html += '</tr>';
                        });

                        document.querySelector("#Modal-Eventos-Horario-Tabla tbody").innerHTML = html;

                        $(".clsHoraInicio").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });
                        $(".clsHoraFin").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });


                        $("#Modal-Eventos-Horario").modal({
                            keyboard: true,
                            backdrop: 'static'
                        })


                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    }); 

            })
        }

        let Modal_Eventos_Cuestionario_Horarios = document.querySelector("#Modal-Eventos-Cuestionario-Horarios");
        if (Modal_Eventos_Cuestionario_Horarios) {
            Modal_Eventos_Cuestionario_Horarios.addEventListener("click", (e) => {
                e.preventDefault();

                let codigo = document.querySelector("#Modal-Eventos").dataset.codigo;
                let endpoint = Eventos.BaseUrl + "Obtener/" + codigo;
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

                        document.querySelector("#Modal-Eventos-Horario-Tabla tbody").innerHTML = "";
                        let html = '';
                        response.fechas.forEach((item, i) => {
                            var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
                            var date = new Date(item.fecha);
                            var fecha = formatter.format(date);

                            html += '<tr>';
                            html += '<td>' + (i + 1) + '</td>';
                            html += '<td data-fecha="' + item.fecha + '">' + fecha + '</td>';
                            html += '<td><input class="form-control clsHoraInicio" type="text" value="' + item.horA_INICIO + '" /></td>';
                            html += '<td><input class="form-control clsHoraFin" type="text" value="' + item.horA_FIN + '" /></td>';
                            html += '<td><div class="checkbox"><input  id="Modal-Eventos-Horario-Tabla-' + (i + 1) + '" type="checkbox" ' + (item.eS_ENCUESTA == "S" ? ' checked="" ' : ' ') + '><label for="Modal-Eventos-Horario-Tabla-' + (i + 1) + '">Encuesta</label></div></td>';
                            html += '<td><div class="checkbox"><input  id="Eventos-Horario-Tabla-Cuestionario-' + (i + 1) + '" type="checkbox" ' + (item.eS_CUESTIONARIO == "S" ? ' checked="" ' : ' ') + '><label for="Eventos-Horario-Tabla-Cuestionario-' + (i + 1) + '">Cuestionario</label></div></td>';


                            html += '</tr>';
                        });

                        document.querySelector("#Modal-Eventos-Horario-Tabla tbody").innerHTML = html;

                        $(".clsHoraInicio").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });
                        $(".clsHoraFin").inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });


                        $("#Modal-Eventos-Horario").modal({
                            keyboard: true,
                            backdrop: 'static'
                        })


                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    }); 

            })
        }


        let Modal_Eventos_Horario_Guardar = document.querySelector("#Modal-Eventos-Horario-Guardar");
        if (Modal_Eventos_Horario_Guardar) {
            Modal_Eventos_Horario_Guardar.addEventListener("click", e => {
                e.preventDefault();

                let fechas = [];
                Array.from(document.querySelectorAll("#Modal-Eventos-Horario-Tabla tbody tr")).forEach((item, i) => {
                    let horario = {};
                    $(item).find('td').each(function (iTD, itemTD) {
                        
                        if (iTD == 1) {
                            horario.fecha = $(itemTD).data("fecha")
                        }

                        if (iTD == 2) {
                            horario.horA_INICIO = $(itemTD).find("input").val() + ":00"
                        }

                        if (iTD == 3) {
                            horario.horA_FIN = $(itemTD).find("input").val() + ":00"
                        }

                        if (iTD == 4) {
                            horario.eS_ENCUESTA = ($(itemTD).find("input")[0].checked ? "S" : "N");
                            horario.iD_ENCUESTA = ($(itemTD).find("input")[0].checked ? parseInt(document.querySelector("#Modal-Eventos-Encuesta").value) : null);
                        }

                        if (iTD == 5) {
                            horario.eS_CUESTIONARIO = ($(itemTD).find("input")[0].checked ? "S" : "N")
                            horario.iD_CUESTIONARIO = ($(itemTD).find("input")[0].checked ? parseInt(document.querySelector("#Modal-Eventos-Cuestionario").value) : null);
                        }

                    })
                    fechas.push(horario);
                })

                let endpoint = Eventos.BaseUrl + "ActualizarFechas?id=" + document.querySelector("#Modal-Eventos").dataset.codigo;
                fetch(endpoint, {
                    method: "PUT",
                    body: JSON.stringify(fechas),
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
                                $("#Modal-Eventos-Horario").modal('hide');
                                Eventos.DataTable.ajax.reload();
                            });
                        } else {
                            swal(response.titulo, response.mensaje, "warning");
                        }

                    }).catch((error) => {

                        swal("Upss, algo salio mal", error, "warning");
                    });

            });
        }

        let Modal_Eventos_Banner_Ver = document.querySelector("#Modal-Eventos-Banner-Ver");
        if (Modal_Eventos_Banner_Ver) {
            Modal_Eventos_Banner_Ver.addEventListener("click", e => {
                e.preventDefault();

                let codigo = document.querySelector("#Modal-Eventos").dataset.codigo;
                let endpoint = Eventos.BaseUrl + "ObtenerBanner?id=" + codigo;
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

                        if (response.success) {
                            window.open(response.url, '_blank').focus();
                        } else {
                            swal(response.titulo, response.mensaje, "warning");
                        }


                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    });

            });
        }

        let Modal_Eventos_Formato_Ver = document.querySelector("#Modal-Eventos-Formato-Ver");        
        if (Modal_Eventos_Formato_Ver) {
            Modal_Eventos_Formato_Ver.addEventListener("click", e => {
                e.preventDefault();

                let codigo = document.querySelector("#Modal-Eventos").dataset.codigo;
                let endpoint = Eventos.BaseUrl + "ObtenerFormato?id=" + codigo;
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

                        if (response.success) {
                            window.open(response.url, '_blank').focus();
                        } else {
                            swal(response.titulo, response.mensaje, "warning");
                        }


                    }).catch((error) => {
                        document.querySelector(".page-loader-wrapper").style.display = "none";
                        swal("Upss, algo salio mal", error, "warning");

                    });
                     
            });
        }

    },
 
    List: function () {
        let endpoint = Eventos.BaseUrl + "Paginar";

        Eventos.DataTable = $(Eventos.Table)
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
                            modalidad: parseInt(document.querySelector("#Eventos-Buscar-Modalidad").value),
                            tipo: parseInt(document.querySelector("#Eventos-Buscar-Tipo").value),
                            estado: parseInt(document.querySelector("#Eventos-Buscar-Estado").value)
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
                        targets: [3],
                        class: "text-right",
                        width: "5%",
                    },

                    {
                        targets: [5,6],
                        class: "text-center",
                        width: "5%",
                    },

                ], 

                columns: [
                    { data: "iD_EVENTO", name: "iD_EVENTO", width: "5%" },
                    { data: "nombre", name: "nombre", width: "40%" },                    
                    {
                        data: "iD_EVENTO",
                        name: "iD_EVENTO",
                        width: "10%",
                        render: function (data, type, row) {
                            var formatter = new Intl.DateTimeFormat('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });

                            var date = new Date(row.fechA_INICIO);                            
                            var fechaInicio = formatter.format(date);

                            date = new Date(row.fechA_FIN);                            
                            var fechaFin = formatter.format(date);

                            return fechaInicio + "<br>" + fechaFin
                        },
                    },
                    { data: "nuM_PARTICIPANTES", name: "nuM_PARTICIPANTES", width: "5%" },                    
                    {
                        data: "iD_EVENTO",
                        name: "iD_EVENTO",
                        width: "10%",
                        render: function (data, type, row) {                            
                            return row.estado.nombre;
                        },
                    },

                    {
                        data: "iD_EVENTO",
                        name: "iD_EVENTO",
                        width: "5%",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-info btn-icon  btn-icon-mini btn-round" onclick="javascript:Eventos.Edit(' + row.iD_EVENTO + ');" title="Editar">' +
                                '<i class= "material-icons"> border_color</i>' +
                                '</button>';

                            return html;
                        },
                    },

                    {
                        data: "iD_EVENTO",
                        width: "5%",
                        name: "iD_EVENTO",
                        render: function (data, type, row) {
                            let html =
                                '<button type="button" class="btn btn-danger btn-icon  btn-icon-mini btn-round" onclick="javascript:Eventos.Delete(' + row.iD_EVENTO + ');" title="Eliminar">' +
                                '<i class= "material-icons"> delete_forever</i>' +
                                '</button> ';

                            return html;
                        },
                    },
 
                ],
                select: true,
                buttons: [],

                language: {
                    url: "/assets/plugins/datatables/Spanish.json",
                },
            });

        
    },

    Remove: function(obj){
      
    },

    Edit: function (codigo) {
        let endpoint = Eventos.BaseUrl + "Obtener/" + codigo; 
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
                Eventos.Clear();
                 
                document.querySelector("#Modal-Eventos").dataset.codigo = response.iD_EVENTO;
                document.querySelector("#Modal-Eventos-Codigo").value = response.coD_EVENTO;
                document.querySelector("#Modal-Eventos-Participantes").value = response.nuM_PARTICIPANTES;
                document.querySelector("#Modal-Eventos-Nombre").value = response.nombre;

                document.querySelector("#Modal-Eventos-Modalidad").value = response.iD_MODALIDAD;
                $('#Modal-Eventos-Modalidad').val(response.iD_MODALIDAD).trigger('change').trigger('select2:select');

                document.querySelector("#Modal-Eventos-Tipo").value = response.iD_TIPO_EVENTO;
                document.querySelector("#Modal-Eventos-Tipo").dispatchEvent(new Event("change"));

                document.querySelector("#Modal-Eventos-Descripcion").value = response.descripcion;

                document.querySelector("#Modal-Eventos-Banner").value = "";
                document.querySelector("#txt_file_text").innerHTML = response.banner;

                document.querySelector("#Modal-Eventos-Formato").value = "";
                document.querySelector("#Modal-Eventos-Formato-Label").innerHTML = response.formato;


                document.querySelector("#Modal-Eventos-Banner-Ver").disabled = (response.banner == null ? true: false);
                document.querySelector("#Modal-Eventos-Formato-Ver").disabled = (response.formato == null ? true : false);


                document.querySelector("#Modal-Eventos-Wasape").value = response.enlacE_WHATSAPP;

                document.querySelector("#Modal-Eventos-Estado").value = response.iD_ESTADO;
                document.querySelector("#Modal-Eventos-Estado").dispatchEvent(new Event("change"));


                document.querySelector("#Modal-Eventos-HInicio").value = response.horA_INICIO;
                document.querySelector("#Modal-Eventos-HFin").value = response.horA_FIN;

                document.querySelector("#Modal-Eventos-Lugar").value = response.nombrE_LUGAR;

                document.querySelector("#Modal-Eventos-Distrito").value = response.iD_UBIGEO;
                document.querySelector("#Modal-Eventos-Distrito").dispatchEvent(new Event("change"));

                document.querySelector("#Modal-Eventos-Enlace").value = response.reD_SOCIAL;


                document.querySelector("#Modal-Eventos-Encuesta").value = (response.iD_ENCUESTA == null ? 0 : response.iD_ENCUESTA) ;
                document.querySelector("#Modal-Eventos-Encuesta").dispatchEvent(new Event("change"));
                if (response.iD_ENCUESTA == null) {
                    document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = true;
                } else {
                    document.querySelector("#Modal-Eventos-Encuesta-Horarios").disabled = false;
                }
                

                document.querySelector("#Modal-Eventos-Cuestionario").value = (response.iD_CUESTIONARIO == null ? 0 : response.iD_CUESTIONARIO); ;
                document.querySelector("#Modal-Eventos-Cuestionario").dispatchEvent(new Event("change"));
                if (response.iD_CUESTIONARIO == null) {
                    document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = true;
                } else {
                    document.querySelector("#Modal-Eventos-Cuestionario-Horarios").disabled = false;
                }

                document.querySelector("#Modal-Eventos-Ticket").checked = (response.generA_TICKET == "S" ? true : false);
                document.querySelector("#Modal-Eventos-Correo").checked = (response.enviA_CORREO == "S" ? true : false);
                
                document.querySelector("#Modal-Eventos-Requiere").checked = (response.requierE_DOCUMENTO == "S" ? true : false);

                if (response.requierE_DOCUMENTO == "S") {
                    document.querySelector("#Modal-Eventos-Documentos").parentElement.style.display = "block";
                    $("#Modal-Eventos-Documentos").val(response.documentos.map(d => d.iD_TIPO_DOCUMENTO_REQUERIDO)).trigger('change')

                } else {
                    $("#Modal-Eventos-Documentos").val('').trigger('change')
                    document.querySelector("#Modal-Eventos-Documentos").parentElement.style.display = "none";
                }
                 
                $('#Modal-Eventos-Inicio').datepicker('setStartDate', new Date(1990, 1, 1));


                var fechas = [];
                response.fechas.forEach((item, i) => {
                    fechas.push(new Date(item.fecha));
                });
                $('#Modal-Eventos-Inicio').datepicker('setDates', fechas);

 
                $("#Modal-Eventos").modal({
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
        Eventos.base64 = '';
        Eventos.base64Formato = '';

        document.querySelector("#Modal-Eventos-Codigo").value = "";
        document.querySelector("#Modal-Eventos-Participantes").value = "";
        document.querySelector("#Modal-Eventos-Nombre").value = "";

        document.querySelector("#Modal-Eventos-Modalidad").value = "0";
        document.querySelector("#Modal-Eventos-Modalidad").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Eventos-Tipo").value = "0";
        document.querySelector("#Modal-Eventos-Tipo").dispatchEvent(new Event("change"));


        document.querySelector("#Modal-Eventos-Wasape").value = "";

        document.querySelector("#Modal-Eventos-Enlace").value = "";
        document.querySelector("#Modal-Eventos-Enlace").disabled = true;

        document.querySelector("#Modal-Eventos-Descripcion").value = "";

        document.querySelector("#Modal-Eventos-HInicio").value = "";
        document.querySelector("#Modal-Eventos-HFin").value = "";

        document.querySelector("#Modal-Eventos-Lugar").value = "";
        document.querySelector("#Modal-Eventos-Lugar").disabled = true;

        document.querySelector("#Modal-Eventos-Distrito").value = "0";
        document.querySelector("#Modal-Eventos-Distrito").dispatchEvent(new Event("change"));
        document.querySelector("#Modal-Eventos-Distrito").disabled = true;
        
        document.querySelector("#Modal-Eventos-Banner").value = "";
        document.querySelector("#txt_file_text").innerHTML = "";

        document.querySelector("#Modal-Eventos-Encuesta").value = "0";
        document.querySelector("#Modal-Eventos-Encuesta").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Eventos-Cuestionario").value = "0";
        document.querySelector("#Modal-Eventos-Cuestionario").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Eventos-Estado").value = "0";
        document.querySelector("#Modal-Eventos-Estado").dispatchEvent(new Event("change"));

        document.querySelector("#Modal-Eventos-Ticket").checked = true;
        document.querySelector("#Modal-Eventos-Correo").checked = true;
        document.querySelector("#Modal-Eventos-Requiere").checked = false;

        document.querySelector("#Modal-Eventos-Banner-Ver").disabled = true;
        document.querySelector("#Modal-Eventos-Formato-Ver").disabled = true;

        $("#Modal-Eventos-Documentos").val('').trigger('change');
        document.querySelector("#Modal-Eventos-Documentos").parentElement.style.display = "none";

    },

    onFileSelectedEvento: function (event) {
        var selectedFile = event.target.files[0];
        $('#txt_file_text').text(selectedFile.name);

        new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(selectedFile);
            reader.onload = () => resolve(Eventos.base64 = reader.result.split(',')[1]);
            reader.onerror = error => reject(error);
        }); 
    },

    onFileSelectedFormato: function (event) {
        var selectedFile = event.target.files[0];
        $('#Modal-Eventos-Formato-Label').text(selectedFile.name);

        new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(selectedFile);
            reader.onload = () => resolve(Eventos.base64Formato = reader.result.split(',')[1]);
            reader.onerror = error => reject(error);
        });
    },

    Delete: function (codigo) {
        grecaptcha.reset();

        let endpoint = Eventos.BaseUrl + "Obtener/" + codigo;
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

                document.querySelector("#Modal-Eventos-Eliminar").dataset.codigo = data.iD_EVENTO;
                document.querySelector("#Modal-Eventos-Eliminar-Nombre").innerHTML = data.nombre;

                $("#Modal-Eventos-Eliminar").modal({
                    keyboard: true,
                    backdrop: 'static'
                })


            }).catch((error) => {
                document.querySelector(".page-loader-wrapper").style.display = "none";
                swal("Upss, algo salio mal", error, "warning");

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