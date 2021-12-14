using Demo_MediatR.Model;

namespace Demo_MediatR
{
    public class DataContext
    {
        public List<User> Users { get; set; } = new List<User>();

        public DataContext()
        {
            Users.Add(new User() { Name = "wangtutu", Age = 12 });
        }
    }
}
