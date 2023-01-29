using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace BrokenH.MGEngine.Utilities
{
	public static class FileManager
	{
		public static readonly string APP_DATA_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Recoil");

		public static bool FileExists(string relativePath, FileSource fileSource)
		{
			string path = fileSource switch
			{
				FileSource.AppData => Path.Combine(APP_DATA_FOLDER, relativePath),
				_ => relativePath
			};
			return File.Exists(path);
		}

		public static string LoadTextFile(string relativePath, FileSource fileSource)
		{
			try
			{
				switch (fileSource)
				{
					case FileSource.SharedProject:	return LoadFromSharedProject(relativePath);
					case FileSource.AppData:		return LoadFromAppData(relativePath);
					case FileSource.Content:		return LoadFromContent(relativePath);
					default:
						throw new NotImplementedException($"File source {fileSource} not implimented");
				}
			}
			catch (Exception e)
			{
				throw new FileNotFoundException($"Unable to load file with relative path: {relativePath}", e);
			}
		}
		// TODO: This is labeled incorrectly
		private static string LoadFromSharedProject(string relativePath)
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", relativePath);
			return File.ReadAllText(path);
		}
		private static string LoadFromAppData(string relativePath)
		{
			string path = Path.Combine(APP_DATA_FOLDER, relativePath);
			return File.ReadAllText(path);
		}
		private static string LoadFromContent(string relativePath)
		{
			string path = Path.Combine("Content", relativePath);
			using (var stream = TitleContainer.OpenStream(path))
			using (var reader = new StreamReader(stream))
			return reader.ReadToEnd();
		}

		public static void SaveTextFile(string relativePath, string content)
		{
			string path = Path.GetDirectoryName(relativePath) ?? "";
			string file = Path.GetFileName(relativePath);
			path = Path.Combine(APP_DATA_FOLDER, path);

			// If directory doesn't exist, create it
			System.IO.Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, file), content);
		}

		public static T LoadJson<T>(string relativePath, FileSource fileSource)
		{
			try
			{
				T? result;
				result = JsonConvert.DeserializeObject<T>(LoadTextFile(relativePath, fileSource));
				_ = result ?? throw new InvalidDataException("JsonConvert.DeserializeObject returned a null.");
				return result;
			}
			catch (Exception e)
			{
				throw new InvalidDataException("Failed to deserialize file.", e);
			}
		}
		public static void SaveJson(string relativePath, object value)
		{
			string json = JsonConvert.SerializeObject(value, Formatting.Indented);
			SaveTextFile(relativePath, json);
		}

		public static Texture2D LoadImage(string relativePath, FileSource fileSource, GraphicsDevice device)
		{
			switch (fileSource)
			{
				case FileSource.SharedProject:
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", relativePath);
					return Texture2D.FromFile(device, path);
				default: throw new NotImplementedException($"File source {fileSource} not implimented for {nameof(LoadImage)}");
			}
		}
	}

	public enum FileSource
	{
		AppData,
		Content,
		SharedProject,
	}
}