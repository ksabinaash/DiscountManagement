﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

namespace OfferManagement.Helpers
{
    public class GoogleSheetsHelper
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };

        static string ApplicationName = "DiscountManagement";

        static string SpreadSheetId = "1BL0kWy6sfE_Nf0_QXkzb2h65FCdBXUdwBqUoav5kA38";

        private readonly SheetsService _sheetsService;

        private readonly string _spreadsheetId;

        public GoogleSheetsHelper()
        {
            var path = HttpContext.Current.Server.MapPath("~/medilabdiscount-97c500b563df.json");

            var credential = GoogleCredential.FromStream(new FileStream(path, FileMode.Open)).CreateScoped(Scopes);

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            _spreadsheetId = SpreadSheetId;
        }

        public void CreateEntry(DiscountTransaction transaction)
        {
            string sheetName = "Transactions";

            var range = $"{sheetName}!A:J";

            var valueRange = new ValueRange();

            var oblist = new List<object>() { transaction.CustomerName, transaction.UserEmail, transaction.MobileNumber, transaction.ShopName, transaction.BilledValue, transaction.Discount, transaction.DiscountReason, transaction.OTP, transaction.MessageTemplate, transaction.BilledDateTime };
            
            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
            
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            
            var appendReponse = appendRequest.Execute();
        }

        public List<string> GetUserEmailsFromSheet(GoogleSheetParameters googleSheetParameters)
        {
            List<ExpandoObject> sheetData = GetDataFromSheet(googleSheetParameters);
            //List<object> userEmails = new List<object>();
            List<string> userEmail = new List<string>();
            //IDictionary<String, object> keyValues;
            //var cal = sheetData as IDictionary<String, object>;

            //userEmail = sheetData as IDictionary<String, object>();


            foreach (ExpandoObject ex in sheetData)
            {
                foreach (KeyValuePair<string, object> obj in ex)
                {
                    if (string.Equals(obj.Key.ToString(), "USEREMAIL", StringComparison.OrdinalIgnoreCase))
                        userEmail.Add(obj.Value.ToString());
                }
            }


            return userEmail;
        }

        public List<ExpandoObject> GetDataFromSheet(GoogleSheetParameters googleSheetParameters)
        {
            googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
            var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
            var columnNames = new List<string>();
            var returnValues = new List<ExpandoObject>();

            if (!googleSheetParameters.FirstRowIsHeaders)
            {
                for (var i = 0; i <= numberOfColumns; i++)
                {
                    columnNames.Add($"Column{i}");
                }
            }

            var response = request.Execute();

            int rowCounter = 0;
            IList<IList<Object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
                    {
                        for (var i = 0; i <= numberOfColumns; i++)
                        {
                            columnNames.Add(row[i].ToString());
                        }
                        rowCounter++;
                        continue;
                    }

                    var expando = new ExpandoObject();
                    var expandoDict = expando as IDictionary<String, object>;
                    var columnCounter = 0;
                    foreach (var columnName in columnNames)
                    {
                        expandoDict.Add(columnName, row[columnCounter].ToString());
                        columnCounter++;
                    }
                    returnValues.Add(expando);
                    rowCounter++;
                }
            }

            return returnValues;
        }

        public void AddCells(GoogleSheetParameters googleSheetParameters, List<GoogleSheetRow> rows)
        {
            var requests = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

            var sheetId = GetSheetId(_sheetsService, _spreadsheetId, googleSheetParameters.SheetName);

            GridCoordinate gc = new GridCoordinate
            {
                ColumnIndex = googleSheetParameters.RangeColumnStart - 1,
                RowIndex = googleSheetParameters.RangeRowStart - 1,
                SheetId = sheetId
            };

            var request = new Request { UpdateCells = new UpdateCellsRequest { Start = gc, Fields = "*" } };

            var listRowData = new List<RowData>();

            foreach (var row in rows)
            {
                var rowData = new RowData();
                var listCellData = new List<CellData>();
                foreach (var cell in row.Cells)
                {
                    var cellData = new CellData();
                    var extendedValue = new ExtendedValue { StringValue = cell.CellValue };

                    cellData.UserEnteredValue = extendedValue;
                    var cellFormat = new CellFormat { TextFormat = new TextFormat() };

                    if (cell.IsBold)
                    {
                        cellFormat.TextFormat.Bold = true;
                    }

                    cellFormat.BackgroundColor = new Color { Blue = (float)cell.BackgroundColor.B / 255, Red = (float)cell.BackgroundColor.R / 255, Green = (float)cell.BackgroundColor.G / 255 };

                    cellData.UserEnteredFormat = cellFormat;
                    listCellData.Add(cellData);
                }
                rowData.Values = listCellData;
                listRowData.Add(rowData);
            }
            request.UpdateCells.Rows = listRowData;

            // It's a batch request so you can create more than one request and send them all in one batch. Just use reqs.Requests.Add() to add additional requests for the same spreadsheet
            requests.Requests.Add(request);

            _sheetsService.Spreadsheets.BatchUpdate(requests, _spreadsheetId).Execute();
        }

        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];
            return value;
        }

        private GoogleSheetParameters MakeGoogleSheetDataRangeColumnsZeroBased(GoogleSheetParameters googleSheetParameters)
        {
            googleSheetParameters.RangeColumnStart = googleSheetParameters.RangeColumnStart - 1;
            googleSheetParameters.RangeColumnEnd = googleSheetParameters.RangeColumnEnd - 1;
            return googleSheetParameters;
        }

        private int GetSheetId(SheetsService service, string spreadSheetId, string spreadSheetName)
        {
            var spreadsheet = service.Spreadsheets.Get(spreadSheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == spreadSheetName);
            int sheetId = (int)sheet.Properties.SheetId;
            return sheetId;
        }
    }

    public class GoogleSheetCell
    {
        public string CellValue { get; set; }
        public bool IsBold { get; set; }
        public System.Drawing.Color BackgroundColor { get; set; } = System.Drawing.Color.White;
    }

    public class GoogleSheetParameters
    {
        public int RangeColumnStart { get; set; }
        public int RangeRowStart { get; set; }
        public int RangeColumnEnd { get; set; }
        public int RangeRowEnd { get; set; }
        public string SheetName { get; set; }
        public bool FirstRowIsHeaders { get; set; }
    }

    public class GoogleSheetRow
    {
        public GoogleSheetRow() => Cells = new List<GoogleSheetCell>();

        public List<GoogleSheetCell> Cells { get; set; }
    }

    public class userSheetModel
    {
        public string userName { get; set; }
        public string userEmail { get; set; }
    }
}