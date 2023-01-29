using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrokenH.MGEngine.Utilities
{
	/// <summary>
	/// Holds a list of strings to draw to the screen
	/// Usefull for debugging
	/// </summary>
	public class TextPanel
	{
		private SpriteFont _font;
		private Color _textColor;
		private Vector2 _position;
		private List<string> _lines;

		/// <param name="position">The top left corner of the panel</param>
		public TextPanel(SpriteFont font, Vector2 position)
		{
			this._font = font;
			this._position = position;

			_textColor = Color.White;

			_lines = new List<string>();
		}

		public void AddLine(string line) => this._lines.Add(line);
		public void ClearLines() => this._lines.Clear();

		public void Draw(SpriteBatch spriteBatch)
		{
			int i = 0;
			foreach (string line in _lines)
			{
				spriteBatch.DrawString(_font, line, _position + new Vector2(0, i * _font.LineSpacing), _textColor);
				i++;
			}
		}
	}
}