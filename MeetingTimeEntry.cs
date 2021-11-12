using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLogger
{
    public class MeetingTimeEntry : TimeEntry, ICloneable, IEquatable<MeetingTimeEntry>
    {
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value;  }
        }

        public MeetingTimeEntry()
        {
            _Description = String.Empty;
        }

        public MeetingTimeEntry(int row, DateTime date, int durationInMinutes, string text, string description)
        {
            Row = row;
            Date = date;
            DurationInMinutes = durationInMinutes;
            Text = text;
            Description = description;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }


        #region Operators: Equals, ==, !=

        public override bool Equals(object obj) => this.Equals(obj as MeetingTimeEntry);

        public bool Equals(MeetingTimeEntry m)
        {
            if (m is null || this.GetType() != m.GetType())
            {
                return false;
            }
            bool result = ((m.Description ?? "").Equals(this.Description) && m.Date == this.Date);
            return result;
        }

        public static bool operator ==(MeetingTimeEntry left, MeetingTimeEntry right)
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

        public static bool operator !=(MeetingTimeEntry left, MeetingTimeEntry right) => !(left == right);

        public override int GetHashCode()
        {
            return new { Row, Date, DurationInMinutes, Text, Description }.GetHashCode();
        }


        #endregion

    }
}
