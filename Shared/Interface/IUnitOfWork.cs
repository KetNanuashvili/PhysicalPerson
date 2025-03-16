using Project.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IPhysicalPersonRepository PhysicalPerson { get; }
        Task<int> SaveChangesAsync();
  
    }
}
