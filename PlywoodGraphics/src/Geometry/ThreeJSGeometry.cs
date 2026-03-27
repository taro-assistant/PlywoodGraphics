namespace PlywoodGraphics.Geometry;

/// <summary>
/// Three.js風ジオメトリ - 頂点バッファとインデックスバッファを管理
/// </summary>
public abstract class Geometry : IDisposable
{
    public abstract int VertexCount { get; }
    public abstract int IndexCount { get; }
    public abstract ReadOnlyMemory<float> Positions { get; }
    public abstract ReadOnlyMemory<float> Normals { get; }
    public abstract ReadOnlyMemory<float< TexCoords { get; }
    public abstract ReadOnlyMemory<uint> Indices { get; }
    
    /// <summary>
    /// AABB（軸並行境界ボックス）
    /// </summary>
    public abstract BoundingBox Bounds { get; }
    
    public abstract void Dispose();
}

/// <summary>
/// バウンディングボックス
/// </summary>
public readonly record struct BoundingBox(Vector3 Min, Vector3 Max)
{
    public readonly Vector3 Center = new(
        (Min.X + Max.X) * 0.5f,
        (Min.Y + Max.Y) * 0.5f,
        (Min.Z + Max.Z) * 0.5f
    );
    
    public readonly Vector3 Size = new(
        Max.X - Min.X,
        Max.Y - Min.Y,
        Max.Z - Min.Z
    );
}

/// <summary>
/// Three.js風のVector3
/// </summary>
public readonly record struct Vector3(float X, float Y, float Z)
{
    public static readonly Vector3 Zero = new(0, 0, 0);
    public static readonly Vector3 One = new(1, 1, 1);
    public static readonly Vector3 Up = new(0, 1, 0);
    public static readonly Vector3 Right = new(1, 0, 0);
    public static readonly Vector3 Forward = new(0, 0, 1);
    
    public Vector3(float v) : this(v, v, v) { }
    
    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 v, float s) => new(v.X * s, v.Y * s, v.Z * s);
    public static Vector3 operator *(float s, Vector3 v) => v * s;
    public static Vector3 operator /(Vector3 v, float s) => new(v.X / s, v.Y / s, v.Z / s);
}

/// <summary>
/// Three.js風のVector2
/// </summary>
public readonly record struct Vector2(float X, float Y)
{
    public static readonly Vector2 Zero = new(0, 0);
    public static readonly Vector2 One = new(1, 1);
}

/// <summary>
/// BoxGeometry - 立方体
/// Three.js: new THREE.BoxGeometry(width, height, depth)
/// </summary>
public class BoxGeometry : Geometry
{
    private readonly float[] _positions;
    private readonly float[] _normals;
    private readonly float[] _texCoords;
    private readonly uint[] _indices;
    private readonly BoundingBox _bounds;
    
    public override int VertexCount => _positions.Length / 3;
    public override int IndexCount => _indices.Length;
    public override ReadOnlyMemory<float> Positions => _positions;
    public override ReadOnlyMemory<float> Normals => _normals;
    public override ReadOnlyMemory<float> TexCoords => _texCoords;
    public override ReadOnlyMemory<uint> Indices => _indices;
    public override BoundingBox Bounds => _bounds;
    
