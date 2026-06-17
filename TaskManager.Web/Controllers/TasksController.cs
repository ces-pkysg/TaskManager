using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;
using TaskManager.Web.Services;
using TaskManager.Web.Utilities.Exceptions;

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskApiClient _client;

        public TasksController(ITaskApiClient client)
        {
            _client = client;
        }


        //Metodo Index
        [HttpGet]
        public async Task <IActionResult>Search(TaskSearchViewModel filters)//Se cambió a Search para que no sea la principal
        {
            if (filters.Page == 0)
                filters.Page = 1;

            if (filters.PageSize == 0)
                filters.PageSize = 5;

            var result = await _client.AdvancedSearchAsync(filters);

            return View("Index",result);
        }

        //public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        //{
            //var result = await _client.GetTasksAsync(page, pageSize);
            //return View(result);
        //}


        //metodo search
        public async Task<IActionResult> Index(TaskSearchViewModel model)//Se cambió a Index para que sea la principal
        {
            // Si es la primera carga de la página
            if (model.Page == 0)
                model.Page = 1;

            model.Result = await _client.SearchTasksAsync(model);

            return View("Index1", model);
        }

        //entra por el get para que se muestre el formulario
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateTaskViewModel());
        }


        //entra por el post para que se muestre el formulario
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos Inválidos",
                    errors = ModelState
                });
            }

            await _client.CreateTaskAsync(model);
            //throw new ApiException("Error al crear la tarea", 500);
            return Ok(new
            {
                success = true,
                message = "Tarea creada correctamente"
            });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _client.GetTaskByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Datos invalidos"
                });

            await _client.UpdateTaskAsync(model);

            return Ok(new
            {
                success = true,
                message = "Tarea actualizada correctamente"
            });

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _client.DeleteTaskAsync(id);
                TempData["Success"] = "La tarea fue eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la tarea: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        //Import excel
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }


        //post excel
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo Excel.";
                return View();
            }

            try
            {
                var resultMessage = await _client.ImportTaskFromExcelAsync(file);
                TempData["Success"] = resultMessage;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al importar el archivo: " + ex.Message;
            }

            return View();
        }

        //details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _client.GetTaskDetailAsync(id);

            if (task == null)
            {
                TempData["Error"] = "La tarea no existe.";
                return RedirectToAction("Index");
            }

            return View(task);
        }


        [HttpGet]
        public async Task<IActionResult> Index2(TaskSearchViewModel filters)
        {
            var result = await _client.AdvancedSearchAsync(filters);
            filters.Result = result;
            return View(filters); // regresamos siempre el modelo completo
        }


        //5/feb
        [HttpGet]
        public IActionResult AjaxDemo()
        {
            return View();
        }


        //Partial View 
        [HttpGet]
        public async Task<IActionResult> LoadTablePartial(TaskSearchViewModel filters)
        {
            if (filters.Page == 0)
                filters.Page = 1;

            if (filters.PageSize == 0)
                filters.PageSize = 5;

            var result = await _client.AdvancedSearchAsync(filters);
            filters.Result = result;

            return PartialView("_TaskTablePartial", filters);//.Items);//regresa result.Items no tiene info de paginacion
        }



        

        [HttpGet]
        public IActionResult CreatePartial()
        {
            var model = new TaskFormViewModel
            {
                Id = 0,              // crear
                Title = string.Empty,
                CategoryId = 0,
                Step = 1,
                IsCompleted = false
            };

            return PartialView("_TaskFormPartial", model);
        }



        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var task = await _client.GetTaskByIdAsync(id);
                    
            if (task == null)
            {
                // puedes decidir qué hacer aquí (redirigir, mensaje, etc.)
                return NotFound();
            }


            // prueba 
            //System.Diagnostics.Debug.WriteLine($"DEBUG: Titulo={task.Title}, Step={task.Step}, Cat={task.CategoryId}");


            var model = new TaskFormViewModel
            {
                Id = task.Id,
                Title = task.Title,
                CategoryId = task.CategoryId,
                Step = task.Step,
                IsCompleted = task.IsCompleted
            };

            return PartialView("_TaskFormPartial", model);
        }



        //duda con este metodo 
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                await _client.DeleteTaskAsync(id);

                return Ok(new
                {
                    success = true, //dice si la peticion estuvo bien 
                    message = "La tarea se eliminó correctamente."
                });
            }
            catch (ApiException ex)
            {
                // Error controlado que viene de la API (404, 400, reglas de negocio…)
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                // Error inesperado (problema de red, bug, etc.)
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al eliminar la tarea."
                });
            }
        }


        //crear un view model
    }
}
