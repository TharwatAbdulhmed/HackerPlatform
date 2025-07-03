using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.Entities;

namespace HackerPlatform.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Level> Levels { get; }
        IRepository<Module> Modules { get; }
        IRepository<Vulnerability> Vulnerabilities { get; }
        IRepository<Achievement> Achievements { get; }
        IRepository<LevelProgress> LevelProgresses { get; }

        Task<int> CompleteAsync(); // SaveChanges
    }
}
