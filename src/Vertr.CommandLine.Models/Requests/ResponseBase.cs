namespace Vertr.CommandLine.Models.Requests;
public abstract class ResponseBase
{
    public string? ErrorMessage { get; init; }

    public bool HasErrors => !String.IsNullOrEmpty(ErrorMessage);
}
