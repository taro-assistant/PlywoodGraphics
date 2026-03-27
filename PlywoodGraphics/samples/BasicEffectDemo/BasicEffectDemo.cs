// MonoGame BasicEffect風サンプル - 理想的なAPI使用例
// 注意: このサンプルは将来の完全実装を見据えた「理想的な記述」

using PlywoodGraphics.Core;
using PlywoodGraphics.Geometry;
using System.Numerics;

namespace PlywoodGraphics.Samples.BasicEffectDemo;

/// <summary>
/// MonoGameのBasicEffectに似た機能を提供するマテリアル
/// 将来的な完全実装を見据えた理想的なAPI
/// </summary>
public class BasicEffectMaterial : IDisposable
{
    private readonly IGraphicsDevice _device;
    private IShaderProgram? _shaderProgram;
    private IPipelineState? _pipelineState;
    
    // マテリアルパラメータ（CPU側で管理）
    public Vector3 DiffuseColor { get; set; } = Vector3.One;
    public Vector3 AmbientLightColor { get; set; } = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 DirectionalLightDirection { get; set; } = Vector3.Normalize(new Vector3(-1, -1, -1));
    public Vector3 DirectionalLightDiffuseColor { get; set; } = Vector3.One;
    public Vector3 DirectionalLightSpecularColor { get; set; } = Vector3.One;
    public float SpecularPower { get; set; } = 16f;
    public Vector3 EmissiveColor { get; set; } = Vector3.Zero;
    
    // テクスチャ
    public ITexture? Texture { get; set; }
    public bool TextureEnabled { get; set; } = false;
    
    // ライティング
    public bool LightingEnabled { get; set; } = true;
    public bool PerPixelLighting { get; set; } = true;
    
    // 行列
    public Matrix4x4 World { get; set; } = Matrix4x4.Identity;
    public Matrix4x4 View { get; set; } = Matrix4x4.Identity;
    public Matrix4x4 Projection { get; set; } = Matrix4x4.Identity;
    
    public BasicEffectMaterial(IGraphicsDevice device)
    {
        _device = device ?? throw new ArgumentNullException(nameof(device));
        InitializeShaders();
    }
    
    /// <summary>
    /// シェーダー初期化（将来的に自動生成/プリコンパイルされる）
    /// </summary>
    private void InitializeShaders()
    {
        // Ideal API: シェーダーは事前にコンパイル済みのバイナリを使用
        // パスは自動的にプラットフォームに応じて選択される
        // metal3/gles32/vulkan13 など
        _shaderProgram = _device.CreateShaderProgram(new ShaderDescriptor
        {
            VertexShaderName = "BasicEffectVertex",
            FragmentShaderName = "BasicEffectFragment",
            Language = ShaderLanguage.Compiled  // 事前コンパイル済み
        });
        
        // パイプライン状態の作成
        _pipelineState = _device.CreatePipelineState(new PipelineStateDescriptor
        {
            ShaderProgram = _shaderProgram,
            VertexLayout = CreateVertexLayout(),
            PrimitiveType = PrimitiveType.Triangle,
            BlendMode = BlendMode.Opaque,
            DepthStencilMode = DepthStencilMode.ReadWrite
        });
    }
    
    /// <summary>
    /// 頂点レイアウトの作成（Position + Normal + TextureCoordinate）
    /// </summary>
    private VertexLayout CreateVertexLayout()
    {
        // Three.js風: 位置、法線、UV
        return new VertexLayout
        {
            Stride = 32, // 8 floats * 4 bytes
            Attributes = new[]
            {
                new VertexAttribute 
                { 
                    Name = "POSITION", 
                    Location = 0, 
                    Format = VertexFormat.Float3, 
                    Offset = 0 
                },
                new VertexAttribute 
                { 
                    Name = "NORMAL", 
                    Location = 1, 
                    Format = VertexFormat.Float3, 
                    Offset = 12 
                },
                new VertexAttribute 
                { 
                    Name = "TEXCOORD", 
                    Location = 2, 
                    Format = VertexFormat.Float2, 
                    Offset = 24 
                }
            }
        };
    }
    
