using System;
using System.ComponentModel.DataAnnotations;

namespace AICodeAssistant.Api.Models;

public class QueryLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string InputCode { get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = string.Empty;

    [Required]
    public string Result { get; set; } = string.Empty;

    public string? Model { get; set; }

    public string? Endpoint { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
