namespace Web.Models;

public record AccountViewModel(string Id, string? Email);

public record DeleteViewModel(IEnumerable<string> Errors);

public record ChangePasswordViewModel(IEnumerable<string> Errors);