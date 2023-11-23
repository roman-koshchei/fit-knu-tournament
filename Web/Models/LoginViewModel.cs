namespace Web.Models;

/// <summary>
/// Represents the view model for the login page.
/// </summary>
public record LoginViewModel(bool IsRegistered, string? Error = null);

