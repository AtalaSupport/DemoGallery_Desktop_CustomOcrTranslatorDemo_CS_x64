using System;
using System.IO;
using Atalasoft.Ocr;

namespace PigLatin
{
	/// <summary>
	/// The PigLatinTranslator class shows how to write a custom translator.  The point is to extend the 
	/// IForeignTranslator class and override the appropiate methods and variables.
	/// </summary>
	public class PigLatinTranslator : IForeignTranslator
	{
		// this is the list of mime-types that will be handled by this translator.
		// this translator only translates to one type of text, pig latin.
		static string[] _supported = { "text/pig-latin" };
		static string _vowels = "aeiouAEIOU";
		public PigLatinTranslator()
		{
		}
		#region xxIForeignTranslator Members
		public object Prepare(OcrEngine engine, OcrDocument doc)
		{
			return null;
		}
		public void Finish(OcrEngine engine, OcrDocument doc, bool successful, object translationObject)
		{
		}

		// this method is called from within the OcrEngine class.
		public void Translate(OcrEngine engine, OcrDocument doc, string mimeType, System.IO.Stream outStream, object translationObject)
		{
			StreamWriter writer = new StreamWriter(outStream);
			if (!Supports(mimeType)) {
				throw new NotImplementedException("Unknown output format: " + mimeType);
			}
			// traverse the OcrDocument so we can get to the text.
			foreach (OcrPage page in doc.Pages) 
			{
				foreach (OcrRegion region in page.Regions) 
				{
					if (region is OcrTextRegion) 
					{
						OcrTextRegion textRegion = (OcrTextRegion)region;
						foreach (OcrLine line in textRegion.Lines) 
						{
							foreach (OcrWord word in line.Words) 
							{
								// this is where we access each individual word, and we change it into its pig-latin form.
								string igpay = Igpay(word.Text);
								// write the translated word into the output stream.
								writer.Write(igpay + " ");
							}
						}
						// lets seperate the lines with 2 line breaks.
						writer.WriteLine("");
						writer.WriteLine("");
					}
				}
			}
			// we're done!
			writer.Flush();
		}

		private bool IsVowel(char c)
		{
			return _vowels.IndexOf(c) >= 0;
		}

		private int FindFirstVowel(string s)
		{
			for (int i=0; i < s.Length; i++) 
			{
				if (IsVowel(s[i])) 
				{
					return i;
				}
			}
			return -1;
		}

		private string Igpay(string word) 
		{
			char[] output = new char[word.Length + 2];
			bool toCaps = char.IsUpper(word[0]);

			// starts with a vowell, add on "ay"
			if (IsVowel(word[0])) 
			{
				int i;
				for (i=0; i < word.Length; i++) 
				{
					output[i] = word[i];
				}
				output[i] = 'a'; output[i+1] = 'y';
			}
			else 
			{
				// starts with a consonant, find the first vowell
				int firstVowel, j, k;
				firstVowel = FindFirstVowel(word);

				// no vowell?  Don't translate
				if (firstVowel == -1)
					return word;

				// copy everything from the first vowell on
				for (j=firstVowel; j < word.Length; j++) 
				{
					output[j-firstVowel] = word[j];
				}

				// copy everything up to the first vowell onto the end
				for (k=0; k < firstVowel; k++) 
				{
					char c = word[k];
					if (toCaps && k == 0) 
						c = char.ToLower(c);
					output[k + word.Length - firstVowel] = c;
				}
				// add ay
				output[k + word.Length - firstVowel] = 'a'; output[k + word.Length - firstVowel + 1] = 'y';
				// correct for upper/lower case
				if (toCaps)
					output[0] = char.ToUpper(output[0]);
			}
			return new string(output);
		}

		public void Translate(OcrEngine engine, OcrDocument doc, string mimeType, string outFileName, object translationObject)
		{
			FileStream stream = new FileStream(outFileName, FileMode.Open, FileAccess.Read);
			try {
				Translate(engine, doc, mimeType, stream, translationObject);
			}
			catch (Exception e) {
				throw e;
			}
			finally {
				stream.Close();
			}
		}

		#endregion

		#region ITranslator Members

		public bool Supports(string mimeType)
		{
			foreach (string s in _supported) 
			{
				if (mimeType == s)
					return true;
			}
			return false;
		}

		public bool IsNative()
		{
			return false;
		}

		public string[] Supported()
		{
			return _supported;
		}

		public bool CanStream()
		{
			return true;
		}

		#endregion

	}
}
