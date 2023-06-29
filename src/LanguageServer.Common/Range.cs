using System;

namespace MSBuildProjectTools.LanguageServer
{
    // BUG: Range comparisons are currently broken. Add tests to prove that [1,1..1,10] < [1,1..1,5] (etc).

    /// <summary>
    ///     Represents a range in a text document.
    /// </summary>
    /// <remarks>
    ///     The range includes the start position, but does not (for the purposes of <see cref="Contains(Position)"/> include the end position.
    /// </remarks>
    public readonly struct Range
        : IEquatable<Range>, IComparable<Range>
    {
        /// <summary>
        ///     The zero range [0,0..0,0).
        /// </summary>
        /// <remarks>
        ///     Indicates lack of a range value; use <see cref="Empty"/> to indicate the origin.
        /// </remarks>
        public static readonly Range Zero = new Range(start: Position.Zero, end: Position.Zero);

        /// <summary>
        ///     The empty range [1,1..1,1).
        /// </summary>
        public static readonly Range Empty = new Range(start: Position.Origin, end: Position.Origin);

        /// <summary>
        ///     Create a new <see cref="Range"/>.
        /// </summary>
        /// <param name="startLine">
        ///     The range's (1-based) starting line number.
        /// </param>
        /// <param name="startColumn">
        ///     The range's (1-based) starting column number.
        /// </param>
        /// <param name="endLine">
        ///     The range's (1-based) ending line number.
        /// </param>
        /// <param name="endColumn">
        ///     The range's (1-based) ending column number.
        /// </param>
        public Range(int startLine, int startColumn, int endLine, int endColumn)
            : this(start: new Position(startLine, startColumn), end: new Position(endLine, endColumn))
        {
        }

        /// <summary>
        ///     Create a new <see cref="Range"/>.
        /// </summary>
        /// <param name="start">
        ///     The range's starting position.
        /// </param>
        /// <param name="end">
        ///     The range's ending position.
        /// </param>
        public Range(Position start, Position end)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Start position cannot be greater than end position.");

            Start = start;
            End = end;
        }

        /// <summary>
        ///     The range's starting position.
        /// </summary>
        public Position Start { get; }

        /// <summary>
        ///     The range's ending position.
        /// </summary>
        public Position End { get; }

        /// <summary>
        ///     Is the range empty?
        /// </summary>
        public bool IsEmpty => Start == End;

        /// <summary>
        ///     Create a copy of the <see cref="Range"/> with the specified starting position.
        /// </summary>
        /// <param name="start">
        ///     The new starting position.
        /// </param>
        /// <returns>
        ///     The new <see cref="Range"/>.
        /// </returns>
        public Range WithStart(Position start) => new Range(start, End);

        /// <summary>
        ///     Create a copy of the <see cref="Range"/> with the specified ending position.
        /// </summary>
        /// <param name="end">
        ///     The new ending position.
        /// </param>
        /// <returns>
        ///     The new <see cref="Range"/>.
        /// </returns>
        public Range WithEnd(Position end) => new Range(Start, end);

        /// <summary>
        ///     Transform the range by moving the start and end positions.
        /// </summary>
        /// <param name="moveStartLines">
        ///     The number of lines (if any) to move the start position.
        /// </param>
        /// <param name="moveStartColumns">
        ///     The number of columns (if any) to move the start position.
        /// </param>
        /// <param name="moveEndLines">
        ///     The number of lines (if any) to move the end position.
        /// </param>
        /// <param name="moveEndColumns">
        ///     The number of columns (if any) to move the start position.
        /// </param>
        /// <returns>
        ///     The number of columns (if any) to move the end position.
        /// </returns>
        public Range Transform(int moveStartLines = 0, int moveStartColumns = 0, int moveEndLines = 0, int moveEndColumns = 0)
        {
            return new Range(
                Start.Move(moveStartLines, moveStartColumns),
                End.Move(moveEndLines, moveEndColumns)
            );
        }

