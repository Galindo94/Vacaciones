﻿(function (c) { c['fn']['inputFilter'] = function (d) { return this['on']('input\x20keydown\x20keyup\x20mousedown\x20mouseup\x20select\x20contextmenu\x20drop', function () { if (d(this['value'])) { this['oldValue'] = this['value']; this['oldSelectionStart'] = this['selectionStart']; this['oldSelectionEnd'] = this['selectionEnd']; } else if (this['hasOwnProperty']('oldValue')) { this['value'] = this['oldValue']; this['setSelectionRange'](this['oldSelectionStart'], this['oldSelectionEnd']); } }); }; }(jQuery)); function crearCalendarioPaginaPrincipal(e, f) { var e = '/' + $('#inicioFecha')['val']()['substring'](0x3, $('#inicioFecha')['val']()['length'] - 0x3) + '/'; var f = '/' + $('#finFecha')['val']()['substring'](0x3, $('#finFecha')['val']()['length'] - 0x3) + '/'; var i = []; var j = $('#fechasConcatendas')['val']()['split'](','); for (var k = 0x0; k < j['length']; k++) { i['push'](new Date(j[k])); } $('.kendocalendar')['kendoDatePicker']({ 'format': 'dd/MM/yyyy', 'min': e, 'max': f, 'disableDates': i, 'change': function () { if ($('#NumDias0')['val']() != '') { $['getJSON']('/Anotador/CalcularFechaFin', { 'NumeroDias': parseInt($('#NumDias0')['val']()), 'FechaInicio': $('#kendocalendar_0')['val'](), 'DiasFestivosSabadosDomingos': $('#fechasConcatendas')['val']() }, function (l) { switch (l['Codigo']) { case '0': var j = l['Resultado']['Data']['split']('/'); $('#kendocalendar_1')['val'](new Date(parseInt(j[0x2]), parseInt(j[0x1] - 0x1), parseInt(j[0x0]))['toJSON']()['slice'](0x0, 0xa)['split']('-')['reverse']()['join']('/')); break; case '-1': $('#kendocalendar_1')['val'](''); swal['fire']({ 'text': respuesta['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } } }); } function crearGrid() { $('#rowSelection')['kendoGrid']({ 'dataSource': { 'data': '', 'pageSize': 0xa }, 'selectable': ![], 'resizable': !![], 'pageable': { 'buttonCount': 0x5 }, 'scrollable': !![], 'columns': [{ 'field': 'nmroDcmnto', 'title': 'Documento', 'width': 0x64, 'media': '(min-width:\x2050px)', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' } }, { 'field': 'nmbre_cmplto', 'title': 'Nombre\x20completo', 'width': 0x64, 'media': '(min-width:\x2050px)', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' } }, { 'field': 'nmro_ds', 'title': 'Nro.\x20de\x20días\x20a\x20disfrutar', 'width': 0x64, 'media': '(min-width:\x2050px)', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' } }, { 'field': 'fcha_inco_vccns', 'title': 'Inicio\x20de\x20vacaciones', 'width': 0x64, 'media': '(min-width:\x2050px)', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' }, 'template': '#=\x20kendo.toString(kendo.parseDate(fcha_inco_vccns),\x20\x27dd/MM/yyyy\x27)\x20#' }, { 'field': 'fcha_fn_vcc', 'title': 'Fin\x20de\x20vacaciones', 'width': 0x64, 'media': '(min-width:\x2050px)', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' }, 'template': '#=\x20kendo.toString(kendo.parseDate(fcha_fn_vcc),\x20\x27dd/MM/yyyy\x27)\x20#' }, { 'field': 'nmbrs_slctnte', 'title': 'Nombres', 'width': 0x64, 'media': '(min-width:\x20100px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'apllds_slctnte', 'title': 'Apellidos', 'width': 0x64, 'media': '(min-width:\x20100px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'sbdo_hbl', 'title': 'Sabado\x20Habil', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'fcha_hra_aprvc', 'title': 'Fecha\x20Aprobacion', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'fcha_hra_rgstro_nvdd', 'title': 'Fecha\x20Registro', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'crreo_slctnte', 'title': 'Correo\x20Solicitante', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'crreo_jfe_slctnte', 'title': 'Correo\x20Jefe\x20Solicitante', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'codEmpldo', 'title': 'Codigo\x20Empleado', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'idEstdoSlctd', 'title': 'Estado\x20Solicitud', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'scdd', 'title': 'Sociedad', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'nmro_ds_dspnbls', 'title': 'Numero\x20de\x20dias\x20disponibles', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'MinimoDias', 'title': 'Minimo\x20dias\x20motor\x20de\x20reglas', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'InicioFecha', 'title': 'Fecha\x20de\x20inicio\x20para\x20el\x20calendario\x20de\x20la\x20modal', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'FinFecha', 'title': 'Fecha\x20de\x20Fin\x20para\x20el\x20calendario\x20de\x20la\x20modal', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'DiasFestivosSabadosDomingos', 'title': 'Cadena\x20de\x20string\x20con\x20los\x20dias\x20festivos,\x20sabados\x20y\x20domingos', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'NroMinDiasCorreoCompensacion', 'title': 'Minimo\x20de\x20dias\x20donde\x20se\x20debe\x20enviar\x20el\x20correo\x20de\x20compensacion\x20y\x20nomina', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'field': 'CorreoCompensacion', 'title': 'Lista\x20de\x20correos\x20de\x20compensacion\x20y\x20nomina', 'width': 0x64, 'media': '(min-width:\x2050px)', 'hidden': !![], 'headerAttributes': { 'style': 'text-align:\x20center' } }, { 'title': 'Acciones', 'headerAttributes': { 'style': 'text-align:\x20center;\x20font-size:\x2012px;\x20font-weight:\x20bold;' }, 'columns': [{ 'width': 0x50, 'command': [{ 'name': 'Edit', 'text': 'Editar', 'click': function (n) { var o = this['dataItem']($(n['currentTarget'])['closest']('tr')); $('#ModalOtrosColaboradores')['modal']({ 'backdrop': 'static' }); $('#txtCedulaOtros')['val'](o['nmroDcmnto']); $('#SpanNombreEmpleado1')['html'](o['nmbrs_slctnte']); $('#SpanApellidoEmpleado1')['html'](o['apllds_slctnte']); $('#SpanNumDias1')['html'](o['nmro_ds_dspnbls']); $('#NumDias1')['val'](o['nmro_ds']); $('#MinimoDiasModal')['val'](o['MinimoDias']); $('#SabadoHabilModal')['val'](o['sbdo_hbl']); $('#DiasFestivosSabadosDomingosModal')['val'](o['DiasFestivosSabadosDomingos']); $('#btnAgregar2')['hide'](); $('#btnAgregarModal')['hide'](); $('#btnEditarModal')['show'](); var p = o['DiasFestivosSabadosDomingos']; var q = o['InicioFecha']; var r = o['FinFecha']; var s = []; var t = p['split'](','); for (var u = 0x0; u < t['length']; u++) { s['push'](new Date(t[u])); } var v = $('#datepicker')['data']('kendoDatePicker'); kendo['destroy'](v); $('#datepickerDiv')['empty'](); $('#datepickerDiv')['append']('<input\x20id=\x22datepicker\x22\x20style=\x22width:inherit\x22\x20/>'); $('#datepicker')['kendoDatePicker']({ 'format': 'dd/MM/yyyy', 'min': q, 'max': r, 'disableDates': s, 'change': function () { if ($('#NumDias1')['val']() != '') { $['getJSON']('/Anotador/CalcularFechaFin', { 'NumeroDias': parseInt($('#NumDias1')['val']()), 'FechaInicio': $('#datepicker')['val'](), 'DiasFestivosSabadosDomingos': $('#DiasFestivosSabadosDomingosModal')['val']() }, function (w) { switch (w['Codigo']) { case '0': var x = w['Resultado']['Data']['split']('/'); $('#kendocalendar_2')['val'](new Date(parseInt(x[0x2]), parseInt(x[0x1] - 0x1), parseInt(x[0x0]))['toJSON']()['slice'](0x0, 0xa)['split']('-')['reverse']()['join']('/')); break; case '-1': swal['fire']({ 'text': w['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] })['then'](y => { if (y['value']) { } }); break; } }); } else { $('#kendocalendar_2')['val'](''); } } }); $('#datepicker')['attr']('readonly', !![]); $('#datepicker')['attr']('placeholder', 'DD/MM/AAAA'); $('#datepicker')['val'](kendo['toString'](kendo['parseDate'](o['fcha_inco_vccns']), 'dd/MM/yyyy')); $('#kendocalendar_2')['val'](kendo['toString'](kendo['parseDate'](o['fcha_fn_vcc']), 'dd/MM/yyyy')); } }] }, { 'width': 0x5a, 'command': [{ 'name': 'Delete', 'text': 'Eliminar', 'click': function (z) { swal['fire']({ 'text': '¿Está\x20seguro\x20que\x20desea\x20eliminar\x20este\x20registro?', 'type': 'warning', 'confirmButtonText': 'Eliminar', 'confirmButtonColor': '#00AFF0', 'showCancelButton': !![], 'cancelButtonText': 'Cancelar', 'cancelButtonColor': '#5a6268', 'allowOutsideClick': ![], 'allowEscapeKey': ![] })['then'](A => { if (A['value']) { var B = this['dataItem']($(z['currentTarget'])['closest']('tr')); var C = $('#rowSelection')['data']('kendoGrid'); $(C['dataSource']['data']())['each'](function (D) { if (this['nmroDcmnto'] == B['nmroDcmnto']) { rowIndex = D; row = C['tbody']['find']('>tr:not(.k-grouping-row)')['eq'](rowIndex); C['removeRow'](row); swal['fire']({ 'text': 'Registro\x20eliminado\x20correctamente', 'type': 'success', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } }); } }); } }] }] }] }); }; function validarNumDias() { $('#NumDias0')['focusout'](function () { if ($('#NumDias0')['val']() != '') { $['getJSON']('/Anotador/ValidarCantidadDias', { 'NumeroDias': parseInt($('#NumDias0')['val']()), 'NumDiasDisponibles': parseFloat($('#SpanNumDias0')['html']()), 'MinimoDias': parseInt($('#MinimoDias')['val']()) }, function (E) { switch (E['Codigo']) { case '1': $('#kendocalendar_1')['val'](''); $('#NumDias0')['val'](''); $('#NumDias0')['focus'](); swal['fire']({ 'text': E['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '2': $('#kendocalendar_1')['val'](''); $('#NumDias0')['val'](''); $('#NumDias0')['focus'](); swal['fire']({ 'text': E['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '3': $('#kendocalendar_1')['val'](''); $('#NumDias0')['val'](''); $('#NumDias0')['focus'](); swal['fire']({ 'text': E['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { $('#kendocalendar_1')['val'](''); } }); } function calcularFechaFin() { $('#NumDias0')['change'](function () { if ($('#NumDias0')['val']() != '' && $('#kendocalendar_0')['val']() != '') { $['getJSON']('/Anotador/CalcularFechaFin', { 'NumeroDias': parseInt($('#NumDias0')['val']()), 'FechaInicio': $('#kendocalendar_0')['val'](), 'DiasFestivosSabadosDomingos': $('#fechasConcatendas')['val']() }, function (F) { switch (F['Codigo']) { case '0': var G = F['Resultado']['Data']['split']('/'); $('#kendocalendar_1')['val'](new Date(parseInt(G[0x2]), parseInt(G[0x1] - 0x1), parseInt(G[0x0]))['toJSON']()['slice'](0x0, 0xa)['split']('-')['reverse']()['join']('/')); break; case '-1': swal['fire']({ 'text': F['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] })['then'](H => { if (H['value']) { $('#kendocalendar_1')['val'](''); } }); break; } }); } }); } function agregarItemLista() { $('#btnAgregar0')['click'](function () { if ($('#NroIdentificacionHidden')['val']() != '' && $('#NumDias0')['val']() != '' && $('#kendocalendar_0')['val']() != '') { $('#DivFondoTrasparente')['show'](); $('#DivCirculoAzul')['show'](); $['post']('../Anotador/ConsultaFechasSolicitudExistentes', { 'oIdentificacion': $('#NroIdentificacionHidden')['val'](), 'FechaInicio': $('#kendocalendar_0')['val'](), 'FechaFin': $('#kendocalendar_1')['val']() }, function (I) { $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); switch (I['Codigo']) { case '-1': swal['fire']({ 'text': I['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-3': swal['fire']({ 'text': I['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '1': $('#DivFondoTrasparente')['show'](); $('#DivCirculoAzul')['show'](); if ($('#TieneVacacione')['val']() == 'SI') { $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': 'Usted\x20ya\x20cuenta\x20con\x20una\x20solicitud\x20de\x20vacaciones\x20pendiente\x20de\x20aprobación', 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } else { $['post']('../Anotador/AgregarOEditarEmpleado', { 'NroIdentificacion': $('#NroIdentificacionHidden')['val'](), 'NombresEmpleado': $('#NombresEmpleado')['val'](), 'ApellidosEmpleado': $('#ApellidosEmpleado')['val'](), 'NumeroDias': $('#NumDias0')['val'](), 'NumeroDiasDisponibles': $('#SpanNumDias0')['html'](), 'EsEdit': ![], 'EsModal': ![], 'FechaInicio': $('#kendocalendar_0')['val'](), 'FechaFin': $('#kendocalendar_1')['val'](), 'DataActual': JSON['stringify']($('#rowSelection')['data']('kendoGrid')['dataSource']['data']()), 'oRespuestaSAP': $('#oRespuestaSAPModels')['val'](), 'DiasFestivosSabadosDomingos': $('#fechasConcatendas')['val'](), 'oRespuestaMotor': $('#oRespuestaMotor')['val'](), 'oMinimoDiasCorreoCompensacion': $('#NroMinDiasCorreoCompensacion')['val'](), 'oCorreoCompensacion': $('#CorreoCompensacion')['val']() }, function (J) { switch (J['Codigo']) { case '1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](J['Resultado']['Data']); $('#NumDias0')['val'](''); $('#kendocalendar_0')['val'](''); $('#kendocalendar_1')['val'](''); $('#ModalOtrosColaboradores')['modal']('hide'); swal['fire']({ 'text': J['Mensaje'], 'type': 'success', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '2': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](J['Resultado']['Data']); swal['fire']({ 'text': J['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](J['Resultado']['Data']); swal['fire']({ 'text': J['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': J['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } break; case '2': swal['fire']({ 'text': I['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { swal['fire']({ 'text': 'Debe\x20ingresar\x20el\x20número\x20de\x20días\x20y\x20la\x20fecha\x20de\x20inicio\x20para\x20la\x20solicitud\x20de\x20vacaciones', 'type': 'warning', 'confirmButtonText': 'Ok', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); }; }); } function validarNumDiasModal() { $('#NumDias1')['focusout'](function () { if ($('#NumDias1')['val']() != '') { $['getJSON']('/Anotador/ValidarCantidadDias', { 'NumeroDias': parseInt($('#NumDias1')['val']()), 'NumDiasDisponibles': parseFloat($('#SpanNumDias1')['html']()), 'MinimoDias': $('#MinimoDiasModal')['val']() }, function (K) { switch (K['Codigo']) { case '1': $('#kendocalendar_2')['val'](''); $('#NumDias1')['val'](''); $('#NumDias1')['focus'](); swal['fire']({ 'text': K['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '2': $('#kendocalendar_2')['val'](''); $('#NumDias1')['val'](''); $('#NumDias1')['focus'](); swal['fire']({ 'text': K['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '3': $('#kendocalendar_2')['val'](''); $('#NumDias1')['val'](''); $('#NumDias1')['focus'](); swal['fire']({ 'text': K['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { $('#kendocalendar_2')['val'](''); } }); } function calcularFechaFinModal() { $('#NumDias1')['change'](function () { if ($('#NumDias1')['val']() != '' && $('#datepicker')['val']() != '') { $['getJSON']('/Anotador/CalcularFechaFin', { 'NumeroDias': parseInt($('#NumDias1')['val']()), 'FechaInicio': $('#datepicker')['val'](), 'DiasFestivosSabadosDomingos': $('#DiasFestivosSabadosDomingosModal')['val']() }, function (L) { switch (L['Codigo']) { case '0': var M = L['Resultado']['Data']['split']('/'); $('#kendocalendar_2')['val'](new Date(parseInt(M[0x2]), parseInt(M[0x1] - 0x1), parseInt(M[0x0]))['toJSON']()['slice'](0x0, 0xa)['split']('-')['reverse']()['join']('/')); break; case '-1': $('#kendocalendar_2')['val'](''); swal['fire']({ 'text': L['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { $('#kendocalendar_2')['val'](''); } }); } function buscarEmpleado() { $('#btnOtrosColaboradoresModal')['click'](function () { if ($('#txtCedulaOtros')['val']() != '') { var N = ![]; $($('#rowSelection')['data']('kendoGrid')['dataSource']['data']())['each'](function (O) { if (this['nmroDcmnto'] == $('#txtCedulaOtros')['val']()) { N = !![]; } }); if (!N) { $('#DivFondoTrasparente')['show'](); $('#DivCirculoAzul')['show'](); $['getJSON']('/Anotador/ConsultarUserSAP', { 'NroDocumento': parseInt($('#txtCedulaOtros')['val']()) }, function (P) { switch (P['Codigo']) { case '6': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': 'No\x20se\x20ha\x20identificado\x20la\x20información\x20del\x20jefe\x20inmediato\x20del\x20trabajador.\x20Comuníquese\x20con\x20Gestión\x20Humana', 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '5': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': 'El\x20trabajador\x20no\x20cuenta\x20con\x20días\x20disponibles\x20para\x20disfrute\x20de\x20vacaciones', 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '4': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $['post']('../Anotador/ConsultaMotorDeReglas', { 'RespuestaSAP': JSON['stringify'](P['Resultado']['Data']) }, function (Q) { $('#DivFondoTrasparente')['show'](); $('#DivCirculoAzul')['show'](); switch (Q['Codigo']) { case '1': $['post']('../Anotador/ArmarObjetoPantallaModal', { 'RespuestaMotor': JSON['stringify'](Q['Resultado']['Data']), 'RespuestaSAP': JSON['stringify'](P['Resultado']['Data']) }, function (R) { switch (R['Codigo']) { case '1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $('#ModalOtrosColaboradores')['modal']({ 'backdrop': 'static' }); $('#btnAgregarModal')['show'](); $('#btnCancelarModal')['show'](); $('#btnEditarModal')['hide'](); $('#btnAgregar2')['hide'](); $('#MinimoDiasModal')['val'](R['Resultado']['Data']['MinimoDias']); $('#InicioFechaModal')['val'](R['Resultado']['Data']['InicioFecha']); $('#FinFechaModal')['val'](R['Resultado']['Data']['FinFecha']); $('#SabadoHabilModal')['val'](R['Resultado']['Data']['SabadoHabil']); $('#DiasFestivosSabadosDomingosModal')['val'](R['Resultado']['Data']['DiasFestivosSabadosDomingos']); $('#CorreoSolicitanteModal')['val'](R['Resultado']['Data']['CorreoSolicitante']); $('#CorreoJefeSolicitanteModal')['val'](R['Resultado']['Data']['CorreoJefeSolicitante']); $('#CodigoEmpleadoModal')['val'](R['Resultado']['Data']['CodigoEmpleado']); $('#SociedadModal')['val'](R['Resultado']['Data']['Sociedad']); $('#MinimoDiasCorreoCompensacionModal')['val'](R['Resultado']['Data']['NroMinDiasCorreoCompensacion']); $('#CorreoCompensacionModal')['val'](R['Resultado']['Data']['CorreoCompensacion']); $('#SpanNumDias1')['html'](R['Resultado']['Data']['NroDias']); $('#SpanNombreEmpleado1')['html'](R['Resultado']['Data']['NombreEmpleado']); $('#SpanApellidoEmpleado1')['html'](R['Resultado']['Data']['ApellidoEmpleado']); var S = $('#DiasFestivosSabadosDomingosModal')['val'](); var T = $('#InicioFechaModal')['val'](); var U = $('#FinFechaModal')['val'](); var V = []; var W = S['split'](','); for (var X = 0x0; X < W['length']; X++) { V['push'](new Date(W[X])); } var Y = $('#datepicker')['data']('kendoDatePicker'); kendo['destroy'](Y); $('#datepickerDiv')['empty'](); $('#datepickerDiv')['append']('<input\x20id=\x22datepicker\x22\x20style=\x22width:inherit\x22\x20/>'); $('#datepicker')['kendoDatePicker']({ 'format': 'dd/MM/yyyy', 'min': T, 'max': U, 'disableDates': V, 'change': function () { if ($('#NumDias1')['val']() != '') { $['getJSON']('/Anotador/CalcularFechaFin', { 'NumeroDias': parseInt($('#NumDias1')['val']()), 'FechaInicio': $('#datepicker')['val'](), 'DiasFestivosSabadosDomingos': $('#DiasFestivosSabadosDomingosModal')['val']() }, function (Z) { switch (Z['Codigo']) { case '0': var a0 = Z['Resultado']['Data']['split']('/'); $('#kendocalendar_2')['val'](new Date(parseInt(a0[0x2]), parseInt(a0[0x1] - 0x1), parseInt(a0[0x0]))['toJSON']()['slice'](0x0, 0xa)['split']('-')['reverse']()['join']('/')); break; case '-1': swal['fire']({ 'text': Z['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] })['then'](a1 => { if (a1['value']) { $('#kendocalendar_2')['val'](''); } }); break; } }); } } }); $('#datepicker')['attr']('readonly', !![]); $('#datepicker')['attr']('placeholder', 'DD/MM/AAAA'); break; case '-3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': R['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); break; case '-1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': Q['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-2': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': Q['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': Q['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); break; case '3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '2': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-2': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-4': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-5': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': P['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { swal['fire']({ 'text': 'El\x20empleado\x20ya\x20se\x20encuentra\x20agregado\x20en\x20la\x20lista.\x20Verifique\x20la\x20información\x20e\x20inténtelo\x20de\x20nuevo', 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } } else { swal['fire']({ 'text': 'Debe\x20ingresar\x20un\x20Nro.\x20de\x20identificación', 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } }); } function guardarPantallaPrincipal(a2) { $('#btnAgregar1')['click'](function () { if ($($('#rowSelection')['data']('kendoGrid')['dataSource']['data']())['length'] > 0x0) { $('#DivFondoTrasparente')['show'](); $('#DivCirculoAzul')['show'](); $['post']('../Anotador/GuardarSolicitud', { 'NroIdentificacionAnotador': $('#NroIdentificacionHidden')['val'](), 'NombresEmpleadoAnotador': $('#SpanNombreEmpleado0')['html'](), 'ApellidosEmpleadoAnotador': $('#SpanApellidoEmpleado0')['html'](), 'oRespuestaMotor': $('#oRespuestaMotor')['val'](), 'oDataActual': JSON['stringify']($('#rowSelection')['data']('kendoGrid')['dataSource']['data']()), 'oCorreoAnotador': $('#CorreoAnotador')['val']() }, function (a3) { switch (a3['Codigo']) { case '1': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); $['post']('../Anotador/EnviarNotificacionFlow', { 'oDataActual': JSON['stringify']($('#rowSelection')['data']('kendoGrid')['dataSource']['data']()), 'oIdSolicitud': a3['Resultado']['Data']['Resultado'], 'oRespuestaSAP': $('#oRespuestaSAPModels')['val']() }); swal['fire']({ 'text': a3['Mensaje'], 'type': 'success', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] })['then'](a4 => { if (a4['value']) { var a5 = a2; window['location']['href'] = a5; } }); break; case '-3': $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': a3['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); swal['fire']({ 'text': 'Debe\x20ingresar\x20mínimo\x20un\x20ítem\x20a\x20la\x20lista', 'type': 'warning', 'confirmButtonText': 'Ok', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } }); } function limpiarPantalla() { $('#ModalOtrosColaboradores')['on']('hidden.bs.modal', function () { $('#datepicker')['data']('kendoDatePicker')['value'](null); $('#kendocalendar_2')['data']('kendoDatePicker')['value'](null); $('#NumDias1')['val'](''); $('#txtCedulaOtros')['val'](''); $('#MinimoDiasModal')['val'](''); $('#InicioFechaModal')['val'](''); $('#FinFechaModal')['val'](''); $('#SabadoHabilModal')['val'](''); $('#DiasFestivosSabadosDomingosModal')['val'](''); $('#CorreoSolicitanteModal')['val'](''); $('#CorreoJefeSolicitanteModal')['val'](''); $('#CodigoEmpleadoModal')['val'](''); $('#SociedadModal')['val'](''); $('#MinimoDiasCorreoCompensacionModal')['val'](''); $('#CorreoCompensacionModal')['val'](''); $('#SpanNumDias1')['html'](''); $('#SpanNombreEmpleado1')['html'](''); $('#SpanApellidoEmpleado1')['html'](''); }); } function agregarPantallaModal() { $('#btnAgregarModal')['click'](function () { if ($('#txtCedulaOtros')['val']() != '' && $('#NumDias1')['val']() != '' && $('#datepicker')['val']() != '') { $['post']('../Anotador/ConsultaFechasSolicitudExistentes', { 'oIdentificacion': $('#txtCedulaOtros')['val'](), 'FechaInicio': $('#datepicker')['val'](), 'FechaFin': $('#kendocalendar_2')['val']() }, function (a6) { $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); switch (a6['Codigo']) { case '-1': swal['fire']({ 'text': a6['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-3': swal['fire']({ 'text': a6['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '1': $['post']('../Anotador/AgregarOEditarEmpleado', { 'NroIdentificacion': $('#txtCedulaOtros')['val'](), 'NombresEmpleado': $('#SpanNombreEmpleado1')['html'](), 'ApellidosEmpleado': $('#SpanApellidoEmpleado1')['html'](), 'NumeroDias': $('#NumDias1')['val'](), 'NumeroDiasDisponibles': $('#SpanNumDias1')['html'](), 'EsEdit': ![], 'EsModal': !![], 'FechaInicio': $('#datepicker')['val'](), 'FechaFin': $('#kendocalendar_2')['val'](), 'DataActual': JSON['stringify']($('#rowSelection')['data']('kendoGrid')['dataSource']['data']()), 'SabadoHabil': $('#SabadoHabilModal')['val'](), 'CorreoSolicitante': $('#CorreoSolicitanteModal')['val'](), 'CorreoJefeSolicitante': $('#CorreoJefeSolicitanteModal')['val'](), 'CodigoEmpleado': $('#CodigoEmpleadoModal')['val'](), 'Sociedad': $('#SociedadModal')['val'](), 'MinimoDias': $('#MinimoDiasModal')['val'](), 'InicioFecha': kendo['toString'](kendo['parseDate']($('#InicioFechaModal')['val']()), 'dd/MM/yyyy'), 'FinFecha': kendo['toString'](kendo['parseDate']($('#FinFechaModal')['val']()), 'dd/MM/yyyy'), 'DiasFestivosSabadosDomingos': $('#DiasFestivosSabadosDomingosModal')['val'](), 'oMinimoDiasCorreoCompensacion': $('#MinimoDiasCorreoCompensacionModal')['val'](), 'oCorreoCompensacion': $('#CorreoCompensacionModal')['val']() }, function (a7) { switch (a7['Codigo']) { case '1': $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](a7['Resultado']['Data']); $('#NumDias0')['val'](''); $('#kendocalendar_0')['val'](''); $('#kendocalendar_1')['val'](''); $('#ModalOtrosColaboradores')['modal']('hide'); swal['fire']({ 'text': a7['Mensaje'], 'type': 'success', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '2': $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](a7['Resultado']['Data']); swal['fire']({ 'text': a7['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '3': $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](a7['Resultado']['Data']); swal['fire']({ 'text': a7['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-1': $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](a7['Resultado']['Data']); swal['fire']({ 'text': a7['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); break; case '2': swal['fire']({ 'text': a6['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { swal['fire']({ 'text': 'Debe\x20ingresar\x20el\x20número\x20de\x20días\x20y\x20la\x20fecha\x20de\x20inicio\x20para\x20la\x20solicitud\x20de\x20vacaciones', 'type': 'warning', 'confirmButtonText': 'Ok', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); }; }); } function editarPantallaModal() { $('#btnEditarModal')['click'](function () { if ($('#txtCedulaOtros')['val']() != '' && $('#NumDias1')['val']() != '' && $('#datepicker')['val']() != '') { $['post']('../Anotador/ConsultaFechasSolicitudExistentes', { 'oIdentificacion': $('#txtCedulaOtros')['val'](), 'FechaInicio': $('#datepicker')['val'](), 'FechaFin': $('#kendocalendar_2')['val']() }, function (a8) { $('#DivFondoTrasparente')['hide'](); $('#DivCirculoAzul')['hide'](); switch (a8['Codigo']) { case '-1': swal['fire']({ 'text': a8['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '-3': swal['fire']({ 'text': a8['Mensaje'], 'type': 'error', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; case '1': $['post']('../Anotador/AgregarOEditarEmpleado', { 'NroIdentificacion': $('#txtCedulaOtros')['val'](), 'NombresEmpleado': $('#SpanNombreEmpleado1')['html'](), 'ApellidosEmpleado': $('#SpanApellidoEmpleado1')['html'](), 'NumeroDias': $('#NumDias1')['val'](), 'NumeroDiasDisponibles': $('#SpanNumDias1')['html'](), 'EsEdit': !![], 'EsModal': !![], 'FechaInicio': $('#datepicker')['val'](), 'FechaFin': $('#kendocalendar_2')['val'](), 'DataActual': JSON['stringify']($('#rowSelection')['data']('kendoGrid')['dataSource']['data']()), 'SabadoHabil': $('#SabadoHabilModal')['val'](), 'CorreoSolicitante': $('#CorreoSolicitanteModal')['val'](), 'CorreoJefeSolicitante': $('#CorreoJefeSolicitanteModal')['val'](), 'CodigoEmpleado': $('#CodigoEmpleadoModal')['val'](), 'Sociedad': $('#SociedadModal')['val'](), 'MinimoDias': $('#MinimoDiasModal')['val'](), 'InicioFecha': kendo['toString'](kendo['parseDate']($('#InicioFechaModal')['val']()), 'dd/MM/yyyy'), 'FinFecha': kendo['toString'](kendo['parseDate']($('#FinFechaModal')['val']()), 'dd/MM/yyyy'), 'DiasFestivosSabadosDomingos': $('#DiasFestivosSabadosDomingosModal')['val'](), 'oMinimoDiasCorreoCompensacion': $('#MinimoDiasCorreoCompensacionModal')['val'](), 'oCorreoCompensacion': $('#CorreoCompensacionModal')['val']() }, function (a9) { if (a9['Codigo'] == 0x2) { swal['fire']({ 'text': a9['Mensaje'], 'type': 'warning', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); } else { $('#rowSelection')['data']('kendoGrid')['dataSource']['data'](a9['Resultado']['Data']); swal['fire']({ 'text': a9['Mensaje'], 'type': 'success', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); $('#ModalOtrosColaboradores')['modal']('hide'); } }); break; case '2': swal['fire']({ 'text': a8['Mensaje'], 'type': 'warning', 'confirmButtonText': 'OK', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); break; } }); } else { swal['fire']({ 'text': 'Debe\x20ingresar\x20el\x20número\x20de\x20días\x20y\x20la\x20fecha\x20de\x20inicio\x20para\x20la\x20solicitud\x20de\x20vacaciones', 'type': 'warning', 'confirmButtonText': 'Ok', 'confirmButtonColor': '#00AFF0', 'allowOutsideClick': ![], 'allowEscapeKey': ![] }); }; }); }