using Microsoft.EntityFrameworkCore;
using Project.Infrastructure;
using Project.Interface;
using System;
using System.Threading.Tasks;

namespace Project.Business
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhysicalPersonDbContext _dbContext;
        public IPhysicalPersonRepository PhysicalPerson { get; }

        public UnitOfWork(
            PhysicalPersonDbContext dbContext, 
            IPhysicalPersonRepository physicalPersonRepository)
        {
            _dbContext = dbContext;
            PhysicalPerson = physicalPersonRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

    }
}
