using Microsoft.EntityFrameworkCore;
using Project.commands;
using Project.Interface;
using Project.Models;
using Shared.commands;
using System.Threading.Tasks;

namespace Project.Business
{
    public class PersonsService : IPhysicalPersonService
    {
        public readonly IUnitOfWork _unitOfWork;

        public PersonsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<int> CreatePersonAsync(CreatePersonCommandDto personDto)
        {
            var person = new PhysicalPerson
            {
                FirstName = personDto.Name,
                LastName = personDto.LastName,
                BirthDate = personDto.BirthDate,
                Gender = personDto.Gender,
                PersonalId = personDto.PersonalId,
                PhoneNumbers = personDto.PhoneNumbers?.Select(x => new PhoneNumber
                {
                    Number = x.Number,
                    Type = x.Type
                }).ToList(),
            };

            if (personDto.City != null)
            {
                var existingCity = await _unitOfWork.PhysicalPerson.GetCityByNameAsync(personDto.City.Name);
                if (existingCity != null)
                {
                    person.City = existingCity;
                }
                else
                {
                    person.City = new City
                    {
                        Name = personDto.City.Name
                    };
                }
            }

            // Save the person data
            return await _unitOfWork.PhysicalPerson.AddPhysicalPersonAsync(person);
        }

        public async Task DeletePersonAsync(int personId)
        {
            await _unitOfWork.PhysicalPerson.DeletePhysicalPersonAsync(personId);
            await _unitOfWork.SaveChangesAsync();  

        }

        public async Task DeleteRelationAsync(int fromId, int toId)
        {
            await _unitOfWork.PhysicalPerson.DeleteRelationAsync(fromId, toId);
        }

        public async Task AddRelationAsync(int fromId, int toId, RelationType relationType)
        {
            var personFrom = await _unitOfWork.PhysicalPerson.GetPhysicalPersonAsync(fromId);
            var personTo = await _unitOfWork.PhysicalPerson.GetPhysicalPersonAsync(toId);

            if (personFrom == null || personTo == null)
            {
                throw new ArgumentException("One or both persons not found");
            }

            var relation = new Relation
            {
                FromId = personFrom.Id,
                ToId = personTo.Id,
                RelationType = relationType
            };

            await _unitOfWork.PhysicalPerson.AddRelationAsync(relation);
        }


        public async Task<bool> UpdatePersonAsync(int id, UpdatePersonCommandDto updatedPerson)
        {
            var existingPerson = await _unitOfWork.PhysicalPerson.GetPhysicalPersonAsync(id);

            if (existingPerson == null)
            {
                return false;
            }

            // Update existing fields
            existingPerson.FirstName = updatedPerson.FirstName ?? existingPerson.FirstName;
            existingPerson.LastName = updatedPerson.LastName ?? existingPerson.LastName;
            existingPerson.BirthDate = updatedPerson.BirthDate ?? existingPerson.BirthDate;
            existingPerson.Gender = updatedPerson.Gender ?? existingPerson.Gender;
            existingPerson.PersonalId = updatedPerson.PersonalId ?? existingPerson.PersonalId;
            existingPerson.ImagePath = updatedPerson.ImagePath ?? existingPerson.ImagePath;

            // Update phone numbers
            if (updatedPerson.PhoneNumbers != null)
            {
                existingPerson.PhoneNumbers.Clear();
                foreach (var phone in updatedPerson.PhoneNumbers.Take(2)) 
                {
                    existingPerson.PhoneNumbers.Add(new PhoneNumber
                    {
                        Number = phone.Number,
                        Type = phone.Type,
                        PhysicalPersonId = id
                    });
                }
            }


            // Update city
            if (updatedPerson.City != null && !string.IsNullOrWhiteSpace(updatedPerson.City.Name))
            {
                var existingCity = await _unitOfWork.PhysicalPerson.GetCityByNameAsync(updatedPerson.City.Name);
                existingPerson.City = existingCity ?? new City { Name = updatedPerson.City.Name };
            }
            // დაკავშირებული პირები
            if (updatedPerson.RelatedPersons != null)
            {
                foreach (var updatedRelation in updatedPerson.RelatedPersons)
                {
                    var existingRelation = existingPerson.RelatedTo.FirstOrDefault(r => r.RelatedTo.FirstName == updatedRelation.FirstName &&
                                                                                        r.RelatedTo.LastName == updatedRelation.LastName);

                    if (existingRelation != null)
                    {
                        // Update existing relation type
                        existingRelation.RelationType = Enum.Parse<RelationType>(updatedRelation.Relationship);
                    }
                    else
                    {
                        // Add new relation if it doesn't exist
                        existingPerson.RelatedTo.Add(new Relation
                        {
                            RelatedTo = new PhysicalPerson
                            {
                                FirstName = updatedRelation.FirstName,
                                LastName = updatedRelation.LastName
                            },
                            RelationType = Enum.Parse<RelationType>(updatedRelation.Relationship)
                        });
                    }
                }
            }


            // მონაცემების შენახვა
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task SetProfilePictureAsync(int id, string imagePath)
        {
            await _unitOfWork.PhysicalPerson.SetProfilePictureAsync(id, imagePath);
        }

        public async Task<string> GetProfilePictureAsync(int id)
        {
            return await _unitOfWork.PhysicalPerson.GetProfilePictureAsync(id);
        }


        public async Task DeleteProfilePictureAsync(int id)
        {
             await _unitOfWork.PhysicalPerson.DeleteProfilePictureAsync(id);
        }

        public async Task<PhysicalPerson> GetPhysicalPersonAsync(int id)
        {
          return  await _unitOfWork.PhysicalPerson.GetPhysicalPersonAsync(id);
        }


        public async Task<IEnumerable<PhysicalPerson>> QuickSearchPhysicalPersonsAsync(string searchTerm)
        {
            return await _unitOfWork.PhysicalPerson.QuickSearchPhysicalPersonsAsync(searchTerm);
        }

        public async Task<(IEnumerable<UpdatePersonCommandDto> persons, int totalCount)> SearchPhysicalPersonsAsync(
            string searchTerm, int pageNumber, int pageSize)
        {
           
            var (persons, totalCount) = await _unitOfWork.PhysicalPerson.SearchPhysicalPersonsAsync(searchTerm, pageNumber, pageSize);

          
            var result = persons.Select(p => new UpdatePersonCommandDto
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                PersonalId = p.PersonalId, 
                City = new UpdateCityDbo { Name = p.City?.Name },
                PhoneNumbers = p.PhoneNumbers.Select(ph => new UpdatePhoneNumberDto
                {
                    Number = ph.Number,
                    Type = ph.Type
                }).ToList(),
                ImagePath = p.ImagePath
            });

            return (result, totalCount);
        }

        public Task<(IEnumerable<UpdatePersonCommandDto> persons, int totalCount)> GetAllPhysicalPersonsAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PhysicalPerson>> AllGetPhysicalPersonAsync()
        {
            return await _unitOfWork.PhysicalPerson.AllGetPhysicalPersonAsync();
        }

    }

}