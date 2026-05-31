using TaskManager.Core.Entities;
using TaskManager.Data.Database;

namespace TaskManager.Data.Services;

public class DatabaseTaskService
{
    private readonly AppDbContext _context = new();

    public List<TaskItem> GetAllTasks()
    {
        return _context.Tasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Deadline)
            .ToList();
    }

    public void AddTask(TaskItem task)
    {
        _context.Tasks.Add(task);
        _context.SaveChanges();
    }

    public void RemoveTask(Guid id)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return;
        }

        _context.Tasks.Remove(task);
        _context.SaveChanges();
    }

    public void UpdateTask(TaskItem updatedTask)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);

        if (task == null)
        {
            return;
        }

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Priority = updatedTask.Priority;
        task.Type = updatedTask.Type;
        task.Deadline = updatedTask.Deadline;
        task.IsCompleted = updatedTask.IsCompleted;

        _context.SaveChanges();
    }

    public void ToggleComplete(Guid id)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return;
        }

        task.IsCompleted = !task.IsCompleted;

        _context.SaveChanges();
    }
}