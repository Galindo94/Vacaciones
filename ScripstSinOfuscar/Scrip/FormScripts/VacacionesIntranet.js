$("#DivFondoTrasparente").show();
$("#DivCirculoAzul").show();

function cargarPagina() {

    $.post("../../VacacionesIntranet/ConsultarUsuarioDirectorioActivo", { NombreUsuario: $("#UsuarioIntranet").val() },
        function (oRespuestaDA) {
            switch (oRespuestaDA.Codigo) {

                case "0":
                    $.post("../VacacionesIntranet/ConsultarUsuarioServicioSAP", { UserDA: JSON.stringify(oRespuestaDA.Resultado.Data) }, function (oRespuestaSAP) {

                        switch (oRespuestaSAP.Codigo) {

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

                                $.post("../VacacionesIntranet/ConsultaMotorDeReglas", { RespuestaSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, function (oRespuestaMotorReglas) {

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    switch (oRespuestaMotorReglas.Codigo) {

                                        case "1":

                                            var resultado = oRespuestaMotorReglas.Resultado.Data.Escenario[0].Url.split('$');
                                            var url = $("#RedirectTo").val();
                                            url = url.replace("ActionName", resultado[0]);
                                            url = url.replace("ControllerName", resultado[1]);
                                            $.redirect(url, { oDatosFormulario: JSON.stringify(oRespuestaMotorReglas.Resultado.Data), oDatosSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, "POST");

                                            break;

                                        case "-1":

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
                                    type: 'error',
                                    confirmButtonText: "OK",
                                    confirmButtonColor: '#00AFF0',
                                    allowOutsideClick: false,
                                    allowEscapeKey: false
                                }).then((result) => {
                                    if (result.value) {

                                        var url = $("#RedirectTo").val();
                                        url = url.replace("ActionName", "Index");
                                        url = url.replace("ControllerName", "Home");
                                        $.redirect(url, {}, "POST");

                                    };

                                });

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

                                $.post("../VacacionesIntranet/ConsultaMotorDeReglas", { RespuestaSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, function (oRespuestaMotorReglas) {

                                    $("#DivFondoTrasparente").hide();
                                    $("#DivCirculoAzul").hide();

                                    switch (oRespuestaMotorReglas.Codigo) {

                                        case "1":

                                            var resultado = oRespuestaMotorReglas.Resultado.Data.Escenario[0].Url.split('$');
                                            var url = $("#RedirectTo").val();
                                            url = url.replace("ActionName", resultado[0]);
                                            url = url.replace("ControllerName", resultado[1]);
                                            $.redirect(url, { oDatosFormulario: JSON.stringify(oRespuestaMotorReglas.Resultado.Data), oDatosSAP: JSON.stringify(oRespuestaSAP.Resultado.Data) }, "POST");

                                            break;

                                        case "-1":

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
                    break;

                //El usuario no puede ser vacio
                case "-1":

                    $("#DivFondoTrasparente").hide();
                    $("#DivCirculoAzul").hide();

                    var url = $("#RedirectTo").val();
                    url = url.replace("ActionName", "Index");
                    url = url.replace("ControllerName", "Home");
                    $.redirect(url, {}, "POST");


                    break;

                // El usuario no se encontro en el DA
                case "-2":

                    $("#DivFondoTrasparente").hide();
                    $("#DivCirculoAzul").hide();

                    var url = $("#RedirectTo").val();
                    url = url.replace("ActionName", "Index");
                    url = url.replace("ControllerName", "Home");
                    $.redirect(url, {}, "POST");



                    break;

                // Error en el Servicio del DA
                case "-3":

                    $("#DivFondoTrasparente").hide();
                    $("#DivCirculoAzul").hide();

                    swal.fire({
                        text: oRespuestaDA.Mensaje,
                        type: 'error',
                        confirmButtonText: "OK",
                        confirmButtonColor: '#00AFF0',
                        allowOutsideClick: false,
                        allowEscapeKey: false
                    });

                    break;

                // Se encontro usuario pero no tiene informacion basica
                case "-4":

                    $("#DivFondoTrasparente").hide();
                    $("#DivCirculoAzul").hide();

                    var url = $("#RedirectTo").val();
                    url = url.replace("ActionName", "Index");
                    url = url.replace("ControllerName", "Home");
                    $.redirect(url, {}, "POST");


                    break;
            }
        });
}

