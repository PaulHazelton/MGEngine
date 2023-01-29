using Microsoft.Xna.Framework.Input;

namespace BrokenH.MGEngine.Input.Interfaces
{
	public interface ICommandHandler
	{
		void CommandEvent(int command, ButtonState eventType);
	}
}