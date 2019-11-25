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

function crearCalendario(inicioFecha, finFecha) {

    var inicioFecha = "/" + $("#inicioFecha").val().substring(3, $("#inicioFecha").val().length - 3) + "/";
    var finFecha = "/" + $("#finFecha").val().substring(3, $("#finFecha").val().length - 3) + "/";

    //Control de kendoCalendar
    var dates = [];
    var fechas = $("#fechasConcatendas").val().split(",")

    for (var index = 0; index < fechas.length; index++) {
        dates.push(new Date(fechas[index]));
    }

    $(".kendocalendar").kendoDatePicker({
        format: "dd/MM/yyyy",
        min: inicioFecha,
        max: finFecha,
        disableDates: dates
        , change: function () {
            if ($("#NumDias0").val() != '') {
                //Calcular la fecha fin de vacaciones
                $.getJSON("/Formulario/CalcularFechaFin", {
                    NumeroDias: parseInt($("#NumDias0").val()),
                    FechaInicio: $("#kendocalendar_0").val(),
                    DiasFestivosSabadosDomingos: $("#fechasConcatendas").val(),
                }, function (oRespuesta) {
                    switch (oRespuesta.Codigo) {

                        case "0":
                            var fechas = oRespuesta.Resultado.Data.split("/");
                            $("#kendocalendar_1").val(new Date(parseInt(fechas[2]), parseInt(fechas[1] - 1), parseInt(fechas[0])).toJSON().slice(0, 10).split('-').reverse().join('/'));
                            break;

                        case "-1":
                            swal.fire({
                                text: respuesta.Mensaje,
                                type: 'warning',
                                confirmButtonText: "OK",
                                confirmButtonColor: '#00AFF0',
                                allowOutsideClick: false,
                                allowEscapeKey: false
                            }).then((result) => {
                                if (result.value) {
                                    $("#kendocalendar_1").val('');
                                }
                            });
                            break;
                    }
                })
            }
        }
    });
}

function validarNumDias() {
    //Validacion del campo numero de dias en pantalla principal
    $("#NumDias0").focusout(function () {
        if ($("#NumDias0").val() != '') {        

            $.getJSON("/Formulario/ValidarCantidadDias", { NumeroDias: parseInt($("#NumDias0").val()), NumDiasDisponibles: parseFloat($("#SpanNumDias0").html()), MinimoDias: parseInt($("#MinimoDias").val()) }, function (respuesta) {
               
                switch (respuesta.Codigo) {

                    case "1":
                        swal.fire({
                            text: respuesta.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        }).then((result) => {
                            if (result.value) {
                                setTimeout(function () {
                                    $("#kendocalendar_1").val('');
                                    $("#NumDias0").val('');
                                    $("#NumDias0").focus();
                                }, 500)
                            }
                        });
                        break;

                    case "2":
                        swal.fire({
                            text: respuesta.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        }).then((result) => {
                            if (result.value) {
                                setTimeout(function () {
                                    $("#kendocalendar_1").val('');
                                    $("#NumDias0").val('');
                                    $("#NumDias0").focus();
                                }, 500)
                            }
                        });
                        break;
                }
            })
        }
    });

}

function calcularFechaFin() {

    $("#NumDias0").change(function () {
        if ($("#NumDias0").val() != '' && $("#kendocalendar_0").val() != '') {
          
            $.getJSON("/Formulario/CalcularFechaFin", {
                NumeroDias: parseInt($("#NumDias0").val()),
                FechaInicio: $("#kendocalendar_0").val(),
                DiasFestivosSabadosDomingos: $("#fechasConcatendas").val()
            }, function (oRespuesta) {
               
                switch (oRespuesta.Codigo) {

                    case "0":
                        var fechas = oRespuesta.Resultado.Data.split("/");
                        $("#kendocalendar_1").val(new Date(parseInt(fechas[2]), parseInt(fechas[1] - 1), parseInt(fechas[0])).toJSON().slice(0, 10).split('-').reverse().join('/'));
                        break;

                    case "-1":
                        swal.fire({
                            text: respuesta.Mensaje,
                            type: 'warning',
                            confirmButtonText: "OK",
                            confirmButtonColor: '#00AFF0',
                            allowOutsideClick: false,
                            allowEscapeKey: false
                        }).then((result) => {
                            if (result.value) {
                                $("#kendocalendar_1").val('');
                            }
                        });
                        break;
                }
            })
        }
    });

}

