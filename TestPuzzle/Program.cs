using System;
using System.Collections.Generic;
using System.Text;
using Portal2;
using System.IO;

namespace TestPuzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure there was no startup error.
            if (Game.Error != Portal2StartupError.None)
            {
                return;
            }

            // NOTE: Each of these parameters controls which parts of the test are generated.
            // They can't all be generated at once - it is too many items for Portal 2 to
            // compile. See the Test class for more information.
            Puzzle puzzle = Test.GetTestPuzzle(
                true, false, true,
                false, false, true,
                false, true, false,
                false, true, true,
                true, true, true,
                true, true, true,
                true);

            // Save the puzzle.
            puzzle.Save(Path.Combine(Game.PuzzleUserPaths[0], "test.p2c"));
        }
    }
}
