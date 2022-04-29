using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sokoban {
	public static class Tools {
		public static void Serialize<T>(T dictionary, Stream stream) {
			try { // try to serialize the collection to a file
				using (stream) {
					// create BinaryFormatter
					var bin = new BinaryFormatter();
					// serialize the collection (EmployeeList1) to file (stream)
					bin.Serialize(stream, dictionary);
				}
			}
			catch (IOException e) {
				Console.Write(e);
			}
		}

		public static T Deserialize<T>(Stream stream) where T : new() {
			var ret = CreateInstance<T>();
			try {
				using (stream) {
					// create BinaryFormatter
					var bin = new BinaryFormatter();
					// deserialize the collection (Employee) from file (stream)
					ret = (T) bin.Deserialize(stream);
				}
			}
			catch (IOException e) {
				Console.Write(e);
			}
			return ret;
		}

		// function to create instance of T
		public static T CreateInstance<T>() where T : new() {
			return (T) Activator.CreateInstance(typeof(T));
		}
	}
}