using FluentValidation;

namespace TodoApi.Todos;

public record class CreateTodoParameters(
    string Name, 
    bool? IsComplete = false)
{
    public sealed class Validator : AbstractValidator<CreateTodoParameters>
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
