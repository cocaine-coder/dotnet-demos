using Demo_MediatR.Model;
using MediatR;

namespace Demo_MediatR.Request
{
    public class AddUserRequest: User,IRequest<bool>
    {
    }

    public class AddUserHandler : IRequestHandler<AddUserRequest, bool>
    {
        private readonly DataContext _dataContext;

        public AddUserHandler(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public Task<bool> Handle(AddUserRequest request, CancellationToken cancellationToken)
        {
            _dataContext.Users.Add(request);

            return Task.FromResult(true);
        }
    }
}
