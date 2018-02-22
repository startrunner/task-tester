using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TaskTester.Spreadsheets
{
    public class SpreadsheetExporter<TData> : IEnumerable
    {
        static readonly int TitleRow = 1;
        static readonly int HeaderRow = 2;
        static readonly int DataStartRow = 3;

        readonly List<string> mHeaders = new List<string>();
        readonly List<Func<TData, IEnumerable<string>>> mCellSelectors = new List<Func<TData, IEnumerable<string>>>();
        readonly Dictionary<string, Bitmap> mIcons = new Dictionary<string, Bitmap>();

        public string Title { get; set; }
        public SpreadsheetExporter(string title) => Title = title;


        public void Add(string header, Func<TData, string> cellSelector) =>
            Add(header, data => new[] { cellSelector(data) });

        public void Add(string header, Func<TData, IEnumerable<string>> cellsSelector)
        {
            mCellSelectors.Add(cellsSelector);
            mHeaders.Add(header);
        }

        public void Export(string path, IEnumerable<TData> data)
        {
            var workbook = new XLWorkbook();

            //Excel will throw an exception when a
            //worksheet title is longer than 31 characters.
            string shortTitle = string.Join("", Title.Take(25));
            IXLWorksheet sheet = workbook.AddWorksheet(shortTitle);

            sheet.Row(TitleRow).Style.Font.SetBold();
            sheet.Row(HeaderRow).Style.Font.SetBold();

            ExportCells(sheet, data, out int[] headerColumnSpans);
            ExportHeaders(sheet, headerColumnSpans);
            ExportTitle(sheet, headerColumnSpans.Sum());
            sheet.Columns().AdjustToContents();

            workbook.SaveAs(path);
        }

        private void ExportTitle(IXLWorksheet sheet, int sheetColumnSpan)
        {
            IXLCell fistCell = sheet.Cell(TitleRow, 1);
            IXLCell lastCell = sheet.Cell(TitleRow, sheetColumnSpan);
            IXLRange range = sheet.Range(fistCell.Address, lastCell.Address).Merge();
            range.Value = Title;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void ExportHeaders(IXLWorksheet sheet, int[] headerColumnSpans)
        {
            int column = 1;
            for (int i = 0; i < mHeaders.Count; i++)
            {
                IXLCell fistCell = sheet.Cell(HeaderRow, column);
                IXLCell lastCell = sheet.Cell(HeaderRow, column + headerColumnSpans[i] - 1);
                IXLRange range = sheet.Range(fistCell.Address, lastCell.Address).Merge();
                range.Value = mHeaders[i];

                column += headerColumnSpans[i];
            }
        }

        private void ExportCells(IXLWorksheet sheet, IEnumerable<TData> data, out int[] headerColumnSpans)
        {
            headerColumnSpans = new int[mHeaders.Count];

            int rowNumber = DataStartRow;
            foreach (TData row in data)
            {
                int headerIndex = 0;
                int columnNumber = 1;
                foreach (Func<TData, IEnumerable<string>> selector in mCellSelectors)
                {
                    IEnumerable<string> selectedCells = selector(row);
                    headerColumnSpans[headerIndex] = Math.Max(headerColumnSpans[headerIndex], selectedCells.Count());
                    foreach (string cell in selectedCells)
                    {
                        IXLCell xlCell = sheet.Cell(rowNumber, columnNumber);
                        xlCell.SetValue(cell);

                        columnNumber++;
                    }
                    headerIndex++;
                }
                rowNumber++;
            }

            for (int i = 0; i < headerColumnSpans.Length; i++)
            {
                headerColumnSpans[i] = Math.Max(headerColumnSpans[i], 1);
            }
        }

        public IEnumerator GetEnumerator() => throw new NotImplementedException();
    }
}
