using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLogger
{
    public class JiraTimeEntry : TimeEntry, IEquatable<JiraTimeEntry>, ICloneable
    {
        /// <summary>
        /// Jira ticket ID (e.g. CRM-1234)
        /// </summary>
        public string JiraId
        {
            get { return _JiraId;  }
            set { _JiraId = value; }
        }
        private string _JiraId;

        /// <summary>
        /// Summary (title) of the Jira ticket.
        /// </summary>
        public string Summary
        {
            get { return _Summary; }
            set { _Summary = value; }
        }
        private string _Summary;

        /// <summary>
        /// Given the cell text, these are the lines below the first line, which contains the jira ID and the Summary.
        /// </summary>
        public List<string> CommentLines
        {
            get { return _CommentLines; }
        }
        private List<string> _CommentLines;

        /// <summary>
        /// Adds a comment line.
        /// </summary>
        /// <param name="commentLine"></param>
        public void AddCommentLine(string commentLine)
        {
            _CommentLines.Add(commentLine);
        }

        /// <summary>
        /// Adds a list of comment lines. Existing comment lines are preserved.
        /// </summary>
        /// <param name="commentLines"></param>
        public void AddCommentLines(List<string> commentLines)
        {
            _CommentLines.AddRange(commentLines);
        }

        /// <summary>
        /// Deletes all comment lines.
        /// </summary>
        public void ClearCommentLines()
        {
            _CommentLines.Clear();
        }
        
        public JiraTimeEntry()
        {
            _JiraId = String.Empty;
            _Summary = String.Empty;
            _CommentLines = new List<string>();
        }

        public JiraTimeEntry(int row, DateTime date, int durationInMinutes, string text, string project, string jiraId, string summary)
        {
            Row = row;
            Date = date;
            DurationInMinutes = durationInMinutes;
            Text = text;
            Project = project;
            JiraId = jiraId;
            Summary = summary;
            _CommentLines = new List<string>();
        }


        /// <summary>
        /// Removes duplicates from the Comment Lines.
        /// </summary>
        public void RemoveDuplicatesFromCommentLines()
        {
            _CommentLines = _CommentLines.Distinct<string>().ToList();
        }


        #region Operators: Equals, ==, !=

        public override bool Equals(object obj) => this.Equals(obj as JiraTimeEntry);

        public bool Equals(JiraTimeEntry j)
        {
            if (j is null || this.GetType() != j.GetType())
            {
                return false;
            }
            return (j.JiraId == this.JiraId && j.Project == this.Project);
        }

        public static bool operator ==(JiraTimeEntry left, JiraTimeEntry right)
        {
            if (left is null)
            {
                if (right is null)
                {
                    return true;
                }
                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return left.Equals(right);
        }

        public static bool operator !=(JiraTimeEntry left, JiraTimeEntry right) => !(left == right);

        public override int GetHashCode()
        {
            return new { JiraId, Date, DurationInMinutes, Text, Summary }.GetHashCode();
        }


        #endregion


        public object Clone()
        {
            JiraTimeEntry j = (JiraTimeEntry)this.MemberwiseClone();
            j._CommentLines = this._CommentLines.Select(item => (string)item.Clone()).ToList();
            return j;
        }
    }
}
