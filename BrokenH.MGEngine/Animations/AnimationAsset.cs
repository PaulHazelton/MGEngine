using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrokenH.MGEngine.Animations
{
	public class AnimationAsset
	{
		public Texture2D Texture { get; private set; }
		public AnimationSpec Spec { get; private set; }

		public AnimationAsset(Texture2D texture, AnimationSpec spec)
		{
			Texture = texture;
			Spec = spec;
		}
		public AnimationAsset(string path, ContentManager contentManager, Func<string, AnimationSpec> fileLoader)
		{
			Texture = contentManager.Load<Texture2D>(path);
			Spec = fileLoader(path + ".json");
		}
	}
}