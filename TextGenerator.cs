using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLogger
{
    public class TextGenerator
    {

        /// <summary>
        /// Get the text with all the meetings grouped by date.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static string GetTextGroupedByDate(List<TimeEntry> entries)
        {
            List<TimeEntry> consolidatedEntries = Tools.ConsolidateTimeEntries(entries);

            // Identify the Jiras with the higher times in each Date. We will add the Meetings for that Date into that Jira.
            Dictionary<DateTime, JiraTimeEntry> longestJiraPerDay = Tools.GetLongestJiraPerDay(consolidatedEntries);
            // Get the Meeting Time Entry for each day
            Dictionary<DateTime, MeetingTimeEntry> meetingsForEachDay = Tools.GetMeetingsForEachDay(consolidatedEntries);
            // Group entries by Date
            Dictionary<DateTime, List<TimeEntry>> entriesByDate = Tools.GetTimeEntriesGroupedByDate(consolidatedEntries);

            StringBuilder outputStr = new StringBuilder();
            foreach (DateTime day in entriesByDate.Keys.OrderBy(e => e.Ticks))
            {
                int totalTime = 0;
                StringBuilder validJirasStr = new StringBuilder();
                StringBuilder invalidJirasStr = new StringBuilder();
                foreach (TimeEntry t in entriesByDate[day])
                {
                    if (t is JiraTimeEntry)
                    {
                        StringBuilder jiraOutput = new StringBuilder();
                        JiraTimeEntry jt = (JiraTimeEntry)t;
                        int duration = jt.DurationInMinutes;
                        jiraOutput.AppendFormat("Jira#{0}: {1}:\r\n", jt.JiraId, jt.Summary);
                        foreach (string line in jt.CommentLines)
                        {
                            jiraOutput.AppendLine(line);
                        }
                        if (meetingsForEachDay.ContainsKey(day) && jt.JiraId.Equals(longestJiraPerDay[day].JiraId))
                        {
                            duration += meetingsForEachDay[day].DurationInMinutes;
                            jiraOutput.AppendLine("Meetings:");
                            jiraOutput.AppendLine(meetingsForEachDay[day].Description);
                        }

                        validJirasStr.AppendLine(FormatMinutesToHours(duration));
                        validJirasStr.AppendLine(jiraOutput.ToString());
                        totalTime += duration;
                    }
                    if (t is InvalidTimeEntry)
                    {
                        if (invalidJirasStr.Length == 0)
                        {
                            invalidJirasStr.AppendLine("\r\nErrors:");
                        }
                        InvalidTimeEntry invEntry = (InvalidTimeEntry)t;
                        invalidJirasStr.AppendFormat("  Row {0}: {1}\r\n", invEntry.Row, invEntry.ValidationErrorMessage);
                    }
                }
                outputStr.AppendFormat("{0}  -  {1}\r\n", day.ToString("MMMM d"), FormatMinutesToHours(totalTime));
                outputStr.AppendFormat("{0}\r\n{1}\r\n\r\n", invalidJirasStr.ToString(), validJirasStr.ToString());
            }

            string output = outputStr.ToString();
            return output;
        }


        /// <summary>
        /// Given a value of minutes, returns the string repreentation in hh:mm format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static private string FormatMinutesToHours(int minutes)
        {
            string result = String.Format("{0}:{1}", minutes / 60, (minutes % 60).ToString("00"));
            return result;
        }


        /// <summary>
        /// Get the text with the entries groups by Jira.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static string GetTextGroupedByJira(List<TimeEntry> entries)
        {
            // Sample format:
            //
            // Time: 120m
            // Date: 10/May/21 12:00 PM
            // Work Description:
            //  - a
            //  - b

            List<TimeEntry> consolidatedEntries = Tools.ConsolidateTimeEntries(entries);

            // Identify the Jiras with the higher times in each Date. We will add the Meetings for that Date into that Jira.
            Dictionary<DateTime, JiraTimeEntry> longestJiraPerDay = Tools.GetLongestJiraPerDay(consolidatedEntries);
            // Get the Meeting Time Entry for each day
            Dictionary<DateTime, MeetingTimeEntry> meetingsForEachDay = Tools.GetMeetingsForEachDay(consolidatedEntries);
            // Group entries by Date
            Dictionary<string, List<JiraTimeEntry>> entriesByJiraId = Tools.GetJiraEntriesGroupedByJiraId(consolidatedEntries);

            StringBuilder outputStr = new StringBuilder();
            foreach (string jiraId in entriesByJiraId.Keys.OrderBy(id => id))
            {
                outputStr.AppendFormat("Jira {0}: {1}\r\n\r\n", jiraId, entriesByJiraId[jiraId][0].Summary);
                foreach (JiraTimeEntry entry in entriesByJiraId[jiraId].OrderBy(j => j.Date))
                {
                    string meetingsStr = String.Empty;
                    int durationInMinutes = entry.DurationInMinutes;
                    if (longestJiraPerDay[entry.Date] == entry && meetingsForEachDay.ContainsKey(entry.Date))
                    {
                        meetingsStr = String.Format("Meetings:\r\n{0}\r\n", meetingsForEachDay[entry.Date].Description);
                        durationInMinutes += meetingsForEachDay[entry.Date].DurationInMinutes;
                    }
                    outputStr.AppendFormat("Time: {0}m\r\n", durationInMinutes);
                    outputStr.AppendFormat("{0}\r\n", entry.Date.ToString("dd/MMM/yy hh:mm tt"));
                    outputStr.AppendFormat("Work Description:\r\n");
                    foreach (string line in entry.CommentLines)
                    {
                        outputStr.AppendLine(line);
                    }
                    outputStr.AppendLine(meetingsStr);
                }
                outputStr.AppendLine("\r\n");
            }

            return outputStr.ToString();
        }


        /// <summary>
        /// Get the CSV text to import using Jira Assistant.
        /// See: https://chrome.google.com/webstore/detail/jira-assistant-worklog-sp/momjbjbjpbcbnepbgkkiaofkgimihbii
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static string GetCsvTextToImportUsingJiraAssistant(List<TimeEntry> entries)
        {
            // Sample format:

            //Ticket No, Start Date,Timespent,Comment
            //CRM - 3822,2022 - 06 - 21 19:01,11m,"X
            //        - Line 1
            //        - Line 2"
            //CRM - 3822,2022 - 06 - 21 19:02,12m,"X
            //        - Line ""1""
            //        - Line ""2""
            //"


            List<TimeEntry> consolidatedEntries = Tools.ConsolidateTimeEntries(entries);

            // Identify the Jiras with the higher times in each Date. We will add the Meetings for that Date into that Jira.
            Dictionary<DateTime, JiraTimeEntry> longestJiraPerDay = Tools.GetLongestJiraPerDay(consolidatedEntries);
            // Get the Meeting Time Entry for each day
            Dictionary<DateTime, MeetingTimeEntry> meetingsForEachDay = Tools.GetMeetingsForEachDay(consolidatedEntries);
            // Group entries by Date
            Dictionary<string, List<JiraTimeEntry>> entriesByJiraId = Tools.GetJiraEntriesGroupedByJiraId(consolidatedEntries);

            StringBuilder outputStr = new StringBuilder();
            outputStr.AppendLine("Ticket No,Start Date,Timespent,Comment");
            foreach (string jiraId in entriesByJiraId.Keys.OrderBy(id => id))
            {
                foreach (JiraTimeEntry entry in entriesByJiraId[jiraId].OrderBy(j => j.Date))
                {
                    string meetingsStr = String.Empty;
                    int durationInMinutes = entry.DurationInMinutes;
                    if (longestJiraPerDay[entry.Date] == entry && meetingsForEachDay.ContainsKey(entry.Date))
                    {
                        meetingsStr = String.Format("\r\nMeetings:\r\n{0}\r\n", meetingsForEachDay[entry.Date].Description);
                        durationInMinutes += meetingsForEachDay[entry.Date].DurationInMinutes;
                    }
                    outputStr.AppendFormat(@"{0},{1},{2}m,""", jiraId, entry.Date.ToString("yyyy-MM-dd HH:mm"), durationInMinutes);
                    foreach (string line in entry.CommentLines)
                    {
                        outputStr.AppendLine(line);
                    }
                    outputStr.AppendLine(meetingsStr + @"""");
                }
            }

            return outputStr.ToString();
        }



        /// <summary>
        /// Get the text with the entries groups by Jira, but without including Meetings nor sub-grouping by dates.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static string GetTextGroupedByJiraWithoutMeetingsAndDates(List<TimeEntry> entries)
        {
            // Sample format:
            //
            // Time: 120m
            // Work Description:
            //  - a
            //  - b

            List<TimeEntry> consolidatedEntries = Tools.ConsolidateTimeEntries(entries);

            // Group entries by Date
            Dictionary<string, List<JiraTimeEntry>> entriesByJiraId = Tools.GetJiraEntriesGroupedByJiraId(consolidatedEntries);

            StringBuilder outputStr = new StringBuilder();
            outputStr.AppendLine("Jira|Summary|Time|Description");
            foreach (string jiraId in entriesByJiraId.Keys.OrderBy(id => id))
            {
                string workDescrptionStr = String.Empty;
                int durationInMinutes = 0;
                foreach (JiraTimeEntry entry in entriesByJiraId[jiraId].OrderBy(j => j.Date))
                {
                    durationInMinutes += entry.DurationInMinutes;
                    foreach (string line in entry.CommentLines)
                    {
                        workDescrptionStr += line + Environment.NewLine;
                    }
                }
                outputStr.AppendFormat(@"{0}|{1}|{2}|""{3}""
", jiraId, entriesByJiraId[jiraId][0].Summary.Replace(@"""", ""), FormatMinutesToHours(durationInMinutes), workDescrptionStr.Replace(@"""", ""));
            }

            return outputStr.ToString();
        }

    }
}
