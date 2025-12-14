namespace Stackbuld.Assessment.CSharp.Infrastructure.Configurations;

public record EncryptionSettings
{
    public const string Path = "Security:Encryption";

    public string Key { get; set; } = string.Empty;
};