namespace TaskManager.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public PriorityLevel Priority { get; set; }

    public TaskType Type { get; set; }

    public bool IsCompleted { get; set; }
}