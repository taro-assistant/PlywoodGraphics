namespace PlywoodGraphics.Core;

/// <summary>
/// グラフィックスデバイスの抽象化インターフェース
/// Metal、OpenGL ES、将来的にVulkanを統一
/// </summary>
public interface IGraphicsDevice : IDisposable
{
    /// <summary>
    /// バックエンドAPI名（"Metal", "OpenGLES", "Vulkan"など）
    /// </summary>
    string BackendName { get; }
    
    /// <summary>
    /// APIバージョン（"3.0", "3.2"など）
    /// </summary>
    Version ApiVersion { get; }
    
    /// <summary>
    /// 現在のフレームバッファサイズ
    /// </summary>
    (int Width, int Height) FramebufferSize { get; }
    
    /// <summary>
    /// レンダーパスを開始
    /// </summary>
    IRenderPass BeginRenderPass(RenderPassDescriptor descriptor);
    
    /// <summary>
    /// レンダーパスを終了して表示
    /// </summary>
    void Present();
    
    /// <summary>
    /// バッファを作成
    /// </summary>
    IBuffer CreateBuffer(BufferDescriptor descriptor);
    
    /// <summary>
    /// テクスチャを作成
    /// </summary>
    ITexture CreateTexture(TextureDescriptor descriptor);
    
    /// <summary>
    /// シェーダープログラムを作成
    /// </summary>
    IShaderProgram CreateShaderProgram(ShaderDescriptor descriptor);
    
    /// <summary>
    /// パイプライン状態を作成
    /// </summary>
    IPipelineState CreatePipelineState(PipelineStateDescriptor descriptor);
}

/// <summary>
/// レンダーパス記述子
/// </summary>
public record RenderPassDescriptor
{
    public required Color ClearColor { get; init; }
    public float ClearDepth { get; init; } = 1.0f;
    public uint ClearStencil { get; init; } = 0;
}

/// <summary>
/// カラー構造体（RGBA）
/// </summary>
public record struct Color(float R, float G, float B, float A)
{
    public static readonly Color Black = new(0, 0, 0, 1);
    public static readonly Color White = new(1, 1, 1, 1);
    public static readonly Color Transparent = new(0, 0, 0, 0);
    public static readonly Color Red = new(1, 0, 0, 1);
    public static readonly Color Green = new(0, 1, 0, 1);
    public static readonly Color Blue = new(0, 0, 1, 1);
}

/// <summary>
/// バッファ記述子
/// </summary>
public record BufferDescriptor
{
    public required BufferUsage Usage { get; init; }
    public required int Size { get; init; }
    public ReadOnlyMemory<byte>? InitialData { get; init; }
}

[Flags]
public enum BufferUsage
{
    Vertex = 1,
    Index = 2,
    Uniform = 4,
    Storage = 8
}

/// <summary>
/// テクスチャ記述子
/// </summary>
public record TextureDescriptor
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public TextureFormat Format { get; init; } = TextureFormat.RGBA8Unorm;
    public TextureUsage Usage { get; init; } = TextureUsage.ShaderRead;
}

public enum TextureFormat
{
    RGBA8Unorm,
    RGB8Unorm,
    RG8Unorm,
    R8Unorm,
    RGBA16Float,
    RGBA32Float,
    Depth24Stencil8
}

[Flags]
public enum TextureUsage
{
    ShaderRead = 1,
    ShaderWrite = 2,
    RenderTarget = 4
}

/// <summary>
/// シェーダー記述子
/// </summary>
public record ShaderDescriptor
{
    public required string VertexShaderName { get; init; }
    public required string FragmentShaderName { get; init; }
    public ShaderLanguage Language { get; init; } = ShaderLanguage.Compiled;
}

public enum ShaderLanguage
{
    Plywood,      // C#サブセット DSL
    Compiled      // 事前コンパイル済み
}

/// <summary>
/// パイプライン状態記述子
/// </summary>
public record PipelineStateDescriptor
{
    public required IShaderProgram ShaderProgram { get; init; }
    public required VertexLayout VertexLayout { get; init; }
    public PrimitiveType PrimitiveType { get; init; } = PrimitiveType.Triangle;
    public BlendMode BlendMode { get; init; } = BlendMode.Opaque;
    public DepthStencilMode DepthStencilMode { get; init; } = DepthStencilMode.None;
}

public enum PrimitiveType
{
    Point,
    Line,
    LineStrip,
    Triangle,
    TriangleStrip
}

public enum BlendMode
{
    Opaque,
    AlphaBlend,
    Additive
}

public enum DepthStencilMode
{
    None,
    Read,
    ReadWrite
}

/// <summary>
/// 頂点レイアウト
/// </summary>
public record VertexLayout
{
    public required int Stride { get; init; }
    public required VertexAttribute[] Attributes { get; init; }
}

public record VertexAttribute
{
    public required string Name { get; init; }
    public required int Offset { get; init; }
    public required VertexFormat Format { get; init; }
    public required int Location { get; init; }
}

public enum VertexFormat
{
    Float,
    Float2,
    Float3,
    Float4,
    Int,
    Int2,
    Int3,
    Int4
}

// 各リソースインターフェース
public interface IRenderPass : IDisposable
{
    void SetPipelineState(IPipelineState pipeline);
    void SetVertexBuffer(IBuffer buffer, int slot = 0);
    void SetIndexBuffer(IBuffer buffer);
    void SetUniformBuffer(IBuffer buffer, int slot = 0);
    void SetTexture(ITexture texture, int slot = 0);
    void Draw(int vertexCount, int instanceCount = 1, int firstVertex = 0);
    void DrawIndexed(int indexCount, int instanceCount = 1, int firstIndex = 0, int vertexOffset = 0);
}

public interface IBuffer : IDisposable
{
    int Size { get; }
    void SetData(ReadOnlySpan<byte> data, int offset = 0);
}

public interface ITexture : IDisposable
{
    int Width { get; }
    int Height { get; }
    TextureFormat Format { get; }
    void SetData(ReadOnlySpan<byte> data);
}

public interface IShaderProgram : IDisposable
{
    string VertexShaderName { get; }
    string FragmentShaderName { get; }
}

public interface IPipelineState : IDisposable
{
    PipelineStateDescriptor Descriptor { get; }
}
