using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Proyecto_DAS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarBoletaPorEmailAsync(string emailDestino, string nombreCliente, byte[] pdfBytes, int ventaId)
        {
            try
            {
                // Verificar si el envío de emails está habilitado
                var enabled = _configuration.GetValue<bool>("EmailSettings:Enabled");
                if (!enabled)
                {
                    _logger.LogInformation("Envío de emails deshabilitado en configuración");
                    return false;
                }

                var host = _configuration["EmailSettings:Host"];
                var port = _configuration.GetValue<int>("EmailSettings:Port");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];
                var enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl");

                // Validar configuración
                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Configuración de email incompleta");
                    return false;
                }

                // Crear el mensaje
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(nombreCliente, emailDestino));
                message.Subject = $"Tu Boleta de Compra - YUNAN (Pedido #{ventaId})";

                // Crear el cuerpo del mensaje con HTML
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .header {{ background-color: #f1149b; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                                .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
                                .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
                                .highlight {{ color: #f1149b; font-weight: bold; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>¡Gracias por tu compra!</h1>
                                    <p>YUNAN - Brilla Bonito</p>
                                </div>
                                <div class='content'>
                                    <p>Hola <strong>{nombreCliente}</strong>,</p>
                                    <p>Hemos recibido tu pedido <span class='highlight'>#{ventaId}</span> correctamente.</p>
                                    <p>Adjunto encontrarás tu boleta de compra en formato PDF.</p>
                                    <p><strong>Información importante:</strong></p>
                                    <ul>
                                        <li>Puedes descargar tu boleta en cualquier momento desde tu panel de compras</li>
                                        <li>Recibirás actualizaciones sobre el estado de tu envío</li>
                                        <li>Recuerda que no se aceptan devoluciones</li>
                                    </ul>
                                    <p>Si tienes alguna consulta, no dudes en contactarnos.</p>
                                    <p>¡Gracias por confiar en YUNAN!</p>
                                </div>
                                <div class='footer'>
                                    <p>&copy; 2025 YUNAN - Brilla Bonito. Todos los derechos reservados.</p>
                                    <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                                </div>
                            </div>
                        </body>
                        </html>
                    "
                };

                // Adjuntar el PDF
                bodyBuilder.Attachments.Add($"Boleta_YUNAN_{ventaId}.pdf", pdfBytes, new ContentType("application", "pdf"));
                message.Body = bodyBuilder.ToMessageBody();

                // Enviar el email
                using (var client = new SmtpClient())
                {
                    // Configurar timeout de 10 segundos para evitar bloqueos largos
                    client.Timeout = 10000;

                    await client.ConnectAsync(host, port, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email enviado exitosamente a {emailDestino} para venta {ventaId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                return false;
            }
        }
    }
}
