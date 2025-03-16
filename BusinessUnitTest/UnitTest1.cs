 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Project.Business;
using Project.commands;
using Project.Interface;
using Project.Models;
using Shared.commands;
using Xunit;

namespace BusinessUnitTest
{
    public class PersonsServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PersonsService _personsService;

        public PersonsServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _personsService = new PersonsService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldReturnPersonId_WhenSuccessful()
        {
            // Arrange
            var createPersonDto = new CreatePersonCommandDto
            {
                Name = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                PersonalId = "123456789",
                PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto { Number = "123456789", Type = PhoneNumberType.Mobile } }
,                City = new CreateCityDbo { Name = "New York" }
            };




            var expectedPerson = new PhysicalPerson
            {
                Id = 1,
                FirstName = createPersonDto.Name,
                LastName = createPersonDto.LastName,
                BirthDate = createPersonDto.BirthDate,
                Gender = createPersonDto.Gender,
                PersonalId = createPersonDto.PersonalId,
                PhoneNumbers = createPersonDto.PhoneNumbers
            .Select(p => new PhoneNumber { Number = p.Number, Type = p.Type })
            .ToList(),
                City = new City { Name = createPersonDto.City.Name }
            };


            _unitOfWorkMock.Setup(u => u.PhysicalPerson.AddPhysicalPersonAsync(It.IsAny<PhysicalPerson>()))
                .ReturnsAsync(1);



            // Act
            var result = await _personsService.CreatePersonAsync(createPersonDto);

            // Assert
            Assert.Equal(expectedPerson.Id, result);
            Assert.Equal(expectedPerson.FirstName, createPersonDto.Name);
            Assert.Equal(expectedPerson.LastName, createPersonDto.LastName);
            Assert.Equal(expectedPerson.BirthDate, createPersonDto.BirthDate);
            Assert.Equal(expectedPerson.Gender, createPersonDto.Gender);
            Assert.Equal(expectedPerson.PersonalId, createPersonDto.PersonalId);
            Assert.Equal(expectedPerson.City.Name, createPersonDto.City.Name);
            Assert.Single(expectedPerson.PhoneNumbers);
            Assert.Equal(expectedPerson.PhoneNumbers.First().Number, createPersonDto.PhoneNumbers.First().Number);
            Assert.Equal(expectedPerson.PhoneNumbers.First().Type, createPersonDto.PhoneNumbers.First().Type);
        }
        [Fact]
        public async Task DeletePersonAsync_ShouldCallRepositoryMethod()
        {
            // Arrange
            int personId = 1;

            // `PhysicalPerson` Mock-ის შექმნა და კონფიგურაცია
            var physicalPersonMock = new Mock<IPhysicalPersonRepository>();
            _unitOfWorkMock.Setup(u => u.PhysicalPerson).Returns(physicalPersonMock.Object);

            physicalPersonMock.Setup(p => p.DeletePhysicalPersonAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _personsService.DeletePersonAsync(personId);

            // Assert
            physicalPersonMock.Verify(p => p.DeletePhysicalPersonAsync(It.Is<int>(id => id == personId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task GetPhysicalPersonAsync_ShouldReturnPerson_WhenExists()
        {
            // Arrange
            var personId = 1;
            var expectedPerson = new PhysicalPerson { Id = personId, FirstName = "John", LastName = "Doe" };
            _unitOfWorkMock.Setup(u => u.PhysicalPerson.GetPhysicalPersonAsync(personId))
                .ReturnsAsync(expectedPerson);

            // Act
            var result = await _personsService.GetPhysicalPersonAsync(personId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personId, result.Id);
        }

        [Fact]
        public async Task QuickSearchPhysicalPersonsAsync_ShouldReturnPersons_WhenMatchesExist()
        {
            // Arrange
            string searchTerm = "John";
            var persons = new List<PhysicalPerson>
            {
                new PhysicalPerson { Id = 1, FirstName = "John", LastName = "Doe" }
            };
            _unitOfWorkMock.Setup(u => u.PhysicalPerson.QuickSearchPhysicalPersonsAsync(searchTerm))
                .ReturnsAsync(persons);

            // Act
            var result = await _personsService.QuickSearchPhysicalPersonsAsync(searchTerm);

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
        }
    }
}
