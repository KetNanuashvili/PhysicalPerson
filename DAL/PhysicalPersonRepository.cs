using Project.Infrastructure;
using Project.Interface;
using Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Shared.commands;
public class PhysicalPersonRepository : IPhysicalPersonRepository
{
    private readonly PhysicalPersonDbContext _dbContext;
    private readonly ILogger<PhysicalPersonRepository> _logger;

    public PhysicalPersonRepository(PhysicalPersonDbContext dbContext, ILogger<PhysicalPersonRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> AddPhysicalPersonAsync(PhysicalPerson person)
    {
        await _dbContext.PhysicalPerson.AddAsync(person); 
        await _dbContext.SaveChangesAsync();
        return person.Id;
    }

    public async Task<City?> GetCityByNameAsync(string cityName)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(x => x.Name == cityName);
    }
    public async Task<PhysicalPerson?> GetPhysicalPersonAsync(int id)
    {
        return await _dbContext.PhysicalPerson
            .Include(p => p.City)
             .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedTo)
                .ThenInclude(r => r.RelatedTo) 
            .Include(p => p.RelatedFrom)
                .ThenInclude(r => r.RelatedFrom) 
            .FirstOrDefaultAsync(p => p.Id == id);
    }


 

    public async Task AddRelationAsync(Relation relation)
    {
        if (relation.FromId == relation.ToId)
        {
            throw new InvalidOperationException("A person cannot have a relation to themselves.");
        }

        var existingRelation = await _dbContext.Relations
            .FirstOrDefaultAsync(r =>
                (r.FromId == relation.FromId && r.ToId == relation.ToId) ||
                (r.FromId == relation.ToId && r.ToId == relation.FromId));

        if (existingRelation != null)
        {
            throw new InvalidOperationException("Relation already exists.");
        }

        await _dbContext.Relations.AddAsync(relation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Relation?> GetRelationAsync(int fromId, int toId)
    {
        return await _dbContext.Relations
            .FirstOrDefaultAsync(r =>
                (r.FromId == fromId && r.ToId == toId) ||
                (r.FromId == toId && r.ToId == fromId));
    }

    public async Task DeleteRelationAsync(int fromId, int toId)
    {
        var relation = await _dbContext.Relations
            .FirstOrDefaultAsync(r =>
                (r.FromId == fromId && r.ToId == toId) ||
                (r.FromId == toId && r.ToId == fromId));

        if (relation == null)
        {
            throw new ArgumentException("Relation not found.");
        }

        _dbContext.Relations.Remove(relation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePhysicalPersonAsync(int personId)
    {
        var person = await _dbContext.PhysicalPerson
            .Include(p => p.RelatedFrom)
            .Include(p => p.RelatedTo)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null)
        {
            throw new ArgumentException($"Person with ID {personId} not found.");
        }

        _dbContext.Relations.RemoveRange(person.RelatedFrom);
        _dbContext.Relations.RemoveRange(person.RelatedTo);

        _dbContext.PhysicalPerson.Remove(person);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdatePhysicalPersonAsync(PhysicalPerson person)
    {
        var existingPerson = await _dbContext.PhysicalPerson
            .Include(p => p.PhoneNumbers)
            .Include(p => p.City)
            .Include(p => p.RelatedTo)  // If you have related persons to update
            .FirstOrDefaultAsync(p => p.Id == person.Id);

        if (existingPerson == null)
        {
            return false;
        }

        // Update the existing person with the new values
        _dbContext.Entry(existingPerson).CurrentValues.SetValues(person);

        // Handle phone numbers (after clearing old ones)
        existingPerson.PhoneNumbers.Clear();
        foreach (var phone in person.PhoneNumbers)
        {
            existingPerson.PhoneNumbers.Add(new PhoneNumber
            {
                Number = phone.Number,
                Type = phone.Type,
                PhysicalPersonId = person.Id
            });
        }

        // Handle RelatedPersons (if provided)
        //existingPerson.RelatedTo.Clear();
        //foreach (var relatedPerson in person.RelatedTo)
        //{
        //    existingPerson.RelatedTo.Add(new RelatedPerson
        //    {
        //        FirstName = relatedPerson.FirstName,
        //        LastName = relatedPerson.LastName,
        //        Relationship = relatedPerson.Relationship
        //    });
        //}

        // Handle ImagePath (if provided)
        if (!string.IsNullOrEmpty(person.ImagePath))
        {
            existingPerson.ImagePath = person.ImagePath;
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
        return true;
    }


    public async Task SetProfilePictureAsync(int personId, string imagePath)
    {
        var person = await _dbContext.PhysicalPerson.FindAsync(personId);
        if (person == null)
        {
            throw new ArgumentException($"Person with ID {personId} not found.");
        }

        try
        {
     
            await DeleteProfilePictureAsync(personId);

      
            person.ImagePath = imagePath;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Profile picture updated for person ID {personId}: {imagePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating profile picture for person ID {personId}: {ex.Message}");
            throw;
        }
    }


    public async Task<string> GetProfilePictureAsync(int id)
    {
        var person = await _dbContext.PhysicalPerson
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        if (person == null)
        {
      
            throw new Exception("Person not found");
        }

        return person.ImagePath;
    }

    public async Task DeleteProfilePictureAsync(int personId)
    {
        var person = await _dbContext.PhysicalPerson.FindAsync(personId);
        try
        {
            if (person != null && !string.IsNullOrEmpty(person.ImagePath))
            {
             
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", person.ImagePath);

           
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);  
                    _logger.LogInformation($"File {filePath} deleted successfully.");
                }
                else
                {
                    _logger.LogWarning($"File {filePath} not found.");
                }

              
                person.ImagePath = null;

             
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Person with ID {personId} not found or does not have a profile picture.");
            }
        }
        catch (Exception ex)
        {
            
            _logger.LogError($"Error deleting file for person ID {personId}: {ex.Message}");
            throw; 
        }
    }

    public async Task<IEnumerable<PhysicalPerson>> QuickSearchPhysicalPersonsAsync(string searchTerm)
    {
        return await _dbContext.PhysicalPerson
            .Where(p => EF.Functions.Like(p.PersonalId, $"%{searchTerm}%") ||
                        EF.Functions.Like(p.FirstName, $"%{searchTerm}%") ||
                        EF.Functions.Like(p.LastName, $"%{searchTerm}%"))
            .Select(p => new PhysicalPerson
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                PersonalId = p.PersonalId,
          
            })
            .ToListAsync();
    }

    public async Task<(IEnumerable<UpdatePersonCommandDto> persons, int totalCount)> SearchPhysicalPersonsAsync(
        string searchTerm, int pageNumber, int pageSize)
    {
        var query = _dbContext.PhysicalPerson
            .Where(p => p.FirstName.Contains(searchTerm)
                     || p.LastName.Contains(searchTerm)
                     || p.PersonalId.Contains(searchTerm) 
                     || p.PhoneNumbers.Any(ph => ph.Number.Contains(searchTerm))) 
            .Include(p => p.City) 
            .Include(p => p.PhoneNumbers) 
            .AsQueryable();

        int totalCount = await query.CountAsync(); 

        var persons = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

       
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



    public async Task<List<PhysicalPerson>> AllGetPhysicalPersonAsync()
    {
        return await _dbContext.PhysicalPerson
            .Include(p => p.City)
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedTo)
                .ThenInclude(r => r.RelatedTo)
            .Include(p => p.RelatedFrom)
                .ThenInclude(r => r.RelatedFrom)
            .ToListAsync();
    }


}

