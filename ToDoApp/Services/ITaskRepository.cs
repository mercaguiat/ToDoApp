using ToDoApp.Models;

namespace ToDoApp.Services
{
    public interface ITaskRepository
    {
        List<TaskModel> GetAll();
        TaskModel Get(Guid id);
        TaskModel Create(TaskModel item);
        void Update(TaskModel item);
        void Delete(Guid id);
        string ExportJson();
        void ImportJson(string json, bool replace = false);
    }
}
