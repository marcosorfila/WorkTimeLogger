using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;


namespace WorkTimeLogger
{
    public abstract class TimeEntry
    {
        /// <summary>
        /// Row number in the spreadsheet.
        /// </summary>
        public int Row
        {
            get { return _Row; }
            set { _Row = value; }
        }
        protected int _Row;

        /// <summary>
        /// Date of the time entry.
        /// </summary>
        public DateTime Date
        {
            get { return _Date;  }
            set { _Date = value; }
        }
        protected DateTime _Date;

        /// <summary>
        /// Duration in minutes of the time entry.
        /// </summary>
        public int DurationInMinutes
        {
            get { return _DurationInMinutes; }
            set { _DurationInMinutes = value; }
        }
        protected int _DurationInMinutes;

        /// <summary>
        /// Raw text of the time entry.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        protected string _Text;


        public TimeEntry()
        {
            _Row = -1;
            _Date = DateTime.Now.Date;
            _DurationInMinutes = 0;
            _Text = String.Empty;
        }


        /// <summary>
        /// Retuns a time entry based on the parameters.
        /// If the time entry is incorect
        /// </summary>
        /// <param name="date"></param>
        /// <param name="durationInMinutes"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        static public TimeEntry CreateTimeEntry(int row, DateTime date, int durationInMinutes, string text)
        {
            TimeEntry entry;

            if (durationInMinutes <= 0)
            {
                return new InvalidTimeEntry(row, date, durationInMinutes, text, "The duration must be greater than zero");
            }

            string cleanedUpText = (text ?? "").Replace("-", "").Replace(".", "").Trim();
            if (durationInMinutes > 0 && String.IsNullOrEmpty(cleanedUpText))
            {
                entry = new InvalidTimeEntry(row, date, durationInMinutes, text, "The row has time, but no text");
            }
            else if (durationInMinutes == 0 && !String.IsNullOrEmpty(cleanedUpText))
            {
                entry = new InvalidTimeEntry(row, date, durationInMinutes, text, "The row has text, but no time");
            }
            else
            {
                // Try to get the Jira ID
                Regex pattern = new Regex(@"Jira#(?<jiraId>[^:]+):\s*(?<summary>[^:]+):*");
                Match match = pattern.Match(text);
                string jiraId = match.Groups["jiraId"].Value.ToString();
                string summary = match.Groups["summary"].Value.ToString();


                if (!String.IsNullOrEmpty(jiraId) && !String.IsNullOrEmpty(summary))
                {
                    JiraTimeEntry jiraEntry = new JiraTimeEntry(row, date, durationInMinutes, text, jiraId, summary);
                    using (StringReader sr = new StringReader(text))
                    {
                        string line;
                        int lineNum = 1;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (lineNum > 1 && // Exclude the first line, which contains the Jira ID and Summary
                                !String.IsNullOrEmpty(line.Replace("-", "").Trim())) // Exclude lines that only contains dashes and white spaces, which are virtually empty
                            {
                                jiraEntry.AddCommentLine(line);
                            }
                            lineNum++;
                        }
                    }
                    entry = jiraEntry;
                }
                else
                {
                    pattern = new Regex(@"Meeting:\s*(?<meetingDescription>.+)");
                    match = pattern.Match(text);
                    string meetingDescription = match.Groups["meetingDescription"].Value.ToString();
                    if (!String.IsNullOrEmpty(meetingDescription))
                    {
                        entry = new MeetingTimeEntry(row, date, durationInMinutes, text, meetingDescription);
                    }
                    else
                    {
                        entry = new InvalidTimeEntry(row, date, durationInMinutes, text, "This is neither a Jira nor a Meeting");
                    }
                }
            }

            return entry;
        }
    }
}
