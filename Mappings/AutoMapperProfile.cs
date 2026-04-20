using AutoMapper;
using RecetArreAPI2.DTOs;
using RecetArreAPI2.DTOs.Categorias;
using RecetArreAPI2.DTOs.Ingredientes;
using RecetArreAPI2.DTOs.Recetas;
using RecetArreAPI2.DTOs.Ratings;
using RecetArreAPI2.Models;
using RecetArreAPI2.DTOs.Comentarios;

namespace RecetArreAPI2.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ApplicationUser <-> ApplicationUserDto
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();

            // Categoria mappings
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<CategoriaCreacionDto, Categoria>();
            CreateMap<CategoriaModificacionDto, Categoria>();

            // Ingrediente mappings
            CreateMap<Ingrediente, IngredienteDto>();
            CreateMap<IngredienteCreacionDto, Ingrediente>();
            CreateMap<IngredienteModificacionDto, Ingrediente>();

            // Receta mappings
            CreateMap<Receta, RecetaDto>()
                .ForMember(dest => dest.CategoriaIds, opt => opt.MapFrom(src => src.Categorias.Select(c => c.Id)))
                .ForMember(dest => dest.IngredienteIds, opt => opt.MapFrom(src => src.Ingredientes.Select(i => i.Id)))
                .ForMember(dest => dest.PromedioCalificacion, opt => opt.MapFrom(src =>
                    src.Ratings != null && src.Ratings.Any()
                        ? src.Ratings.Average(r => r.Calificacion)
                        : 0));
            CreateMap<RecetaCreacionDto, Receta>();
            CreateMap<RecetaModificacionDto, Receta>();

            // Comentario mappings
            CreateMap<Comentario, ComentarioDto>();
            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap<ComentarioModificacionDto, Comentario>();

            //Rating Mappings
            CreateMap<Rating, RatingDto>();
            CreateMap<RatingCreacionDto, Rating>();
            CreateMap<RatingModificacionDto, Rating>();
        }
    }
}