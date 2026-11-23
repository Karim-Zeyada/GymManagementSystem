using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GymManagementSystem.DAL.Models;

public class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public DateOnly DateOfBirth { get; set; } = default!;

    public Gender Gender { get; set; } = default!;

    public Address Address { get; set; } = default!;

}