    /// <summary>
    /// レンダーパスにマテリアルパラメータを適用
    /// </summary>
    public void Apply(IRenderPass renderPass)
    {
        if (_pipelineState == null)
            throw new InvalidOperationException("Material not initialized");
        
        // パイプライン状態を設定
        renderPass.SetPipelineState(_pipelineState);
        
        // ユニフォームバッファの更新（将来的に自動化）
        // 現在の行列とライティングパラメータをGPUに送信
        var worldViewProj = World * View * Projection;
        var worldInverseTranspose = Matrix4x4.Transpose(Invert(World));
        
        // Ideal API: 構造体を直接バインド、内部で自動変換
        var parameters = new BasicEffectParameters
        {
            World = World,
            WorldViewProjection = worldViewProj,
            WorldInverseTranspose = worldInverseTranspose,
            DiffuseColor = new Vector4(DiffuseColor, 1),
            AmbientLightColor = new Vector4(AmbientLightColor, 1),
            LightDirection = DirectionalLightDirection,
            LightDiffuseColor = DirectionalLightDiffuseColor,
            LightSpecularColor = DirectionalLightSpecularColor,
            SpecularPower = SpecularPower,
            EmissiveColor = new Vector4(EmissiveColor, 1),
            TextureEnabled = TextureEnabled ? 1 : 0,
            LightingEnabled = LightingEnabled ? 1 : 0
        };
        
        // 将来的に: renderPass.SetUniformBuffer(0, parameters);
        // 現在は手動でシリアライズが必要かも
    }
    
    /// <summary>
    /// 行列の逆行列を計算（ヘルパー）
    /// </summary>
    private static Matrix4x4 Invert(Matrix4x4 matrix)
    {
        if (!Matrix4x4.Invert(matrix, out var inverted))
            throw new InvalidOperationException("Matrix is not invertible");
        return inverted;
    }
    
    public void Dispose()
    {
        _shaderProgram?.Dispose();
        _pipelineState?.Dispose();
    }
    
    /// <summary>
    /// BasicEffect用ユニフォームバッファ構造体
    /// 将来的に各プラットフォームの要求する形式に自動変換される
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    private struct BasicEffectParameters
    {
        public Matrix4x4 World;
        public Matrix4x4 WorldViewProjection;
        public Matrix4x4 WorldInverseTranspose;
        public Vector4 DiffuseColor;
        public Vector4 AmbientLightColor;
        public Vector3 LightDirection;
        public float SpecularPower;
        public Vector3 LightDiffuseColor;
        public float TextureEnabled;
        public Vector3 LightSpecularColor;
        public float LightingEnabled;
        public Vector4 EmissiveColor;
    }
}

/// <summary>
/// メインゲームクラス（MonoGame風）
/// </summary>
public class BasicEffectGame : IDisposable
{
    private IGraphicsDevice? _graphicsDevice;
    private BasicEffectMaterial? _effect;
    private Geometry? _cubeGeometry;
    private IBuffer? _vertexBuffer;
    private IBuffer? _indexBuffer;
    private ITexture? _whiteTexture;
    
    // カメラ
    private Vector3 _cameraPosition = new Vector3(0, 0, 5);
    private Vector3 _cameraTarget = Vector3.Zero;
    private Vector3 _cameraUp = Vector3.UnitY;
    
    // 回転
    private float _rotationX = 0f;
    private float _rotationY = 0f;
    
    public void Initialize(IGraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        
        // BasicEffectマテリアルの作成
        _effect = new BasicEffectMaterial(_graphicsDevice);
        
        // ジオメトリの作成（Three.js風）
        _cubeGeometry = new BoxGeometry(1, 1, 1);
        
        // バッファの作成
        CreateBuffers();
        
        // デフォルトテクスチャ（白）の作成
        CreateDefaultTexture();
        
        // マテリアル設定
        ConfigureMaterial();
    }
    
    private void CreateBuffers()
    {
        if (_cubeGeometry == null || _graphicsDevice == null) return;
        
        // 頂点データの構築（Position + Normal + TexCoord）
        var positions = _cubeGeometry.Positions.Span;
        var normals = _cubeGeometry.Normals.Span;
        var texCoords = _cubeGeometry.TexCoords.Span;
        
        int vertexCount = _cubeGeometry.VertexCount;
        var vertexData = new float[vertexCount * 8]; // 8 floats per vertex
        
        for (int i = 0; i < vertexCount; i++)
        {
            // Position (3 floats)
            vertexData[i * 8 + 0] = positions[i * 3 + 0];
            vertexData[i * 8 + 1] = positions[i * 3 + 1];
            vertexData[i * 8 + 2] = positions[i * 3 + 2];
            
            // Normal (3 floats)
            vertexData[i * 8 + 3] = normals[i * 3 + 0];
            vertexData[i * 8 + 4] = normals[i * 3 + 1];
            vertexData[i * 8 + 5] = normals[i * 3 + 2];
            
            // TexCoord (2 floats)
            vertexData[i * 8 + 6] = texCoords[i * 2 + 0];
            vertexData[i * 8 + 7] = texCoords[i * 2 + 1];
        }
        
        // 頂点バッファの作成
        _vertexBuffer = _graphicsDevice.CreateBuffer(new BufferDescriptor
        {
            Usage = BufferUsage.Vertex,
            Size = vertexData.Length * sizeof(float),
            InitialData = MemoryMarshal.AsBytes(vertexData.AsSpan())
        });
        
        // インデックスバッファの作成
        var indices = _cubeGeometry.Indices.Span;
        var indexData = new byte[indices.Length * sizeof(uint)];
        Buffer.BlockCopy(indices.ToArray(), 0, indexData, 0, indexData.Length);
        
        _indexBuffer = _graphicsDevice.CreateBuffer(new BufferDescriptor
        {
            Usage = BufferUsage.Index,
            Size = indexData.Length,
            InitialData = indexData
        });
    }
    
