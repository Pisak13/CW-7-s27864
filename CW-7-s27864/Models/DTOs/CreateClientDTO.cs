using System.ComponentModel.DataAnnotations;

namespace CW_7_s27864.DTOs;

public class CreateClientDTO
{
    public int IdClient { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string LastName { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 1)]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(120)]
    public string Phone { get; set; }

    [StringLength(120)]
    public string Pesel { get; set; }
}