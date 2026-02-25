using Helper.Log;
using Microsoft.AspNetCore.Mvc;
using TrovaLibroLib.Dto;
using TrovaLibroLib.Factory;

namespace ApiTrovaLibro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvinceDetailController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILog _log;

        public ProvinceDetailController(ProvinceDetailFactory repository, ILog log)
        {
            _repository = repository;
            _log = log;
        }

        /**
         * @api {get} /api/provincedetail Recupera tutte le province
         * @apiName GetAllProvinces
         * @apiGroup Province
         * @apiVersion 1.0.0
         * @apiDescription Restituisce l'elenco completo delle province con le relative città.
         *
         * @apiSuccess {Object[]} results Lista di province.
         * @apiSuccess {Number} results.id Identificativo della provincia.
         * @apiSuccess {String} results.provinceName Nome della provincia.
         * @apiSuccess {String} results.provinceCode Codice sigla della provincia.
         * @apiSuccess {Object[]} results.cities Lista delle città associate.
         */
        [HttpGet]
        public ActionResult<List<ProvinceDetailDto>> GetAll()
        {
            try
            {
                var results = _repository.GetAll<ProvinceDetailDto>();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero delle province: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/provincedetail/:id Recupera provincia per ID
         * @apiName GetProvinceById
         * @apiGroup Province
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id Identificativo univoco della provincia.
         *
         * @apiSuccess {Number} id ID provincia.
         * @apiSuccess {String} provinceName Nome provincia.
         * @apiSuccess {String} provinceCode Codice provincia.
         * @apiSuccess {Object[]} cities Lista città incluse.
         *
         * @apiError (404) NotFound La provincia con l'ID specificato non esiste.
         */
        [HttpGet("{id}")]
        public ActionResult<ProvinceDetailDto> GetById(long id)
        {
            try
            {
                var result = _repository.GetById<ProvinceDetailDto>(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero della provincia {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {post} /api/provincedetail Crea nuova provincia
         * @apiName CreateProvince
         * @apiGroup Province
         * @apiVersion 1.0.0
         *
         * @apiBody {String} provinceName Nome della provincia.
         * @apiBody {String} provinceCode Codice (es. RM, MI).
         *
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 201 Created
         * {
         * "id": 101,
         * "provinceName": "Roma",
         * "provinceCode": "RM",
         * "cities": []
         * }
         */
        [HttpPost]
        public ActionResult<ProvinceDetailDto> Create([FromBody] ProvinceDetailDto dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var created = _repository.Create<ProvinceDetailDto>(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la creazione della provincia: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {put} /api/provincedetail/:id Aggiorna provincia
         * @apiName UpdateProvince
         * @apiGroup Province
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id ID della provincia nell'URL.
         * @apiBody {Number} id ID nel body.
         * @apiBody {String} provinceName Nuovo nome.
         */
        [HttpPut("{id}")]
        public ActionResult<ProvinceDetailDto> Update(long id, [FromBody] ProvinceDetailDto dto)
        {
            try
            {
                if (dto == null || id != dto.Id) return BadRequest();
                try
                {
                    var updated = _repository.Update<ProvinceDetailDto>(dto);
                    return Ok(updated);
                }
                catch (Exception)
                {
                    return NotFound($"Provincia con ID {id} non trovata.");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'aggiornamento della provincia: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {delete} /api/provincedetail/:id Elimina provincia
         * @apiName DeleteProvince
         * @apiGroup Province
         * @apiVersion 1.0.0
         */
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var existing = _repository.GetById<ProvinceDetailDto>(id);
                if (existing == null) return NotFound();

                _repository.Delete<ProvinceDetailDto>(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'eliminazione della provincia {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/provincedetail/search Ricerca per nome o codice
         * @apiName FindProvince
         * @apiGroup Province
         * @apiVersion 1.0.0
         *
         * @apiQuery {String} query Stringa da ricercare nel nome o nel codice.
         */
        [HttpGet("search")]
        public ActionResult<List<ProvinceDetailDto>> Find([FromQuery] string query)
        {
            try
            {
                var results = _repository.Find<ProvinceDetailDto>(p =>
                    p.ProvinceName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.ProvinceCode.Contains(query, StringComparison.OrdinalIgnoreCase));

                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la ricerca della provincia {query}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }
    }
}