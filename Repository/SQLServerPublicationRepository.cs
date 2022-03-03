using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Repository
{
    public class SQLServerPublicationRepository : IRepository<Publication>
    {
        private ApplicationContext _context;
        public SQLServerPublicationRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void Add(Publication entity)
        {
            _context.Publications.Add(entity);
        }

        public Publication Get(int id)
        {
            return _context.Publications.Include(p => p.Items).Where(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<Publication> GetAll()
        {
            return _context.Publications.Include(p => p.Items).Include(p => p.User).ThenInclude(p => p.Avatar).ToList();
        }
        public async Task<IEnumerable<Publication>> GetAllAsync()
        {
            return await _context.Publications.Include(p => p.Items).ToListAsync();
        }

        public async Task<Publication> GetAsync(int id)
        {
            return await _context.Publications.Include(p => p.Items).Where(p => p.Id == id).FirstOrDefaultAsync();
        }
        public void Remove(Publication entity)
        {
            _context.Publications.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Publication item)
        {
            _context.Publications.Update(item);
        }
    }
}
