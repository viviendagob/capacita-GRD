
var Login = {
    BaseUrl: "/login/",
    DataTable: null,
    Paises: null,
    Distritos: null,
    Profesiones: null,
    Cargos: null,
    Entidades: null,
    Table: null,
    base64: null,
    base64Formato: null,
    esVirtual: 2,

    Init: () => {

        setTimeout(() => {
            Login.Events();

            Login.List();
        }, 500);

    },

    Events: () => {

        let Login_Ingresar = document.querySelector("#Login-Ingresar");
        if (Login_Ingresar) {
            Login_Ingresar.addEventListener("click", e => {
                e.preventDefault();

                let params = {
                    Username: document.querySelector("#Login-Usuario").value,
                    Password: document.querySelector("#Login-Clave").value
                }

                let errores = [];

                if (params.Username == "") {
                    errores.push("* Ingrese el Usuario");
                }

                if (params.Password == "") {
                    errores.push("* Ingrese la Clave");
                }

                if (errores.length > 0) {
                    swal("Advertencia", errores.join('\n'), "warning");

                    return;
                }

                let endpoint = Login.BaseUrl +'Login';
                let metodo = "POST";

            /*
                swal({
                    title: "Confirme?",
                    text: "Esta seguro que desea acceder",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#FF3636",
                    confirmButtonText: "Si, ingresar",
                    closeOnConfirm: false
                }, function (isConfirm) {
                    if (isConfirm) {
                        document.querySelector(".page-loader-wrapper").style.display = "block";
            */
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
                                //document.querySelector(".page-loader-wrapper").style.display = "none";


                                if (response.success) {
                                    swal({
                                        title: response.titulo,
                                        text: response.mensaje,
                                        type: "success"
                                    }, () => {

                                        window.location.href = '/Principal/BandejaPrincipal';
                                    });
                                } else {
                                    swal(response.titulo, response.mensaje, "warning");
                                }

                            }).catch((error) => {
                                //document.querySelector(".page-loader-wrapper").style.display = "none";
                                swal("Upss, algo salio mal", error, "warning");
                            });
            /*
                    }
                });
            */

                

            });
        }

        let Login_Cerrar_Session = document.querySelector("#Login-Cerrar-Session");
        if (Login_Cerrar_Session) {

            Login_Cerrar_Session.addEventListener("click", e => {
                e.preventDefault();

                let params = {
                }

                let endpoint = Login.BaseUrl + 'CerrarSesion';
                let metodo = "POST";

                
                swal({
                        title: "Confirme?",
                        text: "Esta seguro que desea salir",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#FF3636",
                        confirmButtonText: "Si, salir",
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
                                        window.location.href = '/Principal/BandejaPrincipal';
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

        
    },
 
    List: function () {

    },

    Remove: function(){
      
    },

    Edit: function () {
 
    },

    Update : () =>{
        
    },
 
    Clear: () => {
        Login.base64 = '';
        Login.base64Formato = '';

        document.querySelector("#Login-Usuario").value = "";
        document.querySelector("#Login-Clave").value = "";

    },

    onFileSelectedEvento: function (event) {

    },

    onFileSelectedFormato: function (event) {

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