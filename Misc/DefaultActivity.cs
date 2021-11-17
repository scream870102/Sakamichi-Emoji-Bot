using Discord;

namespace Seb.Misc
{
	public class DefaultActivity : IActivity
	{
		public DefaultActivity()
		{
		}

		public string Name => "モニモニダンスを踊ろ";

		public ActivityType Type => ActivityType.Playing;

		public ActivityProperties Flags => ActivityProperties.None;

		public string Details => "我覺得桃推應該都不是個臭DD吧";
	}
}
