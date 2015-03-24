using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using xl = Microsoft.Office.Interop.Excel;

namespace RotaMaker.Models.Utilities
{
    public class WeeklyOffDutyWriter
    {
        #region Constructor

        public WeeklyOffDutyWriter(WardModel model, string fileName, DateTime dateSelected)
        {
            // store the params
            _model = model;
            _outputFileName = fileName;
            _startDate = WardModel.GetMondayBeforeDate(dateSelected);

            // open up excel
            _app = new Microsoft.Office.Interop.Excel.Application();
            _app.Visible = true;
            _app.WindowState = xl.XlWindowState.xlMaximized;

        }
        #endregion

        #region Private Data

        private WardModel _model;
        private string _outputFileName;
        private DateTime _startDate;
        private xl.Application _app;

        #endregion

        #region Public Functions

        public void WriteToFile()
        {
            // create the workbook and worksheet
            xl.Workbook wb = _app.Workbooks.Add(xl.XlWBATemplate.xlWBATWorksheet);
            xl.Worksheet ws = wb.Worksheets[1];

            // write out the headers
            WriteOutOffDutyHeaders(ws);

            // write out the nurse's shifts
            WriteOutNurseShifts(ws);
            
            // save the file 
            wb.SaveAs(_outputFileName);
        }
        
        #endregion

        #region Utility Functions

        private void WriteOutOffDutyHeaders(xl.Worksheet ws)
        {
            // write the ward name
            ws.Range["C1"].Value = _model.WardName;

            // write the month weeks and the dates row
            WriteMonthAndDatesHeaderRow(ws);

            WriteGradeAnadDayNamesRow(ws);


        }

        private static void WriteGradeAnadDayNamesRow(xl.Worksheet ws)
        {
            ws.Range["A3"].Value = "GRADE";
            ws.Range["C3"].Value = "Name";
            ws.Range["D3"].Value = "MON";
            ws.Range["E3"].Value = "TUE";
            ws.Range["F3"].Value = "WED";
            ws.Range["G3"].Value = "THU";
            ws.Range["H3"].Value = "FRI";
            ws.Range["I3"].Value = "SAT";
            ws.Range["J3"].Value = "SUN";
            ws.Range["K3"].Value = "Wk HRS";

            // set the font to small and bold
            SetCellRangeFonts(ws, "A3", "K3", 8, true, "Arial");

            AddBordersAroundCellsForRow(ws,3);
        }

        private static void AddBordersAroundCellsForRow(xl.Worksheet ws, int row)
        {
            string[] columnNames = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

            foreach (var colName in columnNames)
            {
                string cellName = colName + row.ToString();

                ws.get_Range(cellName, cellName).BorderAround2();
            }

        }

        private static void SetCellRangeFonts(xl.Worksheet ws, string start, string end,
            int size, bool bold, string fontName)
        {
            ws.get_Range(start, end).Font.Size = size;
            ws.get_Range(start, end).Font.Bold = bold;
            ws.get_Range(start, end).Font.Name = fontName;
        }

        private void WriteMonthAndDatesHeaderRow(xl.Worksheet ws)
        {
            DateTime monday = _startDate;
            DateTime tuesday = _startDate.AddDays(1);
            DateTime wednesday = _startDate.AddDays(2);
            DateTime thursday = _startDate.AddDays(3);
            DateTime friday = _startDate.AddDays(4);
            DateTime saturday = _startDate.AddDays(5);
            DateTime sunday = _startDate.AddDays(6);

            string monthStr = monday.ToString("MMMM", CultureInfo.InvariantCulture);
            if (monday.Month != sunday.Month)
            {
                monthStr += "-";
                monthStr += sunday.ToString("MMMM", CultureInfo.InvariantCulture);
            }
            ws.Range["C2"].Value = monthStr;

            ws.get_Range("D2", "J2").NumberFormat = "@";
            ws.Range["D2"].Value = monday.Day.ToString();
            ws.Range["E2"].Value = tuesday.Day.ToString();
            ws.Range["F2"].Value = wednesday.Day.ToString();
            ws.Range["G2"].Value = thursday.Day.ToString();
            ws.Range["H2"].Value = friday.Day.ToString();
            ws.Range["I2"].Value = saturday.Day.ToString();
            ws.Range["J2"].Value = sunday.Day.ToString();
            ws.Range["L2"].Value = "WEEK";

            // set Month cell to big & bold
            SetCellRangeFonts(ws, "C2", "C2", 14, true, "Arial");

            // set the other numbers to small and bold
            SetCellRangeFonts(ws, "D2", "J2", 7, true, "Arial");

            // set the Week to Medium and bold
            SetCellRangeFonts(ws, "L2", "L2", 10, true, "Arial");

            // put a border around the cells
            AddBordersAroundCellsForRow(ws, 2);
        }

