using System.Linq;

namespace ContactManagerApplication.Utilities
{
	public static class StringExtensions
	{
		/// <summary>
		/// Used for cleaning the phone number from unnecessary symbols
		/// </summary>
		/// <param name="str">Original string</param>
		/// <example>RemoveAllNonDigitSymbols("+380 (12) 3456789") -> "380123456789"</example>
		public static string RemoveAllNonDigitSymbols(this string str)
		{
			return new string(str.Where(char.IsDigit).ToArray());
		}
	}
}
