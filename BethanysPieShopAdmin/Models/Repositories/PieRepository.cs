﻿using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class PieRepository : IPieRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public PieRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }

        public async Task<IEnumerable<Pie>> GetAllPiesAsync()
        {
            return await _bethanysPieShopDbContext.Pies.OrderBy(c => c.PieId).AsNoTracking().ToListAsync();
        }

        public async Task<Pie?> GetPieByIdAsync(int pieId)
        {
            return await _bethanysPieShopDbContext.Pies.Include(p => p.Ingredients).Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.PieId == pieId);
        }

        async Task<int> IPieRepository.AddPieAsync(Pie pie)
        {
            _bethanysPieShopDbContext.Pies.Add(pie);
            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePieAsync(Pie pie)
        {
            var pieToUpdate = await _bethanysPieShopDbContext.Pies.FirstOrDefaultAsync(c => c.PieId == pie.PieId);

            if (pieToUpdate != null)
            {
                pieToUpdate.CategoryId = pie.CategoryId;
                pieToUpdate.ShortDescription = pie.ShortDescription;
                pieToUpdate.LongDescription = pie.LongDescription;
                pieToUpdate.Price = pie.Price;
                pieToUpdate.AllergyInformation = pie.AllergyInformation;
                pieToUpdate.ImageThumbnailUrl = pie.ImageThumbnailUrl;
                pieToUpdate.ImageUrl = pie.ImageUrl;
                pieToUpdate.InStock = pie.InStock;
                pieToUpdate.IsPieOfTheWeek = pie.IsPieOfTheWeek;
                pieToUpdate.Name = pie.Name;

                _bethanysPieShopDbContext.Pies.Update(pieToUpdate);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The pie to update can't be found.");
            }
        }

        public async Task<int> DeletePieAsync(int id)
        {
            var pieToDelete = await _bethanysPieShopDbContext.Pies.FirstOrDefaultAsync(c => c.PieId == id);

            if (pieToDelete != null)
            {
                _bethanysPieShopDbContext.Pies.Remove(pieToDelete);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The pie to delete can't be found.");
            }
        }

        async Task<int> IPieRepository.GetAllPiesCountAsync()
        {
            var count = await _bethanysPieShopDbContext.Pies.CountAsync();

            return count;
        }

        async Task<IEnumerable<Pie>> IPieRepository.GetPiesPagedAsync(int? pageNumber, int pageSize)
        {
            IQueryable<Pie> pies = from p in _bethanysPieShopDbContext.Pies select p;

            pageNumber ??= 1;
            pies = pies.Skip((pageNumber.Value-1) * pageSize).Take(pageSize);

            return await pies.AsNoTracking().ToListAsync();
        }
    }
}
