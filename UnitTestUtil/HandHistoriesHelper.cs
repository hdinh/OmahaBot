using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnitTestUtil
{
    public static class HandHistoriesHelper
    {
        public static string GetFile(string handHistoryName)
        {
            return File.ReadAllText(Path.Combine(@"..\..\..\Tests\HandHistories", handHistoryName + ".txt"));
        }
    }
}
