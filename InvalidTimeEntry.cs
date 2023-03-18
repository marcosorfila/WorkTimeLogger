using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLogger
{
    public class InvalidTimeEntry : TimeEntry, ICloneable
    {
        /// <summary>
        /// Reason why it's invalid.
        /// </summary>
        public string ValidationErrorMessage
        {
            get { return _ValidationErrorMessage; }
        }
        protected string _ValidationErrorMessage;


        public InvalidTimeEntry()
        {
            _ValidationErrorMessage = "";
        }

        public InvalidTimeEntry(string validationErrorMessage)
        {
            _ValidationErrorMessage = validationErrorMessage;
        }

        public InvalidTimeEntry(int row, DateTime date, int durationInMinutes, string text, string project, string validationErrorMessage = "")
        {
            Row = row;
            Date = date;
            DurationInMinutes = durationInMinutes;
            Text = text;
            Project = project;
            _ValidationErrorMessage = validationErrorMessage;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
