﻿using System.ComponentModel.DataAnnotations;
using System.Data;
using HealthApi.Models;
public class User
{
    [Key]
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Sex { get; set; } = null!;
    public string Job { get; set; } = null!;
    public string? AreaName { get; set; }    // Optional field for area name
    public bool? IsFlagged { get; set; } = false;
    public int RoleId { get; set; } = 1; // Default to User role
    public Role Role { get; set; } = null!;

    public int EPassStatusId { get; set; } = 4;
    public EPass EPassStatus { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public DateTime CreatedAtTime { get; set; } = DateTime.UtcNow;

    public int? LocationId { get; set; }

    //navigation
    public UserLocation UserLocation { get; set; }

    public ICollection<TestRecord> TestRecords { get; set; } = new List<TestRecord>();
    public ICollection<VaccineRecord> VaccineRecords { get; set; } = new List<VaccineRecord>();
    public ICollection<PermitRequest> PermitRequests { get; set; } = new List<PermitRequest>();
    public ICollection<SelfAssessment> SelfAssessments { get; set; } = new List<SelfAssessment>();
}
