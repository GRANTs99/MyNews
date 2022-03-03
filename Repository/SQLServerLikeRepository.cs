using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Repository
{
    public class SQLServerLikeRepository : IRepository<Like>
    {
        private ApplicationContext _context;
        public SQLServerLikeRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void Add(Like entity)
        {
            _context.Likes.Add(entity);
        }

        public Like Get(int id)
        {
            return _context.Likes.Where(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<Like> GetAll()
        {
            return _context.Likes.ToList();
        }

        public async Task<IEnumerable<Like>> GetAllAsync()
        {
            return await _context.Likes.ToListAsync();
        }

        public async Task<Like> GetAsync(int id)
        {
            return await _context.Likes.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public void Remove(Like entity)
        {
            _context.Likes.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Like item)
        {
            _context.Likes.Update(item);
        }
    }
}
