using Project.commands;
using Project.Models;
using Shared.commands;

namespace Project.Interface
{
    public interface IPhysicalPersonService
    {
        Task<int> CreatePersonAsync(CreatePersonCommandDto personDto);
        Task<PhysicalPerson> GetPhysicalPersonAsync(int id);

        Task AddRelationAsync(int from, int to, RelationType relationType);
        Task DeletePersonAsync(int personId);
        Task DeleteRelationAsync(int fromId, int toId);

        Task<bool> UpdatePersonAsync(int id, UpdatePersonCommandDto updatedPerson);

        public Task SetProfilePictureAsync(int id, string imagePath);
        public Task<string> GetProfilePictureAsync(int id);
        public Task DeleteProfilePictureAsync(int id);

        Task<IEnumerable<PhysicalPerson>> QuickSearchPhysicalPersonsAsync(string searchTerm);
        Task<(IEnumerable<UpdatePersonCommandDto> persons, int totalCount)> SearchPhysicalPersonsAsync(
            string searchTerm, int pageNumber, int pageSize);

        Task<List<PhysicalPerson>> AllGetPhysicalPersonAsync();


    }

}