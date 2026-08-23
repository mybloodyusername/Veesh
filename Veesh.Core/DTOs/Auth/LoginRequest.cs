using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.Auth;

public record LoginRequest([Required] string Username, [Required] string Password);