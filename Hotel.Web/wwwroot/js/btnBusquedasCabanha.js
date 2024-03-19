document.querySelector("#formBuscarTexto").addEventListener('submit', buscarTexto);
document.querySelector("#formBuscarTipo").addEventListener('submit', buscarTipo);
document.querySelector("#formBuscarCant").addEventListener('submit', buscarCant);
document.querySelector("#formBuscarHab").addEventListener('submit', buscarHab);

function buscarTexto(e) {

    e.preventDefault();
    let txtBuscarTexto = document.querySelector("#txtBuscarTexto").value;

    if (txtBuscarTexto != "") {
        this.submit();
    } else {
        document.querySelector("#errorBuscarTexto").innerHTML = "Debe ingresar algún texto a buscar.";
    }
}

function buscarTipo(e) {

    e.preventDefault();
    let slcTipoCabanha = document.querySelector("#slcTipoCabanha").value;

    if (slcTipoCabanha > 0) {
        this.submit();
    } else {
        document.querySelector("#errorSlcTipoCab").innerHTML = "Debe seleccionar un tipo de cabaña.";
    }
}

function buscarCant(e) {

    e.preventDefault();
    let numbCantCabanha = document.querySelector("#numbCantCabanha").value;

    if (numbCantCabanha > 0) {
        this.submit();
    } else {
        document.querySelector("#errorNumCant").innerHTML = "La cantidad de personas debe ser mayor a 0 (cero).";
    }
}

function buscarHab(e) {

    e.preventDefault();


    let checkCabHab = document.querySelector("#checkCabHab").checked;

    if (checkCabHab) {
        this.submit();
    } else {
        document.querySelector("#errorHab").innerHTML = "Debe activar el check para poder filtrar.";
    }
}