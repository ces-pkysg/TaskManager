using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Models;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.TaskItemDTOs;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Utilities.Exceptions;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;


//Este archivo recibe las peticiones de HTTP
namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITaskService _taskService; //ESTABLECE LA CONEXIÓN CON LA INTERFACE
        public TasksController(AppDbContext context, ITaskService taskService) //SE LE PASAN LOS PARAMETROS A APPDBCONTEXT Y SE INTEGRA LA INTERFACE
        {
            _context = context;
            _taskService = taskService;
        }


        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> Get()
        {
            var result = await _taskService.GetAllAsync();
            return Ok(result);

            /*var tasks = await _context.Tasks
                .Select(t => new TaskItemResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsComplete = t.IsComplete
                })
                .ToListAsync();

            return Ok(tasks);*/
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItemResponse>> GetById(int id)
        {
            var result = await _taskService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);

            /*var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            var dto = new TaskItemResponse
            {
                Id = task.Id,
                Title = task.Title,
                IsComplete = task.IsComplete
            };

            return Ok(dto);*/
        }



        [HttpPost]
        public async Task<ActionResult<TaskItemResponse>> Create([FromBody] CreateTaskRequest request)
        {
            //Llamada al service EL CONTROLLER NO MANEJA BASE DE DATOS, SOLO LLAMA AL SERVICE
            var result = await _taskService.CreateAsync(request);

            //Respuesta HTTP
            //Devuelve HTTP 201 Created y Ubicacion del recurso PJ Location: /api/tasks/10
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

            /*if (request == null)
                return BadRequest("Body requerido.");

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title es requerido.");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw new BusinessException("La categoría no existe.", 404);

            var entity = new TaskItem
            {
                Title = request.Title.Trim(),
                IsComplete = request.IsComplete, //se inicializa como negativo (false)
                CategoryId = request.CategoryId,
            };

            //registro
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new TaskItemResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                IsComplete = entity.IsComplete
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);*/
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _taskService.UpdateAsync(id, request);
            
            if (!updated)
                return NotFound();
            return NoContent();
        }


            /*var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            if (request == null) return BadRequest("Body requerido.");
            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title es requerido.");

            task.Title = request.Title.Trim();

            if (request.IsComplete.HasValue) { task.IsComplete = request.IsComplete.Value; }
            
            await _context.SaveChangesAsync();

            return NoContent(); // 204
            }*/
        



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();

            /*var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();*/
        }



        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskSearchResult>>> Search(
            [FromQuery] string? text,
            [FromQuery] bool? completed,
            [FromQuery] int? step,
            [FromQuery] string? orderBy,
            [FromQuery] TaskSearchRequest request
        )
        {
            var result = await _taskService.SearchAsync(text, completed, step, orderBy, request);

            return Ok(result);
            /*var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(t => t.Title.Contains(text));

            if (completed.HasValue)
                query = query.Where(t => t.IsComplete == completed);

            if (step.HasValue)
                query = query.Where(t => t.Step == step);

            query = orderBy switch
            {
                "title" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "date" => query.OrderBy(t => t.CreatedAt),
                "date_desc" => query.OrderByDescending(t => t.CreatedAt),
                "step" => query.OrderBy(t => t.Step),
                "step_desc" => query.OrderByDescending(t => t.Step),
                _ => query.OrderBy(t => t.Id)
            };

            //Paginación
            query = query
                //.OrderBy(t => t.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);


            var results = await query
                .Select(t => new TaskSearchResult
                {
                    Identificador = t.Id,
                    Titulo = t.Title,
                    Completado = t.IsComplete,
                    PasoActual = t.Step,
                    FechaCreacion = t.CreatedAt
                })
                .ToListAsync();

            return Ok(results);*/
        }




        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<TaskSearchResult>>> GetPaged(
         [FromQuery] TaskSearchRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _taskService.GetPagedAsync(request);

            return Ok(result);


            /*var query = _context.Tasks
                .OrderBy(t => t.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            var result = await query.Select(t => new TaskSearchResult
                {
                    Identificador = t.Id,
                    Titulo = t.Title,
                    Completado = t.IsComplete,
                    PasoActual = t.Step,
                    FechaCreacion = t.CreatedAt
                })
                .ToListAsync();

            return Ok(result);*/
        }


        [HttpGet("with-category")]
        public async Task<ActionResult<IEnumerable<TaskWithCategoryDTO>>> GetWithCategory()
        {
            var result = await _taskService.GetWithCategoryAsync();
            return Ok(result);

            /*var result = await _context.Tasks
                .Include(t => t.Category) //FORMA ESTANDAR DE HACER IN JOIN
                .OrderBy(t => t.Id)
                .Select(static t => new TaskWithCategoryDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsComplete,
                    Step = t.Step,
                    CreatedAt = t.CreatedAt,

                    //Antes :Category Id = (intt.Category=errorde exepcion: Debido al Casting null a int)
                    CategoryId = t.CategoryId ?? 0,// si es null, establecer 0 en su lugar (evita la exepcion)
                    CategoryName = t.Category.Name,

                })
                .ToListAsync();

            return Ok(result);*/
        }

        //Busqueda avanzada
        [HttpGet("advanced-search")]
        public async Task<ActionResult<PagedResultDTO<TaskWithCategoryDTO>>> AdvancedSearch(
            [FromQuery] string? text,
            [FromQuery] bool? completed,
            [FromQuery] int? step,
            [FromQuery] int? categoryId,
            [FromQuery] string? categoryName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10            
            )
        {
            //throw new Exception("La categoría no existe.");

            if (page <= 0) return BadRequest("Page debe ser mayor a 0.");
            if (pageSize <= 0 || pageSize > 100) return BadRequest("PageSize debe estar entre 1 y 100.");

            var result = await _taskService.AdvancedSearchAsync(
                text, completed, step, categoryId, categoryName, page, pageSize);

            return Ok(result);


        }


        //importar excel en taskcontroller
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se recibió ningún archivo o está vacío.");

            var tasks = new List<TaskItem>(); //se asigna para task del modelo TaskItem.cs (Tasks tambien se usa par validaciones)

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; //nos aseguramos de ir al inicio

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First(); //tomamos la primera hoja
                    var rows = worksheet.RangeUsed().RowsUsed();

                    bool isHeader = true;

                    foreach (var row in rows)
                    {
                        if (isHeader)
                        {
                            //saltar la fila de cabeceras
                            isHeader = false;
                            continue;
                        }

                        var title = row.Cell(1).GetString();          // Col A
                        var isCompleteCell = row.Cell(2).GetString(); // Col B
                        var stepCell = row.Cell(3).GetString();           // Col C
                        var categoryIdCell = row.Cell(4).GetString(); // Col D
                        var isDeletedCell = row.Cell(5).GetString();      // Col E

                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        bool isComplete = false;
                        bool.TryParse(isCompleteCell, out isComplete);

                        int step = 0;
                        int.TryParse(stepCell, out step);

                        int? categoryId = null;
                        if (int.TryParse(categoryIdCell, out int cid))
                            categoryId = cid;

                        bool isDeleted = false;
                        bool.TryParse(isDeletedCell, out isDeleted);

                        var task = new TaskItem
                        {
                            Title = title.Trim(),
                            IsCompleted = isComplete,
                            Step = step,
                            CategoryId = categoryId,
                            IsDeleted = isDeleted,
                        };

                        tasks.Add(task);
                    }
                }
            }

            /*
            //validaciones
            // Opcional: filtrar duplicados por Name en la misma importación
            tasks = tasks
                .GroupBy(c => c.Title.ToLower())
                .Select(g => g.First())
                .ToList();

            // Opcional: evitar insertar categorías que ya existan en la BD
            var existingTitles = _context.Tasks
                .Select(c => c.Title.ToLower())
                .ToHashSet();

            var newTasks = tasks
                .Where(c => !existingTitles.Contains(c.Title.ToLower()))
                .ToList(); */ 


            //guardar en base de datos
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Se importaron {tasks.Count} tareas."
            });

        }



        
        /*
        //
        [HttpGet("ajax-search")]
        public async Task<IActionResult> AjaxSearch([FromQuery] string? text)
        {
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(t => t.Title.Contains(text));

            var results = await query
                .OrderBy(t => t.Id)
                .Take(50)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.CategoryName,
                    t.IsCompleted,
                    t.Step
                })
                .ToListAsync();

            return Ok(results);
        }   */

    }
}