var Configuracion = {
    BaseUrl: "/configuracion/",
    DataTable: null,

    Table: null,


    Init: () => {
         
        setTimeout(() => {
            CKEDITOR.replace('Configuracion-Notificacion-Texto-Editor');
            CKEDITOR.replace('Configuracion-Notificacion-Adjunto-Editor');
            CKEDITOR.replace('Configuracion-Certificado-Editor');
            CKEDITOR.replace('Configuracion-Constancia-Editor')
            CKEDITOR.replace('Configuracion-Documentos-Editor');

            CKEDITOR.config.height = 500;

             Configuracion.List();            
        }, 100);

        Configuracion.Events(); 
 
    },

    Events: () => {
        let Configuracion_Guardar = document.querySelector("#Configuracion-Guardar");
        if (Configuracion_Guardar) {
            Configuracion_Guardar.addEventListener("click", e => {
                e.preventDefault();

                let params = {
                    "textO_NOTIFICACION": CKEDITOR.instances["Configuracion-Notificacion-Texto-Editor"].getData(),
                    "textO_NOTIFICACION_ADJUNTO": CKEDITOR.instances["Configuracion-Notificacion-Adjunto-Editor"].getData(),
                    "textO_CERTIFICADO": CKEDITOR.instances["Configuracion-Certificado-Editor"].getData(),
                    "textO_CONSTANCIA": CKEDITOR.instances["Configuracion-Constancia-Editor"].getData(),
                    "textO_DOCUMENTO": CKEDITOR.instances["Configuracion-Documentos-Editor"].getData(),
                }

                let endpoint = Configuracion.BaseUrl + "Actualizar";
                let metodo = "PUT";

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
         
    },

    List: function () {
        
             
    },

    Remove: function(obj){
         
    },

    Edit: function (codigo) {
         
    },

    Update : () =>{
        
    },
 
    Clear: () => {

    }, 

    Delete: function (codigo) {
        
    },

    getFormData: function (object) {
        const formData = new FormData();
        Object.keys(object).forEach(key => formData.append(key, object[key]));
        return formData;
    },

    groupBy: function (objectArray, property) {
        return objectArray.reduce(function (acc, obj) {
            var key = obj[property];
            if (!acc[key]) {
                acc[key] = [];
            }
            acc[key].push(obj);
            return acc;
        }, {});
    }
}

