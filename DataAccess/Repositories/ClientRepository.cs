using DataAccess.EntityFramework;
using Domain.Classes.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class ClientRepository(DatabaseContext context) : RepositoryBase<Client>(context), IClientRepository
{
    public Client CreateClient(Client entity) => Create(entity);

    public Agency CreateAgency(Agency entity) => context.Agencies.Add(entity).Entity;

    public IQueryable<Client> GetAllClients() => GetAll();

    public async Task<Client?> GetClientByIdAsync(int id) => await Get(id);

    public async Task<Agency?> GetAgencyByIdAsync(int id) => await context.Agencies.FindAsync(id);

    public Client DeleteClient(Client client) => Delete(client);

    public async Task<Agency?> GetAgencyByNameAsync(string name) => await context.Agencies.Where(agency => agency.Name == name).FirstOrDefaultAsync();
}
