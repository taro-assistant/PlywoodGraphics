# PlywoodGraphics

.NET 10 MAUI向けクロスプラットフォームグラフィックスライブラリ

## 概要

PlywoodGraphicsはiOSとAndroidに特化した抽象化グラフィックスライブラリです。MetalとOpenGL ES 3.2を抽象化し、将来のVulkanサポートも見据えた設計になっています。

## コンセプト

### シンプルさの追求
- **ドローコール統一**: 1種類のドローコールに限定
- **ハードウェアインスタンシング強制**: どんなに小さなモデルでも
- **インデックスバッファ強制**: 常に使用
- **テクスチャ強制**: 頂点カラーは廃止、単色テクスチャを動的生成

これにより学習コストが劇的に削減されます。

### C#サブセットシェーダー
- C#サブセットでプログラマブルシェーダーを記述
- CPU実行モードでデバッグ可能（シミュレータ）
- リリース時は各プラットフォームのネイティブ形式に事前変換
- zipファイル格納（ディレクトリ名: `metal3`, `gles32` など）

### データ構造の動的変換
- C#構造体を実行時に各プラットフォームの要求形式に変換
- Metal/OpenGL ES: 事前決定可能
- Vulkan: プラットフォームごとのメモリアライメントに対応（将来）

## サポートAPI

| API | バージョン | プラットフォーム | 状態 |
|-----|-----------|-----------------|------|
| Metal | 3+ | iOS | 計画中 |
| OpenGL ES | 3.2 | Android | 計画中 |
| Vulkan | 1.3+ | Android（将来） | 予定 |

## ジオメトリ

Three.jsに似たジオメトリを標準でサポート：

```csharp
var geometry = new PlywoodGeometry.BoxGeometry(1, 1, 1);
var mesh = new PlywoodMesh(geometry, material);
```

将来的に物理演算エンジンとの連携も見据えています。

## 物理演算（将来）

Jitter Physicsを参考に、シングルスレッド性能を追求した物理演算エンジンを予定。

## プロジェクト構造

```
PlywoodGraphics/
├── src/
│   ├── Core/
│   ├── Shaders/
│   ├── Platforms/
│   │   ├── Metal/
│   │   └── OpenGLES/
│   └── Geometry/
├── samples/
└── tests/
```

## ライセンス

MIT License

## 関連プロジェクト

- [Jitter Physics](https://github.com/notgiven688/jitterphysics) - 参考にする物理エンジン
