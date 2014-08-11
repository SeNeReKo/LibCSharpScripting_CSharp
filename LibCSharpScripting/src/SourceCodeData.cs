using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LibCSharpScripting.src
{

	/// <summary>
	/// Instances of this class represent a source code file. It is ment as some kind of buffer: Fill it with
	/// data and then send this source code to compilation.
	/// </summary>
	public class SourceCodeData : IEnumerable<ISourceCodeLine>
	{

		private class SourceCodeLine : ISourceCodeLine
		{
			public readonly string FileName;
			public readonly int FilePart;
			public readonly int LineNo;
			public string LineText
			{
				get;
				private set;
			}

			public SourceCodeLine(SourceCodeLocation location, string lineText)
			{
				if (location == null) {
					this.FileName = null;
					this.FilePart = -1;
					this.LineNo = -1;
				} else {
					this.FileName = location.FileName;
					this.FilePart = location.FilePart;
					this.LineNo = location.LineNo;
				}
				this.LineText = lineText;
			}

			public SourceCodeLine(string fileName, int filePart, int lineNo, string lineText)
			{
				this.FileName = fileName;
				this.FilePart = filePart;
				this.LineNo = lineNo;
				this.LineText = lineText;
			}

			public override string ToString()
			{
				return LineText;
			}
		}

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		private static readonly string CRLF = "" + (char)13 + (char)10;

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		List<SourceCodeLine> sourceCodeLines;

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public SourceCodeData()
		{
			sourceCodeLines = new List<SourceCodeLine>();
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public void Append(string lineText)
		{
			sourceCodeLines.Add(new SourceCodeLine(null, lineText));
		}

		public void Append(string fileName, int filePart, int lineNo, string lineText)
		{
			sourceCodeLines.Add(new SourceCodeLine(fileName, filePart, lineNo, lineText));
		}

		public void Append(SourceCodeLocation location, string lineText)
		{
			sourceCodeLines.Add(new SourceCodeLine(location, lineText));
		}

		public void Append(SourceCodeLocation startLocation, string[] textLines)
		{
			for (int i = 0; i < textLines.Length; i++) {
				sourceCodeLines.Add(new SourceCodeLine(startLocation.FileName, startLocation.FilePart, startLocation.LineNo + i, textLines[i]));
			}
		}

		public void Append(string[] textLines)
		{
			for (int i = 0; i < textLines.Length; i++) {
				sourceCodeLines.Add(new SourceCodeLine(null, textLines[i]));
			}
		}

		public void Append(string fileName, int filePart, int startLineNo, string[] textLines)
		{
			for (int i = 0; i < textLines.Length; i++) {
				sourceCodeLines.Add(new SourceCodeLine(fileName, filePart, startLineNo + i, textLines[i]));
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < sourceCodeLines.Count; i++) {
				sb.Append(sourceCodeLines[i].LineText);
				sb.Append(CRLF);
			}
			return sb.ToString();
		}

		public SourceCodeLocation GetLocation(int lineNo)
		{
			if ((lineNo < 0) || (lineNo >= sourceCodeLines.Count))
				return null;
			SourceCodeLine line = sourceCodeLines[lineNo];
			if ((line.LineNo < 0) && (lineNo > 0)) {
				line = sourceCodeLines[lineNo - 1];
				if (line.LineNo < 0) {
					line = sourceCodeLines[lineNo - 2];
					if (line.LineNo < 0)
						return null;
				}
			}
			return new SourceCodeLocation(line.FileName, line.FilePart, line.LineNo);
		}

		public IEnumerator<ISourceCodeLine> GetEnumerator()
		{
			foreach (SourceCodeLine sl in sourceCodeLines) {
				yield return sl;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (SourceCodeLine sl in sourceCodeLines) {
				yield return (ISourceCodeLine)sl;
			}
		}

		public static string[] SplitLines(string text)
		{
			string[] lines = text.Split('\n');
			for (int i = 0; i < lines.Length; i++) {
				lines[i] = lines[i].Replace("\r", "");
			}
			return lines;
		}

	}

}
