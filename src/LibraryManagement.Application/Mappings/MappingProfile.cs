using AutoMapper;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Queries -> GetBookedBuQueryHandler
        // Book -> BookDto
        CreateMap<Book, BookDto>();

        // Loan -> LoanDto (sa custom mapiranjem)
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.BookTitle,
                       opt => opt.MapFrom(src => src.BookCopy.Book.Title))
            .ForMember(dest => dest.BookAuthor,
                       opt => opt.MapFrom(src => src.BookCopy.Book.Author));

        // Reservation -> ReservationDto (sa custom mapiranjem)
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.BookTitle,
                       opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookAuthor,
                       opt => opt.MapFrom(src => src.Book.Author));
    }
}