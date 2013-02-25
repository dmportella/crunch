namespace crunch
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NDesk.Options;
    using System;
    using System.Collections.Generic;

    internal static class Program
    {
        private static AutoResetEvent completed = new AutoResetEvent(false);
        private static long errorCount = 0;

        static int Main(string[] args)
        {
            string folder = string.Empty;
            string searchFilter = "*.gif, *.png, *.jpg";
            bool showHelp = false;

            // Option values for the arguments
            var p = new OptionSet() 
            {
                { "f|folder=", "the {FOLDER} to be processed.", v => folder = v },
                { "s|search=", "the {SEARCH} file search filter to be used.", v => searchFilter = v},
                { "h|help",  "show this message and exit", v => showHelp = v != null }
            };

            // Extra values found we will ingnore those.
            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                // On error print help message and quit with error code 1.
                Console.Write("crunch: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return 1;
            }

            // user requested the help message show it and quit application.
            if (showHelp)
            {
                ShowHelp(p);
                return 0;
            }

            // check if input file exists if not exit with error code 2.
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Folder '{0}' doesnt exists.", folder);
                return 2;
            }

            Stopwatch stopwatch = new Stopwatch();
            
            ImageCruncher.Cruncher cruncher = new ImageCruncher.Cruncher();

            List<string> files = new List<string>(DirectoryExtensions.GetFilesEx(folder, searchFilter, SearchOption.AllDirectories));

            cruncher.Completed += cruncher_Completed;
            cruncher.Progress += cruncher_Progress;

            stopwatch.Start();
            
            cruncher.CrunchImages(files.ToArray());

            stopwatch.Stop();

            completed.WaitOne();

            Console.WriteLine("Finished processing files, time taken '{0}' in milliseconds.", stopwatch.Elapsed.TotalMilliseconds);
            Console.WriteLine("Number of error '{0}'.", errorCount);
            Console.WriteLine("Hmm...Muffins.");

            return 0;
        }

        private static void cruncher_Progress(object sender, ImageCruncher.CruncherEventArgs e)
        {
            Console.WriteLine("'{0}', Has error='{1}', Saved='{2}', Service='{3}'", e.Result.FileName, e.Result.HasError, e.Result.PercentSaved, e.Result.Service);
            
            if (e.Result.HasError)
            {
                errorCount++;
            }
        }

        private static void cruncher_Completed(object sender, EventArgs e)
        {
            completed.Set();
        }

        /// <summary>
        /// Shows the help message for the tool.
        /// </summary>
        /// <param name="p">The option set to be displayed.</param>
        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: crunch [OPTIONS]");
            Console.WriteLine("Optimise images inside a folder structure using punypng or smush.it.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("crunch.exe -f \"c:\folder\" -s \"*.gif, *.png, *.jpg\"");
        }
    }
}
