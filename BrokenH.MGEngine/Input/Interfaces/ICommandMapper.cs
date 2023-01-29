using Microsoft.Xna.Framework.Input;

namespace BrokenH.MGEngine.Input.Interfaces
{
	public interface ICommandMapper
	{
		(Keys, Keys) GetKeys(int command);
	}
}