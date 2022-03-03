using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Repository
{
    public class SQLServerPublicationItemRepository : IRepository<PublicationItem>
    {
        private ApplicationContext _context;
        public SQLServerPublicationItemRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void Add(PublicationItem entity)
        {
            _context.PublicationItems.Add(entity);
        }

        public PublicationItem Get(int id)
        {
            return _context.PublicationItems.Where(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<PublicationItem> GetAll()
        {
            return _context.PublicationItems.ToList();
        }
        public async Task<IEnumerable<PublicationItem>> GetAllAsync()
        {
            return await _context.PublicationItems.ToListAsync();
        }

        public async Task<PublicationItem> GetAsync(int id)
        {
            return await _context.PublicationItems.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public void Remove(PublicationItem entity)
        {
            _context.PublicationItems.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(PublicationItem item)
        {
            _context.PublicationItems.Update(item);
        }
    }
}
