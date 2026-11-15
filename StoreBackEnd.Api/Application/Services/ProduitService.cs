using AutoMapper;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Produit;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Exceptions;
using StoreBackEnd.Api.Core.Interfaces;
using System.Linq.Expressions;

namespace StoreBackEnd.Api.Application.Services
{
    public class ProduitService : IProduitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProduitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDTO<ProduitDTO>> GetPagedAsync(
            PaginationParamsDTO pagination, ProduitFiltreDTO filtre)
        {
            var predicat = ConstruirePredicat(filtre);

            var (items, total) = await _unitOfWork.Produits.GetPagedAsync(
                pagination.Page,
                pagination.PageSize,
                predicat,
                p => p.Categorie  
            );

            return new PagedResultDTO<ProduitDTO>
            {
                Items = _mapper.Map<List<ProduitDTO>>(items),
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = total
            };
        }

        public async Task<ProduitDTO> GetByIdAsync(int id)
        {
            var produit = await _unitOfWork.Produits.GetByIdAsync(id)
                ?? throw new NotFoundException("Produit", id);

            return _mapper.Map<ProduitDTO>(produit);
        }

        public async Task<ProduitDTO> CreateAsync(ProduitCreateDTO dto)
        {
            var categorie = await _unitOfWork.Categories.GetByIdAsync(dto.CategorieID)
                ?? throw new NotFoundException("Categorie", dto.CategorieID);

            var produit = _mapper.Map<Produit>(dto);
            await _unitOfWork.Produits.AddAsync(produit);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProduitDTO>(produit);
        }

        public async Task<ProduitDTO> UpdateAsync(int id, ProduitUpdateDTO dto)
        {
            var produit = await _unitOfWork.Produits.GetByIdAsync(id)
                ?? throw new NotFoundException("Produit", id);

            var categorie = await _unitOfWork.Categories.GetByIdAsync(dto.CategorieID)
                ?? throw new NotFoundException("Categorie", dto.CategorieID);

            _mapper.Map(dto, produit);

            await _unitOfWork.Produits.UpdateAsync(produit);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProduitDTO>(produit);
        }

        public async Task DeleteAsync(int id)
        {
            var supprime = await _unitOfWork.Produits.DeleteAsync(id);
            if (!supprime)
                throw new NotFoundException("Produit", id);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ProduitDTO>> GetProduitsSousStockMinimumAsync()
        {
            var produits = await _unitOfWork.Produits.GetProduitsSousStockMinimumAsync();
            return _mapper.Map<List<ProduitDTO>>(produits);
        }
        private static Expression<Func<Produit, bool>>? ConstruirePredicat(ProduitFiltreDTO filtre)
        {
            Expression<Func<Produit, bool>> predicat = p => true; 

            if (filtre.CategorieID.HasValue)
            {
                var id = filtre.CategorieID.Value;
                predicat = CombinerEt(predicat, p => p.CategorieID == id);
            }

            if (!string.IsNullOrWhiteSpace(filtre.Recherche))
            {
                var recherche = filtre.Recherche.ToLower();
                predicat = CombinerEt(predicat, p => p.Nom.ToLower().Contains(recherche));
            }

            if (filtre.PrixMin.HasValue)
            {
                var min = filtre.PrixMin.Value;
                predicat = CombinerEt(predicat, p => p.Prix >= min);
            }

            if (filtre.PrixMax.HasValue)
            {
                var max = filtre.PrixMax.Value;
                predicat = CombinerEt(predicat, p => p.Prix <= max);
            }

            return predicat;
        }

        private static Expression<Func<T, bool>> CombinerEt<T>(
            Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parametre = Expression.Parameter(typeof(T));
            var corps = Expression.AndAlso(
                Expression.Invoke(expr1, parametre),
                Expression.Invoke(expr2, parametre));

            return Expression.Lambda<Func<T, bool>>(corps, parametre);
        }
    }
}