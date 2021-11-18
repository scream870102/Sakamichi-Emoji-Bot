using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Seb.Misc;

namespace Seb.Sheet
{
	static class SpreadSheet
	{
		// Sheet Link : https://docs.google.com/spreadsheets/d/1ONaFSfaRiLkatqb_hMh8d0PEZkREwYcEXpO-AJ1XRWs/edit#gid=0
		public static Dictionary<ulong, List<SheetRawValue>> RawValues { get; private set; }
		private static string ApplicationName = "Sakamichi Emoji Bot";
		private static SheetsService service;
		private static Spreadsheet spreadSheet;
		private static HashSet<string> sheetTitlesCache = new();

		private static readonly string spreadsheetId = "1ONaFSfaRiLkatqb_hMh8d0PEZkREwYcEXpO-AJ1XRWs";
		private static readonly string headerRange = "A:Z";
		private static readonly string range = "A2:ZZZ";

		public static void Init()
		{
			ServiceAccountCredential credential = GetCredential();
			service = CreateSheetService(credential);
			UpdateAllValueFromRemote();
		}


		public static bool TryAdd(SheetRawValue value, ulong guild)
		{
			var sheetTitle = guild.ToString();
			if (!IsSheetContains(sheetTitle))
			{
				if (!TryAddNewSheet(sheetTitle))
				{
					return false;
				}
			}

			var content = new ValueRange();
			IList<IList<object>> list = new List<IList<object>>();
			list.Add(new List<object>());
			var current = list[0];
			current.Add(value.Id);
			current.Add(value.Author.ToString());
			current.Add(value.Url);
			foreach (var tag in value.Tags)
			{
				current.Add(tag);
			}
			content.Values = list;
			var setRequest = service.Spreadsheets.Values.Append(content, spreadsheetId, GetRange(sheetTitle));
			setRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

			var resp = setRequest.Execute();
			if (resp != null)
			{
				DataHandler.AddValue(value, guild);
			}
			return resp != null;
		}

		// //TODO:
		// public static bool TryUpdateExistContent(Guid id, string[] tags, ulong guild)
		// {
		// 	if (DataHandler.TryUpdateValue(id, tags, guild))
		// 	{

		// 	}

		// 	return false;
		// }

		public static void UpdateAllValueFromRemote()
		{
			RawValues = new Dictionary<ulong, List<SheetRawValue>>();

			var getSpreadSheetReq = service.Spreadsheets.Get(spreadsheetId);
			sheetTitlesCache.Clear();
			spreadSheet = getSpreadSheetReq.Execute();
			foreach (var sheet in spreadSheet.Sheets)
			{
				var title = sheet.Properties.Title;
				if (!ulong.TryParse(title, out var numericTitle))
				{
					continue;
				}
				if (!RawValues.ContainsKey(numericTitle))
				{
					RawValues.Add(numericTitle, new List<SheetRawValue>());
				}
				sheetTitlesCache.Add(title);
				var request = service.Spreadsheets.Values.Get(spreadsheetId, GetRange(title));
				ValueRange response = request.Execute();
				IList<IList<Object>> values = response.Values;
				if (values != null
					&& values.Count > 0)
				{
					foreach (var row in values)
					{
						var tags = new List<string>();
						for (int i = 3; i < row.Count; i++)
						{
							tags.Add((string)row[i]);
						}
						var raw = new SheetRawValue((string)row[0], (string)row[1], (string)row[2], tags.ToArray());
						RawValues[numericTitle].Add(raw);
					}
				}
				else
				{
					Console.WriteLine("No data found.");
				}
			}
		}

		private static string GetRange(string title)
		{
			return $"{title}!{range}";
		}

		private static bool TryAddNewSheet(string title)
		{
			var req = new AddSheetRequest
			{
				Properties = new SheetProperties()
			};
			req.Properties.Title = title;
			var batchRequest = new BatchUpdateSpreadsheetRequest
			{
				Requests = new List<Request>()
			};
			batchRequest.Requests.Add(new Request { AddSheet = req });
			var batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchRequest, spreadsheetId);

			var resps = batchUpdateRequest.Execute();
			if (resps != null)
			{
				foreach (var resp in resps.Replies)
				{

					if (resp.AddSheet != null)
					{
						sheetTitlesCache.Add(resp.AddSheet.Properties.Title);
						AddHeader(title);
						return true;
					}
				}
			}
			return false;
		}

		private static void AddHeader(string title)
		{
			var context = new ValueRange();
			IList<IList<object>> list = new List<IList<object>>();
			list.Add(new List<object>());
			var current = list[0];
			current.Add("Id");
			current.Add("Author");
			current.Add("Url");
			current.Add("Tags");
			context.Values = list;
			var setRequest = service.Spreadsheets.Values.Append(context, spreadsheetId, $"{title}!{headerRange}");
			setRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

			setRequest.Execute();
		}

		private static SheetsService CreateSheetService(ServiceAccountCredential credential)
		{
			// Create Google Sheets API service.
			return new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
		}

		private static bool IsSheetContains(string title)
		{
			return sheetTitlesCache.Contains(title);
		}

		private static ServiceAccountCredential GetCredential()
		{
			ServiceAccountCredential credential;
			string[] Scopes = { SheetsService.Scope.Spreadsheets };
			string serviceAccountEmail = "sakamichi-emoji@tactile-flash-317105.iam.gserviceaccount.com";
			string jsonfile = "token.json";
			using (Stream stream = new FileStream(@jsonfile, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream).UnderlyingCredential;

				var initializer = new ServiceAccountCredential.Initializer(credential.Id)
				{
					User = serviceAccountEmail,
					Key = credential.Key,
					Scopes = Scopes
				};
				credential = new ServiceAccountCredential(initializer);
			}

			return credential;
		}
	}
}