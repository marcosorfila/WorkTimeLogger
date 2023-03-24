using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harvest.Api;
using System.Windows.Forms;


namespace WorkTimeLogger
{
    public class HarvestHelper
    {
        /// <summary>
        /// Send the given Time Entries to Harvest.
        /// </summary>
        /// <param name="entries"></param>
        public static async void SendTimesToHarvest(Form1 form, string accessToken, List<TimeEntry> entries)
        {
            // Set backward compatibility with TLS 1.2 and previous versions
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;



            // Harvest API client:
            // https://github.com/VolodymyrBaydalka/Harvest.Api

            // We can manage the Harvest tokens from the Developers page:
            // https://id.getharvest.com/developers

            // Account ID: 
            long accountId = 174347;

            HarvestClient client = HarvestClient.FromAccessToken("Harvest API Example", accessToken);
            client.DefaultAccountId = accountId;
            client.Authorize(accessToken);

            try
            {
                form.progressBar1.Show();
                form.Enabled = false;

                Dictionary<string, Harvest.Api.TimeEntry> sampleEntries = await GetSampleEntriesForEntryProjects(client, entries);
                List<string> validationErrors = await ValidateEntries(client, entries, sampleEntries);
                if (validationErrors.Count > 0)
                {
                    string msg = "";
                    foreach (string errMsg in validationErrors)
                    {
                        msg += errMsg + "<br>";
                    }
                    MessageBox.Show(msg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Task<string> task1 = SendTimesToHarvest_Step2(entries, sampleEntries);
                    string text1 = await task1;
                }
            }
            catch (Exception ex)
            {
                form.progressBar1.Hide();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Enabled = true;
            }
            form.progressBar1.Hide();
        }


        /// <summary>
        /// If the entries are not valid, return the validation errors.
        /// </summary>
        protected static async Task<List<string>> ValidateEntries(HarvestClient client, List<TimeEntry> entries, Dictionary<string, Harvest.Api.TimeEntry> sampleEntries)
        {
            List<string> validationErrors = new List<string>();

            // Validate that the we can get sample Harvest entries for all entries (i.e. for all the project values in the entries).
            foreach (TimeEntry te in entries)
            {
                if (te.DurationInMinutes > 0)
                {
                    // Validate that there's a sample entry for the entry's project
                    if (!sampleEntries.ContainsKey(te.Project) || sampleEntries[te.Project] == null)
                    {
                        validationErrors.Add(String.Format(@"Couldn't find a sample entry in Harvest for project name ""{0}"". Time entry info: Date is {1}, Text is ""{2}""", te.Project, te.Date.ToString("yyyy-MM-dd"), te.Text));
                    }
                }
            }

            return validationErrors;
        }


        /// <summary>
        /// Returns a list of existing sample Harvest entries for each of the Projects in the entries list.
        /// If we can't find an existing sample Harvest entry for the project of an entries, the associated sample Harvest entry will be null.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        protected static async Task<Dictionary<string, Harvest.Api.TimeEntry>> GetSampleEntriesForEntryProjects(HarvestClient client, List<TimeEntry> entries)
        {
            Dictionary<string, Harvest.Api.TimeEntry> sampleTimeEntries = new Dictionary<string, Harvest.Api.TimeEntry>();
            foreach (TimeEntry te in entries)
            {
                if (te.DurationInMinutes > 0 && !sampleTimeEntries.Keys.Contains(te.Project))
                {
                    Harvest.Api.TimeEntry sampleTimeEntry = await GetSampleTimeEntryByProjectName(client, te.Project);
                    sampleTimeEntries.Add(te.Project, sampleTimeEntry);
                }
            }
            return sampleTimeEntries;
        }


        protected static async Task<string> SendTimesToHarvest_Step2(
            List<TimeEntry> entries,
            Dictionary<string, Harvest.Api.TimeEntry> sampleEntries
            )
        {
            // Harvest API client:
            // https://github.com/VolodymyrBaydalka/Harvest.Api

            // We can manage the Harvest tokens from the Developers page:
            // https://id.getharvest.com/developers

            // Account ID: 
            long accountId = 174347;
            string accessToken = "121910.pt.yE9TZcsWWkwYc7kqxqwDL_u4SjLARa4LwYp9m3yyZ1sFOJ_uZwnKn4oRqOxjcSO13rDbrQiQLC1-jMazy2CLPA";


            HarvestClient client = HarvestClient.FromAccessToken("Harvest API Example", accessToken);
            //Account myAccount = client.WhoAmI();
            client.DefaultAccountId = accountId;
            client.Authorize(accessToken);
            UserDetails harvestUser = await client.GetMe(accountId);

            // The following methods either fail or cannot be used with my permissions:
            //ProjectsResponse projects = await client.GetProjectsAsync(accountId: accountId);
            //ProjectAssignmentsResponse projAssRes = await client.GetProjectAssignmentsAsync(userId: harvestUser.Id, accountId: accountId);
            //TimeReportResponse r = await client.GetProjectsReportAsync(fromDate: DateTime.Parse("2022-06-10"), toDate: DateTime.Parse("2022-06-20"));


            //// Example 1:
            //// Get the harvest time entries from 6/10/2022 to 6/20/2022
            //TimeEntriesResponse resp = await client.GetTimeEntriesAsync(userId: harvestUser.Id,
            //    fromDate: DateTime.Parse("2022-06-10"), toDate: DateTime.Parse("2022-06-20"), accountId: accountId);

            // Example 2:
            // Create a Harvest entry
            //
            //            DateTime timeEntryDate = DateTime.Parse("2022-06-30 13:00");
            //            decimal hours = 1.25m;
            //            string notes = @"
            //  - Test note 1
            //  - Test note 2
            //";
            //            Harvest.Api.TimeEntry sampleTimeEntry = sampleEntries["Meritus - Vacation"];
            //            Harvest.Api.TimeEntry newTimeEntry = await client.CreateTimeEntryAsync(
            //                projectId: sampleTimeEntry.Project.Id,
            //                taskId: sampleTimeEntry.Task.Id,
            //                spentDate: timeEntryDate,
            //                hours: hours,
            //                notes: notes,
            //                userId: harvestUser.Id,
            //                accountId: accountId
            //                );



            List<TimeEntry> consolidatedEntries = Tools.ConsolidateTimeEntries(entries);
            List<TimeEntry> invalidEntries = consolidatedEntries.Where(entry => entry is InvalidTimeEntry).ToList();
            if (invalidEntries.Count > 0)
            {
                throw new Exception("Unable to upload to Harvest: there are invalid entries.");
            }

            // Identify the Jiras with the higher times in each Date. We will add the Meetings for that Date into that Jira.
            Dictionary<DateTime, JiraTimeEntry> longestJiraPerDay = Tools.GetLongestJiraPerDay(consolidatedEntries);
            // Get the Meeting Time Entry for each day
            Dictionary<DateTime, MeetingTimeEntry> meetingsForEachDay = Tools.GetMeetingsForEachDay(consolidatedEntries);
            // Group entries by Date
            Dictionary<DateTime, List<TimeEntry>> entriesByDate = Tools.GetTimeEntriesGroupedByDate(consolidatedEntries);

            foreach (DateTime day in entriesByDate.Keys.OrderBy(e => e.Ticks))
            {
                foreach (TimeEntry t in entriesByDate[day])
                {
                    if (t is JiraTimeEntry)
                    {
                        StringBuilder timeEntryNotes = new StringBuilder();
                        JiraTimeEntry jt = (JiraTimeEntry)t;
                        int duration = jt.DurationInMinutes;
                        timeEntryNotes.AppendFormat("Jira {0}: {1}:\r\n", jt.JiraId, jt.Summary);
                        foreach (string line in jt.CommentLines)
                        {
                            timeEntryNotes.AppendLine(line);
                        }
                        if (meetingsForEachDay.ContainsKey(day) && jt.JiraId.Equals(longestJiraPerDay[day].JiraId))
                        {
                            duration += meetingsForEachDay[day].DurationInMinutes;
                            timeEntryNotes.AppendLine("Meetings:");
                            timeEntryNotes.AppendLine(meetingsForEachDay[day].Description);
                        }

                        Harvest.Api.TimeEntry sampleTimeEntry = sampleEntries[jt.Project];
                        Harvest.Api.TimeEntry newTimeEntry = await client.CreateTimeEntryAsync(
                            projectId: sampleTimeEntry.Project.Id,
                            taskId: sampleTimeEntry.Task.Id,
                            userId: harvestUser.Id,
                            accountId: accountId,
                            spentDate: jt.Date,
                            hours: Convert.ToDecimal(duration) / 60,
                            notes: timeEntryNotes.ToString()
                            );
                    }
                }
            }

            return "OK";
        }


        /// <summary>
        /// Get a sample time entry given a Project name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="projectNameInSpreadsheet"></param>
        /// <returns></returns>
        protected static async System.Threading.Tasks.Task<Harvest.Api.TimeEntry> GetSampleTimeEntryByProjectName(Harvest.Api.HarvestClient client, string projectNameInSpreadsheet)
        {
            Harvest.Api.TimeEntry sampleTimeEntry = null;

            UserDetails harvestUser = await client.GetMe();
            TimeEntriesResponse resp;

            switch (projectNameInSpreadsheet)
            {
                case "LA - CRM":
                    resp = await client.GetTimeEntriesAsync(fromDate: DateTime.Parse("2022-06-30"), toDate: DateTime.Parse("2022-06-30"));
                    foreach (Harvest.Api.TimeEntry te in resp.TimeEntries)
                    {
                        if ("Lord Abbett".Equals(te.Client.Name) && "LA CRM".Equals(te.Project.Name) && "Development".Equals(te.Task.Name))
                        {
                            sampleTimeEntry = te;
                            break;
                        }
                    }
                    break;
                case "Meritus - Internal":
                    resp = await client.GetTimeEntriesAsync(fromDate: DateTime.Parse("2022-03-01"), toDate: DateTime.Parse("2022-03-01"));
                    foreach (Harvest.Api.TimeEntry te in resp.TimeEntries)
                    {
                        if ("Meritus".Equals(te.Client.Name) && "Internal".Equals(te.Project.Name) && "Vacation".Equals(te.Task.Name))
                        {
                            sampleTimeEntry = te;
                            break;
                        }
                    }
                    break;
                case "Meritus - Training":
                    resp = await client.GetTimeEntriesAsync(fromDate: DateTime.Parse("2022-05-12"), toDate: DateTime.Parse("2022-05-12"));
                    foreach (Harvest.Api.TimeEntry te in resp.TimeEntries)
                    {
                        if ("Meritus".Equals(te.Client.Name) && "Training".Equals(te.Project.Name) && "Training".Equals(te.Task.Name))
                        {
                            sampleTimeEntry = te;
                            break;
                        }
                    }
                    break;
                case "Meritus - Vacation":
                    resp = await client.GetTimeEntriesAsync(fromDate: DateTime.Parse("2022-02-28"), toDate: DateTime.Parse("2022-02-28"));
                    foreach (Harvest.Api.TimeEntry te in resp.TimeEntries)
                    {
                        if ("Meritus".Equals(te.Client.Name) && "Internal".Equals(te.Project.Name) && "Vacation".Equals(te.Task.Name))
                        {
                            sampleTimeEntry = te;
                            break;
                        }
                    }
                    break;
            }

            if (sampleTimeEntry == null)
            {
                throw new Exception(String.Format(@"Unable to find a sample Harvest Entry for the project: {0}", projectNameInSpreadsheet));
            }

            return sampleTimeEntry;
        }
    }
}
