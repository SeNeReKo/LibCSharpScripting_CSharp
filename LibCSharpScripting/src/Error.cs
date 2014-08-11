using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LibCSharpScripting.src
{

	public class Error
	{

		////////////////////////////////////////////////////////////////
		// Constants
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Variables
		////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////
		// Constructors
		////////////////////////////////////////////////////////////////

		public Error(string errorNumber, string text, SourceCodeLocation location)
		{
			this.ErrorNumber = errorNumber;
			this.Location = location;
			this.Text = text;
		}

		////////////////////////////////////////////////////////////////
		// Properties
		////////////////////////////////////////////////////////////////

		public string ErrorNumber
		{
			get;
			private set;
		}

		public SourceCodeLocation Location
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		////////////////////////////////////////////////////////////////
		// Methods
		////////////////////////////////////////////////////////////////

		public override string ToString()
		{
			if (Location != null) {
				if (Location.FileName != null) {
					return Location.FileName + ":" + Location.LineNo + " : " + ErrorNumber + " " + Text;
				} else {
					return Location.LineNo + " : " + ErrorNumber + " " + Text;
				}
			} else {
				return ErrorNumber + " " + Text;
			}
		}

	}

}
