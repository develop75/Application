using Helper.Log;
using Microsoft.AspNetCore.Mvc;
using TrovaLibroLib.Dto;
using TrovaLibroLib.Factory;

namespace ApiTrovaLibro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILog _log;

        public CategoriesController(IRepository repository, ILog log)
        {
            _repository = repository;
            _log = log;
        }

        /**
         * @api {get} /api/categories Recupera tutte le categorie
         * @apiName GetAllCategories
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         * @apiDescription Restituisce l'elenco completo delle categorie.
         *
         * @apiSuccess {Object[]} results Lista di categorie.
         * @apiSuccess {Number} results.id Identificativo della categoria.
         * @apiSuccess {String} results.name Nome della categoria.
         */
        [HttpGet]
        public ActionResult<List<CategoryDto>> GetAll()
        {
            try
            {
                var results = _repository.GetAll<CategoryDto>();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero delle categorie: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/categories/:id Recupera categoria per ID
         * @apiName GetCategoryById
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id Identificativo univoco della categoria.
         *
         * @apiSuccess {Number} id ID categoria.
         * @apiSuccess {String} name Nome categoria.
         *
         * @apiError (404) NotFound La categoria con l'ID specificato non esiste.
         */
        [HttpGet("{id}")]
        public ActionResult<CategoryDto> GetById(long id)
        {
            try
            {
                var result = _repository.GetById<CategoryDto>(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero della categoria {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {post} /api/categories Crea nuova categoria
         * @apiName CreateCategory
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         *
         * @apiBody {String} name Nome della categoria da creare.
         *
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 201 Created
         * {
         * "id": 12,
         * "name": "Nuova Categoria"
         * }
         *
         * @apiError (400) BadRequest Dati inviati non validi o nulli.
         */
        [HttpPost]
        public ActionResult<CategoryDto> Create([FromBody] CategoryDto dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var created = _repository.Create<CategoryDto>(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la creazione della categoria: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {put} /api/categories/:id Aggiorna categoria
         * @apiName UpdateCategory
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id ID della categoria nell'URL.
         * @apiBody {Number} id ID della categoria nel body (deve coincidere con l'URL).
         * @apiBody {String} name Nuovo nome della categoria.
         *
         * @apiError (400) BadRequest ID non corrispondenti o body nullo.
         * @apiError (404) NotFound Categoria non trovata nel database.
         */
        [HttpPut("{id}")]
        public ActionResult<CategoryDto> Update(long id, [FromBody] CategoryDto dto)
        {
            try
            {
                if (dto == null || id != dto.Id) return BadRequest();
                try
                {
                    var updated = _repository.Update<CategoryDto>(dto);
                    return Ok(updated);
                }
                catch (Exception)
                {
                    return NotFound($"Categoria con ID {id} non trovata.");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'aggiornamento della categoria: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {delete} /api/categories/:id Elimina categoria
         * @apiName DeleteCategory
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id ID della categoria da eliminare.
         *
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 204 No Content
         */
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var existing = _repository.GetById<CategoryDto>(id);
                if (existing == null) return NotFound();

                _repository.Delete<CategoryDto>(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'eliminazione della categoria {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/categories/search Ricerca categorie per nome
         * @apiName FindCategoryByName
         * @apiGroup Categorie
         * @apiVersion 1.0.0
         *
         * @apiQuery {String} name Stringa da ricercare nel nome della categoria.
         *
         * @apiSuccess {Object[]} results Lista delle categorie che corrispondono alla ricerca.
         */
        [HttpGet("search")]
        public ActionResult<List<CategoryDto>> FindByName([FromQuery] string name)
        {
            try
            {
                var results = _repository.Find<CategoryDto>(c =>
                    c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la ricerca della categoria {name}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }
    }
}