/**
 * Validaciones globales para formularios YUNAN
 *
 * Tipos disponibles:
 * - direccion: letras, números, espacios, comas, puntos, guiones, #
 * - descripcion: letras, números, espacios, comas, puntos, guiones
 * - soloLetras: solo letras y espacios
 * - soloNumeros: solo números
 * - letrasNumerosEspacios: letras, números y espacios
 * - email: formato de correo electrónico
 * - telefono: solo números, máximo 9 dígitos
 * - ruc: solo números, exactamente 11 dígitos
 * - dni: solo números, exactamente 8 dígitos
 * - codigoPostal: solo números, máximo 5 dígitos
 * - sinEspaciosInicio: no permite espacios al inicio
 */

const ValidacionesYUNAN = (function () {
    // Expresiones regulares centralizadas (consistentes con ValidationHelper.cs)
    const regex = {
        direccion: /^(?!.*\.{2})(?!.*#{2})(?!.*-{2})(?!.*,{2})[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ][a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-#]*$/,
        descripcion: /^(?!.*\.{2})(?!.*-{2})(?!.*,{2})[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ][a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-]*$/,
        soloLetras: /^(?!.*\s{2})[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ][a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]*$/,
        soloNumeros: /^[0-9]+$/,
        letrasNumerosEspacios: /^(?!.*\s{2})[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ][a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s]*$/,
        email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
        telefono: /^[0-9]{0,9}$/,
        ruc: /^[0-9]{0,11}$/,
        dni: /^[0-9]{0,8}$/,
        codigoPostal: /^[0-9]{0,6}$/,
        contrasenaSegura: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.-])[A-Za-z\d@$!%*?&.-]{8,}$/,
        codigoCupon: /^[A-Z0-9]+$/
    };

    // Mensajes de error personalizados
    const mensajes = {
        direccion: 'Debe iniciar con letra o número. Se permiten: espacios, comas, puntos (no consecutivos), guiones y #',
        descripcion: 'Debe iniciar con letra o número. No se permiten caracteres especiales consecutivos (.., --, ,,)',
        soloLetras: 'Debe iniciar con letra. No se permiten espacios consecutivos',
        soloNumeros: 'Solo se permiten números',
        letrasNumerosEspacios: 'Debe iniciar con letra o número. No se permiten espacios consecutivos',
        email: 'Formato de correo inválido',
        telefono: 'Solo números, máximo 9 dígitos',
        ruc: 'Solo números, exactamente 11 dígitos',
        dni: 'Solo números, exactamente 8 dígitos',
        codigoPostal: 'Solo números, entre 4 y 6 dígitos',
        sinEspaciosInicio: 'No se permiten espacios al inicio',
        contrasenaSegura: 'Mínimo 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&.-)',
        codigoCupon: 'Solo letras mayúsculas y números'
    };

    /**
     * Valida un valor según el tipo de validación
     * @param {string} valor - Valor a validar
     * @param {string} tipo - Tipo de validación
     * @returns {boolean} - true si es válido
     */
    function validar(valor, tipo) {
        if (!valor) return true;

        if (tipo === 'sinEspaciosInicio') {
            return !valor.startsWith(' ');
        }

        const regexTipo = regex[tipo];
        if (!regexTipo) return true;

        return regexTipo.test(valor);
    }

    /**
     * Limpia una dirección eliminando caracteres no permitidos y consecutivos
     * @param {string} valor - Valor a limpiar
     * @returns {string} - Valor limpio
     */
    function limpiarDireccion(valor) {
        valor = valor.replace(/[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-#]/g, '');
        valor = valor.replace(/\.{2,}/g, '.');
        valor = valor.replace(/#{2,}/g, '#');
        valor = valor.replace(/-{2,}/g, '-');
        valor = valor.replace(/,{2,}/g, ',');
        valor = valor.replace(/^[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ]+/, '');
        return valor;
    }

    /**
     * Limpia una descripción eliminando caracteres no permitidos y consecutivos
     * @param {string} valor - Valor a limpiar
     * @returns {string} - Valor limpio
     */
    function limpiarDescripcion(valor) {
        valor = valor.replace(/[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-]/g, '');
        valor = valor.replace(/\.{2,}/g, '.');
        valor = valor.replace(/-{2,}/g, '-');
        valor = valor.replace(/,{2,}/g, ',');
        valor = valor.replace(/\s{2,}/g, ' ');
        valor = valor.replace(/^[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ]+/, '');
        return valor;
    }

    /**
     * Limpia un nombre (solo letras y espacios) eliminando espacios consecutivos
     * @param {string} valor - Valor a limpiar
     * @returns {string} - Valor limpio
     */
    function limpiarSoloLetras(valor) {
        valor = valor.replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]/g, '');
        valor = valor.replace(/\s{2,}/g, ' ');
        valor = valor.replace(/^[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]+/, '');
        return valor;
    }

    /**
     * Limpia un nombre (letras, números y espacios) eliminando espacios consecutivos
     * @param {string} valor - Valor a limpiar
     * @returns {string} - Valor limpio
     */
    function limpiarLetrasNumerosEspacios(valor) {
        valor = valor.replace(/[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s]/g, '');
        valor = valor.replace(/\s{2,}/g, ' ');
        valor = valor.replace(/^[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ]+/, '');
        return valor;
    }

    /**
     * Aplica validación en tiempo real a un elemento
     * @param {HTMLElement} elemento - Elemento input
     * @param {string} tipo - Tipo de validación
     * @param {function} callback - Función opcional para mostrar mensajes
     */
    function aplicarValidacion(elemento, tipo, callback) {
        if (!elemento) return;

        elemento.addEventListener('input', function () {
            let valor = this.value;

            if (tipo === 'sinEspaciosInicio' || tipo.includes('sinEspaciosInicio')) {
                if (valor.startsWith(' ')) {
                    this.value = valor.trimStart();
                    if (callback) callback(mensajes.sinEspaciosInicio);
                    return;
                }
            }

            const tipoPrincipal = tipo.replace('sinEspaciosInicio', '').replace(',', '').trim();

            // Manejo especial para dirección: limpiar caracteres consecutivos
            if (tipoPrincipal === 'direccion') {
                let valorLimpio = limpiarDireccion(valor);
                if (valor !== valorLimpio) {
                    this.value = valorLimpio;
                    if (callback) callback(mensajes.direccion);
                }
                return;
            }

            // Manejo especial para descripción: limpiar caracteres consecutivos
            if (tipoPrincipal === 'descripcion') {
                let valorLimpio = limpiarDescripcion(valor);
                if (valor !== valorLimpio) {
                    this.value = valorLimpio;
                    if (callback) callback(mensajes.descripcion);
                }
                return;
            }


            if (tipoPrincipal === 'soloLetras') {
                let valorLimpio = limpiarSoloLetras(valor);
                if (valor !== valorLimpio) {
                    this.value = valorLimpio;
                    if (callback) callback(mensajes.soloLetras);
                }
                return;
            }


            if (tipoPrincipal === 'letrasNumerosEspacios') {
                let valorLimpio = limpiarLetrasNumerosEspacios(valor);
                if (valor !== valorLimpio) {
                    this.value = valorLimpio;
                    if (callback) callback(mensajes.letrasNumerosEspacios);
                }
                return;
            }

            if (tipoPrincipal && regex[tipoPrincipal]) {
                if (valor && !regex[tipoPrincipal].test(valor)) {
                    this.value = valor.slice(0, -1);
                    if (callback) callback(mensajes[tipoPrincipal]);
                }
            }
        });

        // Para direcciones y descripciones, también validar al pegar
        if (tipo === 'direccion' || tipo.includes('direccion')) {
            elemento.addEventListener('paste', function (e) {
                var input = this;
                setTimeout(function () {
                    var valor = input.value;
                    var valorLimpio = limpiarDireccion(valor);
                    if (valor !== valorLimpio) {
                        input.value = valorLimpio;
                    }
                }, 10);
            });
        }

        if (tipo === 'descripcion' || tipo.includes('descripcion')) {
            elemento.addEventListener('paste', function (e) {
                var input = this;
                setTimeout(function () {
                    var valor = input.value;
                    var valorLimpio = limpiarDescripcion(valor);
                    if (valor !== valorLimpio) {
                        input.value = valorLimpio;
                    }
                }, 10);
            });
        }

        if (tipo === 'soloLetras' || tipo.includes('soloLetras')) {
            elemento.addEventListener('paste', function (e) {
                var input = this;
                setTimeout(function () {
                    var valor = input.value;
                    var valorLimpio = limpiarSoloLetras(valor);
                    if (valor !== valorLimpio) {
                        input.value = valorLimpio;
                    }
                }, 10);
            });
        }

        if (tipo === 'letrasNumerosEspacios' || tipo.includes('letrasNumerosEspacios')) {
            elemento.addEventListener('paste', function (e) {
                var input = this;
                setTimeout(function () {
                    var valor = input.value;
                    var valorLimpio = limpiarLetrasNumerosEspacios(valor);
                    if (valor !== valorLimpio) {
                        input.value = valorLimpio;
                    }
                }, 10);
            });
        }
    }

    /**
     * Inicializa todas las validaciones en el documento
     * Busca elementos con atributo data-validacion
     */
    function inicializar() {
        const elementos = document.querySelectorAll('[data-validacion]');

        elementos.forEach(function (elemento) {
            const tipo = elemento.getAttribute('data-validacion');
            aplicarValidacion(elemento, tipo);
        });
    }

    /**
     * Aplica validación por ID del elemento
     * @param {string} id - ID del elemento
     * @param {string} tipo - Tipo de validación
     * @param {function} callback - Función opcional para mensajes
     */
    function validarPorId(id, tipo, callback) {
        const elemento = document.getElementById(id);
        aplicarValidacion(elemento, tipo, callback);
    }

    /**
     * Aplica validación por selector CSS
     * @param {string} selector - Selector CSS
     * @param {string} tipo - Tipo de validación
     * @param {function} callback - Función opcional para mensajes
     */
    function validarPorSelector(selector, tipo, callback) {
        const elementos = document.querySelectorAll(selector);
        elementos.forEach(function (elemento) {
            aplicarValidacion(elemento, tipo, callback);
        });
    }

    /**
     * Valida que un campo contenga al menos una letra
     * @param {string} valor - Valor a validar
     * @returns {boolean} - true si contiene letras
     */
    function contieneLetras(valor) {
        return /[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]/.test(valor);
    }

    /**
     * Valida rango de precios (min < max)
     * @param {number} min - Precio mínimo
     * @param {number} max - Precio máximo
     * @returns {boolean} - true si el rango es válido
     */
    function validarRangoPrecios(min, max) {
        if (!min || !max) return true;
        return parseFloat(min) < parseFloat(max);
    }

    /**
     * Valida que un campo contenga al menos una letra (para nombres, etc.)
     * @param {string} inputId - ID del input
     * @param {string} feedbackId - ID del elemento para mostrar feedback (opcional)
     * @param {string} mensajeError - Mensaje de error personalizado (opcional)
     */
    function validarContieneLetras(inputId, feedbackId, mensajeError) {
        const input = document.getElementById(inputId);
        const feedback = feedbackId ? document.getElementById(feedbackId) : null;
        const mensaje = mensajeError || 'Este campo debe contener al menos una letra';

        if (!input) return;

        input.addEventListener('blur', function () {
            const valor = this.value.trim();

            if (valor && !contieneLetras(valor)) {
                if (feedback) {
                    feedback.innerHTML = '<small class="text-danger"><i class="bi bi-exclamation-circle me-1"></i>' + mensaje + '</small>';
                    feedback.style.display = 'block';
                }
                input.classList.add('is-invalid');
            } else {
                if (feedback) {
                    feedback.innerHTML = '';
                    feedback.style.display = 'none';
                }
                input.classList.remove('is-invalid');
            }
        });

        input.addEventListener('input', function () {
            const valor = this.value.trim();
            if (valor && contieneLetras(valor)) {
                if (feedback) {
                    feedback.innerHTML = '';
                    feedback.style.display = 'none';
                }
                input.classList.remove('is-invalid');
            }
        });
    }

    /**
     * Valida que una descripción contenga al menos una letra
     * @param {string} inputId - ID del input de descripción
     * @param {string} feedbackId - ID del elemento para mostrar feedback (opcional)
     */
    function validarDescripcionConLetras(inputId, feedbackId) {
        const input = document.getElementById(inputId);
        const feedback = feedbackId ? document.getElementById(feedbackId) : null;

        if (!input) return;

        input.addEventListener('blur', function () {
            const valor = this.value.trim();

            if (valor && !contieneLetras(valor)) {
                if (feedback) {
                    feedback.innerHTML = '<small class="text-danger"><i class="bi bi-exclamation-circle me-1"></i>La descripción debe contener al menos una letra</small>';
                    feedback.style.display = 'block';
                }
                input.classList.add('is-invalid');
            } else {
                if (feedback) {
                    feedback.innerHTML = '';
                    feedback.style.display = 'none';
                }
                input.classList.remove('is-invalid');
            }
        });

        input.addEventListener('input', function () {
            const valor = this.value.trim();
            if (valor && contieneLetras(valor)) {
                if (feedback) {
                    feedback.innerHTML = '';
                    feedback.style.display = 'none';
                }
                input.classList.remove('is-invalid');
            }
        });
    }

    /**
     * Aplica validación especial para código de cupón (mayúsculas y números)
     * @param {string} inputId - ID del input
     */
    function validarCodigoCupon(inputId) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
        });
    }

    /**
     * Valida contraseña segura y retorna detalles de lo que falta
     * @param {string} contrasena - Contraseña a validar
     * @returns {object} - { valida: boolean, errores: string[], cumple: object }
     */
    function validarContrasena(contrasena) {
        const resultado = {
            valida: false,
            errores: [],
            cumple: {
                longitud: false,
                mayuscula: false,
                minuscula: false,
                numero: false,
                especial: false
            }
        };

        if (!contrasena) {
            resultado.errores.push('La contraseña es requerida');
            return resultado;
        }


        resultado.cumple.longitud = contrasena.length >= 8;
        resultado.cumple.mayuscula = /[A-Z]/.test(contrasena);
        resultado.cumple.minuscula = /[a-z]/.test(contrasena);
        resultado.cumple.numero = /\d/.test(contrasena);
        resultado.cumple.especial = /[@$!%*?&.-]/.test(contrasena);


        if (!resultado.cumple.longitud) resultado.errores.push('Mínimo 8 caracteres');
        if (!resultado.cumple.mayuscula) resultado.errores.push('Al menos una mayúscula');
        if (!resultado.cumple.minuscula) resultado.errores.push('Al menos una minúscula');
        if (!resultado.cumple.numero) resultado.errores.push('Al menos un número');
        if (!resultado.cumple.especial) resultado.errores.push('Al menos un carácter especial (@$!%*?&.-)');

        resultado.valida = resultado.errores.length === 0;
        return resultado;
    }

    /**
     * Aplica validación de contraseña con feedback visual a un input
     * @param {string} inputId - ID del input de contraseña
     * @param {string} feedbackId - ID del elemento donde mostrar feedback (opcional)
     */
    function validarContrasenaEnTiempoReal(inputId, feedbackId) {
        const input = document.getElementById(inputId);
        const feedback = feedbackId ? document.getElementById(feedbackId) : null;

        if (!input) return;

        input.addEventListener('input', function () {
            const resultado = validarContrasena(this.value);

            if (feedback) {
                if (this.value === '') {
                    feedback.innerHTML = '';
                    feedback.style.display = 'none';
                } else if (resultado.valida) {
                    feedback.innerHTML = '<span class="text-success"><i class="bi bi-check-circle-fill me-1"></i>Contraseña segura</span>';
                    feedback.style.display = 'block';
                    input.classList.remove('is-invalid');
                    input.classList.add('is-valid');
                } else {
                    let html = '<small class="text-danger">';
                    html += '<i class="bi bi-exclamation-circle me-1"></i>Requisitos faltantes:<ul class="mb-0 ps-3">';
                    resultado.errores.forEach(function (error) {
                        html += '<li>' + error + '</li>';
                    });
                    html += '</ul></small>';
                    feedback.innerHTML = html;
                    feedback.style.display = 'block';
                    input.classList.remove('is-valid');
                    input.classList.add('is-invalid');
                }
            }
        });
    }

    return {
        regex: regex,
        mensajes: mensajes,
        validar: validar,
        aplicarValidacion: aplicarValidacion,
        inicializar: inicializar,
        validarPorId: validarPorId,
        validarPorSelector: validarPorSelector,
        contieneLetras: contieneLetras,
        validarRangoPrecios: validarRangoPrecios,
        validarContrasena: validarContrasena,
        validarContrasenaEnTiempoReal: validarContrasenaEnTiempoReal,
        validarDescripcionConLetras: validarDescripcionConLetras,
        validarContieneLetras: validarContieneLetras,
        validarCodigoCupon: validarCodigoCupon,
        limpiarDireccion: limpiarDireccion,
        limpiarDescripcion: limpiarDescripcion,
        limpiarSoloLetras: limpiarSoloLetras,
        limpiarLetrasNumerosEspacios: limpiarLetrasNumerosEspacios
    };
})();

// Auto-inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function () {
    ValidacionesYUNAN.inicializar();
});
