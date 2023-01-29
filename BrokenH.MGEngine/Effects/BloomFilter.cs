using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrokenH.MGEngine.Effects
{
	public class BloomFilter : IDisposable
	{
		public enum BloomPreset
		{
			Wide,
			Focussed,
			Small,
			SuperWide,
			Cheap,
			One
		};

		#region Private fields

		//resolution
		private int _width;
		private int _height;

		//RenderTargets
		private RenderTargetBinding[]? _oldRenderTargets = null;
		private RenderTarget2D[] _mips;
		private RenderTarget2D _finalRenderTarget;

		private SurfaceFormat _renderTargetFormat;

		//Objects
		private GraphicsDevice _graphicsDevice;
		private QuadRenderer _quadRenderer;

		//Shader + variables
		private Effect _bloomEffect;

		private EffectPass _passPrefilter;
		private EffectPass _passDownsample;
		private EffectPass _passUpsample;
		private EffectPass _passBlend;

		private EffectParameter _paramOriginalTexture;
		private EffectParameter _paramPreviousMip;
		private EffectParameter _paramInverseResolution;
		private EffectParameter _paramRadius;
		private EffectParameter _paramStrength;
		private EffectParameter _paramStreakLength;
		private EffectParameter _paramThreshold;

		//Preset variables for different mip levels
		private float[] _radii;
		private float[] _strengths;

		#endregion

		// Shader Parameters
		private Texture2D OriginalTexture { set => _paramOriginalTexture.SetValue(value); }
		private Texture2D PreviousMip { set => _paramPreviousMip.SetValue(value); }

		private Vector2 _inverseResolution;
		private Vector2 InverseResolution
		{
			get => _inverseResolution;
			set
			{
				if (_inverseResolution == value)
					return;
				_inverseResolution = value;
				_paramInverseResolution.SetValue(_inverseResolution);
			}
		}

		private float _radius;
		private float _radiusScalingCorrection = 1.0f;
		private float Radius
		{
			get => _radius;
			set
			{
				_radius = value;
				_paramRadius.SetValue(_radius * _radiusScalingCorrection * RadiusMultiplier);
			}
		}

		private float _strength;
		private float Strength
		{
			get => _strength;
			set
			{
				_strength = value;
				_paramStrength.SetValue(_strength * StrengthMultiplier);
			}
		}

		// Public Properties
		private BloomPreset _preset;
		public BloomPreset Preset
		{
			get => _preset;
			set
			{
				if (_preset == value)
					return;
				_preset = value;
				SetBloomPreset(_preset);
			}
		}

		private int _numberOfPasses = 5;
		public int NumberOfPasses
		{
			get => _numberOfPasses;
			set => _numberOfPasses = Math.Clamp(value, 1, 5);
		}

		private float _threshold;
		public float Threshold
		{
			get => _threshold;
			set
			{
				_threshold = value;
				_paramThreshold.SetValue(_threshold);
			}
		}

		private float _streakLength;
		public float StreakLength
		{
			get => _streakLength;
			set
			{
				_streakLength = value;
				_paramStreakLength.SetValue(_streakLength);
			}
		}

		public float StrengthMultiplier { get; set; } = 1.0f;
		public float RadiusMultiplier { get; set; } = 1.0f;


		/// <summary>
		/// Loads all needed components for the BloomEffect. This effect won't work without calling load
		/// </summary>
		/// <param name="graphicsDevice"></param>
		/// <param name="content"></param>
		/// <param name="width">initial value for creating the rendertargets</param>
		/// <param name="height">initial value for creating the rendertargets</param>
		/// <param name="renderTargetFormat">The intended format for the rendertargets. For normal, non-hdr, applications color or rgba1010102 are fine NOTE: For OpenGL, SurfaceFormat.Color is recommended for non-HDR applications.</param>
		/// <param name="quadRenderer">if you already have quadRenderer you may reuse it here</param>
		public BloomFilter(GraphicsDevice graphicsDevice, ContentManager content, int width, int height, SurfaceFormat renderTargetFormat = SurfaceFormat.Color)
		{
			_mips = new RenderTarget2D[6];
			_radii = new float[5];
			_strengths = new float[5];

			_graphicsDevice = graphicsDevice;
			_renderTargetFormat = renderTargetFormat;
			_quadRenderer = new QuadRenderer(graphicsDevice);
			UpdateResolution(width, height);

			//Load the shader and parameters and passes for cheap and easy access
			_bloomEffect = content.Load<Effect>("Shaders/BloomFilter/Bloom");

			_paramInverseResolution = _bloomEffect.Parameters["InverseResolution"];
			_paramRadius = _bloomEffect.Parameters["Radius"];
			_paramStrength = _bloomEffect.Parameters["Strength"];
			_paramStreakLength = _bloomEffect.Parameters["StreakLength"];
			_paramThreshold = _bloomEffect.Parameters["Threshold"];

			_passPrefilter = _bloomEffect.Techniques["Prefilter"].Passes[0];
			_passDownsample = _bloomEffect.Techniques["Downsample"].Passes[0];
			_passUpsample = _bloomEffect.Techniques["Upsample"].Passes[0];
			_passBlend = _bloomEffect.Techniques["Blend"].Passes[0];

			// For DirectX / Windows
			// If we are on OpenGL it's different, load the other one then!
			_paramOriginalTexture = _bloomEffect.Parameters["OriginalTexture"] ?? _bloomEffect.Parameters["LinearSampler+OriginalTexture"];
			_paramPreviousMip = _bloomEffect.Parameters["PreviousMip"] ?? _bloomEffect.Parameters["LinearSampler+PreviousMip"];

			//Setup the default preset values.
			Threshold = 0.8f;
			Preset = BloomPreset.Focussed;
		}

		private void SetBloomPreset(BloomPreset preset)
		{
			StreakLength = 1;
			switch (preset)
			{
				case BloomPreset.Wide:
					{
						_strengths[0] = 0.5f;
						_strengths[1] = 1;
						_strengths[2] = 2;
						_strengths[3] = 1;
						_strengths[4] = 2;
						_radii[4] = 4.0f;
						_radii[3] = 4.0f;
						_radii[2] = 2.0f;
						_radii[1] = 2.0f;
						_radii[0] = 1.0f;
						NumberOfPasses = 5;
						break;
					}
				case BloomPreset.SuperWide:
					{
						_strengths[0] = 0.9f;
						_strengths[1] = 1;
						_strengths[2] = 1;
						_strengths[3] = 2;
						_strengths[4] = 6;
						_radii[4] = 4.0f;
						_radii[3] = 2.0f;
						_radii[2] = 2.0f;
						_radii[1] = 2.0f;
						_radii[0] = 2.0f;
						NumberOfPasses = 5;
						break;
					}
				case BloomPreset.Focussed:
					{
						_strengths[0] = 0.8f;
						_strengths[1] = 1;
						_strengths[2] = 1;
						_strengths[3] = 1;
						_strengths[4] = 2;
						_radii[4] = 4.0f;
						_radii[3] = 2.0f;
						_radii[2] = 2.0f;
						_radii[1] = 2.0f;
						_radii[0] = 2.0f;
						NumberOfPasses = 5;
						break;
					}
				case BloomPreset.Small:
					{
						_strengths[0] = 0.8f;
						_strengths[1] = 1;
						_strengths[2] = 1;
						_strengths[3] = 1;
						_strengths[4] = 1;
						_radii[4] = 1;
						_radii[3] = 1;
						_radii[2] = 1;
						_radii[1] = 1;
						_radii[0] = 1;
						NumberOfPasses = 5;
						break;
					}
				case BloomPreset.Cheap:
					{
						_strengths[0] = 0.8f;
						_strengths[1] = 2;
						_radii[1] = 2;
						_radii[0] = 2;
						NumberOfPasses = 2;
						break;
					}
				case BloomPreset.One:
					{
						_strengths[0] = 4f;
						_strengths[1] = 1;
						_strengths[2] = 1;
						_strengths[3] = 1;
						_strengths[4] = 2;
						_radii[4] = 1.0f;
						_radii[3] = 1.0f;
						_radii[2] = 1.0f;
						_radii[1] = 1.0f;
						_radii[0] = 1.0f;
						NumberOfPasses = 5;
						break;
					}
			}
		}

		/// <summary>
		/// Main draw function
		/// </summary>
		/// <param name="inputTexture">the image from which we want to extract bright parts and blur these</param>
		public Texture2D Draw(Texture2D inputTexture, bool blendWithOriginal = true) => Draw(inputTexture, _width, _height, blendWithOriginal);

		/// <summary>
		/// Main draw function
		/// </summary>
		/// <param name="inputTexture">the image from which we want to extract bright parts and blur these</param>
		/// <param name="width">width of our target. If different to the input.Texture width our final texture will be smaller/larger.
		/// For example we can use half resolution. Input: 1280px wide -> width = 640px
		/// The smaller this value the better performance and the worse our final image quality</param>
		/// <param name="height">see: width</param>
		public Texture2D Draw(Texture2D inputTexture, int width, int height, bool blendWithOriginal = true)
		{
			// Store old render target to avoid sideaffects
			_oldRenderTargets = _graphicsDevice.GetRenderTargets();

			// Check if we are initialized
			if (_graphicsDevice == null)
				throw new Exception("Module not yet Loaded / Initialized. Use Load() first");

			// Change renderTarget resolution if different from what we expected. If lower than the inputTexture we gain performance.
			if (width != _width || height != _height)
			{
				UpdateResolution(width, height);

				// Adjust the blur so it looks consistent across diferrent scalings
				_radiusScalingCorrection = (float)width / inputTexture.Width;
			}

			// Set graphics device state
			_graphicsDevice.RasterizerState = RasterizerState.CullNone;
			_graphicsDevice.BlendState = BlendState.Opaque;
			_graphicsDevice.SamplerStates[1] = new SamplerState {
				BorderColor = Color.Black,
				AddressU = TextureAddressMode.Border,
				AddressV = TextureAddressMode.Border,
				Filter = TextureFilter.Linear
			};

			// Reset InverseResolution
			InverseResolution = new Vector2(1.0f / _width, 1.0f / _height);

			OriginalTexture = inputTexture;

			Extract(inputTexture);
			SamplePass();

			if (blendWithOriginal)
			{
				Blend(inputTexture);

				// Restore old render targets
				_graphicsDevice.SetRenderTargets(_oldRenderTargets);

				return _finalRenderTarget;
			}
			else
			{
				// Restore old render targets
				_graphicsDevice.SetRenderTargets(_oldRenderTargets);

				return _mips[0];
			}
		}

		private void Extract(Texture2D inputTexture)
		{
			// We extract the bright values which are above the Threshold and save them to Mip0
			PreviousMip = inputTexture;	// Note: Is setRenderTargets better?
			_graphicsDevice.SetRenderTarget(_mips[0]);

			_passPrefilter.Apply();
			_quadRenderer.RenderQuad(_graphicsDevice);
		}

		private void SamplePass(int pass = 1)
		{
			if (pass > NumberOfPasses)
			{
				// Prepare for upsampling
				_graphicsDevice.BlendState = BlendState.Additive;
				return;
			}

			// Downsample to mip n+1
			PreviousMip = _mips[pass - 1];
			_graphicsDevice.SetRenderTarget(_mips[pass]);

			// Pass
			_passDownsample.Apply();
			_quadRenderer.RenderQuad(_graphicsDevice);

			InverseResolution *= 2;

			// Do next pass
			SamplePass(pass + 1);

			// Upsample to mip n
			InverseResolution /= 2;

			PreviousMip = _mips[pass];
			_graphicsDevice.SetRenderTarget(_mips[pass - 1]);

			// Adjust strength and radius for each upsample
			Strength = _strengths[pass - 1];
			Radius = _radii[pass - 1];

			_passUpsample.Apply();
			_quadRenderer.RenderQuad(_graphicsDevice);
		}

		private void Blend(Texture2D originalTexture)
		{
			OriginalTexture = originalTexture;
			PreviousMip = _mips[0];
			_graphicsDevice.SetRenderTarget(_finalRenderTarget);

			_passBlend.Apply();
			_quadRenderer.RenderQuad(_graphicsDevice);
		}

		/// <summary>
		/// Update the InverseResolution of the used rendertargets. This should be the InverseResolution of the processed image
		/// We use SurfaceFormat.Color, but you can use higher precision buffers obviously.
		/// </summary>
		/// <param name="width">width of the image</param>
		/// <param name="height">height of the image</param>
		[MemberNotNull(nameof(_finalRenderTarget))]
		public void UpdateResolution(int width, int height)
		{
			_width = width;
			_height = height;

			_finalRenderTarget = new RenderTarget2D(_graphicsDevice,
				(int)(width),
				(int)(height), false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

			int scale = 1;
			for (int i = 0; i < _mips.Length; i++)
			{
				_mips[i] = new RenderTarget2D(_graphicsDevice,
					(int)(width / scale),
					(int)(height / scale), false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				scale <<= 1;	// Multiply by 2 each time (each mip is half the size of the last in each dimension)
			}
		}

		/// <summary>
		/// Dispose our RenderTargets. This is not covered by the Garbage Collector so we have to do it manually
		/// </summary>
		public void Dispose()
		{
			for (int i = 0; i < _mips.Length; i++)
				_mips[i].Dispose();

			_quadRenderer.Dispose();
		}

		/// <summary>
		/// Renders a simple quad to the screen. Uncomment the Vertex / Index buffers to make it a static fullscreen quad.
		/// The performance effect is barely measurable though and you need to dispose of the buffers when finished!
		/// </summary>
		private class QuadRenderer : IDisposable
		{
			//buffers for rendering the quad
			private readonly VertexPositionTexture[] _vertexBuffer;
			private readonly short[] _indexBuffer;

			private VertexBuffer _vBuffer;
			private IndexBuffer _iBuffer;

			public QuadRenderer(GraphicsDevice graphicsDevice)
			{
				_vertexBuffer = new VertexPositionTexture[4];

				// |2|3|
				// |0|1|
				_vertexBuffer[0] = new VertexPositionTexture(new Vector3(-1,  1, 1), new Vector2(0, 0));
				_vertexBuffer[1] = new VertexPositionTexture(new Vector3( 1,  1, 1), new Vector2(1, 0));
				_vertexBuffer[2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
				_vertexBuffer[3] = new VertexPositionTexture(new Vector3( 1, -1, 1), new Vector2(1, 1));

				_indexBuffer = new short[] { 0, 3, 2, 0, 1, 3 };

				_vBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
				_iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

				_vBuffer.SetData(_vertexBuffer);
				_iBuffer.SetData(_indexBuffer);

				graphicsDevice.SetVertexBuffer(_vBuffer);
				graphicsDevice.Indices = _iBuffer;
			}

			public void RenderQuad(GraphicsDevice graphicsDevice)
			{
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
			}

			public void Dispose()
			{
				_vBuffer.Dispose();
				_iBuffer.Dispose();
			}
		}
	}
}