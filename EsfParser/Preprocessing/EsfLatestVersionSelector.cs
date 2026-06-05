using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Preprocessing;

/// <summary>
/// Pure (no I/O) selection of the latest version of each logical program from a set of file
/// paths. Grouping is case-insensitive on the program name; versions are compared numerically.
/// Deterministic tie-break for files at the same highest version: ordinal-ascending full path.
/// </summary>
public static class EsfLatestVersionSelector
{
    public static LatestVersionSelectionResult Select(IEnumerable<string> esfFilePaths)
    {
        if (esfFilePaths is null) throw new ArgumentNullException(nameof(esfFilePaths));

        var infos = esfFilePaths.Select(EsfFileVersionInfo.Parse).ToList();

        var selected = new List<EsfFileVersionInfo>();
        var ignored = new List<EsfFileVersionInfo>();
        var duplicates = new List<DuplicateLatestWarning>();

        foreach (var group in infos.GroupBy(i => i.ProgramName, StringComparer.OrdinalIgnoreCase))
        {
            long maxVersion = group.Max(i => i.Version);

            var atMax = group
                .Where(i => i.Version == maxVersion)
                .OrderBy(i => i.FullPath, StringComparer.Ordinal)
                .ToList();

            var winner = atMax[0];
            selected.Add(winner);

            // Files at a lower version are simply ignored.
            ignored.AddRange(group.Where(i => i.Version != maxVersion));

            // More than one file at the highest version → warn and ignore the losers.
            if (atMax.Count > 1)
            {
                duplicates.Add(new DuplicateLatestWarning(
                    winner.ProgramName,
                    maxVersion,
                    atMax.Select(i => i.FullPath).ToList()));
                ignored.AddRange(atMax.Skip(1));
            }
        }

        return new LatestVersionSelectionResult
        {
            Selected = selected,
            Ignored = ignored,
            DuplicateWarnings = duplicates,
        };
    }
}
