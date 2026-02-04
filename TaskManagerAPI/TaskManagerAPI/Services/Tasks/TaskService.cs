using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.TaskItemDTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Utilities.Exceptions;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

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
            query = query.Where(t => t.IsComplete == completed);

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
                IsCompleted = t.IsComplete,
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
                IsCompleted = t.IsComplete,
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
            Completado = t.IsComplete,
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
            query = query.Where(t => t.IsComplete == completed);

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
            Completado = t.IsComplete,
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

        task.Title = request.Title.Trim();
        task.Step = request.Step; 
        task.CategoryId = request.CategoryId;

        if (request.IsComplete.HasValue)
            task.IsComplete = request.IsComplete.Value;

        await _context.SaveChangesAsync();
        return true;
    }



    //post
    //Aquí no hay BadRequest, no hay CreatedAtAction, solo negocio y datos.
    public async Task<TaskItemResponse> CreateAsync(CreateTaskRequest request)
    {
        if (request == null)
            throw new BusinessException("Body requerido.", 400);

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BusinessException("Title es requerido.", 400);

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId);

        if (!categoryExists)
            throw new BusinessException("La categoría no existe.", 404);

        var entity = new TaskItem
        {
            Title = request.Title.Trim(),
            IsComplete = request.IsComplete,
            CategoryId = request.CategoryId
        };

        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();

        return new TaskItemResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            IsComplete = entity.IsComplete
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
            IsComplete = task.IsComplete
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
                IsComplete = t.IsComplete
            })
            .ToListAsync();
    }

}