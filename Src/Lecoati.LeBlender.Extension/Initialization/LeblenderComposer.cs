using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Lecoati.LeBlender.Extension.Events
{
	public class LeblenderComposer : IUserComposer
	{
		public void Compose( Composition composition )
		{
			composition.Components().Append<ApplicationInitializer>();
		}
	}
}