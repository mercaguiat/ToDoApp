using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public enum TaskStatus
    {
        Todo = 0,
        InProgress = 1,
        Blocked = 2,
        Completed = 3,
        Cancelled = 4
    }
    public class TaskModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(100, ErrorMessage="Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Details cannot be longer than 500 characters.")]
        public string Details { get; set; }

        [Display(Name = "Assigned To")]
        public string? AssignedTo { get; set; }

        public string Priority { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        [Display(Name = "Date Started")]
        [DataType(DataType.Date)]
        public DateTime? DataStarted { get; set; }

        [Display(Name = "Date Completed")]
        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }

    }
}
