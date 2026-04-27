using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IClientRepository
{
    public Client CreateClient(Client entity);

    public IQueryable<Client> GetAllClients();

    public Task<Client?> GetClientByIdAsync(int id);

    public Agency CreateAgency(Agency entity);

    public Task<Agency?> GetAgencyByIdAsync(int id);

    public Task<Agency?> GetAgencyByNameAsync(string name);

    public Client DeleteClient(Client client);
}
