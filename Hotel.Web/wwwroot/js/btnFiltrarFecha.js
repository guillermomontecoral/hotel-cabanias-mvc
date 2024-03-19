document.querySelector("#formValidarFechas").addEventListener('submit', validarFechas);

function validarFechas(e) {

    e.preventDefault();

    let f1 = document.querySelector("#fch1").value;
    let f2 = document.querySelector("#fch2").value;

    if (f1 != "" && f2 != "") {
        this.submit();
    } else {
        document.querySelector("#errorFiltro").innerHTML = "Debe seleccionar las dos fechas para filtrar.";
    }
}