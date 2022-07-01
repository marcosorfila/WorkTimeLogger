using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harvest.Api;


namespace WorkTimeLogger
{
    public class Tools
    {
        /// <summary>
        /// Given a list of Time Entries, it returns, for each date, the Jira that has more time.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DateTime, JiraTimeEntry> GetLongestJiraPerDay(List<TimeEntry> list)
        {
            Dictionary<DateTime, JiraTimeEntry> dic = new Dictionary<DateTime, JiraTimeEntry>();

            foreach (TimeEntry entry in list)
            {
                // If the Jira with max duration for the entry data has not been set or its durationis lower, then the current entry has the higher duration for that date (at least up to now).
                if (entry is JiraTimeEntry && (!dic.ContainsKey(entry.Date) || dic[entry.Date].DurationInMinutes < ((JiraTimeEntry)entry).DurationInMinutes))
                {
                    dic[entry.Date] = (JiraTimeEntry)entry;
                }
            }
            return dic;
        }


        /// <summary>
        /// Given the list of all Time Entries, returns one Meeting Time Entry for each day with all the meetings for that day.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, MeetingTimeEntry> GetMeetingsForEachDay(List<TimeEntry> list)
        {
            // The consolidated list contains only one MeetingTimeEntry per day
            List<TimeEntry> consolidatedList = ConsolidateTimeEntries(list);

            Dictionary<DateTime, MeetingTimeEntry> dic = new Dictionary<DateTime, MeetingTimeEntry>();

            foreach (TimeEntry entry in list)
            {
                // If the Jira with max duration for the entry data has not been set or its durationis lower, then the current entry has the higher duration for that date (at least up to now).
                if (entry is MeetingTimeEntry)
                {
                    dic[entry.Date] = (MeetingTimeEntry)entry;
                }
            }
            return dic;
        }


        /// <summary>
        /// Groups the Time Entries by Date.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, List<TimeEntry>> GetTimeEntriesGroupedByDate(List<TimeEntry> list)
        {
            Dictionary<DateTime, List<TimeEntry>> entriesByDate = new Dictionary<DateTime, List<TimeEntry>>();
            foreach (TimeEntry entry in list)
            {
                if (!entriesByDate.ContainsKey(entry.Date))
                {
                    entriesByDate[entry.Date] = new List<TimeEntry>();
                }
                entriesByDate[entry.Date].Add(entry);
            }
            return entriesByDate;
        }


        /// <summary>
        /// Groups the Time Entries by Jira.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Dictionary<string, List<JiraTimeEntry>> GetJiraEntriesGroupedByJiraId(List<TimeEntry> list)
        {
            Dictionary<string, List<JiraTimeEntry>> entriesByJira = new Dictionary<string, List<JiraTimeEntry>>();
            foreach (TimeEntry entry in list)
            {
                if (entry is JiraTimeEntry)
                {
                    JiraTimeEntry jt = (JiraTimeEntry)entry;
                    if (!entriesByJira.ContainsKey(jt.JiraId))
                    {
                        entriesByJira[jt.JiraId] = new List<JiraTimeEntry>();
                    }
                    entriesByJira[jt.JiraId].Add(jt);
                }
            }
            return entriesByJira;
        }


        /// <summary>
        /// Consolidates multiple entries of the same Jira in one entry, and all meeting entries in one entry.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<TimeEntry> ConsolidateTimeEntries(List<TimeEntry> list)
        {
            List<TimeEntry> result = new List<TimeEntry>();
            Dictionary<DateTime, Dictionary<string,JiraTimeEntry>> jiras = new Dictionary<DateTime, Dictionary<string, JiraTimeEntry>>();
            Dictionary<DateTime, MeetingTimeEntry> meetings = new Dictionary<DateTime, MeetingTimeEntry>();
            List<MeetingTimeEntry> meetingsAlreadyAdded = new List<MeetingTimeEntry>();

            foreach (TimeEntry entry in list)
            {
                if (entry is JiraTimeEntry)
                {
                    JiraTimeEntry jiraEntry = (JiraTimeEntry)entry;
                    if (!jiras.ContainsKey(jiraEntry.Date))
                    {
                        // There are no Jiras for the Date
                        jiras.Add(jiraEntry.Date, new Dictionary<string, JiraTimeEntry>() { { jiraEntry.JiraId, (JiraTimeEntry)jiraEntry.Clone() } });
                    }
                    else
                    {
                        // Check if there's a JiraTimeEntry for the Jira ID in the Date
                        if (!jiras[jiraEntry.Date].ContainsKey(jiraEntry.JiraId))
                        {
                            // Add the Jira ID for this Date
                            jiras[jiraEntry.Date].Add(jiraEntry.JiraId, (JiraTimeEntry)jiraEntry.Clone());
                        }
                        else
                        {
                            // For this Date, there's a JiraTimeEntry for the Jira ID.
                            // Add the time and comments.
                            jiras[jiraEntry.Date][jiraEntry.JiraId].DurationInMinutes += jiraEntry.DurationInMinutes;
                            jiras[jiraEntry.Date][jiraEntry.JiraId].AddCommentLines(jiraEntry.CommentLines);
                        }
                    }
                }
                else if (entry is MeetingTimeEntry)
                {
                    MeetingTimeEntry meetingEntry = (MeetingTimeEntry)entry;

                    // Add a dash before the description if necessary
                    if (!"-".Equals(meetingEntry.Description.Trim().Substring(0,1)))
                    {
                        meetingEntry.Description = " - " + meetingEntry.Description;
                    }

                    if (!meetings.ContainsKey(meetingEntry.Date))
                    {
                        // There are no Meetings for the Date
                        meetings.Add(meetingEntry.Date, (MeetingTimeEntry)meetingEntry.Clone());
                    }
                    else
                    {
                        // There's a MeetingTimeEntry entry for this Date.
                        // Add the description if not added yet
                        if (!meetingsAlreadyAdded.Contains(meetingEntry))
                        {
                            meetings[meetingEntry.Date].Description += Environment.NewLine + meetingEntry.Description;
                        }
                        meetings[meetingEntry.Date].DurationInMinutes += meetingEntry.DurationInMinutes;
                    }
                    meetingsAlreadyAdded.Add(meetingEntry);
                }
                else
                {
                    result.Add(entry);
                }
            }

            // Add the consolidated Jiras to the result
            foreach(DateTime d in jiras.Keys)
            {
                foreach(string jiraId in jiras[d].Keys)
                {
                    JiraTimeEntry j = jiras[d][jiraId];
                    j.RemoveDuplicatesFromCommentLines();
                    if (j.CommentLines.Count == 0)
                    {
                        result.Add(new InvalidTimeEntry(j.Row, j.Date, j.DurationInMinutes, j.Text, String.Format("Jira {0} has no comments", j.JiraId)));
                    }
                    else if (j.CommentLines.Count > 0 && j.DurationInMinutes <= 0)
                    {
                        result.Add(new InvalidTimeEntry(j.Row, j.Date, j.DurationInMinutes, j.Text, String.Format("Jira {0} has comments, but no time", j.JiraId)));
                    }
                    else
                    {
                        j.DurationInMinutes = RoundValue(j.DurationInMinutes, 15);
                        result.Add(j);
                    }
                }
            }
            // Add the consolidated Meetings to the result
            foreach (DateTime d in meetings.Keys)
            {
                meetings[d].DurationInMinutes = RoundValue(meetings[d].DurationInMinutes, 15);
                result.Add(meetings[d]);
            }

            return result;
        }


