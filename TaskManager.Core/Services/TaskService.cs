using TaskManager.Core.Entities;

namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = [];

    public List<TaskItem> GetAllTasks()
    {
        return _tasks;
    }

    public void AddTask(TaskItem task)
    {
        _tasks.Add(task);
    }

    public void RemoveTask(Guid id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task != null)
        {
            _tasks.Remove(task);
        }
    }
}