        /// <summary>
        ///     Create a new <see cref="Range"/> relative to the specified target position.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     The new <see cref="Range"/>.
        /// </returns>
        public Range RelativeTo(Position position)
        {
            return new Range(
                Start.RelativeTo(position),
                End.RelativeTo(position)
            );
        }

        /// <summary>
        ///     Create a new <see cref="Range"/> using the specified target position as the origin.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     The new <see cref="Range"/>.
        /// </returns>
        public Range WithOrigin(Position position)
        {
            return new Range(
                Start.WithOrigin(position),
                End.WithOrigin(position)
            );
        }

        /// <summary>
        ///     Determine whether the range contains the specified position.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     <c>true></c>, if the range contains the target position; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     The range includes the start position, but does not (for the purposes of <see cref="Contains(Position)"/> include the end position.
        /// </remarks>
        public bool Contains(Position position)
        {
            return position >= Start && position < End;
        }

        /// <summary>
        ///     Determine whether the range entirely contains another range.
        /// </summary>
        /// <param name="range">
        ///     The target range.
        /// </param>
        /// <returns>
        ///     <c>true></c>, if the range entirely contains the target range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Range range)
        {
            return range.Start >= Start && range.End <= End;
        }

        /// <summary>
        ///     Determine whether the range is equal to another object.
        /// </summary>
        /// <param name="other">
        ///     The other object.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the range and object are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            if (other is Range otherRange)
                return Equals(otherRange);

            return false;
        }

        /// <summary>
        ///     Get a hash code to represent the range.
        /// </summary>
        /// <returns>
        ///     The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode() + 17 * End.GetHashCode();
        }

        /// <summary>
        ///     Determine whether the range is equal to another range.
        /// </summary>
        /// <param name="other">
        ///     The other range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the ranges are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Range other)
        {
            return other.Start == Start && other.End == End;
        }

        /// <summary>
        ///     Compare the range to another range.
        /// </summary>
        /// <param name="other">
        ///     The other range.
        /// </param>
        /// <returns>
        ///     0 if the ranges are equal, greater than 0 if the other range is less than the current range, less than 0 if the other range is greater than the current range.
        /// </returns>
        public int CompareTo(Range other)
        {
            int startComparison = Start.CompareTo(other.Start);
            if (startComparison != 0)
                return startComparison;

            return End.CompareTo(other.End);
        }

        /// <summary>
        ///     Get a string representation of the range.
        /// </summary>
        /// <returns>
        ///     The string representation "[Start..End]".
        /// </returns>
        public override string ToString() => string.Format("[{0}..{1})", Start, End);

        /// <summary>
        ///     Create an empty range from the specified position.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     The new range.
        /// </returns>
        public static Range FromPosition(Position position) => new Range(position, position);

        /// <summary>
        ///     Test 2 ranges for equality.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the ranges are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Range range1, Range range2)
        {
            return range1.Equals(range2);
        }

        /// <summary>
        ///     Test 2 ranges for inequality.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the ranges are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Range range1, Range range2)
        {
            return !range1.Equals(range2);
        }

        /// <summary>
        ///     Determine if a range is greater than or equal to another range.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="range1"/> is greater than <paramref name="range2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(Range range1, Range range2)
        {
            return range1.CompareTo(range2) > 0;
        }

        /// <summary>
        ///     Determine if a range is greater than or equal to another range.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="range1"/> is greater than <paramref name="range2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(Range range1, Range range2)
        {
            return range1.CompareTo(range2) >= 0;
        }

        /// <summary>
        ///     Determine if a range is less than another range.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="range1"/> is greater than <paramref name="range2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(Range range1, Range range2)
        {
            return range1.CompareTo(range2) < 0;
        }

        /// <summary>
        ///     Determine if a range is less than another range.
        /// </summary>
        /// <param name="range1">
        ///     The first range.
        /// </param>
        /// <param name="range2">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="range1"/> is greater than <paramref name="range2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(Range range1, Range range2)
        {
            return range1.CompareTo(range2) <= 0;
        }
    }
}
