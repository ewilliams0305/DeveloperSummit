using System.Text;

namespace ModernCsharp.ValueObjects;

/// <summary>
/// A Version number containing an optional pre-release tag
/// </summary>
public sealed class EntityVersion : IComparable<EntityVersion>
{
    /// <summary>
    /// Attempts to parse a version string into a tagged entity version
    /// </summary>
    /// <param name="versionString">1.2.5-alpha.2 M.m.b-tag.rev or 1.2.5</param>
    /// <param name="entityVersion">out version if string is valid</param>
    /// <returns>True if valid</returns>
    public static bool TryParseVersion(string? versionString, out EntityVersion entityVersion)
    {
        if (!string.IsNullOrEmpty(versionString))
        {
            return versionString!.TryParseVersion(out entityVersion);
        }

        entityVersion = Empty;
        return false;
    }

    /// <summary>
    /// Parses a version string into a tagged entity version
    /// </summary>
    /// <param name="versionString">1.2.5-alpha.2 M.m.b-tag.rev or 1.2.5</param>
    /// <returns>new entity version</returns>
    public static EntityVersion ParseVersion(string versionString)
    {
        return versionString.ParseVersion();
    }

    /// <summary>
    /// Creates an empty default version.
    /// </summary>
    public static EntityVersion Empty => new EntityVersion(0, 0, 0);

    /// <summary>
    /// The major minor build version
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// The optional pre-release tag.
    /// <remarks>The pre-release tag will be stored without the hyphen</remarks>
    /// </summary>
    public string? PreReleaseLabel { get; }

    /// <summary>
    /// True when the version is appended with a -xxx.1.1 version
    /// </summary>
    public bool IsPreRelease => PreReleaseLabel is not null;

    /// <summary>
    /// True when the version is an officially released version.
    /// </summary>
    public bool IsRelease => PreReleaseLabel is null;

    /// <summary>
    /// Creates a new entity version with an option pre-release tag
    /// </summary>
    /// <param name="major">Major rev</param>
    /// <param name="minor">Minor rev</param>
    /// <param name="build">Build</param>
    /// <param name="preReleaseLabel">tag</param>
    /// <param name="revision">Revision</param>
    public EntityVersion(int major, int minor, int build, string? preReleaseLabel = null, int revision = 0)
    {
        Version = new Version(major, minor, build, revision);
        PreReleaseLabel = preReleaseLabel;
    }

    /// <summary>
    /// Creates a new entity version with an option pre-release tag
    /// </summary>
    /// <param name="version">version information</param>
    /// <param name="preReleaseLabel">pre-release tag</param>
    public EntityVersion(Version version, string? preReleaseLabel = null)
    {
        Version = version;
        PreReleaseLabel = preReleaseLabel;
    }

    /// <summary>
    /// Returns the version with the pre-lease tag
    /// <example>1.2.5-alpha.2 M.m.b-tag.rev</example>
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        PreReleaseLabel is null
            ? Version.ToString(Version.Revision > 0 ? 4 : 3)
            : new StringBuilder(30).Append(Version.Major).Append('.').Append(Version.Minor).Append('.').Append(Version.Build).Append('-').Append(PreReleaseLabel).Append('.').Append(Version.Revision).ToString();

    /// <inheritdoc />
    public int CompareTo(EntityVersion other)
    {
        var versionComparison = Version.CompareTo(other.Version);
        if (versionComparison != 0)
        {
            return versionComparison;
        }
        if (string.IsNullOrEmpty(PreReleaseLabel) && string.IsNullOrEmpty(other.PreReleaseLabel))
        {
            return 0;
        }
        if (string.IsNullOrEmpty(PreReleaseLabel))
        {
            return 1;
        }
        if (string.IsNullOrEmpty(other.PreReleaseLabel))
        {
            return -1;
        }

        return string.Compare(PreReleaseLabel, other.PreReleaseLabel, StringComparison.Ordinal);
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is EntityVersion other)
        {
            return Version.Equals(other.Version) && string.Equals(PreReleaseLabel, other.PreReleaseLabel, StringComparison.Ordinal);
        }
        return false;
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return (Version.GetHashCode() * 397) ^ (PreReleaseLabel != null ? PreReleaseLabel.GetHashCode() : 0);
        }
    }
}