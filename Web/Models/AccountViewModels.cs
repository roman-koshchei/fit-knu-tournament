namespace Web.Models;

public record AccountViewModel(string Id, string? Email, bool WithExternal);

public record DeleteViewModel(IEnumerable<string> Errors);

public record ChangePasswordViewModel(IEnumerable<string> Errors);