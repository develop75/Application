namespace ApiTrovaLibro.Dto
{
    /**
    * @apiDefine LoginRequest
    * @apiParam {String} email Email dell'utente.
    * @apiParam {String} password Password dell'utente.
    */
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