    private void CreateDefaultTexture()
    {
        if (_graphicsDevice == null) return;
        
        // 1x1の白テクスチャ
        var whitePixel = new byte[] { 255, 255, 255, 255 };
        
        _whiteTexture = _graphicsDevice.CreateTexture(new TextureDescriptor
        {
            Width = 1,
            Height = 1,
            Format = TextureFormat.RGBA8Unorm,
            Usage = TextureUsage.ShaderRead
        });
        
        _whiteTexture.SetData(whitePixel);
    }
    
    private void ConfigureMaterial()
    {
        if (_effect == null) return;
        
        _effect.DiffuseColor = new Vector3(0.8f, 0.3f, 0.3f); // 赤みがかった色
        _effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.3f);
        _effect.DirectionalLightDirection = Vector3.Normalize(new Vector3(-1, -1, -0.5f));
        _effect.DirectionalLightDiffuseColor = Vector3.One;
        _effect.SpecularPower = 32f;
        _effect.Texture = _whiteTexture;
        _effect.TextureEnabled = false; // テクスチャ無し
    }
    
    public void Update(float deltaTime)
    {
        // 回転アニメーション
        _rotationY += deltaTime * 0.5f;
        _rotationX += deltaTime * 0.3f;
    }
    
    public void Draw()
    {
        if (_graphicsDevice == null || _effect == null || _cubeGeometry == null)
            return;
        
        // ビュー行列の更新
        _effect.View = Matrix4x4.CreateLookAt(_cameraPosition, _cameraTarget, _cameraUp);
        
        // プロジェクション行列の更新
        float aspectRatio = _graphicsDevice.FramebufferSize.Width / (float)_graphicsDevice.FramebufferSize.Height;
        _effect.Projection = Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI / 4, // 45度
            aspectRatio,
            0.1f,         // 近クリップ
            100f          // 遠クリップ
        );
        
        // ワールド行列の更新（回転）
        var rotation = Matrix4x4.CreateRotationX(_rotationX) * 
                       Matrix4x4.CreateRotationY(_rotationY);
        _effect.World = rotation;
        
        // レンダーパスの開始
        using var renderPass = _graphicsDevice.BeginRenderPass(new RenderPassDescriptor
        {
            ClearColor = new Color(0.1f, 0.1f, 0.15f, 1), // 濃い青灰色
            ClearDepth = 1.0f
        });
        
        // マテリアルの適用
        _effect.Apply(renderPass);
        
        // バッファの設定
        renderPass.SetVertexBuffer(_vertexBuffer);
        renderPass.SetIndexBuffer(_indexBuffer);
        
        // 描画（インスタンシング使用）
        // BasicEffectでは1つのモデルでもインスタンシングを強制
        renderPass.DrawIndexed(
            indexCount: _cubeGeometry.IndexCount,
            instanceCount: 1,  // 1インスタンス
            firstIndex: 0,
            vertexOffset: 0
        );
        
        // レンダーパスの終了
        _graphicsDevice.Present();
    }
    
    public void Dispose()
    {
        _effect?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _whiteTexture?.Dispose();
        _cubeGeometry?.Dispose();
    }
}

/// <summary>
/// エントリーポイント（MAUIアプリ内で使用）
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // 将来的なMAUI統合例
        // var graphicsDevice = GraphicsDevice.CreateForCurrentView();
        // var game = new BasicEffectGame();
        // game.Initialize(graphicsDevice);
        
        // メインループ
        // while (running) {
        //     game.Update(deltaTime);
        //     game.Draw();
        // }
        
        Console.WriteLine("PlywoodGraphics BasicEffect Demo");
        Console.WriteLine("================================");
        Console.WriteLine("This sample demonstrates the ideal API usage");
        Console.WriteLine("for a MonoGame BasicEffect-compatible material.");
        Console.WriteLine();
        Console.WriteLine("Features:");
        Console.WriteLine("- Three.js-style geometry (BoxGeometry)");
        Console.WriteLine("- BasicEffect-compatible lighting");
        Console.WriteLine("- Forced hardware instancing");
        Console.WriteLine("- Index buffer usage");
        Console.WriteLine("- Single draw call per model");
    }
}