function guardarSolicitud(oURL) {

    //Evento del boton aregar dentro de la ventana Principal
    $("#btnAgregar0").click(function () {
        if ($("#NroIdentificacionHidden").val() != '' && $("#NumDias0").val() != '' && $("#kendocalendar_0").val() != '') {

            $("#DivFondoTrasparente").show();
            $("#DivCirculoAzul").show();

            $.post("../Formulario/ConsultaFechasSolicitudExistentes",
                {
                    oIdentificacion: $("#NroIdentificacionHidden").val(),
                    FechaInicio: $("#kendocalendar_0").val(),
                    FechaFin: $("#kendocalendar_1").val(),
                }, function (oRespuestaFechasSolicitudes) {
                    $("#DivFondoTrasparente").hide();
                    $("#DivCirculoAzul").hide();
                    switch (oRespuestaFechasSolicitudes.Codigo) {
                        case "-1":
                            swal.fire({
                                text: oRespuestaFechasSolicitudes.Mensaje,
                                type: 'error',
                                confirmButtonText: "OK",
                                confirmButtonColor: '#00AFF0',
                                allowOutsideClick: false,
                                allowEscapeKey: false
                            });
                            break;
                        case "-2":
                            swal.fire({
                                text: oRespuestaFechasSolicitudes.Mensaje,
                                type: 'error',
                                confirmButtonText: "OK",
                                confirmButtonColor: '#00AFF0',
                                allowOutsideClick: false,
                                allowEscapeKey: false
                            });
                            break;
                        case "-3":
                            swal.fire({
                                text: oRespuestaFechasSolicitudes.Mensaje,
                                type: 'error',
                                confirmButtonText: "OK",
                                confirmButtonColor: '#00AFF0',
                                allowOutsideClick: false,
                                allowEscapeKey: false
                            });
                            break;
                        case "1":
                            $("#DivFondoTrasparente").show();
                            $("#DivCirculoAzul").show();
                            $.post("../Formulario/GuardarSolicitud",
                                {
                                    NroIdentificacion: $("#NroIdentificacionHidden").val(),
                                    NombresEmpleado: $("#NombresEmpleado").val(),
                                    ApellidosEmpleado: $("#ApellidosEmpleado").val(),
                                    oRespuestaSAP: $("#oRespuestaSAPModels").val(),
                                    oRespuestaMotor: $("#oRespuestaMotor").val(),
                                    NumeroDias: $("#NumDias0").val(),
                                    FechaInicio: $("#kendocalendar_0").val(),
                                    FechaFin: $("#kendocalendar_1").val(),
                                    NroMinDiasCorreoCompensacion: parseInt($("#NroMinDiasCorreoCompensacion").val()),
                                    CorreoCompensacion: $("#CorreoCompensacion").val(),
                                }, function (oRespuestaGuardar) {

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    switch (oRespuestaGuardar.Codigo) {

                                        case "1":
                                            swal.fire({
                                                text: oRespuestaGuardar.Mensaje,
                                                type: 'success',
                                                confirmButtonText: "OK",
                                                confirmButtonColor: '#00AFF0',
                                                allowOutsideClick: false,
                                                allowEscapeKey: false
                                            }).then((result) => {
                                                if (result.value) {
                                                    var url = oURL;
                                                    window.location.href = url
                                                }
                                            });
                                            break;

                                        case "-1":
                                            swal.fire({
                                                text: oRespuestaGuardar.Mensaje,
                                                type: 'error',
                                                confirmButtonText: "OK",
                                                confirmButtonColor: '#00AFF0',
                                                allowOutsideClick: false,
                                                allowEscapeKey: false
                                            });
                                            break;

                                        case "-2":
                                            swal.fire({
                                                text: oRespuestaGuardar.Mensaje,
                                                type: 'error',
                                                confirmButtonText: "OK",
                                                confirmButtonColor: '#00AFF0',
                                                allowOutsideClick: false,
                                                allowEscapeKey: false
                                            });
                                            break;

                                        case "-3":
                                            swal.fire({
                                                text: oRespuestaGuardar.Mensaje,
                                                type: 'error',
                                                confirmButtonText: "OK",
                                                confirmButtonColor: '#00AFF0',
                                                allowOutsideClick: false,
                                                allowEscapeKey: false
                                            });
                                            break;

                                    }
                                })

                            break;

                        case "2":
                            swal.fire({
                                text: oRespuestaFechasSolicitudes.Mensaje,
                                type: 'warning',
                                confirmButtonText: "OK",
                                confirmButtonColor: '#00AFF0',
                                allowOutsideClick: false,
                                allowEscapeKey: false
                            });
                            break;
                    }
                });
        }
        else {
            swal.fire({
                text: "Debe ingresar el número de días y la fecha de inicio para la solicitud de vacaciones",
                type: 'warning',
                confirmButtonText: "OK",
                confirmButtonColor: '#00AFF0',
                allowOutsideClick: false,
                allowEscapeKey: false
            })
        };
    });

}