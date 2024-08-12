using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastucture.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public LookupRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
                
        #region Get Data
        public async Task<List<TEntity>> GetLookupData<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().Where(filter).ToListAsync();
        }
        public async Task<TEntity?> GetLookupDataById<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            var result = await _context.Set<TEntity>().Where(filter).ToListAsync();
            if (result.Count == 0)
                return null;

            return result.FirstOrDefault();
        }

        public async Task<List<YearDTO>> GetYearsListDistinct()
        {
            var result = new List<YearDTO>();

            var partsCatalogs = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences // on p.SequenceId equals s.Id
                    on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                where p.IsActive == true && p.IsDeleted == false
                select new { pc.YearFrom, pc.YearTo }).ToListAsync();

            foreach (var part in partsCatalogs)
            {
                var yrFrom = part.YearFrom;
                var yrTo = part.YearTo;

                for (var i = yrFrom; i <= yrTo; i++)
                {
                    if (!result.Exists(e => e.YearNumber == i))
                    {
                        result.Add(new YearDTO() { YearNumber = i, Year = i.ToString() });
                    }
                }
            }

            return result.OrderByDescending(e => e.YearNumber).ToList(); ;
        }

        public async Task<List<MakeDTO>> GetMakesListDistinct()
        {
            var result = new List<MakeDTO>();
            
            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences // on p.SequenceId equals s.Id
                    on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                where p.IsActive == true && p.IsDeleted == false 
                select new MakeDTO() { Make = pc.Make.Trim() })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Make).ToList();
        }

        public async Task<List<ModelDTO>> GetModelsListDistinct()
        {
            var result = new List<ModelDTO>();

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences // on p.SequenceId equals s.Id
                    on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                where p.IsActive == true && p.IsDeleted == false
                select new ModelDTO() { Model = pc.Model.Trim() })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Model).ToList();
        }

        public async Task<List<CategoryDTO>> GetCategoriesListDistinct()
        {
            var result = new List<CategoryDTO>();

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences // on p.SequenceId equals s.Id
                    on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                where p.IsActive && !p.IsDeleted
                select new CategoryDTO()
                {
                    CatId = c.CatId,
                    Description = c.Description.Trim(),
                    Id = c.Id
                })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Description).ToList(); ;
        }

        public async Task<List<SequenceDTO>> GetSequencesListDistinct()
        {
            var result = new List<SequenceDTO>();

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences // on p.SequenceId equals s.Id
                    on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                where p.IsActive && !p.IsDeleted
                select new SequenceDTO()
                {
                    CategoryId = c.Id,
                    CatId = c.CatId,
                    Id = s.Id
                })
                .Distinct()
                .ToListAsync();

            foreach (var item in result)
            {
                var sequence = await _context.Sequences.FirstOrDefaultAsync(e => e.Id == item.Id);
                if (sequence != null)
                {
                    item.CategoryGroupDescription = sequence.CategoryGroupDescription;
                }
            }

            return result.OrderBy(e => e.CategoryGroupDescription).ToList();
        }

        public async Task<List<MakeDTO>> GetMakesListByYear(ProductFilterDTO productFilterDTO)
        {
            var result = new List<MakeDTO>();
            var year = productFilterDTO.Year;
            var categoryIds = productFilterDTO.CategoryIds;
            var sequenceIds = productFilterDTO.SequenceIds;
            var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
            var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences on p.SequenceId equals s.Id
                where p.IsActive == true && p.IsDeleted == false &&
                    ((year > 0 ? (pc.YearFrom <= year && pc.YearTo >= year) : true) &&
                     (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                     (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                    ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(c.Id) : true) &&
                    ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(s.Id) : true))
                select new MakeDTO() { Make = pc.Make.Trim() })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Make).ToList();
        }

        public async Task<List<ModelDTO>> GetModelsListByMake(ProductFilterDTO productFilterDTO)
        {
            var result = new List<ModelDTO>();
            var year = productFilterDTO.Year;
            var categoryIds = productFilterDTO.CategoryIds;
            var sequenceIds = productFilterDTO.SequenceIds;
            var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
            var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences on p.SequenceId equals s.Id
                where p.IsActive == true && p.IsDeleted == false &&
                    ((year > 0 ? (pc.YearFrom <= year && pc.YearTo >= year) : true) &&
                     (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                     (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                    ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(c.Id) : true) &&
                    ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(s.Id) : true))
                select new ModelDTO() { Model = pc.Model.Trim() })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Model).ToList();
        }
        
        public async Task<List<CategoryDTO>> GetCategoriesListByModel(ProductFilterDTO productFilterDTO)
        {
            var result = new List<CategoryDTO>();
            var year = productFilterDTO.Year;
            var categoryIds = productFilterDTO.CategoryIds;
            var sequenceIds = productFilterDTO.SequenceIds;
            var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
            var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

            result = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join c in _context.Categories on p.CategoryId equals c.Id
                join s in _context.Sequences on p.SequenceId equals s.Id
                where p.IsActive == true && p.IsDeleted == false &&
                    ((year > 0 ? (pc.YearFrom <= year && pc.YearTo >= year) : true) &&
                     (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                     (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                    ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(c.Id) : true) &&
                    ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(s.Id) : true))
                select new CategoryDTO() 
                {
                    CatId = c.CatId,
                    Description = c.Description.Trim(),
                    Id = c.Id
                })
                .Distinct()
                .ToListAsync();

            return result.OrderBy(e => e.Description).ToList();
        }

        public async Task<List<SequenceDTO>> GetSequencesListByCategoryId(ProductFilterDTO productFilterDTO)
        {
            var result = new List<SequenceDTO>();

            try
            {
                var year = productFilterDTO.Year;
                var categoryIds = productFilterDTO.CategoryIds;
                var sequenceIds = productFilterDTO.SequenceIds;
                var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
                var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

                if (year == 0 && string.IsNullOrEmpty(make) && string.IsNullOrEmpty(model) && (categoryIds == null || categoryIds.Count == 0) && (sequenceIds == null || sequenceIds.Count == 0))
                {
                    result = await (
                    from p in _context.Products
                    join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                    join c in _context.Categories on p.CategoryId equals c.Id
                    join s in _context.Sequences // on p.SequenceId equals s.Id
                        on new { ColA = p.SequenceId.Value, ColB = c.CatId } equals new { ColA = s.Id, ColB = s.CatId }
                    where p.IsActive && !p.IsDeleted
                    select new SequenceDTO()
                    {
                        CategoryId = c.Id,
                        CatId = c.CatId,
                        Id = s.Id
                    })
                    .Distinct()
                    .ToListAsync();
                }
                else
                {
                    result = await (
                    from p in _context.Products
                    join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                    join c in _context.Categories on p.CategoryId equals c.Id
                    join s in _context.Sequences on p.SequenceId equals s.Id
                    where p.IsActive == true && p.IsDeleted == false &&
                        ((year > 0 ? pc.YearFrom <= year && pc.YearTo >= year : true) &&
                         (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                         (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                        ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(c.Id) : true) &&
                        ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(s.Id) : true))
                    select new SequenceDTO()
                    {
                        //CategoryGroupDescription = s.CategoryGroupDescription.Trim(),
                        CategoryId = s.CategoryId,
                        CatId = s.CatId,
                        Id = s.Id
                    })
                    .Distinct()
                    .ToListAsync();
                }

                foreach (var item in result)
                {
                    var sequence = await _context.Sequences.FirstOrDefaultAsync(e => e.Id == item.Id);
                    if (sequence != null)
                    {
                        item.CategoryGroupDescription = sequence.CategoryGroupDescription;
                    }
                }

                return result.ToList().Count > 0 ? result.OrderBy(e => e.CategoryGroupDescription).ToList() : result;
            }
            catch (Exception e)
            {
                return result;
            }
            
        }
        #endregion

        #region Save Data
        public Task<TEntity> CreateLookupData<TEntity>(TEntity lookup) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateLookupData<TEntity>(TEntity lookup) where TEntity : class
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
