using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FileDataHandler
{
	private readonly string _dataDirPath;
	private readonly string _dataFileName;

	// --- 1. ВЛОЖЕННЫЕ КОНВЕРТЕРЫ ---
	// Они живут ВНУТРИ этого файла, создавать отдельные скрипты не нужно.

	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x"); writer.WriteValue(value.x);
			writer.WritePropertyName("y"); writer.WriteValue(value.y);
			writer.WritePropertyName("z"); writer.WriteValue(value.z);
			writer.WriteEndObject();
		}

		public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var json = JObject.Load(reader); // Читаем объект { "x": ..., "y": ... }
			return new Vector3(json["x"].ToObject<float>(),
							   json["y"].ToObject<float>(),
							   json["z"].ToObject<float>());
		}
	}

	public class QuaternionConverter : JsonConverter<Quaternion>
	{
		public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x"); writer.WriteValue(value.x);
			writer.WritePropertyName("y"); writer.WriteValue(value.y);
			writer.WritePropertyName("z"); writer.WriteValue(value.z);
			writer.WritePropertyName("w"); writer.WriteValue(value.w);
			writer.WriteEndObject();
		}

		public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var json = JObject.Load(reader);
			return new Quaternion(json["x"].ToObject<float>(),
								  json["y"].ToObject<float>(),
								  json["z"].ToObject<float>(),
								  json["w"].ToObject<float>());
		}
	}

	// --- 2. НАСТРОЙКИ СЕРИАЛИЗАЦИИ ---
	private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Auto,
		Formatting = Formatting.Indented,
		NullValueHandling = NullValueHandling.Ignore,

		Converters = new List<JsonConverter>
		{
			new Newtonsoft.Json.Converters.StringEnumConverter(),
            // Добавляем наши локальные конвертеры сюда
            new FileDataHandler.Vector3Converter(),
			new FileDataHandler.QuaternionConverter()
		},

		Error = (sender, args) =>
		{
			if (args.ErrorContext.Error.GetType().Name.Contains("JsonSerializationException"))
			{
				args.ErrorContext.Handled = true;
			}
		}
	};

	// --- 3. КОНСТРУКТОР И МЕТОДЫ (остались прежними) ---
	public FileDataHandler(string dataDirPath, string dataFileName)
	{
		_dataDirPath = dataDirPath;
		_dataFileName = dataFileName;
	}

	public GameData Load()
	{
		string fullPath = Path.Combine(_dataDirPath, _dataFileName);
		if (!File.Exists(fullPath)) return null;

		try
		{
			string dataToLoad = File.ReadAllText(fullPath);
			return JsonConvert.DeserializeObject<GameData>(dataToLoad, _settings);
		}
		catch (Exception e)
		{
			Debug.LogError("Loading error: " + fullPath + "\n" + e);
			return null;
		}
	}

	public void Save(GameData data)
	{
		string fullPath = Path.Combine(_dataDirPath, _dataFileName);
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

			// ВАЖНО: Используем наш settings со сконфигурированными конвертерами
			string dataToStore = JsonConvert.SerializeObject(data, _settings);
			File.WriteAllText(fullPath, dataToStore);
		}
		catch (Exception e)
		{
			Debug.LogError("Saving error: " + fullPath + "\n" + e);
		}
	}

	public GameData LoadFromFile(string fileName)
	{
		string fullPath = Path.Combine(_dataDirPath, fileName);
		if (!File.Exists(fullPath)) return null;

		try
		{
			string dataToLoad = File.ReadAllText(fullPath);
			return JsonConvert.DeserializeObject<GameData>(dataToLoad, _settings);
		}
		catch (Exception e)
		{
			Debug.LogError("Loading error: " + fullPath + "\n" + e);
			return null;
		}
	}
}