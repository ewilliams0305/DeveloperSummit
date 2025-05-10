using ModernCsharp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsharp;

public class Spans
{
    public void BytesDemo()
    {
        // Suppose we have a large buffer of data
        // And we want the first 84 bytes
        byte[] data = new byte[1024];

        // We can get a span representing the data and slice
        var span = data.AsSpan();

        var slice = span[..84];

        // Now I can access these 84 bytes without copies, I have a pointer to that memory

        foreach(var b in slice)
        {
            Console.WriteLine(b);
        }

        // using a span we can even break down the data in 4byte sections without evey allocating memory

        for (var i = 0; i < 1024; i = i + 4)
        {
            var section = span.Slice(i, 4);
        }

    }
    
    public List<string> StringDemo()
    {
        // Suppose we have a large string
        var data = "The quick brown fox jumps over the lazy river";

        return data.Split(' ').ToList();

        var span = data.AsSpan();

        // Suppose we want each word
        var words = new List<string>();

        var start = 0;

        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || span[i] == ' ')
            {
                // We can grab a slice of data for each word
                var wordSpan = span.Slice(start, i - start);
                if (!wordSpan.IsEmpty)
                {
                    words.Add(wordSpan.ToString());
                }
                start = i + 1;
            }
        }

        return words;
    }
}

// Complicated real world example
public static class EntityVersionExtensions
{
    /// <summary>
    /// Attempts to parse a version string into a tagged entity version
    /// </summary>
    /// <param name="versionString">1.2.5-alpha.2 M.m.b-tag.rev or 1.2.5</param>
    /// <param name="entityVersion">out version if string is valid</param>
    /// <returns>True if valid</returns>
    public static bool TryParseVersion(this string versionString, out EntityVersion entityVersion)
    {
        var span = versionString.AsSpan();

        var first = -1;
        var second = -1;
        var third = -1;

        try
        {
            for (var i = 0; i < span.Length; i++)
            {
                var character = span.Slice(i, 1);

                if (character[0] != '.')
                {
                    continue;
                }
                if (first == -1)
                {
                    first = i;
                    continue;
                }
                if (second == -1)
                {
                    second = i;
                    continue;

                }
                if (third == -1)
                {
                    third = i;
                    continue;
                }
            }

            if (first == -1 && second == -1)
            {
                entityVersion = EntityVersion.Empty;
                return false;
            }

            var majorChar = span.Slice(0, first);
            var minorChar = span.Slice(first + 1, second - (first + 1));

            var hyphenPosition = span.IndexOf('-');

            if (hyphenPosition > 0)
            {
                return TryParseTaggedVersion(out entityVersion, span, hyphenPosition, third, second, majorChar, minorChar);
            }

            return third == -1
                ? TryParsedVersionWithoutRev(out entityVersion, span, second, majorChar, minorChar)
                : TryParseVersionWithRev(out entityVersion, span, second, third, majorChar, minorChar);
        }
        catch (Exception)
        {
            entityVersion = EntityVersion.Empty;
            return false;
        }
    }

    public static EntityVersion ParseVersion(this string versionString)
    {
        var span = versionString.AsSpan();

        var first = -1;
        var second = -1;
        var third = -1;

        for (var i = 0; i < span.Length; i++)
        {
            var character = span.Slice(i, 1);

            if (character[0] != '.')
            {
                continue;
            }
            if (first == -1)
            {
                first = i;
                continue;
            }
            if (second == -1)
            {
                second = i;
                continue;

            }
            if (third == -1)
            {
                third = i;
                continue;
            }
        }

        if (first == -1 && second == -1)
        {
            return EntityVersion.Empty;
        }

        var majorChar = span.Slice(0, first);
        var minorChar = span.Slice(first + 1, second - (first + 1));

        var hyphenPosition = span.IndexOf('-');

        if (hyphenPosition > 0)
        {
            return ParseTaggedVersion(span, hyphenPosition, third, second, majorChar, minorChar);
        }

        return third == -1
            ? ParsedVersionWithoutRev(span, second, majorChar, minorChar)
            : ParseVersionWithRev(span, second, third, majorChar, minorChar);
    }

    private static bool TryParseVersionWithRev(out EntityVersion entityVersion, ReadOnlySpan<char> span, int second, int third, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var build = span.Slice(second + 1, third - (second + 1));
        var revision = span.Slice(third + 1, span.Length - (third + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        var revisionValue = CharArrayToInt(revision);
        entityVersion = new EntityVersion(major, minor, buildValue, revision: revisionValue);
        return true;
    }

    private static bool TryParsedVersionWithoutRev(out EntityVersion entityVersion, ReadOnlySpan<char> span, int second, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var build = span.Slice(second + 1, span.Length - (second + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        entityVersion = new EntityVersion(major, minor, buildValue);
        return true;
    }

    private static bool TryParseTaggedVersion(out EntityVersion entityVersion, ReadOnlySpan<char> span, int hyphenPosition, int third, int second, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var tag = span.Slice(hyphenPosition + 1, third - (hyphenPosition + 1));
        var build = span.Slice(second + 1, hyphenPosition - (second + 1));
        var revision = span.Slice(third + 1, span.Length - (third + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        var revisionValue = CharArrayToInt(revision);
        entityVersion = new EntityVersion(major, minor, buildValue, tag.ToString(), revisionValue);
        return true;
    }

    private static EntityVersion ParseVersionWithRev(ReadOnlySpan<char> span, int second, int third, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var build = span.Slice(second + 1, third - (second + 1));
        var revision = span.Slice(third + 1, span.Length - (third + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        var revisionValue = CharArrayToInt(revision);
        return new EntityVersion(major, minor, buildValue, revision: revisionValue);
    }

    private static EntityVersion ParsedVersionWithoutRev(ReadOnlySpan<char> span, int second, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var build = span.Slice(second + 1, span.Length - (second + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        return new EntityVersion(major, minor, buildValue);
    }

    private static EntityVersion ParseTaggedVersion(ReadOnlySpan<char> span, int hyphenPosition, int third, int second, ReadOnlySpan<char> majorChar, ReadOnlySpan<char> minorChar)
    {
        var tag = span.Slice(hyphenPosition + 1, third - (hyphenPosition + 1));
        var build = span.Slice(second + 1, hyphenPosition - (second + 1));
        var revision = span.Slice(third + 1, span.Length - (third + 1));

        var major = CharArrayToInt(majorChar);
        var minor = CharArrayToInt(minorChar);
        var buildValue = CharArrayToInt(build);
        var revisionValue = CharArrayToInt(revision);
        return new EntityVersion(major, minor, buildValue, tag.ToString(), revisionValue);
    }

    private static int CharArrayToInt(ReadOnlySpan<char> charArray)
    {
        var result = 0;

        foreach (var c in charArray)
        {
            if (c is < '0' or > '9')
            {
                throw new ArgumentException("The array contains non-numeric characters.");
            }

            result = result * 10 + (c - '0');
        }

        return result;
    }
}
