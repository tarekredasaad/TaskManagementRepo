namespace Application.Dtos;

public class UpdateTaskStatusRequest
{
    public Domain.Enum.TaskStatus Status { get; set; }
}
