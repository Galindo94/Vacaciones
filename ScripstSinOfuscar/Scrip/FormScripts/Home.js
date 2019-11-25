


function cargarPagina() {
    //Funcion utilizada para evitar que se pongna decimales, "." y "," dentro del campo numeor de dias
    (function ($) {
        $.fn.inputFilter = function (inputFilter) {
            return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
                if (inputFilter(this.value)) {
                    this.oldValue = this.value;
                    this.oldSelectionStart = this.selectionStart;
                    this.oldSelectionEnd = this.selectionEnd;
                } else if (this.hasOwnProperty("oldValue")) {
                    this.value = this.oldValue;
                    this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
                }
            });
        };
    }(jQuery));

    $("#NroDocumento").inputFilter(function (value) {
        return /^\d*$/.test(value);
    });

    $("#btnAgregar0").click(function () {
        if ($("#NroDocumento").val() != '') {

            $("#DivFondoTrasparente").show();
            $("#DivCirculoAzul").show();

            $.getJSON("/Home/ConsultarUsuariosAPISAP", { NroIdentificacion: $("#NroDocumento").val() }, function (oRespuestaSAP) {
                switch (oRespuestaSAP.Codigo) {

                    case "6":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });
                        break;

                    case "5":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });
                        break;

                    case "4":
                        $.post("../Home/ConsultaMotorDeReglas", { RespuestaSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, function (oRespuestaMotorReglas) {

                            switch (oRespuestaMotorReglas.Codigo) {

                                case "1":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    $("#NroDocumento").val('');

                                    var resultado = oRespuestaMotorReglas.Resultado.Data.Escenario[0].Url.split('$');
                                    var url = $("#RedirectTo").val();
                                    url = url.replace("ActionName", resultado[0]);
                                    url = url.replace("ControllerName", resultado[1]);
                                    $.redirect(url, { oDatosFormulario: JSON.stringify(oRespuestaMotorReglas.Resultado.Data), oDatosSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, "POST", '_blank');

                                    //window.close('', '_parent', '');

                                    break;

                                case "-1":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;

                                case "-2":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;

                                case "-3":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;
                            }
                        });
                        break;

                    case "3":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'error',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });
                        break;

                    case "2":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'error',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });
                        break;

                    case "1":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });
                        break;

                    case "-1":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'error',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });

                        break;

                    case "-2":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'error',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });

                        break;


                    case "-3":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'error',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });

                        break;

                    case "-4":

                        $("#DivFondoTrasparente").hide();
                        $("#DivCirculoAzul").hide();

                        swal.fire({
                            text: oRespuestaSAP.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        });

                        break;

                    case "-5":
                        $.post("../Home/ConsultaMotorDeReglas", { RespuestaSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, function (oRespuestaMotorReglas) {

                            switch (oRespuestaMotorReglas.Codigo) {

                                case "1":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    $("#NroDocumento").val('');

                                    var resultado = oRespuestaMotorReglas.Resultado.Data.Escenario[0].Url.split('$');
                                    var url = $("#RedirectTo").val();
                                    url = url.replace("ActionName", resultado[0]);
                                    url = url.replace("ControllerName", resultado[1]);
                                    $.redirect(url, { oDatosFormulario: JSON.stringify(oRespuestaMotorReglas.Resultado.Data), oDatosSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, "POST", '_blank');


                                    break;

                                case "-1":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;

                                case "-2":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;

                                case "-3":

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    swal.fire({
                                        text: oRespuestaMotorReglas.Mensaje,
                                        type: 'error',
                                        confirmButtonText: "OK",
                                        confirmButtonColor: '#00AFF0',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false
                                    });
                                    break;
                            }
                        });
                        break;
                }
            });
        }
        else {
            swal.fire({
                text: "El campo Nro. de identificación es obligatorio",
                type: 'warning',
                confirmButtonText: "OK",
                confirmButtonColor: '#00AFF0',
                allowOutsideClick: false,
                allowEscapeKey: false
            });
        }
    });
}