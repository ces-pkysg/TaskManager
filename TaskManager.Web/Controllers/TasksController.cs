using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;
using TaskManager.Web.Services;

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskApiClient _client;

        public TasksController(ITaskApiClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var result = await _client.GetTasksAsync(page, pageSize);
            return View(result);
        }


        //metodo search
        public async Task<IActionResult> Search(TaskSearchViewModel model)
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
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _client.CreateTaskAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _client.GetTaskByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //ya no es necesario el try catch porque el middleware captura la excepcion (en controlador y servicio quitar los trycatch)
            try
            {
                await _client.UpdateTaskAsync(model);
                TempData["Success"] = "La tarea fue actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error: " + ex.Message);
                return View(model);
            }
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
            var result = await _client.AdvancedSearchAsync(filters);
            return PartialView("_TaskTablePartial", result.Items);
        }


        //crear un view model
    }
}
