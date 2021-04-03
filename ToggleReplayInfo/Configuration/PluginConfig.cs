using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ToggleReplayInfo.Configuration
{
    public class PluginConfig
    {
        public virtual bool HideReplayInfo { get; set; } = false;
    }
}