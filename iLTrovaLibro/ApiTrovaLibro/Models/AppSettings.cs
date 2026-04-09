namespace ApiTrovaLibro.Models
{
    public class AppSettings
    {
        /// <summary>
        /// Origini consentite per le richieste CORS, separate da virgola.
        /// </summary>
        public static string AllowedOrigins { get; set; }
        /// <summary>
        /// Numero massimo di record caricabili in una singola richiesta.
        /// </summary>
        public static long MaxRecords { get; set; }
        /// <summary>
        /// Limite di richieste consentite per ora.
        /// </summary>
        public static int PermitLimitForHour { get; set; }
        /// <summary>
        /// Valore personalizzato per l'identificazione.
        /// </summary>
        public static string ExpectedHeaderValue { get; set; }
        /// <summary>
        /// Nome dell'header personalizzato per l'identificazione.
        /// </summary>
        public static string CustomHeaderName { get; set; }
        /// <summary>
        /// Chiave segreta utilizzata per la crittografia o altre operazioni sensibili.
        /// </summary>
        public static string KeyCripto { get; set; }
    }
}
