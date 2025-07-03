using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.Entities;
using HackerPlatform.Core.Interfaces;

namespace HackerPlatform.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Levels = new Repository<Level>(_context);
            Modules = new Repository<Module>(_context);
            Vulnerabilities = new Repository<Vulnerability>(_context);
            Achievements = new Repository<Achievement>(_context);
            LevelProgresses = new Repository<LevelProgress>(_context);
        }

        public IRepository<User> Users { get; }
        public IRepository<Level> Levels { get; }
        public IRepository<Module> Modules { get; }
        public IRepository<Vulnerability> Vulnerabilities { get; }
        public IRepository<Achievement> Achievements { get; }
        public IRepository<LevelProgress> LevelProgresses { get; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
