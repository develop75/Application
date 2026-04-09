using Microsoft.AspNetCore.Mvc;
using Helper.Log;
using TrovaLibroLib.Factory;
using TrovaLibroLib.Dto;
using System;
using System.Collections.Generic;

namespace ApiTrovaLibro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysController : ControllerBase
    {
        private readonly ApiRestFactory _apiFactory;
        private readonly ILog _log;

        public SysController(ApiRestFactory apiFactory, ILog log)
        {
            _apiFactory = apiFactory;
            _log = log;
        }

        /**
         * @api {get} /api/Sys/api-settings Recupera tutte le configurazioni API
         * @apiName GetAllApiSettings
         * @apiGroup System
         * @apiVersion 1.0.0
         *
         * @apiSuccess {Object[]} apis Lista di ApiRestDto con relativi Header.
         * @apiError (500) InternalError Errore durante il recupero dei dati.
         */
        [HttpGet("api-settings")]
        public IActionResult GetAllApiSettings()
        {
            try
            {
                var settings = _apiFactory.GetAll<ApiRestDto>();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore recupero settings: {ex.Message}");
                return StatusCode(500, "Errore interno del server.");
            }
        }

        /**
         * @api {get} /api/Sys/api-settings/:id Recupera configurazione singola
         * @apiName GetApiSettingById
         * @apiGroup System
         * 
         * @apiParam {Number} id ID univoco della configurazione.
         *
         * @apiSuccess {Object} api Dati della configurazione API.
         * @apiError (404) NotFound Configurazione non trovata.
         */
        [HttpGet("api-settings/{id}")]
        public IActionResult GetApiSettingById(long id)
        {
            var api = _apiFactory.GetById<ApiRestDto>(id);
            if (api == null) return NotFound($"Configurazione con ID {id} non trovata.");
            return Ok(api);
        }

        /**
         * @api {post} /api/Sys/api-settings Salva nuova configurazione
         * @apiName CreateApiSetting
         * @apiGroup System
         * 
         * @apiParam {String} Name Nome identificativo dell'API.
         * @apiParam {String} Url Endpoint base.
         * @apiParam {Object[]} Headers Lista degli header (Key/Value).
         *
         * @apiSuccess {Object} api La configurazione creata con ID generato.
         */
        [HttpPost("api-settings")]
        public IActionResult CreateApiSetting([FromBody] ApiRestDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Dati non validi.");

                var result = _apiFactory.Create(dto);
                return CreatedAtAction(nameof(GetApiSettingById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore creazione API setting: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        /**
         * @api {put} /api/Sys/api-settings Aggiorna configurazione
         * @apiName UpdateApiSetting
         * @apiGroup System
         * 
         * @apiDescription Aggiorna URL, Nome e sincronizza gli Header associati.
         */
        [HttpPut("api-settings")]
        public IActionResult UpdateApiSetting([FromBody] ApiRestDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0) return BadRequest("ID non valido per l'aggiornamento.");

                var result = _apiFactory.Update(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore aggiornamento API setting: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        /**
         * @api {delete} /api/Sys/api-settings/:id Elimina configurazione
         * @apiName DeleteApiSetting
         * @apiGroup System
         * 
         * @apiParam {Number} id ID della configurazione da eliminare.
         */
        [HttpDelete("api-settings/{id}")]
        public IActionResult DeleteApiSetting(long id)
        {
            try
            {
                _apiFactory.Delete<ApiRestDto>(id);
                return Ok(new { message = "Configurazione eliminata con successo." });
            }
            catch (Exception ex)
            {
                _log.Error($"Errore eliminazione API setting: {ex.Message}");
                return BadRequest("Impossibile eliminare la configurazione.");
            }
        }
    }
}