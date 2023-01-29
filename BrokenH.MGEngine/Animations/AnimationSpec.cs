using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Animations
{
	public class AnimationSpec
	{
		/// <summary>
		/// Duration of the animation in seconds
		/// </summary>
		public float Duration { get; set; }
		public bool IsLooping { get; set; }
		public int FrameWidth { get; set; }
		public int FrameHeight { get; set; }
		public Vector2 Origin { get; set; }
		public bool? RandomTimeOffset { get; set; }
	}
}