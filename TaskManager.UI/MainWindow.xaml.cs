using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskManager.Core.Entities;
using TaskManager.Data.Services;

namespace TaskManager.UI;

public partial class MainWindow : Window
{
    private readonly DatabaseTaskService _taskService = new();
    private readonly SettingsService _settingsService = new();

    private bool _isDarkTheme;

    public MainWindow()
    {
        InitializeComponent();

        PriorityComboBox.SelectedIndex = 1;
        TypeComboBox.SelectedIndex = 0;

        PriorityFilterComboBox.SelectedIndex = 0;
        StatusFilterComboBox.SelectedIndex = 0;

        DeadlinePicker.SelectedDate = DateTime.Now.AddDays(1);
        DeadlineTimeTextBox.Text = "12:00";

        _isDarkTheme = _settingsService.IsDarkTheme();

        ApplyTheme();
        RefreshTasks();
    }

    private void ApplyTheme()
{
    if (_isDarkTheme)
    {
        Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(17, 19, 21));
        Resources["PanelBackground"] = new SolidColorBrush(Color.FromRgb(24, 27, 31));
        Resources["InputBackground"] = new SolidColorBrush(Color.FromRgb(28, 31, 35));
        Resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(50, 53, 58));
        Resources["PrimaryText"] = Brushes.White;
        Resources["SecondaryText"] = new SolidColorBrush(Color.FromRgb(179, 183, 190));

        PriorityComboBox.Foreground = Brushes.Black;
        TypeComboBox.Foreground = Brushes.Black;
        DeadlinePicker.Foreground = Brushes.Black;
        DeadlineTimeTextBox.Foreground = Brushes.Black;

        PriorityFilterComboBox.Foreground = Brushes.Black;
        StatusFilterComboBox.Foreground = Brushes.Black;
    }
    else
    {
        Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(241, 245, 249));
        Resources["PanelBackground"] = Brushes.White;
        Resources["InputBackground"] = new SolidColorBrush(Color.FromRgb(248, 250, 252));
        Resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(203, 213, 225));
        Resources["PrimaryText"] = Brushes.Black;
        Resources["SecondaryText"] = new SolidColorBrush(Color.FromRgb(71, 85, 105));

        PriorityComboBox.Foreground = Brushes.Black;
        TypeComboBox.Foreground = Brushes.Black;
        DeadlinePicker.Foreground = Brushes.Black;
        DeadlineTimeTextBox.Foreground = Brushes.Black;

        PriorityFilterComboBox.Foreground = Brushes.Black;
        StatusFilterComboBox.Foreground = Brushes.Black;
    }

    _settingsService.SaveTheme(_isDarkTheme);
}

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        ApplyTheme();
    }

    private DateTime GetDeadline()
    {
        var date = DeadlinePicker.SelectedDate ?? DateTime.Now;

        if (TimeSpan.TryParse(DeadlineTimeTextBox.Text, out var time))
            return date.Date + time;

        return date;
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            return;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = TaskTitleTextBox.Text,
            Description = DescriptionTextBox.Text,
            Deadline = GetDeadline(),
            Priority = Enum.Parse<PriorityLevel>(((ComboBoxItem)PriorityComboBox.SelectedItem).Content.ToString()!),
            Type = Enum.Parse<TaskType>(((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString()!),
            IsCompleted = false
        };

        _taskService.AddTask(task);

        ClearInputs();
        RefreshTasks();
    }

    private void DeleteTask_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is TaskItem task)
        {
            _taskService.RemoveTask(task.Id);
            RefreshTasks();
        }
    }

    private void ToggleComplete_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is TaskItem task)
        {
            _taskService.ToggleComplete(task.Id);
            RefreshTasks();
        }
    }

    private void UpdateTask_Click(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem is not TaskItem task)
            return;

        task.Title = TaskTitleTextBox.Text;
        task.Description = DescriptionTextBox.Text;
        task.Priority = Enum.Parse<PriorityLevel>(((ComboBoxItem)PriorityComboBox.SelectedItem).Content.ToString()!);
        task.Type = Enum.Parse<TaskType>(((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString()!);
        task.Deadline = GetDeadline();

        _taskService.UpdateTask(task);

        RefreshTasks();
    }

    private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TasksListBox.SelectedItem is not TaskItem task)
            return;

        TaskTitleTextBox.Text = task.Title;
        DescriptionTextBox.Text = task.Description;

        PriorityComboBox.SelectedItem =
            PriorityComboBox.Items
            .Cast<ComboBoxItem>()
            .FirstOrDefault(i => i.Content.ToString() == task.Priority.ToString());

        TypeComboBox.SelectedItem =
            TypeComboBox.Items
            .Cast<ComboBoxItem>()
            .FirstOrDefault(i => i.Content.ToString() == task.Type.ToString());

        DeadlinePicker.SelectedDate = task.Deadline.Date;
        DeadlineTimeTextBox.Text = task.Deadline.ToString("HH:mm");
    }

    private void FilterChanged(object sender, RoutedEventArgs e)
    {
        RefreshTasks();
    }

    private void RefreshTasks()
    {
        var tasks = _taskService
            .GetAllTasks()
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Deadline)
            .ToList();

        var search = SearchTextBox?.Text?.ToLower() ?? "";

        if (!string.IsNullOrWhiteSpace(search))
        {
            tasks = tasks
                .Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    t.Description.ToLower().Contains(search))
                .ToList();
        }

        if (PriorityFilterComboBox?.SelectedItem is ComboBoxItem priorityItem)
        {
            var value = priorityItem.Content.ToString();

            if (value != "All Priorities")
            {
                tasks = tasks
                    .Where(t => t.Priority.ToString() == value)
                    .ToList();
            }
        }

        if (StatusFilterComboBox?.SelectedItem is ComboBoxItem statusItem)
        {
            var value = statusItem.Content.ToString();

            if (value == "Completed")
                tasks = tasks.Where(t => t.IsCompleted).ToList();

            if (value == "In Progress")
                tasks = tasks.Where(t => !t.IsCompleted).ToList();
        }

        TasksListBox.ItemsSource = null;
        TasksListBox.ItemsSource = tasks;
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