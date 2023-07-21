namespace Project.Domain.Exceptions;

public class InvalidOperationDomainException : Exception
{
    public InvalidOperationDomainException(string? message) : base(message) { }
}
