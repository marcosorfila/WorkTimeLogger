using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkTimeLogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessTimeEntries();
        }


        /// <summary>
        /// Process the time entries to generate a compiled the text.
        /// Optionally, send the time entries to Harvest.
        /// </summary>
        /// <param name="sendToHarvest"></param>
        private void ProcessTimeEntries (bool sendToHarvest = false)
        {
            try
            {
                List<Tuple<int, int>> rowGroups = new List<Tuple<int, int>>();

                rowGroups.Add(new Tuple<int, int>(Convert.ToInt32(numFirstRow.Value), Convert.ToInt32(numLastRow.Value)));
                List<TimeEntry> entries = ExcelHelper.ReadExcelFile(
                    filePath: txtFileName.Text,
                    rowGroups: rowGroups,
                    dateColumn: Convert.ToInt32(txtDateColumn.Text),
                    timeColumn: Convert.ToInt32(txtTimeColumn.Text),
                    textColumn: Convert.ToInt32(txtTextColumn.Text),
                    projectColumn: Convert.ToInt32(txtProjectColumn.Text));


                // Sprints when Marcos was the support guy:
                //   Sprint 33(3 / 3 - 3 / 16)-- Rows 28 to 160
                //   Sprint 35(3 / 31 - 4 / 13) – Rows 260 to 403
                //   Sprint 37(4 / 28 - 5 / 11) – Rows 510 to 609
                //rowGroups.Add(new Tuple<int, int>(28, 160));
                //rowGroups.Add(new Tuple<int, int>(260, 403));
                //rowGroups.Add(new Tuple<int, int>(510, 609));
                //output = TextGenerator.GetTextGroupedByJiraWithoutMeetingsAndDates(entries);

                string output1 = TextGenerator.GetTextGroupedByDate(entries);
                //string output2 = TextGenerator.GetTextGroupedByJira(entries);
                string output3 = TextGenerator.GetCsvTextToImportUsingJiraAssistant(entries);

                if (sendToHarvest)
                {
                    HarvestHelper.SendTimesToHarvest(this, entries);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            dlgOpenSpreadsheet.InitialDirectory = @"C:\Projects\WorkTimeLogger";
            dlgOpenSpreadsheet.Title = "Select Excel file";
            dlgOpenSpreadsheet.ShowDialog();
            txtFileName.Text = dlgOpenSpreadsheet.FileName;
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {

        }

        private void numFirstRow_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numLastRow_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ProcessTimeEntries(sendToHarvest: true);
        }
    }
}
