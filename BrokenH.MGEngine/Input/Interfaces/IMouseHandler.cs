using Microsoft.Xna.Framework.Input;

namespace BrokenH.MGEngine.Input.Interfaces
{
	public interface IMouseHandler
	{
		void MouseEvent(MouseButton button, ButtonState state);
	}
}