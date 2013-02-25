using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crunch
{
    internal static class DirectoryExtensions
    {
        public static string[] GetFilesEx(string folder, string filter, SearchOption option)
        {
            List<string> result = new List<string>();

            string[] filters = filter.Split(',');

            for (int i = 0; i != filters.Length; i++)
            {
                result.AddRange(Directory.GetFiles(folder, filters[i].Trim(), option));
            }

            return result.ToArray();
        }
    }
}
