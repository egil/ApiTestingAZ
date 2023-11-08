using FluentValidation;

namespace TodoApi.Todos;

public record class AddOrUpdateTodoDto(
    string Name, 
    bool IsComplete)
{
    public sealed class Validator : AbstractValidator<AddOrUpdateTodoDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithName("name");
        }
    }
}
