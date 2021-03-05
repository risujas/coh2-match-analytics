﻿using System;
using System.IO;

namespace Coh2Stats
{
	internal static class UserIO
	{
		private const string logFile = "log.txt";

		public static void WriteLogLine(string text, params object[] args)
		{
			if (args.Length > 0)
			{
				text = string.Format(text, args);
			}

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] >>\t" + text;

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(message);
			Console.ResetColor();

			using (StreamWriter file = File.AppendText(logFile))
			{
				file.WriteLine(message);
			}
		}

		public static void PrintUIPrompt(string text, params object[] args)
		{
			if (args.Length > 0)
			{
				text = string.Format(text, args);
			}

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] >>\t" + text;

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public static int RunIntegerSelection(int inclusiveMin, int inclusiveMax)
		{
			int selection = 0;

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] <<\t";

			Console.ForegroundColor = ConsoleColor.DarkYellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = int.TryParse(input, out int result);
				if (!goodParse)
				{
					continue;
				}

				if (result < inclusiveMin || result > inclusiveMax)
				{
					continue;
				}

				selection = result;
				good = true;
			}

			Console.ResetColor();

			return selection;
		}

		public static char RunCharSelection(params char[] options)
		{
			char selection = ' ';

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] <<\t";

			Console.ForegroundColor = ConsoleColor.DarkYellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = char.TryParse(input, out char result);
				if (!goodParse)
				{
					continue;
				}

				result = char.ToLower(result);

				bool matchFound = false;
				for (int i = 0; i < options.Length; i++)
				{
					if (result == char.ToLower(options[i]))
					{
						matchFound = true;
						break;
					}
				}
				if (!matchFound)
				{
					continue;
				}

				selection = result;
				good = true;
			}

			Console.ResetColor();

			return selection;
		}

		public static double RunFloatingPointInput()
		{
			double floatInput = 0;

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] <<\t";

			Console.ForegroundColor = ConsoleColor.DarkYellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = double.TryParse(input, out double result);
				if (!goodParse)
				{
					continue;
				}

				floatInput = result;
				good = true;
			}

			Console.ResetColor();

			return floatInput;
		}

		public static string RunStringInput()
		{
			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] <<\t";

			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write(message);
			string input = Console.ReadLine();
			Console.ResetColor();

			return input;
		}
	}
}