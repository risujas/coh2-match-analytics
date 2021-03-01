using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coh2Stats
{
	static class Logger
	{
		private const string logFile = "log.txt";

		public static void WriteLine(string text)
		{
			DateTime dt = DateTime.Now;

			string time = dt.ToLongTimeString();
			string message = time + " - " + text;

			Console.WriteLine(message);
			var file = File.AppendText(logFile);
			file.WriteLine(message);
			file.Close();
		}

		public static void WriteLine(string text, params object[] args)
		{
			DateTime dt = DateTime.Now;

			text = string.Format(text, args);
			string time = dt.ToLongTimeString();
			string message = time + " - " + text;

			Console.WriteLine(message);
			var file = File.AppendText(logFile);
			file.WriteLine(message);
			file.Close();
		}
	}
}
