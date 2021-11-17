namespace Seb.Sheet
{
	class SheetRawValue
	{
		public SheetRawValue(string id, string author, string url, params string[] tags)
		{
			Id = Guid.Parse(id);
			Author = ulong.Parse(author);
			Tags = tags;
			Url = url;
		}

		public Guid Id { get; private set; }
		public ulong Author { get; private set; }
		public string[] Tags { get; private set; }
		public string Url { get; private set; }
	}
}