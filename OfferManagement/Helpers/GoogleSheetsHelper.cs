using Google.Apis.Auth.OAuth2;
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

        static string ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];

        static string SpreadSheetId = System.Configuration.ConfigurationManager.AppSettings["GoogleSheetId"];

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

        public void CreateTransaction(DiscountTransaction transaction)
        {
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["TransactionsSheetName"];

                var range = $"{sheetName}!A:M";

                var valueRange = new ValueRange();

                var oblist = new List<object>() { transaction.CustomerName, transaction.CustomerEmail, transaction.MobileNumber, transaction.UserEmail, transaction.PCCName, transaction.BillValue, transaction.Discount, transaction.BilledValue, transaction.DiscountReason, transaction.OTP, transaction.MessageTemplate, transaction.BilledDateTime, transaction.ValidationStatus };

                valueRange.Values = new List<IList<object>> { oblist };

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);

                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var appendReponse = appendRequest.Execute();
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
        }
        public int GetLastRow()
        {
            IList<DiscountTransaction> transactions = new List<DiscountTransaction>();
            string sheetName = System.Configuration.ConfigurationManager.AppSettings["TransactionsSheetName"];

                var range = $"{sheetName}!A:M";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;

                return values != null && values.Count > 0 ? values.Count : 0;
        }

        public void UpdateValidationStatus(DiscountTransaction transaction)
        {
            try
            {
                UpdateTransaction("M",transaction.ValidationStatus);
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
        }

        public void UpdateMsgTemplate(DiscountTransaction transaction)
        {
            try
            {
                UpdateTransaction("K", transaction.MessageTemplate);
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
        }

        public void UpdateTransaction(string ColumnName, string propertyName)
        {
            try
            {
                var lastrow = GetLastRow();

                string sheetName = System.Configuration.ConfigurationManager.AppSettings["TransactionsSheetName"];

                var range = $"{sheetName}!"+ ColumnName + lastrow;

                var valueRange = new ValueRange();

                var oblist = new List<object>() { propertyName };

                valueRange.Values = new List<IList<object>> { oblist };

                var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);

                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                var appendReponse = updateRequest.Execute();
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
        }

        //not used
        public void UpdateTransaction(DiscountTransaction transaction)
        {
            try
            {
                var lastrow = GetLastRow();

                string sheetName = System.Configuration.ConfigurationManager.AppSettings["TransactionsSheetName"];

                var range = $"{sheetName}!M" + lastrow;

                var valueRange = new ValueRange();

                var oblist = new List<object>() { transaction.ValidationStatus };

                valueRange.Values = new List<IList<object>> { oblist };

                var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);

                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                var appendReponse = updateRequest.Execute();
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
        }

        public IList<DiscountTransaction> ReadTransactions(bool IsFirstRowHeader)
        {
            IList<DiscountTransaction> transactions = new List<DiscountTransaction>();
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["TransactionsSheetName"];

                var range = $"{sheetName}!A:M";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;


                if (values != null && values.Count > 0)
                {
                    if (IsFirstRowHeader)
                    {
                        values = values.Skip(1).ToList();
                    }

                    foreach (var row in values)
                    {
                        DiscountTransaction transaction = new DiscountTransaction();

                        transaction.CustomerName = row[0].ToString();
                        transaction.CustomerEmail = row[1].ToString();
                        transaction.MobileNumber = row[2].ToString();
                        transaction.UserEmail = row[3].ToString();
                        transaction.PCCName = row[4].ToString();
                        transaction.BillValue = Convert.ToDouble(row[5].ToString());
                        transaction.Discount = Convert.ToDouble(row[6].ToString());
                        transaction.DiscountReason = row[8].ToString();
                        transaction.OTP = row[9].ToString();
                        transaction.MessageTemplate = row[10].ToString();
                        transaction.BilledDateTime = Convert.ToDateTime(row[11].ToString());
                        transaction.ValidationStatus = row[12].ToString();

                        transactions.Add(transaction);

                        // Print columns A to F, which correspond to indices 0 and 4.
                        //Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}", row[0], row[1], row[2], row[3], row[4], row[5]);
                    }

                }
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return transactions;
        }

        public IList<string> ReadPCCNames(bool IsFirstRowHeader)
        {
            IList<string> PCCNames = new List<String>();
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["PCCValuesSheetName"];

                var range = $"{sheetName}!A:A";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;



                if (values != null && values.Count > 0)
                {
                    if (IsFirstRowHeader)
                    {
                        values = values.Skip(1).ToList();
                    }
                    foreach (var row in values)
                    {
                        PCCNames.Add(row[0].ToString());
                    }
                }


            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return PCCNames;
        }

        public IList<string> ReadDiscontReasons(bool IsFirstRowHeader)
        {
            IList<string> DiscountReasons = new List<String>();
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["DiscountValuesSheetName"];

                var range = $"{sheetName}!A:A";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;



                if (values != null && values.Count > 0)
                {
                    if (IsFirstRowHeader)
                    {
                        values = values.Skip(1).ToList();
                    }
                    foreach (var row in values)
                    {
                        DiscountReasons.Add(row[0].ToString());
                    }
                }


            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return DiscountReasons;
        }

        public IList<string> ReadSMSTemplates(bool IsFirstRowHeader)
        {
            IList<string> Templates = new List<String>();
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["MsgTemplateValuesSheetName"];

                var range = $"{sheetName}!A:A";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;



                if (values != null && values.Count > 0)
                {
                    if (IsFirstRowHeader)
                    {
                        values = values.Skip(1).ToList();
                    }
                    foreach (var row in values)
                    {
                        Templates.Add(row[0].ToString());
                    }
                }


            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return Templates;
        }

        public IList<UserModel> ReadUsersList(bool IsFirstRowHeader)
        {
            IList<UserModel> usersList = new List<UserModel>();
            try
            {
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["UsersSheetName"];

                var range = $"{sheetName}!A:C";

                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = request.Execute();

                IList<IList<object>> values = response.Values;


                if (values != null && values.Count > 0)
                {
                    if (IsFirstRowHeader)
                    {
                        values = values.Skip(1).ToList();
                    }

                    foreach (var row in values)
                    {
                        UserModel user = new UserModel();

                        user.UserEmail = row[0].ToString();
                        user.UserName = row[1].ToString();
                        user.Role = row[2].ToString();

                        usersList.Add(user);
                    }

                }
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return usersList;
        }


        public List<string> GetUserEmailsFromSheet(GoogleSheetParameters googleSheetParameters)
        {
            List<string> userEmail = new List<string>();
            try
            {
                List<ExpandoObject> sheetData = GetDataFromSheet(googleSheetParameters);
                //List<object> userEmails = new List<object>();

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



            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return userEmail;
        }

        public List<ExpandoObject> GetDataFromSheet(GoogleSheetParameters googleSheetParameters)
        {
            var returnValues = new List<ExpandoObject>();
            try
            {
                googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
                var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

                SpreadsheetsResource.ValuesResource.GetRequest request =
                    _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
                var columnNames = new List<string>();


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


            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
            return returnValues;
        }

        public void AddCells(GoogleSheetParameters googleSheetParameters, List<GoogleSheetRow> rows)
        {
            try
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
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                {
                    Console.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                }
            }
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

        //protected static string GetRange(SheetsService service, string SheetId)
        //{
        //    // Define request parameters.  
        //    String spreadsheetId = SheetId;
        //    String range = "A:A";

        //    SpreadsheetsResource.ValuesResource.GetRequest getRequest =
        //               service.Spreadsheets.Values.Get(spreadsheetId, range);
        //    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        //    ValueRange getResponse = getRequest.Execute();
        //    IList<IList<Object>> getValues = getResponse.Values;
        //    if (getValues == null)
        //    {
        //        // spreadsheet is empty return Row A Column A  
        //        return "A:A";
        //    }

        //    int currentCount = getValues.Count() + 1;
        //    String newRange = "A" + currentCount + ":A";
        //    return newRange;
        //}
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
