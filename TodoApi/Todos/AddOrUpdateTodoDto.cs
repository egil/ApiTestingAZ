using FluentValidation;

namespace Api.Todos;

public record class AddOrUpdateTodoDto(string Name, bool IsCompleted)
{
    public class Validator : AbstractValidator<AddOrUpdateTodoDto>
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
