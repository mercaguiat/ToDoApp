using System.Text.Json;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public class FileTaskRepository : ITaskRepository
    {
        private readonly string _filePath;
        private readonly object _locker = new();
        private List<TaskModel> _tasks = new();

        // Constructor
        public FileTaskRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
            LoadFromFile();
        }

        // Ensures directory & file exists. If not, then create
        private void EnsureFileExists()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(dir))
                File.WriteAllText(_filePath, "[]");
        }

        // Load tasks from JSON into memory or list
        private void LoadFromFile()
        {
            lock (_locker) // Thread safety: only one thread can read/write at a time
            {
                var json = File.ReadAllText(_filePath); // Read JSON file
                _tasks = JsonSerializer.Deserialize<List<TaskModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<TaskModel>(); // Deserialize JSON into List<TodoItem>
            }
        }

        // Save in-memory list into JSON file
        private void SaveToFile()
        {
            lock (_locker)
            {
                var json = JsonSerializer.Serialize(_tasks,
                    new JsonSerializerOptions { WriteIndented = true }); // JSON Pretty print
                File.WriteAllText(_filePath, json);
            }
        }

        // Return all Tasks
        public List<TaskModel> GetAll()
        {
            lock (_locker)
            {
                // Return a copy of the list (so callers can't modify internal data directly)
                return _tasks.Select(i => i).ToList();
            }
        }

        // Get a single Task by Id
        public TaskModel Get(Guid id)
        {
            lock (_locker)
            {
                return _tasks.FirstOrDefault(i => i.Id == id);
            }
        }

        // Create a new Task and save it
        public TaskModel Create(TaskModel task)
        {
            lock (_locker)
            {
                task.Id = Guid.NewGuid();
                _tasks.Add(task);
                SaveToFile();
                return task;
            }
        }

        // Update an exisiting Task
        public void Update(TaskModel task)
        {
            lock (_locker)
            {
                var updateId = _tasks.FindIndex(i => i.Id == task.Id); // Find index of item
                // If found
                if (updateId >= 0) 
                {
                    _tasks[updateId] = task; // Replace item in list
                    SaveToFile();            // Save changes to file
                }
            }
        }

        // Delete Tasks by ID
        public void Delete(Guid id) {
            lock (_locker)
            {
                var existing = _tasks.FirstOrDefault(i => i.Id == id); // Find item
                if (existing != null)
                {
                    _tasks.Remove(existing); 
                    SaveToFile();
                }
            }
        }

        // Export current Tasks as JSON String
        public string ExportJson()
        {
            lock (_locker) 
            {
                return JsonSerializer.Serialize(_tasks,
                    new JsonSerializerOptions { WriteIndented = true });
            }
        }

        // Import JSON array. If replace true => replace entire store, otherwise merge by Id.
        public void ImportJson(string json, bool replace = false)
        {
            lock (_locker)
            {
                var imported = JsonSerializer.Deserialize<List<TaskModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                    ?? new List<TaskModel>();

                if (replace) // Overwrite everything
                {
                    _tasks = imported;
                }
                else // Merge with existing
                {
                    foreach (var task in imported)
                    {
                        if (task.Id == Guid.Empty) // If item has no ID, assign one
                            task.Id = Guid.NewGuid();

                        var existing = _tasks.FirstOrDefault(i => i.Id == task.Id);
                        if (existing != null) // If already exists, update it
                        {
                            _tasks[_tasks.IndexOf(existing)] = task;
                        }
                        else // Otherwise, add as new
                        {
                            _tasks.Add(task);
                        }
                    }
                }

                SaveToFile(); // Save merged/replaced list
            }
        }
    }
}
