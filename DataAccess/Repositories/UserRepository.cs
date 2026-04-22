using DataAccess.EntityFramework;
using Domain.Classes.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class UserRepository(DatabaseContext context) : RepositoryBase<User>(context), IUserRepository
{
    public IQueryable<User> GetAllUsers() => GetAll();

    public async Task<User?> GetUserByIdAsync(int id) => await Get(id);

    public User DeleteUser(User entity) => Delete(entity);
}