        private void WriteOutNurseShifts(xl.Worksheet ws)
        {
            // get this weeks shifts nurses off off duty
            var shiftsForSelectedWeek = _model.GetWeeksRotaForDate(_startDate);
            List<NurseOffDuty> nursesOffDuty = new List<NurseOffDuty>();

            foreach (var nurse in _model.Staff.OrderByDescending(x => x.Band))
            {
                NurseOffDuty nurseOffDuty = new NurseOffDuty(nurse, shiftsForSelectedWeek);
                nursesOffDuty.Add(nurseOffDuty);
            }

            int outputRow = 4;
            for (int i = 0; i < nursesOffDuty.Count; ++i)
            {
                if (i > 1 && nursesOffDuty[i].Band != nursesOffDuty[i - 1].Band)
                    outputRow++;

                WriteOutNurseOffDuty(ws, outputRow, nursesOffDuty[i]);
                outputRow++;
            }

            // write out the totals
            WriteOutTotals(ws, 1+ outputRow);
        }

        private void WriteOutNurseOffDuty(xl.Worksheet ws, int outputRow, NurseOffDuty nurseOffDuty)
        {
            ws.Range["A" + outputRow.ToString()].Value = nurseOffDuty.Band;
            ws.Range["C" + outputRow.ToString()].Value = nurseOffDuty.Name;
            ws.Range["D" + outputRow.ToString()].Value = nurseOffDuty.MondayShifts;
            ws.Range["E" + outputRow.ToString()].Value = nurseOffDuty.TuesdayShifts;
            ws.Range["F" + outputRow.ToString()].Value = nurseOffDuty.WednesdayShifts;
            ws.Range["G" + outputRow.ToString()].Value = nurseOffDuty.ThursdayShifts;
            ws.Range["H" + outputRow.ToString()].Value = nurseOffDuty.FridayShifts;
            ws.Range["I" + outputRow.ToString()].Value = nurseOffDuty.SaturdayShifts;
            ws.Range["J" + outputRow.ToString()].Value = nurseOffDuty.SundayShifts;
            ws.Range["K" + outputRow.ToString()].Value = nurseOffDuty.ExpectedHours.ToString();

            // set the font to small and bold
            SetCellRangeFonts(ws, "A" + outputRow.ToString(), "K" + outputRow.ToString(), 8, true, "Arial");

            AddBordersAroundCellsForRow(ws, outputRow);
        }

        private void WriteOutTotals(xl.Worksheet ws, int startTotalsRow)
        {
            ws.Range["C" + startTotalsRow.ToString()].Value = "Total";
            // What goes here???
            startTotalsRow++;
            ws.Range["C" + startTotalsRow.ToString()].Value = "Nights";
            // What goes here???
            startTotalsRow++;
            ws.Range["C" + startTotalsRow.ToString()].Value = "Annual Leave 15%";
            // What goes here???
            startTotalsRow++;
            ws.Range["C" + startTotalsRow.ToString()].Value = "Bank";
            // What goes here???
            startTotalsRow++;
        }

        #endregion
    }
}
