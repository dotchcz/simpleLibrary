namespace SimpleLibrary.Api.Identity;

public record TokenGenerationRequest(string Email, Dictionary<string, object> CustomClaims);