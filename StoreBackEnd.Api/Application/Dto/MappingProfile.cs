using AutoMapper;

using StoreBackEnd.Api.Application.Dto.Categorie;
using StoreBackEnd.Api.Application.Dto.Produit;
using ProduitEntity = StoreBackEnd.Api.Core.Entities.Produit;
using CategorieEntity = StoreBackEnd.Api.Core.Entities.Categorie;
using UtilisateurEntity = StoreBackEnd.Api.Core.Entities.Utilisateur;
namespace StoreBackEnd.Api.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProduitEntity, ProduitDTO>()
                .ForMember(dest => dest.CategorieNom,
                           opt => opt.MapFrom(src => src.Categorie != null ? src.Categorie.Nom : null));

            CreateMap<ProduitCreateDTO, ProduitEntity>();
            CreateMap<ProduitUpdateDTO, ProduitEntity>();

            CreateMap<CategorieEntity, CategorieDTO>().ReverseMap();
            CreateMap<CategorieCreateDTO, CategorieEntity>();

            CreateMap<UtilisateurEntity, Utilisateur.UtilisateurDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        }
    }
}