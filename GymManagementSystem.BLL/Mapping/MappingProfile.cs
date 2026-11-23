using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;

namespace GymManagementSystem.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ─── Session Mappings ──────────────────────────────────
        CreateMap<Session, SessionViewModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer != null ? src.Trainer.Name : string.Empty))
            .ForMember(dest => dest.BookedSlots, opt => opt.MapFrom(src => src.Bookings != null ? src.Bookings.Count : 0));

        CreateMap<CreateSessionViewModel, Session>();

        CreateMap<Session, SessionDetailsViewModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer != null ? src.Trainer.Name : string.Empty))
            .ForMember(dest => dest.BookedSlots, opt => opt.MapFrom(src => src.Bookings != null ? src.Bookings.Count : 0));

        CreateMap<Session, EditSessionViewModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

        CreateMap<EditSessionViewModel, Session>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Capacity, opt => opt.Ignore());

        CreateMap<Session, DeleteSessionViewModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer != null ? src.Trainer.Name : string.Empty))
            .ForMember(dest => dest.BookedSlots, opt => opt.MapFrom(src => src.Bookings != null ? src.Bookings.Count : 0));

        // ─── Member Mappings ───────────────────────────────────
        CreateMap<Member, MemberViewModel>();
        CreateMap<Member, EditMemberViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address != null ? src.Address.BuildingNumber : 0))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address != null ? src.Address.Street : string.Empty))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : string.Empty));

        CreateMap<CreateMemberViewModel, Member>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
            {
                BuildingNumber = src.BuildingNumber,
                Street = src.Street.Trim(),
                City = src.City.Trim()
            }))
            .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => new HealthRecord
            {
                Height = src.Height,
                Weight = src.Weight,
                BloodType = src.BloodType,
                MedicalConditions = src.Note != null ? src.Note.Trim() : null,
                CreatedAt = DateTime.UtcNow
            }))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateOnly.FromDateTime(DateTime.UtcNow)));

        // ─── Trainer Mappings ──────────────────────────────────
        CreateMap<Trainer, TrainerViewModel>();
        CreateMap<Trainer, EditTrainerViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address != null ? src.Address.BuildingNumber : 0))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address != null ? src.Address.Street : string.Empty))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : string.Empty));

        CreateMap<CreateTrainerViewModel, Trainer>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
            {
                BuildingNumber = src.BuildingNumber,
                Street = src.Street.Trim(),
                City = src.City.Trim()
            }))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // ─── Plan Mappings ─────────────────────────────────────
        CreateMap<Plan, PlanViewModel>();
        CreateMap<Plan, PlanDetailsViewModel>();
        CreateMap<Plan, EditPlanViewModel>();
        CreateMap<EditPlanViewModel, Plan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
