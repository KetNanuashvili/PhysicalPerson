using Project.Models;
using Shared.commands;

namespace Project.Interface
{
    public interface IPhysicalPersonRepository
    {
        Task<int> AddPhysicalPersonAsync(PhysicalPerson person);
        Task<City> GetCityByNameAsync(string cityName);

        Task<PhysicalPerson> GetPhysicalPersonAsync(int id);
        Task AddRelationAsync(Relation relation);

        Task DeletePhysicalPersonAsync(int personId);
        Task DeleteRelationAsync(int fromId, int toId);
        Task<bool> UpdatePhysicalPersonAsync(PhysicalPerson person);
        public Task SetProfilePictureAsync(int id, string imagePath);
        public Task<string> GetProfilePictureAsync(int id);
        public Task  DeleteProfilePictureAsync(int id);

        Task<IEnumerable<PhysicalPerson>> QuickSearchPhysicalPersonsAsync(string searchTerm);
        Task<(IEnumerable<UpdatePersonCommandDto> persons, int totalCount)> SearchPhysicalPersonsAsync(
            string searchTerm, int pageNumber, int pageSize);
        Task<List<PhysicalPerson>> AllGetPhysicalPersonAsync();


    }
}