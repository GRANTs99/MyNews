using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Repository
{
    public class SQLServerAvatarRepository : IRepository<Avatar>
    {
        private ApplicationContext _context;
        public SQLServerAvatarRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void Add(Avatar entity)
        {
            _context.Avatars.Add(entity);
        }

        public Avatar Get(int id)
        {
            return _context.Avatars.Where(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<Avatar> GetAll()
        {
            return _context.Avatars.ToList();
        }

        public async Task<IEnumerable<Avatar>> GetAllAsync()
        {
            return await _context.Avatars.ToListAsync();
        }

        public async Task<Avatar> GetAsync(int id)
        {
            return await _context.Avatars.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public void Remove(Avatar entity)
        {
            _context.Avatars.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Avatar item)
        {
            _context.Avatars.Update(item);
        }
    }
}
