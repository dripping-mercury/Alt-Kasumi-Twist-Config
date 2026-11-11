using AltTwist.Configuration;
using AltTwist.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using CriFs.V2.Hook.Interfaces;
using Ryo.Interfaces;
using BF.File.Emulator;
using BF.File.Emulator.Interfaces;

namespace AltTwist
{
    public class Mod : ModBase
    {
        private readonly IModLoader _modLoader;
        private readonly IReloadedHooks? _hooks;
        private readonly ILogger _logger;
        private readonly IMod _owner;
        private Config _configuration;
        private readonly IModConfig _modConfig;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            // grab modDir and modId
            string modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);
            string modId = _modConfig.ModId;

            var criFsCtl = _modLoader.GetController<ICriFsRedirectorApi>();
            var ryoCtl = _modLoader.GetController<IRyoApi>();
            var BfEmulatorController = _modLoader.GetController<IBfEmulator>();

            if (criFsCtl == null || !criFsCtl.TryGetTarget(out var criFsApi)) { _logger.WriteLine("CRI FS Emu missing → config binds broken.", System.Drawing.Color.Red); return; }
            if (ryoCtl == null || !ryoCtl.TryGetTarget(out var ryoApi)) { _logger.WriteLine("Ryo API missing → Audio configs broken.", System.Drawing.Color.Red); return; }
            if (BfEmulatorController == null || !BfEmulatorController.TryGetTarget(out var _BfEmulator)) { _logger.WriteLine("BF Emulator API missing → file emulation broken.", System.Drawing.Color.Red); return; }

            // Sumire player bustup
            string bustupFolder = Path.Combine("AdditionalModFiles", "Bustup");
            BindAllFilesIn(
                bustupFolder, modDir, criFsApi, modId, _logger
            );

            // Ending select flowscript
            if (_configuration.EndingSelect == Config.EndingEnum.BadEnd)
            {
                var flowscriptFolder = Path.Combine(modDir, "AdditionalModFiles", "Scheduler", "ForceBadEnd");
                if (Directory.Exists(flowscriptFolder))
                {
                    _logger.WriteLine($"Found bad ending flowscript path at {flowscriptFolder}.");
                    _BfEmulator.AddDirectory(flowscriptFolder);
                }
                else
                {
                    _logger.WriteLine($"Couldn't find bad ending flowscript path at {flowscriptFolder}.", System.Drawing.Color.Red);
                }
            }
            else if (_configuration.EndingSelect == Config.EndingEnum.TrueEnd)
            {
                var flowscriptFolder = Path.Combine(modDir, "AdditionalModFiles", "Scheduler", "ForceTrueEnd");
                if (Directory.Exists(flowscriptFolder))
                {
                    _logger.WriteLine($"Found true ending flowscript path at {flowscriptFolder}.");
                    _BfEmulator.AddDirectory(flowscriptFolder);
                }
                else
                {
                    _logger.WriteLine($"Couldn't find true ending flowscript path at {flowscriptFolder}.", System.Drawing.Color.Red);
                }
            }
            else
            {
                var flowscriptFolder = Path.Combine(modDir, "AdditionalModFiles", "Scheduler", "Conditional");
                if (Directory.Exists(flowscriptFolder))
                {
                    _logger.WriteLine($"Found default conditional ending flowscript path at {flowscriptFolder}.");
                    _BfEmulator.AddDirectory(flowscriptFolder);
                }
                else
                {
                    _logger.WriteLine($"Couldn't find default conditional ending flowscript path at {flowscriptFolder}.", System.Drawing.Color.Red);
                }
            }

            // Japanese voice lines
            if (_configuration.Japanese)
            {
                var audioFolder = Path.Combine(modDir, "AdditionalModFiles", "Language", "JP");
                if (Directory.Exists(audioFolder))
                {
                    _logger.WriteLine($"Found audio path at {audioFolder}.");
                    ryoApi.AddAudioPath(audioFolder, null);
                }
                else
                {
                    _logger.WriteLine($"Couldn't find audio path at {audioFolder}.", System.Drawing.Color.Red);
                }
            }
            else
            {
                var audioFolder = Path.Combine(modDir, "AdditionalModFiles", "Language", "EN");
                if (Directory.Exists(audioFolder))
                {
                    _logger.WriteLine($"Found audio path at {audioFolder}.");
                    ryoApi.AddAudioPath(audioFolder, null);
                }
                else
                {
                    _logger.WriteLine($"Couldn't find audio path at {audioFolder}.", System.Drawing.Color.Red);
                }
            }
        }    

        /// <summary>
        /// recursively enumerates all files under the given “subPath” (relative to the mod folder),
        /// and issues a single AddBind(...) per file. If the directory doesn’t exist, it logs a warning and does nothing.
        /// </summary>
        private static void BindAllFilesIn(
            string subPathRelativeToModDir,
            string modDir,
            ICriFsRedirectorApi criFsApi,
            string modId,
            ILogger logger
        )
        {
            string absoluteFolder = Path.Combine(modDir, subPathRelativeToModDir);

            if (!Directory.Exists(absoluteFolder))
            {
                logger.WriteLine($"Folder not found: {absoluteFolder}", System.Drawing.Color.Yellow);
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(absoluteFolder, "*", SearchOption.AllDirectories))
            {
                string relativeCpkKey = Path.GetRelativePath(absoluteFolder, filePath).Replace(Path.DirectorySeparatorChar, '/');

                // logger.WriteLine($"filePath: {filePath}, relativeCpkKey {relativeCpkKey}", System.Drawing.Color.Yellow);
                // logger.WriteLine($"modId: {modId}", System.Drawing.Color.Yellow);

                criFsApi.AddBind(
                    filePath,
                    relativeCpkKey,
                    modId
                );
            }
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}
