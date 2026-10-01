function soloLetras(evento){
    key = evento.keyCode || evento.which;
      teclado = String.fromCharCode(key).toLocaleLowerCase();
      letras = " áéíóúabcdefghijklmnñopqrstuvwxyz";
      especiales = "8-32-37-38-39-46-164";

      teclado_especial = false;
      for (var i in especiales) {
          if (key == especiales[i]) {
              teclado_especial = true; break;
          }
      }
      if (letras.indexOf(teclado) == -1 && !teclado_especial) {
          return false; 
      }
    } 

    function soloNumeros(evento){
        key = evento.keyCode || evento.which;
          teclado = String.fromCharCode(key).toLocaleLowerCase();
          letras = "+0123456789";
          especiales = "-32-37-38-39-46-164";
    
          teclado_especial = false;
          for (var i in especiales) {
              if (key == especiales[i]) {
                  teclado_especial = true; break;
              }
          }
          if (letras.indexOf(teclado) == -1 && !teclado_especial) {
              return false; 
          }
        }     


     //oninput="soloLetras(this)"
const soloLetrasNoNumeros = function(campo) {
    let valor = campo.value;    
    if(/\d/.test(valor)) {    
      campo.value = valor.replace(/\d/g,'');
    }    
  };