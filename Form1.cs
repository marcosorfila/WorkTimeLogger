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
            btnGetCSV.Enabled = false;
            btnSendToHarvest.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtOutput.Text = String.Empty;
            btnGetCSV.Enabled = false;
            btnSendToHarvest.Enabled = false;
            ProcessTimeEntries();
        }


        /// <summary>
        /// Process the time entries to generate a compiled the text.
        /// Optionally, send the time entries to Harvest.
        /// </summary>
        /// <param name="sendToHarvest"></param>
        private void ProcessTimeEntries (bool sendToHarvest = false, bool getCSVforJA = false)
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

                txtOutput.Text = TextGenerator.GetTextGroupedByDate(entries);

                bool entriesHaveErrors = Tools.EntriesContainErrors(entries);
                btnGetCSV.Enabled = !entriesHaveErrors;
                btnSendToHarvest.Enabled = !entriesHaveErrors;

                if (sendToHarvest)
                {
                    HarvestHelper.SendTimesToHarvest(this, txtHarvestAccessToken.Text, entries);
                }
                if (getCSVforJA)
                {
                    string csvText = TextGenerator.GetCsvTextToImportUsingJiraAssistant(entries);
                    System.Windows.Forms.Clipboard.SetText(csvText);
                    MessageBox.Show("The CSV for the Jira Assistant Chrome Extension has been copied to the clipboard", "CSV copied to Clipboardr", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void txtDateColumn_Validating(object sender, CancelEventArgs e)
        {
            if (txtDateColumn.Text.Length != 1)
            {
                e.Cancel = true;
            }
            else
            {
                char columnLetter;
                if (!Char.TryParse(txtDateColumn.Text, out columnLetter))
                {
                    e.Cancel = true;
                }
                else
                {
                    if (Char.ToLower(columnLetter) < 'a' || Char.ToLower(columnLetter) > 'z') {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ProcessTimeEntries(sendToHarvest: true);
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            ProcessTimeEntries(getCSVforJA: true);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
