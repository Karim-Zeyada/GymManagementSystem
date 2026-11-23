using GymManagementSystem.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Trainers;

public class EditTrainerViewModel
{
    public int Id { get; set; }

    // Display-only / Locked fields
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    // Editable fields
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone must be a valid Egyptian number (11 digits starting with 010, 011, 012, or 015).")]
    public string Phone { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "Specialty is required.")]
    public Speciality Speciality { get; set; }
}
