using DrasticAI.Models;

namespace DrasticAI.Tools;

public static class PhiStatic
{
    public static string DefaultPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DrasticAI");

    public static string ToDownloadUrl(this ParameterType type, QuantizationType quantizationType)
    {
        var name = $"Phi-3-mini-{GetParameterString(type)}-instruct";
        // Ex. https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/resolve/main/Phi-3-mini-4k-instruct-fp16.gguf
        // Ex. https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/resolve/main/Phi-3-mini-4k-instruct-q4.gguf
        var link = $"https://huggingface.co/microsoft/{name}-gguf/resolve/main/{name}-{GetQuantizationString(quantizationType)}.gguf";
        return link;
    }

    public static string GetModelPath(this ParameterType type, QuantizationType quantizationType)
        => Path.Combine(DefaultPath, GetQuantizationSubdirectory(quantizationType), type.ToFilename());

    public static string ToFilename(this ParameterType type) => type switch
    {
        ParameterType.K4 => "Phi-3-mini-4k-instruct.gguf",
        ParameterType.K128 => "Phi-3-mini-128k-instruct.gguf",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    public static string GetQuantizationString(this QuantizationType type) => type switch
    {
        QuantizationType.Q4_0 => "q4",
        QuantizationType.F16 => "f16",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    private static string GetParameterString(ParameterType parameter)
    {
        return parameter switch
        {
            ParameterType.K4 => "4k",
            ParameterType.K128 => "128k",
            _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null),
        };
    }

    private static string GetQuantizationSubdirectory(QuantizationType quantization)
    {
        return quantization switch
        {
            QuantizationType.Q4_0 => "q4",
            QuantizationType.F16 => "f16",
            _ => throw new ArgumentOutOfRangeException(nameof(quantization), quantization, null),
        };
    }
}