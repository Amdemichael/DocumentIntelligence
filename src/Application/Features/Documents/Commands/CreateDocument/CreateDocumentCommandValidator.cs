using FluentValidation;

namespace Application.Features.Documents.Commands.CreateDocument;

public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".png", ".docx", ".xlsx" };

    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters")
            .Must(HaveValidExtension).WithMessage($"File extension must be one of: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.BlobUrl)
            .NotEmpty().WithMessage("Blob URL is required")
            .Must(BeValidUrl).WithMessage("Blob URL must be a valid URL");

        RuleFor(x => x.UploadedBy)
            .NotEmpty().WithMessage("Uploader email is required")
            .EmailAddress().WithMessage("Valid email address is required");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0")
            .LessThanOrEqualTo(100 * 1024 * 1024).WithMessage("File size cannot exceed 100MB");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => x.Description != null);
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}