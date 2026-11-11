using AltTwist.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace AltTwist.Configuration
{
    public class Config : Configurable<Config>
    {
        public enum EndingEnum
        {
            [Display(Name = "Conditional")]
            Conditional,
            [Display(Name = "True Ending")]
            TrueEnd,
            [Display(Name = "Bad Ending")]
            BadEnd
        }

        [Category("Ending")]
        [DisplayName("Force Ending")]
		[Description("Force an ending instead of relying on Sumire's confidant rank\nConditional: True ending if Rank 10, bad ending if less than 10\nTrue Ending: Kasumi always survives\nBad Ending: Kasumi always dies")]
		[DefaultValue(EndingEnum.Conditional)]
        [Display(Order = 1)]
		public EndingEnum EndingSelect { get; set; }

        [Category("Language")]
        [DisplayName("Japanese Voice Lines")]
		[Description("Use Japanese voice lines in scenes changed by this mod")]
		[DefaultValue(false)]
        [Display(Order = 2)]
		public bool Japanese { get; set; } = false;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}