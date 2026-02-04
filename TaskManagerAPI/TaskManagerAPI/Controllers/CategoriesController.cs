using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManagerAPI.DTOs.CategoryDTOs;
using TaskManagerAPI.Interfaces.Categories;
using TaskManagerAPI.Services.Category;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;
        public CategoriesController(AppDbContext context, ICategoryService categoryService)
        //public CategoriesController(ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        /*public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get()
        {
            var result = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(result);
        }*/


        //CREATE
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

            // Si usas [ApiController], ModelState se valida automáticamente.
            // Igual puedes explicar que si falla devuelve 400.

            /*var entity = new Category
            {
                Name = request.Name.Trim()
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);*/
        }


        //GET BY ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);

            /*var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return Ok(dto);*/
        }

        //Importacion Excel (al cargar archivos es con metodo Post)
        //migrar a sercivo!!!!!
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)//si es nulo o cero
                return BadRequest("No se recibió ningún archivo o está vacío.");

            var categories = new List<Category>();

            using (var stream = new MemoryStream())//memorystream crea archivo virtual en memoria ram
            {
                await file.CopyToAsync(stream); //copiame el archivo que estoy recibindo a la memoria ram
                stream.Position = 0; // Nos aseguramos de ir al inicio del archivo

                using (var workbook = new XLWorkbook(stream))//con la libreria abre arcivo virtual a la memoria ram xlworkbook
                {
                    var worksheet = workbook.Worksheets.First(); // Tomamos la primera hoja (nos paramos en la primera hoja)
                    var rows = worksheet.RangeUsed().RowsUsed(); //contar casillas hay en uso (entre filas y columnas, rango usado)

                    bool isHeader = true; //saltar la fila de columnas

                    foreach (var row in rows) //se iteran cada una de las filas en uso hasta que acaben
                    {
                        if (isHeader)
                        {
                            // Saltar la fila de cabeceras
                            isHeader = false;
                            continue;
                        }

                        //extrae datos de cada columna en la fila que esta (de izquierda a derecha)
                        var name = row.Cell(1).GetString();      // Columna A (row indica en que fila estoy )
                        var code = row.Cell(2).GetString();      // Columna B
                        var isActiveCell = row.Cell(3).GetString(); // Columna C

                        bool isActive = true;//se crea una variable bool como verdadero (una bandera)
                        if (!string.IsNullOrWhiteSpace(isActiveCell))
                        {
                            // TRUE/FALSE, 1/0, Sí/No... aquí se puede refinar
                            bool.TryParse(isActiveCell, out isActive);
                        }

                        if (string.IsNullOrWhiteSpace(name))
                        {
                            // Se puede decidir saltar o romper
                            continue;
                        }

                        var category = new Category
                        {
                            Name = name.Trim(),
                            Code = code?.Trim(),
                            IsActive = isActive
                        };

                        categories.Add(category);
                    }
                }
            }


            //validaciones
            // Opcional: filtrar duplicados por Name en la misma importación
            categories = categories
                .GroupBy(c => c.Name.ToLower())//agrupa todo en minuscula
                .Select(g => g.First())//
                .ToList();

            // Opcional: evitar insertar categorías que ya existan en la BD
            var existingNames = _context.Categories
                .Select(c => c.Name.ToLower())
                .ToHashSet();

            var newCategories = categories
                .Where(c => !existingNames.Contains(c.Name.ToLower()))
                .ToList();



            // Guardar en base de datos
            _context.Categories.AddRange(newCategories);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Se importaron {categories.Count} categorías."
            });
        }


        //DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent(); // 204
        }



    }
}
