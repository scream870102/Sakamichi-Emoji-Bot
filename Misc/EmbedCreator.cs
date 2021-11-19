using Discord;

namespace Seb.Misc
{
	static class EmbedCreator
	{
		public static EmbedBuilder CreateBasicEmbedBuilder(string title, bool isWithFooter = true)
		{
			var builder = new EmbedBuilder()
				.WithColor(new Color(240, 93, 154))
				.WithTitle(title)
				.WithCurrentTimestamp();
			if (isWithFooter)
			{
				var footer = new EmbedFooterBuilder()
					.WithIconUrl(@"https://i.imgur.com/oC0Yo5s.png")
					.WithText($"Created by scream870102");
				builder.WithFooter(footer);
			}
			return builder;
		}
	}

}