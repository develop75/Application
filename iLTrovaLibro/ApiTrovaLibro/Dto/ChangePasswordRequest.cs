namespace ApiTrovaLibro.Dto
{
    /**
    * @apiDefine ChangePasswordRequest
    * @apiParam {Long} userId ID dell'utente.
    * @apiParam {String} oldPassword Password attuale.
    * @apiParam {String} newPassword Nuova password.
    */
    public class ChangePasswordRequest
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