    public BoxGeometry(float width = 1, float height = 1, float depth = 1)
    {
        float w = width * 0.5f;
        float h = height * 0.5f;
        float d = depth * 0.5f;
        
        // 24頂点（6面 × 4頂点）
        _positions = new float[]
        {
            // 前面 (Z+)
            -w, -h,  d,   w, -h,  d,   w,  h,  d,  -w,  h,  d,
            // 後面 (Z-)
             w, -h, -d,  -w, -h, -d,  -w,  h, -d,   w,  h, -d,
            // 上面 (Y+)
            -w,  h,  d,   w,  h,  d,   w,  h, -d,  -w,  h, -d,
            // 下面 (Y-)
            -w, -h, -d,   w, -h, -d,   w, -h,  d,  -w, -h,  d,
            // 右面 (X+)
             w, -h,  d,   w, -h, -d,   w,  h, -d,   w,  h,  d,
            // 左面 (X-)
            -w, -h, -d,  -w, -h,  d,  -w,  h,  d,  -w,  h, -d
        };
        
        _normals = new float[]
        {
            // 前面
            0, 0, 1,   0, 0, 1,   0, 0, 1,   0, 0, 1,
            // 後面
            0, 0, -1,  0, 0, -1,  0, 0, -1,  0, 0, -1,
            // 上面
            0, 1, 0,   0, 1, 0,   0, 1, 0,   0, 1, 0,
            // 下面
            0, -1, 0,  0, -1, 0,  0, -1, 0,  0, -1, 0,
            // 右面
            1, 0, 0,   1, 0, 0,   1, 0, 0,   1, 0, 0,
            // 左面
            -1, 0, 0,  -1, 0, 0,  -1, 0, 0,  -1, 0, 0
        };
        
        _texCoords = new float[]
        {
            // 各面のUV（0-1範囲）
            0, 0,  1, 0,  1, 1,  0, 1,
            0, 0,  1, 0,  1, 1,  0, 1,
            0, 0,  1, 0,  1, 1,  0, 1,
            0, 0,  1, 0,  1, 1,  0, 1,
            0, 0,  1, 0,  1, 1,  0, 1,
            0, 0,  1, 0,  1, 1,  0, 1
        };
        
        _indices = new uint[]
        {
            // 各面の三角形（時計回り）
             0,  1,  2,   0,  2,  3,   // 前面
             4,  5,  6,   4,  6,  7,   // 後面
             8,  9, 10,   8, 10, 11,   // 上面
            12, 13, 14,  12, 14, 15,   // 下面
            16, 17, 18,  16, 18, 19,   // 右面
            20, 21, 22,  20, 22, 23    // 左面
        };
        
        _bounds = new BoundingBox(new Vector3(-w, -h, -d), new Vector3(w, h, d));
    }
    
    public override void Dispose()
    {
        // リソース解放はGPU側で行う
    }
}

/// <summary>
/// PlaneGeometry - 平面
/// Three.js: new THREE.PlaneGeometry(width, height, segmentsX, segmentsY)
/// </summary>
public class PlaneGeometry : Geometry
{
    private readonly float[] _positions;
    private readonly float[] _normals;
    private readonly float[] _texCoords;
    private readonly uint[] _indices;
    private readonly BoundingBox _bounds;
    
    public override int VertexCount => _positions.Length / 3;
    public override int IndexCount => _indices.Length;
    public override ReadOnlyMemory<float> Positions => _positions;
    public override ReadOnlyMemory<float> Normals => _normals;
    public override ReadOnlyMemory<float> TexCoords => _texCoords;
    public override ReadOnlyMemory<uint> Indices => _indices;
    public override BoundingBox Bounds => _bounds;
    
    public PlaneGeometry(float width = 1, float height = 1, int segmentsX = 1, int segmentsY = 1)
    {
        int vertexCountX = segmentsX + 1;
        int vertexCountY = segmentsY + 1;
        int totalVertices = vertexCountX * vertexCountY;
        int totalIndices = segmentsX * segmentsY * 6;
        
        _positions = new float[totalVertices * 3];
        _normals = new float[totalVertices * 3];
        _texCoords = new float[totalVertices * 2];
        _indices = new uint[totalIndices];
        
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        
        // 頂点生成
        for (int y = 0; y < vertexCountY; y++)
        {
            for (int x = 0; x < vertexCountX; x++)
            {
                float u = x / (float)segmentsX;
                float v = y / (float)segmentsY;
                
                int index = (y * vertexCountX + x) * 3;
                _positions[index + 0] = u * width - halfWidth;
                _positions[index + 1] = v * height - halfHeight;
                _positions[index + 2] = 0;
                
                _normals[index + 0] = 0;
                _normals[index + 1] = 0;
                _normals[index + 2] = 1;
                
                int uvIndex = (y * vertexCountX + x) * 2;
                _texCoords[uvIndex + 0] = u;
                _texCoords[uvIndex + 1] = v;
            }
        }
        
        // インデックス生成
        int idx = 0;
        for (int y = 0; y < segmentsY; y++)
        {
            for (int x = 0; x < segmentsX; x++)
            {
                uint tl = (uint)(y * vertexCountX + x);
                uint tr = tl + 1;
                uint bl = (uint)((y + 1) * vertexCountX + x);
                uint br = bl + 1;
                
                _indices[idx++] = tl;
                _indices[idx++] = bl;
                _indices[idx++] = tr;
                _indices[idx++] = tr;
                _indices[idx++] = bl;
                _indices[idx++] = br;
            }
        }
        
        _bounds = new BoundingBox(
            new Vector3(-halfWidth, -halfHeight, 0),
            new Vector3(halfWidth, halfHeight, 0)
        );
    }
    
    public override void Dispose()
    {
    }
}
