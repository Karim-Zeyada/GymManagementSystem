using GymManagementSystem.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Trainers;

public class CreateTrainerViewModel
{
    // Section 1: Personal Information
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone must be a valid Egyptian number (11 digits starting with 010, 011, 012, or 015).")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public Gender Gender { get; set; }

    // Section 2: Address Information
    [Required(ErrorMessage = "Building number is required.")]
    [Range(1, 10000, ErrorMessage = "Building number must be valid.")]
    [Display(Name = "Building Number")]
    public int BuildingNumber { get; set; }

    [Required(ErrorMessage = "Street is required.")]
    [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string City { get; set; } = string.Empty;

    // Section 3: Professional Information
    [Required(ErrorMessage = "Specialty is required.")]
    [Display(Name = "Specialty")]
    public Speciality Speciality { get; set; }
}
