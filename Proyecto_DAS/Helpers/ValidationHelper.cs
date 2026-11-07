using System.Text.RegularExpressions;

namespace Proyecto_DAS.Helpers
{
    public static class ValidationHelper
    {
        public const int ANIO_CREACION_EMPRESA = 2025;
        public const int MESES_MAXIMOS_CUPON = 3;

        // Nuestra Lista !! Lista completa de caracteres especiales no permitidos en nombres y textos generales
        // ! @ # $ % ^ & * ( ) _ - = + [ ] { } ; : ' " < > , . ? / \ |
        public const string CARACTERES_ESPECIALES = @"!@#$%^&*()_\-=+\[\]{};\:'"",.<>?/\\|";

        public static readonly Regex RegexSoloLetrasYEspacios = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");

        public static readonly Regex RegexLetrasNumerosEspacios = new Regex(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s]+$");

        public static readonly Regex RegexSoloNumeros = new Regex(@"^[0-9]+$");

        public static readonly Regex RegexMayusculasYNumeros = new Regex(@"^[A-Z0-9]+$");

        public static readonly Regex RegexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        // Dirección: inicia con letra/número, permite espacios, comas, puntos, guiones y # (sin caracteres consecutivos)
        public static readonly Regex RegexDireccion = new Regex(@"^(?!.*\.{2})(?!.*#{2})(?!.*-{2})(?!.*,{2})[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ][a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-#]*$");

        public static readonly Regex RegexTelefono = new Regex(@"^9[0-9]{8}$");

        public static readonly Regex RegexContrasenaSegura = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.\-])[A-Za-z\d@$!%*?&.\-]{8,}$");


        // Contraseña segura
        public static bool EsContrasenaSegura(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
                return false;
            contrasena = contrasena.Trim();
            return RegexContrasenaSegura.IsMatch(contrasena);
        }

        // Texto solo con letras y espacios sin caracteres especiales
        public static bool EsSoloLetrasYEspacios(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();
            return RegexSoloLetrasYEspacios.IsMatch(texto);
        }

        // Texto solo con letras, números y espacios
        public static bool EsLetrasNumerosYEspacios(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();
            return RegexLetrasNumerosEspacios.IsMatch(texto);
        }

        // Texto solo con números
        public static bool EsSoloNumeros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();
            return RegexSoloNumeros.IsMatch(texto);
        }

        // Valida DNI solo 8 dígitos
        public static bool EsDniValido(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            dni = dni.Trim();
            return dni.Length == 8 && RegexSoloNumeros.IsMatch(dni);
        }

        // Valida RUC: solo 11 dígitos
        public static bool EsRucValido(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc))
                return false;

            ruc = ruc.Trim();
            return ruc.Length == 11 && RegexSoloNumeros.IsMatch(ruc);
        }

        // Valida código postal entre 4 y 6 dígitos, no puede ser todo ceros
        public static bool EsCodigoPostalValido(string codigoPostal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal))
                return false;

            codigoPostal = codigoPostal.Trim();

            if (codigoPostal.Length < 4 || codigoPostal.Length > 6)
                return false;

            if (!RegexSoloNumeros.IsMatch(codigoPostal))
                return false;

            if (codigoPostal.All(c => c == '0'))
                return false;

            return true;
        }

        // Valida cel 9 dígitos empezando con 9
        public static bool EsTelefonoValido(string celular)
        {
            if (string.IsNullOrWhiteSpace(celular))
                return false;

            celular = celular.Trim();
            return RegexTelefono.IsMatch(celular);
        }

        // Valida código de cupón 6-12 caracteres, solo mayúsculas y números
        public static bool EsCodigoCuponValido(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            codigo = codigo.Trim().ToUpper();

            if (codigo.Length < 6 || codigo.Length > 12)
                return false;

            return RegexMayusculasYNumeros.IsMatch(codigo);
        }

        // Valida que un número decimal sea positivo (mayor a 0)
        public static bool EsNumeroPositivo(decimal numero)
        {
            return numero > 0;
        }

        // Valida que un número entero sea positivo o cero (mayor o igual a 0)
        public static bool EsNumeroNoNegativo(int numero)
        {
            return numero >= 0;
        }

        // Valida que una fecha no sea menor al año de creación de la empresa
        public static bool EsFechaValidaDesdeCreacionEmpresa(DateTime fecha)
        {
            return fecha.Year >= ANIO_CREACION_EMPRESA;
        }

        // Valida que la fecha de vencimiento de un cupón no exceda los 3 meses desde hoy
        public static bool EsFechaVencimientoCuponValida(DateTime fechaVencimiento)
        {
            var fechaMaxima = DateTime.Now.AddMonths(MESES_MAXIMOS_CUPON);
            return fechaVencimiento <= fechaMaxima;
        }

        // Valida porcentaje de descuento: entre 1% y 95%
        public static bool EsPorcentajeDescuentoValido(decimal porcentaje)
        {
            return porcentaje >= 1 && porcentaje <= 95;
        }

        // Limpia espacios múltiples y caracteres especiales de un texto y múltiples espacios consecutivos en uno solo y con trim
        public static string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto.Trim();

            texto = Regex.Replace(texto, @"\s+", " ");

            return texto;
        }

        // Valida que el precio de venta sea mayor que el precio de compra
        public static bool PrecioVentaMayorQueCompra(decimal precioCompra, decimal precioVenta)
        {
            return precioVenta > precioCompra;
        }

        // Valida email con formato básico
        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();
            return RegexEmail.IsMatch(email);
        }

        // Valida dirección: debe iniciar con letra/número, no permite puntos consecutivos
        public static bool EsDireccionValida(string direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion))
                return false;

            direccion = direccion.Trim();
            return RegexDireccion.IsMatch(direccion);
        }

        // Valida que un texto tenga una longitud mínima (después de limpiar espacios)
        public static bool TieneLongitudMinima(string texto, int longitudMinima)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = LimpiarTexto(texto);
            return texto.Length >= longitudMinima;
        }

        // Valida que un texto no contenga caracteres especiales de la lista definida
        public static bool NoContieneCaracteresEspeciales(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return true;

            foreach (char c in CARACTERES_ESPECIALES)
            {
                if (texto.Contains(c))
                    return false;
            }

            return true;
        }

        // Mensaje de error para caracteres especiales
        public static string MensajeCaracteresEspecialesNoPermitidos(string nombreCampo)
        {
            return $"El campo '{nombreCampo}' no puede contener caracteres especiales como: {CARACTERES_ESPECIALES}";
        }

        // Valida que una descripción contenga al menos una letra (no puede ser solo números)
        public static bool EsDescripcionValida(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return false;

            descripcion = descripcion.Trim();

            return Regex.IsMatch(descripcion, @"[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]");
        }

        // Valida que un texto contenga al menos una letra
        public static bool ContieneAlMenosUnaLetra(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            return Regex.IsMatch(texto.Trim(), @"[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]");
        }
    }
}
