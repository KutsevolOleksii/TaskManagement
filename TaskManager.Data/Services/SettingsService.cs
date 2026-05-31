using TaskManager.Core.Entities;
using TaskManager.Data.Database;

namespace TaskManager.Data.Services;

public class SettingsService
{
    private readonly AppDbContext _context = new();

    public bool IsDarkTheme()
    {
        var settings = _context.Settings.FirstOrDefault();

        if (settings == null)
        {
            settings = new AppSettings
            {
                IsDarkTheme = true
            };

            _context.Settings.Add(settings);
            _context.SaveChanges();
        }

        return settings.IsDarkTheme;
    }

    public void SaveTheme(bool isDark)
    {
        var settings = _context.Settings.FirstOrDefault();

        if (settings == null)
        {
            settings = new AppSettings();

            _context.Settings.Add(settings);
        }

        settings.IsDarkTheme = isDark;

        _context.SaveChanges();
    }
}