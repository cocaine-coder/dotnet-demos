using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Demo_FluentValidation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController:ControllerBase
    {
        [HttpPost]
        public IActionResult Add(AddUserCommand command)
        {
            return Ok(command);
        }
    }

    public class AddUserCommandValidator:AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(v => v.Name)
                .MinimumLength(2)
                .MaximumLength(5)
                .NotEmpty();

            RuleFor(v => v.Age)
                .GreaterThan(0)
                .LessThan(150);
        }
    }

    public class AddUserCommand
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
