using FluentValidation;

namespace TodoApi.Todos;

public record class UpdateTodoParameters(
    string Name,
    bool IsComplete)
{
    public sealed class Validator : AbstractValidator<UpdateTodoParameters>
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