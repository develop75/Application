using Microsoft.AspNetCore.Mvc;
using Helper.Log;
using TrovaLibroLib.Factory;
using TrovaLibroLib.Dto;
using System;
using Microsoft.AspNetCore.Identity.Data;
using ApiTrovaLibro.Dto;
using ApiTrovaLibro.Models;

namespace ApiTrovaLibro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserFactory _userFactory;
        private readonly ILog _log;

        public UserController(UserFactory userFactory, ILog log)
        {
            _userFactory = userFactory;
            _log = log;
        }

        /**
         * @api {post} /api/User/register Registrazione Utente
         * @apiName RegisterUser
         * @apiGroup User
         * @apiVersion 1.0.0
         *
         * @apiParam {String} name Nome dell'utente.
         * @apiParam {String} surname Cognome dell'utente.
         * @apiParam {String} email Email (univoca).
         * @apiParam {String} password Password scelta.
         * @apiParam {String} confirmPassword Deve coincidere con password.
         * @apiParam {String} fiscalCode Codice Fiscale (univoco).
         * @apiParam {Long} provinceId ID della provincia.
         * @apiParam {Long} cityId ID della città.
         *
         * @apiSuccess {Long} id ID dell'utente creato.
         * @apiSuccess {String} email Email dell'utente.
         * @apiSuccess {String} name Nome dell'utente.
         *
         * @apiError (400) BadRequest Dati non validi o utente già esistente.
         */
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                if (registrationDto == null) return BadRequest("Dati non validi.");

                // Specifichiamo UserDto come tipo di ritorno atteso
                UserDto newUser = _userFactory.Create(registrationDto, AppSettings.KeyCripto);

                if (newUser == null) return BadRequest("Errore durante la creazione del profilo.");

                return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /**
         * @api {post} /api/User/login Login
         * @apiName LoginUser
         * @apiGroup User
         *
         * @apiParam {String} email Email dell'utente.
         * @apiParam {String} password Password dell'utente.
         *
         * @apiSuccess {Object} user Oggetto UserDto del profilo.
         * @apiError (401) Unauthorized Credenziali errate.
         */
        [HttpPost("login")]
        public IActionResult Login([FromBody] Dto.LoginRequest request)
        {
            try
            {
                var userDto = _userFactory.Login(request.Email, request.Password);
                if (userDto == null)
                    return Unauthorized(new { message = "Email o Password non corretti." });

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore login: {ex.Message}");
                return StatusCode(500, "Errore interno.");
            }
        }

        /**
         * @api {post} /api/User/change-password Cambio Password
         * @apiName ChangePassword
         * @apiGroup User
         *
         * @apiParam {Long} userId ID dell'utente.
         * @apiParam {String} oldPassword Password attuale.
         * @apiParam {String} newPassword Nuova password.
         *
         * @apiSuccess {String} message Messaggio di conferma.
         * @apiError (400) BadRequest Vecchia password errata.
         */
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                _userFactory.ChangePassword(request.UserId, request.OldPassword, request.NewPassword, AppSettings.KeyCripto);
                return Ok(new { message = "Password aggiornata con successo." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /**
         * @api {get} /api/User/:id Recupera Profilo
         * @apiName GetUserById
         * @apiGroup User
         *
         * @apiParam {Long} id ID univoco dell'utente.
         *
         * @apiSuccess {Object} user Dati del profilo utente.
         * @apiError (404) NotFound Utente non trovato.
         */
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var user = _userFactory.GetById<UserDto>(id);
            if (user == null) return NotFound("Utente non trovato.");
            return Ok(user);
        }

        /**
         * @api {put} /api/User/update Aggiorna Profilo
         * @apiName UpdateUser
         * @apiGroup User
         *
         * @apiDescription Aggiorna i dati anagrafici. Non aggiorna la password.
         *
         * @apiSuccess {Object} user Profilo aggiornato.
         */
        [HttpPut("update")]
        public IActionResult Update([FromBody] UserDto userDto)
        {
            try
            {
                var updatedUser = _userFactory.Update(userDto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}