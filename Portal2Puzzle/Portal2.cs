using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using Portal2;

namespace Portal2
{
    /// <summary>
    /// The type of error that occurred at startup.
    /// </summary>
    public enum Portal2StartupError
    {
        None,
        RegistryKeyMissing,
        SteamDirectoryMissing,
        Portal2DirectoryMissing,
        PuzzleDirectoryMissing
    }

    /// <summary>
    /// Contains information on the location of Portal2 files.
    /// </summary>
    public static class Game
    {
        #region Events

        /// <summary>
        /// Register with this if there was a puzzle load error during startup, and it will call the event handler
        /// if a puzzle directory is found while running.
        /// </summary>
        public static event EventHandler PuzzlePathFound;

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not there was an error loading Portal 2 information.
        /// </summary>
        public static Portal2StartupError Error { get; private set; }

        /// <summary>
        /// The error text if there was an error.
        /// </summary>
        public static string ErrorText { get; private set; }

        /// <summary>
        /// The path to the root steam directory.
        /// </summary>
        public static string SteamPath { get; private set; }

        /// <summary>
        /// The path to the root Portal 2 directory.
        /// </summary>
        public static string Portal2Path { get; private set; }

        /// <summary>
        /// The paths to user puzzle directories.
        /// </summary>
        public static string[] PuzzleUserPaths { get; private set; }

        #endregion

        #region Initialization

        static Game()
        {
            Error = Portal2StartupError.None;
            ErrorText = null;

            // Find steam directory
            RegistryKey steamKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            if (steamKey == null)
            {
                SetError(Portal2StartupError.RegistryKeyMissing, "Can't find Steam registry key, make sure you have Steam installed.");
                return;
            }

            SteamPath = steamKey.GetValue("SteamPath") as string;
            if (string.IsNullOrEmpty(SteamPath))
            {
                SetError(Portal2StartupError.SteamDirectoryMissing, "Can't find Steam directory, make sure you have Steam installed.");
                return;
            }

            // Find portal 2 directory
            Portal2Path = Path.Combine(SteamPath, @"steamapps/common/portal 2").Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(Portal2Path))
            {
                SetError(Portal2StartupError.Portal2DirectoryMissing, "Can't find Portal 2 directory, make sure you have Portal 2 installed.");
                return;
            }

            // Find puzzles directories
            // (could be multiple)
            string puzzlesPath = Path.Combine(Portal2Path, @"portal2/puzzles").Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(puzzlesPath))
            {
                // TODO: this shouldn't really be an error - we'll just have to watch for these paths to be created if they don't exist
                SetError(Portal2StartupError.PuzzleDirectoryMissing, "Can't find Portal 2 puzzles directory, make sure you have used the puzzle editor");
                CreateWacher();
                return;
            }

            PuzzleUserPaths = Directory.GetDirectories(puzzlesPath);
            if (PuzzleUserPaths.Length == 0)
            {
                // TODO: This shouldn't be an error - just wait for it to be created
                SetError(Portal2StartupError.PuzzleDirectoryMissing, "No puzzles directory found, make sure you have used the puzzle editor.");
                CreateWacher();
                return;
            }
        }

        static void CreateWacher()
        {
            string puzzlesRoot = Path.Combine(Portal2Path, "portal2").Replace('/', Path.DirectorySeparatorChar);
            FileSystemWatcher puzzlesWatcher = new FileSystemWatcher(puzzlesRoot);
            puzzlesWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Attributes;
            puzzlesWatcher.Created += new FileSystemEventHandler(puzzlesWatcher_Created);
            puzzlesWatcher.EnableRaisingEvents = true;
        }

        static void puzzlesWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // If the puzzle paths don't exist, this isn't the directory creation we were looking for
            string puzzlesPath = Path.Combine(Portal2Path, @"portal2/puzzles").Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(puzzlesPath))
                return;

            PuzzleUserPaths = Directory.GetDirectories(puzzlesPath);
            if (PuzzleUserPaths.Length == 0)
                return;

            // Puzzles exist, unset error and call any event handlers
            Error = Portal2StartupError.None;
            ErrorText = null;

            FileSystemWatcher puzzlesWatcher = sender as FileSystemWatcher;
            if (puzzlesWatcher != null)
                puzzlesWatcher.Created -= new FileSystemEventHandler(puzzlesWatcher_Created);

            if (PuzzlePathFound != null)
                PuzzlePathFound(null, EventArgs.Empty);
        }

        static void SetError(Portal2StartupError errorType, string errorText)
        {
            Error = errorType;
            ErrorText = errorText;
        }

        #endregion
    }
}