        /// <summary>
        /// Returns the rounded value so that (ReturnedValue % mod == 0)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        static private int RoundValue(int value, int mod)
        {
            int result = value;
            if (value % mod < mod/2)
            {
                result -= value % mod;
            }
            else
            {
                result += mod - value % mod;
            }
            return result;
        }

        public static async void SendTimesToHarvest()
        {
            // Backward compatibility with TLS 1.2 and previous versions
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            Task<string> task1 = Tools.SendTimesToHarvest2();
            string text1 = await task1;
        }


        public static async System.Threading.Tasks.Task<string> SendTimesToHarvest2()
        {
            ////HarvestRestClient client = new HarvestRestClient("meritus", "morfila@mbsol.net", "M1a2r3c4");
            //HarvestRestClient client = new HarvestRestClient("meritus", "morfila", "XXXXXXXXX");
            //Account myAccount = client.WhoAmI();




            // Harvest API client:
            // https://github.com/VolodymyrBaydalka/Harvest.Api

            // We can manage the Harvest tokens from the Developers page:
            // https://id.getharvest.com/developers

            // Account ID: 
            long accountId = 174347;
            string accessToken = "121910.pt.yE9TZcsWWkwYc7kqxqwDL_u4SjLARa4LwYp9m3yyZ1sFOJ_uZwnKn4oRqOxjcSO13rDbrQiQLC1-jMazy2CLPA";


            HarvestClient client = HarvestClient.FromAccessToken("Harvest API Example", accessToken);
            client.DefaultAccountId = accountId;

            client.Authorize(accessToken);

            UserDetails harvestUser = await client.GetMe(accountId);
            TimeEntriesResponse resp = await client.GetTimeEntriesAsync(userId: harvestUser.Id,
                fromDate: DateTime.Parse("2022-06-10"), toDate: DateTime.Parse("2022-06-20"), accountId: accountId);

            // The following methods either fail or cannot be used with my permissions:
            //ProjectsResponse projects = await client.GetProjectsAsync(accountId: accountId);
            //ProjectAssignmentsResponse projAssRes = await client.GetProjectAssignmentsAsync(userId: harvestUser.Id, accountId: accountId);
            //TimeReportResponse r = await client.GetProjectsReportAsync(fromDate: DateTime.Parse("2022-06-10"), toDate: DateTime.Parse("2022-06-20"));





            // Get the required data for a new entry from existing Time entries
            Harvest.Api.TimeEntry sampleTimeEntry = null;
            foreach (Harvest.Api.TimeEntry te in resp.TimeEntries)
            {
                if ("Development".Equals(te.Task.Name) && "Lord Abbett".Equals(te.Client.Name) && "LA CRM".Equals(te.Project.Name))
                {
                    sampleTimeEntry = te;
                    break;
                }
            }


// NEXT STEPS:
// In the Time Entry, include the Project column.
// Based on the Project value, get an appropriate sampleTimeEntry. If for any of the time entries it's not possible to find a suitable sampleTimeEntry, do not add any of the entries and throw an exception.
// Ideally, before uploading an entry, we should check if the entry exists (compare Date, Duration, Project and Text). This is to avoid running the tool more than once, duplicating the entries.


            DateTime timeEntryDate = DateTime.Parse("2022-06-30 13:00");
            decimal hours = 1.25m;
            string notes = @"
  - Test note 1
  - Test note 2
";
            Harvest.Api.TimeEntry newTimeEntry = await client.CreateTimeEntryAsync(
                projectId: sampleTimeEntry.Project.Id,
                taskId: sampleTimeEntry.Task.Id, spentDate: timeEntryDate,
                hours: hours,
                notes: notes,
                userId: harvestUser.Id,
                accountId: accountId
                );


            return "a";
        }
    }
}
