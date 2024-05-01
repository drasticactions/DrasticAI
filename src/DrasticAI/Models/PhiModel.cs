using DrasticAI.Tools;

namespace DrasticAI.Models;

public class PhiModel
{
    public PhiModel()
    {
    }

    public PhiModel(ParameterType type, QuantizationType quantizationType)
    {
        this.Parameters = type;
        this.QuantizationType = quantizationType;
        this.Name = $"{quantizationType}";
        this.Type = PhiModelType.Standard;
        this.FileLocation = PhiStatic.GetModelPath(type, quantizationType);
        this.DownloadUrl = type.ToDownloadUrl(quantizationType);

        // TODO: Add descriptions
        var modelDescription = quantizationType switch
        {
            QuantizationType.Q4_0 => "Q4",
            QuantizationType.F16 => "F16",
            _ => throw new NotImplementedException(),
        };

        this.Description = $"{modelDescription} - {quantizationType}";
    }

    public PhiModel(string path)
    {
        if (!System.IO.Path.Exists(path))
        {
            throw new ArgumentException(nameof(path));
        }

        this.FileLocation = path;
        this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        this.Type = PhiModelType.User;
    }
    
    public ParameterType Parameters { get; set; }

    public PhiModelType Type { get; set; }

    public QuantizationType QuantizationType { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FileLocation { get; set; } = string.Empty;

    public string DownloadUrl { get; } = string.Empty;

    public string Description { get; } = string.Empty;

    public bool Exists => !string.IsNullOrEmpty(this.FileLocation) && System.IO.Path.Exists(this.FileLocation);
}