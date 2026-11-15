using AutoMapper;
using StoreBackEnd.Api.Application.Dto.Categorie;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Exceptions;
using StoreBackEnd.Api.Core.Interfaces;

namespace StoreBackEnd.Api.Application.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategorieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDTO<CategorieDTO>> GetPagedAsync(PaginationParamsDTO pagination)
        {
            
            var (items, total) = await _unitOfWork.Categories.GetPagedAsync(
                pagination.Page, pagination.PageSize);

            return new PagedResultDTO<CategorieDTO>
            {
                Items = _mapper.Map<List<CategorieDTO>>(items),
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = total
            };
        }

        public async Task<CategorieDTO> GetByIdAsync(int id)
        {
            var categorie = await _unitOfWork.Categories.GetByIdAsync(id)
                ?? throw new NotFoundException("Categorie", id);

            return _mapper.Map<CategorieDTO>(categorie);
        }

        public async Task<CategorieDTO> CreateAsync(CategorieCreateDTO dto)
        {
            var categorie = _mapper.Map<Categorie>(dto);
            await _unitOfWork.Categories.AddAsync(categorie);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategorieDTO>(categorie);
        }

        public async Task<CategorieDTO> UpdateAsync(int id, CategorieCreateDTO dto)
        {
            var categorie = await _unitOfWork.Categories.GetByIdAsync(id)
                ?? throw new NotFoundException("Categorie", id);

            _mapper.Map(dto, categorie);
            await _unitOfWork.Categories.UpdateAsync(categorie);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategorieDTO>(categorie);
        }

        public async Task DeleteAsync(int id)
        {
            var supprime = await _unitOfWork.Categories.DeleteAsync(id);
            if (!supprime)
                throw new NotFoundException("Categorie", id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}