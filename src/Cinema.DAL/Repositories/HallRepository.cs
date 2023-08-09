using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories
{
    public class HallRepository: IHallRepository
    {
        private readonly ApplicationContext _db;

        public HallRepository(ApplicationContext applicationContext)
        {
            _db = applicationContext;
        }

        public async Task<Hall?> GetAsync(int id)
        {
            return await _db.Halls.FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<Hall>> ListAsync()
        {
            return await _db.Halls.ToListAsync();
        }

        public async Task<IEnumerable<BaseShortModel>> ListInfoAsync()
        {
            return await _db.Halls.Select(h => new BaseShortModel
            {
                Id = h.Id,
                Name = h.Name,
            }).ToListAsync();
        }
    }
}
