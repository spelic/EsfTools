using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsfCore
{

    public class EsfProgramProperties
    {
        public string FileName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int NumberOfLines { get; set; }
        public int NumberOfTags { get; set; }
        public List<string> Tags { get; set; }

        public List<string> LogicFuncs { get; set; }
        public List<string> SQLFuncs { get; set; }

        public DateTime LastModified { get; set; } = DateTime.MinValue;
        public EsfProgramProperties(string fileName, string name, string version, DateTime lastModified, int numLines, int numTags, List<string> tags)
        {
            FileName = fileName;
            Name = name;
            Version = version;
            LastModified = lastModified;
            NumberOfLines = numLines;
            NumberOfTags = numTags;
            Tags = tags ?? new List<string>();
        }

        // to string method to return the program name, version and last modified date, lines, number of tags
        public override string ToString()
        {
            return $"{Name} v{Version} (Last Modified: {LastModified}, Lines: {NumberOfLines}, Tags: {NumberOfTags})";
        }


    }

    public class LogicFunc
    {
        public string Name { get; set; } = string.Empty;
        public string BeforeLogic { get; set; } = string.Empty;
        public string AfterLogic { get; set; } = string.Empty;
        public LogicFunc(string name, string beforeLogic, string afterLogic, List<string> tags)
        {
            Name = name;
            BeforeLogic = beforeLogic;
            AfterLogic = afterLogic;
        }
    }

    public class SQLFunc
    {
        public string Name { get; set; } = string.Empty;
        public string SQLClause { get; set; } = string.Empty;
        public SQLFunc(string name, string sqlClause)
        {
            Name = name;
            SQLClause = sqlClause;
        }
    }

    public static class EsfUtils
    {
        // function with folder string patameter that goes throught all files in that folder with .esf extension and give the list of this files
        public static List<string> GetEsfFilesInFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));
            if (!System.IO.Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
            return System.IO.Directory.GetFiles(folderPath, "*.esf").ToList();
        }

        // function with parameter list of string esf filenames and get list of EsfProgramProperties
        public static List<EsfProgramProperties> GetLatestEsfVersions(List<string> esfFiles, bool parseTags = false)
        {
            if (esfFiles == null || esfFiles.Count == 0)
                throw new ArgumentException("The list of ESF files cannot be null or empty.", nameof(esfFiles));
            var programVersions = new Dictionary<string, EsfProgramProperties>();
            foreach (var file in esfFiles)
            {
                var fileInfo = new System.IO.FileInfo(file);
                if (!fileInfo.Exists)
                    continue;
                // Extract program name and version from the filename
                var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
                var parts = fileNameWithoutExtension.Split('-');
                if (parts.Length < 2)
                    continue; // Invalid format, skip this file
                var programName = parts[0];
                var version = parts[1];
                // If not parsing tags, set defaults
                var numberOfLines = 0;
                var numberOfTags = 0;
                var tags = new List<string>();

                if (parseTags)
                {
                    var lines = File.ReadAllLines(file, Encoding.GetEncoding(1250));
                    numberOfLines = lines.Length;
                    numberOfTags = lines.Count(line => line.StartsWith(":")); // Count lines starting with ':'
                    tags = lines
                        .Where(line => line.StartsWith(":"))
                        .Select(line => line.Trim().Split(' ')[0].Trim(':').ToUpperInvariant())
                        .Distinct()
                        .ToList();
                    // remove all dots . at the end of tags
                    for (int i = 0; i < tags.Count; i++)
                    {
                        if (tags[i].EndsWith('.'))
                            tags[i] = tags[i].TrimEnd('.');
                    }

                    // remove all tags that starts with E except EZZE
                    tags = tags.Where(tag => !tag.StartsWith("E") || tag.Equals("EZZE", StringComparison.OrdinalIgnoreCase)).ToList();

                    // remove all tags that starts with ROW and has dot . inside like ROW.A
                    tags = tags.Where(tag => !tag.StartsWith("ROW") || !tag.Contains('.')).ToList();

                }

                // Check if we already have this program
                if (!programVersions.TryGetValue(programName, out var existingVersion) ||
                    string.Compare(version, existingVersion.Version, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // Update with the latest version
                    programVersions[programName] = new EsfProgramProperties(file, programName, version, fileInfo.LastWriteTime, numberOfLines, numberOfTags, tags);
                }
            }
            return programVersions.Values.ToList();
        }



    }
}
