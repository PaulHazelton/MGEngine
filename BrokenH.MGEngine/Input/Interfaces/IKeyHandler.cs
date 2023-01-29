using Microsoft.Xna.Framework.Input;

namespace BrokenH.MGEngine.Input.Interfaces
{
	public interface IKeyHandler
	{
		void KeyEvent(Keys key, ButtonState eventType);
	}
}