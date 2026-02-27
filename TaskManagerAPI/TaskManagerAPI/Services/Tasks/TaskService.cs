using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.TaskItemDTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Utilities.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;


//Este archivo habla directamente con la base de datos usando Entity Framework.
public class TaskService : ITaskService
{
    //AppDbContext es la conexión a SQL Server.
    private readonly AppDbContext _context;


    //Esto es Dependency Injection.
    //ASP.NET inyecta el DbContext automáticamente.
    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    //Busqueda avanzada
    public async Task<PagedResultDTO<TaskWithCategoryDTO>> AdvancedSearchAsync(
        string? text,
        bool? completed,
        int? step,
        int? categoryId,
        string? categoryName,
        int page,
        int pageSize
    )
    {
        var query = _context.Tasks
            .Include(t => t.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(text))
            query = query.Where(t => t.Title.Contains(text));

        if (completed.HasValue)
            query = query.Where(t => t.IsCompleted == completed);

        if (step.HasValue)
            query = query.Where(t => t.Step == step);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var name = categoryName.Trim();
            query = query.Where(t => t.Category.Name.Contains(name));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskWithCategoryDTO
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                Step = t.Step,
                CreatedAt = t.CreatedAt,
                CategoryId = t.CategoryId ?? 0,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return new PagedResultDTO<TaskWithCategoryDTO>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }


    //With-Category
    public async Task<IEnumerable<TaskWithCategoryDTO>> GetWithCategoryAsync()
    {
        var result = await _context.Tasks
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Id)
            .Select(t => new TaskWithCategoryDTO
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                Step = t.Step,
                CreatedAt = t.CreatedAt,
                CategoryId = t.CategoryId ?? 0,
                CategoryName = t.Category.Name,
            })
            .ToListAsync();

        return result;
    }


    //paged
    public async Task<IEnumerable<TaskSearchResult>> GetPagedAsync(
         TaskSearchRequest request
        )
    {
        var query = _context.Tasks
            .OrderBy(t => t.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var result = await query.Select(t => new TaskSearchResult
        {
            Identificador = t.Id,
            Titulo = t.Title,
            Completado = t.IsCompleted,
            PasoActual = t.Step,
            FechaCreacion = t.CreatedAt
        })
            .ToListAsync();

        return (result);
    }


    //get search
    public async Task<IEnumerable<TaskSearchResult>> SearchAsync(
    string? text,
    bool? completed,
    int? step,
    string? orderBy,
    TaskSearchRequest request
        )
    {
        var query = _context.Tasks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(text))
            query = query.Where(t => t.Title.Contains(text));

        if (completed.HasValue)
            query = query.Where(t => t.IsCompleted == completed);

        if (step.HasValue)
            query = query.Where(t => t.Step == step);

        //ordenamiento
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

        //Proyeccion a DTO
        var results = await query.Select(t => new TaskSearchResult
        {
            Identificador = t.Id,
            Titulo = t.Title,
            Completado = t.IsCompleted,
            PasoActual = t.Step,
            FechaCreacion = t.CreatedAt
        })
            .ToListAsync();

        return (results);
    }


    //Delete
    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }


    //Put
    public async Task<bool> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        if (!string.IsNullOrWhiteSpace(request.Title))
            task.Title = request.Title.Trim();

        if (request.Step.HasValue)
            task.Step = request.Step.Value;

        if (request.CategoryId.HasValue)
            task.CategoryId = request.CategoryId.Value;

        if (request.IsCompleted.HasValue)
            task.IsCompleted = request.IsCompleted.Value;

        await _context.SaveChangesAsync();
        return true;
    }



    //POST
    //Aquí no hay BadRequest, no hay CreatedAtAction, solo negocio y datos.
    //1 valida datos
    //2 crea la entidad
    //3 guarda en base de datos
    //4 devuelve respuesta
    public async Task<TaskItemResponse> CreateAsync(CreateTaskRequest request)
    {
        //Si no hay body → error.
        if (request == null)
            throw new BusinessException("Body requerido.", 400);

        //Si el título está vacío → error.
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BusinessException("Title es requerido.", 400);

        //Verifica que la acategoria exista
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId);

        //Si no existe -> Mensaje error La categoria no existe
        if (!categoryExists)
            throw new BusinessException("La categoría no existe.", 404);

        //Crear la entidad
        //Aquí se crea el objeto que irá a la base de datos.
        var entity = new TaskItem
        {
            Title = request.Title.Trim(),
            IsCompleted = request.IsCompleted,
            CategoryId = request.CategoryId,
            Step = request.Step 
        };

        //Insertar en Base de Datos (Entity Framework marca el objeto como Added.)
        _context.Tasks.Add(entity);
        //Guarda en la base de datos (Aquí EF ejecuta SQL:)
        await _context.SaveChangesAsync();

        //Construccion de respuesta
        //Se devuelve un DTO, ¡¡¡ NO SE DEVUELVE LA ENTIDAD COMPLETA !!!
        //Se mapea o se pasa los datos de la entidad al DTO de respuesta
        return new TaskItemResponse
        {
            //En la respuesta pon el Id que SQL acaba de generar automaticamente
            Id = entity.Id,
            //Pon el titulo que acabamos de generar
            Title = entity.Title,
            //pon si esta completada
            IsCompleted = entity.IsCompleted,
            //pon el step que guardamos
            Step = entity.Step 
        };
    }



    //get by id
    public async Task<TaskItemResponse?> GetByIdAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return null;

        return new TaskItemResponse
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted
        };
    }


    //get
    public async Task<IEnumerable<TaskItemResponse>> GetAllAsync()
    {
        return await _context.Tasks
            .Select(t => new TaskItemResponse
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted
            })
            .ToListAsync();
    }

}