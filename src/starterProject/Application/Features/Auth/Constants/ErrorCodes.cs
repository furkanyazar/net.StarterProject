namespace Application.Features.Auth.Constants;

public static class ErrorCodes
{
    public const string EmailRequired = "Email.Required";
    public const string EmailType = "Email.Type";
    public const string PasswordRequired = "Password.Required";
    public const string PasswordMinLength = "Password.MinLength";
    public const string PasswordAtLeastLetter = "Password.AtLeastLetter";
    public const string PasswordAtLeastDigit = "Password.AtLeastDigit";
}
