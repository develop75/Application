using Helper.Log;
using Microsoft.AspNetCore.Mvc;
using TrovaLibroLib.Dto;
using TrovaLibroLib.Factory;

namespace ApiTrovaLibro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILog _log;

        public BooksController(BookFactory repository, ILog log)
        {
            _repository = repository;
            _log = log;
        }

        /**
         * @api {get} /api/books Recupera tutti i libri
         * @apiName GetAllBooks
         * @apiGroup Libri
         * @apiVersion 1.0.0
         * @apiDescription Restituisce l'elenco completo dei libri comprensivo di dettagli su categorie e target.
         *
         * @apiSuccess {Object[]} results Lista di libri (BookDto).
         */
        [HttpGet]
        public ActionResult<List<BookDto>> GetAll()
        {
            try
            {
                var results = _repository.GetAll<BookDto>();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero dei libri: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/books/:id Recupera libro per ID
         * @apiName GetBookById
         * @apiGroup Libri
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id Identificativo univoco del libro.
         *
         * @apiError (404) NotFound Il libro con l'ID specificato non esiste.
         */
        [HttpGet("{id}")]
        public ActionResult<BookDto> GetById(long id)
        {
            try
            {
                var result = _repository.GetById<BookDto>(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante il recupero del libro {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {post} /api/books Crea nuovo libro
         * @apiName CreateBook
         * @apiGroup Libri
         * @apiVersion 1.0.0
         *
         * @apiBody {Object} BookDto Oggetto libro da creare.
         *
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 201 Created
         */
        [HttpPost]
        public ActionResult<BookDto> Create([FromBody] BookDto dto)
        {
            try
            {
                if (dto == null) return BadRequest();

                var created = _repository.Create<BookDto>(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la creazione del libro: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {put} /api/books/:id Aggiorna libro
         * @apiName UpdateBook
         * @apiGroup Libri
         * @apiVersion 1.0.0
         *
         * @apiParam {Number} id ID del libro nell'URL.
         * @apiBody {Object} BookDto Oggetto libro aggiornato.
         */
        [HttpPut("{id}")]
        public ActionResult<BookDto> Update(long id, [FromBody] BookDto dto)
        {
            try
            {
                if (dto == null || id != dto.Id) return BadRequest();

                try
                {
                    var updated = _repository.Update<BookDto>(dto);
                    return Ok(updated);
                }
                catch (Exception)
                {
                    return NotFound($"Libro con ID {id} non trovato.");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'aggiornamento del libro: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {delete} /api/books/:id Elimina libro
         * @apiName DeleteBook
         * @apiGroup Libri
         * @apiVersion 1.0.0
         */
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var existing = _repository.GetById<BookDto>(id);
                if (existing == null) return NotFound();

                _repository.Delete<BookDto>(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante l'eliminazione del libro {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }

        /**
         * @api {get} /api/books/search Ricerca libri per titolo
         * @apiName FindBookByTitle
         * @apiGroup Libri
         * @apiVersion 1.0.0
         *
         * @apiQuery {String} title Stringa da ricercare nel titolo del libro.
         */
        [HttpGet("search")]
        public ActionResult<List<BookDto>> FindByTitle([FromQuery] string title)
        {
            try
            {
                var results = _repository.Find<BookDto>(b =>
                    !string.IsNullOrEmpty(b.Title) &&
                    b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

                return Ok(results);
            }
            catch (Exception ex)
            {
                _log.Error($"Errore durante la ricerca del libro con titolo {title}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { title = "An internal error has occurred", status = 500, detail = ex.Message });
            }
        }
    }
}