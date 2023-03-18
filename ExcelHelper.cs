using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;


namespace WorkTimeLogger
{
    public class ExcelHelper
    {
        /// <summary>
        /// Reads an Excel file and returns the list of time entries.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rowGroups">Groups of spreadsheet rows, defined by the first and last row of each group</param>
        /// <param name="dateColumn"></param>
        /// <param name="timeColumn"></param>
        /// <param name="textColumn"></param>
        /// <returns></returns>
        public static List<TimeEntry> ReadExcelFile(string filePath, List<Tuple<int,int>> rowGroups, int dateColumn, int timeColumn, int textColumn, int projectColumn)
        {
            List<TimeEntry> result = new List<TimeEntry>();

            FileInfo fileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // Get the number of rows and columns in the sheet
            //int rows = worksheet.Dimension.Rows;
            //int columns = worksheet.Dimension.Columns;

            DateTime date = DateTime.Now.Date;
            DateTime timeAsDate;
            DateTime flagDate = new DateTime(0);
            int timeInMinutes = 0;
            double dateNumber;
            string text = String.Empty;
            string projectName = String.Empty;


            foreach (Tuple<int, int> rowGroup in rowGroups)
            {
                int firstRow = rowGroup.Item1;
                int LastRow = rowGroup.Item2;

                for (int i = firstRow; i <= LastRow; i++)
                {
                    int row = i;
                    try
                    {
                        // Extract the data from the cells
                        if (worksheet.Cells[row, dateColumn].Value != null && Double.TryParse(worksheet.Cells[row, dateColumn].Value.ToString(), out dateNumber))
                        {
                            date = DateTime.FromOADate(dateNumber).Date.AddHours(13); // The time is 1 PM so there's no date confusion due to the timezone
                        }
                        else
                        {
                            date = flagDate;
                            //throw new Exception(String.Format("The Date value in cell [{0},{1}] is invalid", row, dateColumn));
                        }

                        if (worksheet.Cells[row, timeColumn].Value != null && DateTime.TryParse(worksheet.Cells[row, timeColumn].Value.ToString(), out timeAsDate))
                        {
                            timeInMinutes = timeAsDate.Hour * 60 + timeAsDate.Minute;
                        }
                        else
                        {
                            throw new Exception(String.Format("The Time value in cell [{0},{1}] is invalid", row, timeColumn));
                        }

                        if (worksheet.Cells[row, projectColumn].Value != null)
                        {
                            projectName = worksheet.Cells[row, projectColumn].Value.ToString();
                        }
                        else
                        {
                            projectName = String.Empty;
                        }

                        if (worksheet.Cells[row, textColumn].Value != null)
                        {
                            text = worksheet.Cells[row, textColumn].Value.ToString();
                        }
                        else
                        {
                            text = String.Empty;
                        }

                        string cleanedUpText = (text ?? "").Replace("-", "").Replace(".", "").Trim();
                        if (date == flagDate || (String.IsNullOrEmpty(cleanedUpText) && timeInMinutes == 0))
                        {
                            // Ignore rows with no Date or that have neither time nor text
                        }
                        else
                        {
                            result.Add(TimeEntry.CreateTimeEntry(row, date, timeInMinutes, text, projectName));
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Add(new InvalidTimeEntry(row, date, timeInMinutes, text, projectName, ex.Message));
                    }
                }
            }


            return result;
        }

    }
}
