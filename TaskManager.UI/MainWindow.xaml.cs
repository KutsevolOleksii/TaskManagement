using System.Windows;
using System.Windows.Controls;
using TaskManager.Core.Entities;
using TaskManager.Core.Services;

namespace TaskManager.UI;

public partial class MainWindow : Window
{
    private readonly TaskService _taskService = new();

    public MainWindow()
    {
        InitializeComponent();

        PriorityComboBox.SelectedIndex = 1;
        TypeComboBox.SelectedIndex = 0;

        DeadlinePicker.SelectedDate = DateTime.Now.AddDays(1);

        DeadlineTimeTextBox.Text = "12:00";

        RefreshTasks();
    }

    private DateTime GetDeadline()
    {
        var date = DeadlinePicker.SelectedDate ?? DateTime.Now;

        if (TimeSpan.TryParse(DeadlineTimeTextBox.Text, out var time))
        {
            return date.Date + time;
        }

        return date;
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
        {
            MessageBox.Show("Enter task title.");
            return;
        }

        var priority = Enum.Parse<PriorityLevel>(
            ((ComboBoxItem)PriorityComboBox.SelectedItem)
            .Content
            .ToString()!
        );

        var type = Enum.Parse<TaskType>(
            ((ComboBoxItem)TypeComboBox.SelectedItem)
            .Content
            .ToString()!
        );

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = TaskTitleTextBox.Text,
            Description = DescriptionTextBox.Text,
            Deadline = GetDeadline(),
            Priority = priority,
            Type = type,
            IsCompleted = false
        };

        _taskService.AddTask(task);

        ClearInputs();

        RefreshTasks();
    }

    private void DeleteTask_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is TaskItem selectedTask)
        {
            _taskService.RemoveTask(selectedTask.Id);

            RefreshTasks();
        }
    }

    private void ToggleComplete_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is TaskItem selectedTask)
        {
            selectedTask.IsCompleted = !selectedTask.IsCompleted;

            RefreshTasks();
        }
    }

    private void UpdateTask_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is not TaskItem selectedTask)
        {
            return;
        }

        selectedTask.Title = TaskTitleTextBox.Text;

        selectedTask.Description = DescriptionTextBox.Text;

        selectedTask.Priority = Enum.Parse<PriorityLevel>(
            ((ComboBoxItem)PriorityComboBox.SelectedItem)
            .Content
            .ToString()!
        );

        selectedTask.Type = Enum.Parse<TaskType>(
            ((ComboBoxItem)TypeComboBox.SelectedItem)
            .Content
            .ToString()!
        );

        selectedTask.Deadline = GetDeadline();

        RefreshTasks();
    }

    private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TasksListBox.SelectedItem is not TaskItem selectedTask)
        {
            return;
        }

        TaskTitleTextBox.Text = selectedTask.Title;

        DescriptionTextBox.Text = selectedTask.Description;

        PriorityComboBox.SelectedItem =
            PriorityComboBox.Items
            .Cast<ComboBoxItem>()
            .FirstOrDefault(i =>
                i.Content.ToString() == selectedTask.Priority.ToString());

        TypeComboBox.SelectedItem =
            TypeComboBox.Items
            .Cast<ComboBoxItem>()
            .FirstOrDefault(i =>
                i.Content.ToString() == selectedTask.Type.ToString());

        DeadlinePicker.SelectedDate = selectedTask.Deadline.Date;

        DeadlineTimeTextBox.Text =
            selectedTask.Deadline.ToString("HH:mm");
    }

    private void RefreshTasks()
    {
        TasksListBox.ItemsSource = null;
        TasksListBox.ItemsSource = _taskService.GetAllTasks();
    }

    private void ClearInputs()
    {
        TaskTitleTextBox.Clear();

        DescriptionTextBox.Clear();

        PriorityComboBox.SelectedIndex = 1;

        TypeComboBox.SelectedIndex = 0;

        DeadlinePicker.SelectedDate = DateTime.Now.AddDays(1);

        DeadlineTimeTextBox.Text = "12:00";
    }
}