using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Ferr {
	public enum DataStringType {
		Ordered,
		Named
	}
	
	public class DataStringWriterUtil {
		DataStringType _type;
		StringBuilder  _builder;
		char           _separator;

		public DataStringWriterUtil(DataStringType aType, char aSeparator='|') {
			_type      = aType;
			_separator = aSeparator;
			_builder   = new StringBuilder();
		}

		public void Int(int aData) {
			String(aData.ToString());
		}
		public void Int(string aName, int aData) {
			String(aName, aData.ToString());
		}
		public void Bool(bool aData) {
			String(aData.ToString());
		}
		public void Bool(string aName, bool aData) {
			String(aName, aData.ToString());
		}
		public void Float(float aData) {
			String(aData.ToString());
		}
		public void Float(string aName, float aData) {
			String(aName, aData.ToString());
		}
		public void Data(IToFromDataString aData) {
			if (aData == null)
				String("null");
			else
				String(aData.GetType().Name+":"+aData.ToDataString());
		}
		public void Data(string aName, IToFromDataString aData) {
			if (aData == null)
				String(aName, "null");
			else
				String(aName, aData.GetType().Name+":"+aData.ToDataString());
		}
		public void String(string aData) {
			if (_type == DataStringType.Named)
				throw new System.Exception("Need a name for a named list!");

			if (_builder.Length > 0)
				_builder.Append(_separator);
			_builder.Append(aData);
		}
		public void String(string aName, string aData) {
			if (_type == DataStringType.Ordered)
				throw new System.Exception("Name doesn't apply for ordered lists!");

			if (_builder.Length > 0)
				_builder.Append(_separator);
			_builder.Append(aName.Replace("=", "&eq;"));
			_builder.Append("=");
			_builder.Append(aData);
		}

		public override string ToString() {
			return _builder.ToString();
		}
	}

	public class DataStringReaderUtil {
		DataStringType _type;
		char           _separator;
		string[]       _words;
		string[]       _names;
		int            _curr = 0;

		public int NameCount { get { return _names.Length; } }

		public DataStringReaderUtil(string aData, DataStringType aType, char aSeparator = '|') {
			_type  = aType;
			_words = aData.Split(aSeparator);
			
			if (_type == DataStringType.Named) {
				_names = new string[_words.Length];

				for (int i = 0; i < _words.Length; i++) {
					int    sep  = _words[i].IndexOf('=');
					string name = _words[i].Substring(0, sep);
					string data = _words[i].Substring(sep+1);

					_words[i] = data;
					_names[i] = name;
				}
			}
		}

		public string GetName(int aIndex) {
			return _names[aIndex];
		}

		public int Int() {
			return int.Parse(Read());
		}
		public int Int(string aName) {
			return int.Parse(Read(aName));
		}
		public bool Bool() {
			return bool.Parse(Read());
		}
		public bool Bool(string aName) {
			return bool.Parse(Read(aName));
		}
		public float Float() {
			return float.Parse(Read());
		}
		public float Float(string aName) {
			return float.Parse(Read(aName));
		}
		public string String() {
			return Read();
		}
		public string String(string aName) {
			return Read(aName);
		}
		public object Data() {
			return CreateObject(Read());
		}
		public object Data(string aName) {
			return CreateObject(Read(aName));
		}

		private string Read(string aName) {
			if (_type == DataStringType.Ordered)
				throw new System.Exception("Can't do a named read from an ordered list!");

			aName = aName.Replace("=", "&eq;");

			int index = Array.IndexOf(_names, aName);
			if (index == -1)
				throw new System.Exception("Can't find data from given name!");

			return _words[index];
		}
		private string Read() {
			if (_type == DataStringType.Named)
				throw new System.Exception("Can't do an ordered read from a named list!");
			if (_curr >= _words.Length)
				throw new System.Exception("Reading past the end of an ordered data string!");

			string result = _words[_curr];
			_curr += 1;

			return result;
		}

		private object CreateObject(string aDataString) {
			if (aDataString == null)
				return null;

			int    sep      = aDataString.IndexOf(':');
			string typeName = aDataString.Substring(0, sep);
			string data     = aDataString.Substring(sep+1);
			Type t = Type.GetType(typeName);
			object result = null;
			if (typeof(IToFromDataString).IsAssignableFrom(t)) {
				if (typeof(ScriptableObject).IsAssignableFrom(t)) {
					result = ScriptableObject.CreateInstance(t);
				} else {
					result = Activator.CreateInstance(t);
				}
				((IToFromDataString)result).FromDataString(data);
			}
			return result;
		}
	}

	public static class DataStringUtil {
		static string _key = "FerrDataStringUtilDefaultKey";

		public static string Encrypt(string aData, string aKey = null) {
			if (string.IsNullOrEmpty(aKey))
				aKey = _key;

			byte[] clearBytes = Encoding.Unicode.GetBytes(aData);
			using (Aes encryptor = Aes.Create()) {
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(aKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV  = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream()) {
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) {
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					aData = Convert.ToBase64String(ms.ToArray());
				}
			}
			return aData;
		}
		public static string Decrypt(string aData, string aKey = null) {
			if (string.IsNullOrEmpty(aKey))
				aKey = _key;

			aData = aData.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(aData);
			using (Aes encryptor = Aes.Create()) {
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(aKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV  = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream()) {
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)) {
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					aData = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return aData;
		}
	}
}