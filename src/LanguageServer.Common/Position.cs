using System;

namespace MSBuildProjectTools.LanguageServer
{
    /// <summary>
    ///     Represents a position in a text document.
    /// </summary>
    public readonly struct Position
        : IEquatable<Position>, IComparable<Position>, IComparable<Range>
    {
        /// <summary>
        ///     An invalid position (-1,-1).
        /// </summary>
        public static readonly Position Invalid = new Position(-1, -1);

        /// <summary>
        ///     The origin position (1,1).
        /// </summary>
        public static readonly Position Origin = new Position(1, 1);

        /// <summary>
        ///     The zero position (0,0).
        /// </summary>
        public static readonly Position Zero = new Position(0, 0);

        /// <summary>
        ///     Create a new <see cref="Position"/>.
        /// </summary>
        /// <param name="lineNumber">
        ///     The line number (1-based).
        /// </param>
        /// <param name="columnNumber">
        ///     The column number (1-based).
        /// </param>
        public Position(int lineNumber, int columnNumber)
            : this(lineNumber, columnNumber, isZeroBased: false)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Position"/>.
        /// </summary>
        /// <param name="lineNumber">
        ///     The line number (1-based, unless <paramref name="isZeroBased"/> is <c>true</c>).
        /// </param>
        /// <param name="columnNumber">
        ///     The column number (1-based, unless <paramref name="isZeroBased"/> is <c>true</c>).
        /// </param>
        /// <param name="isZeroBased">
        ///     If true, then the position will be treated as 0-based.
        /// </param>
        private Position(int lineNumber, int columnNumber, bool isZeroBased)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            IsZeroBased = isZeroBased;
        }

        /// <summary>
        ///     The line number (1-based, unless <see cref="IsZeroBased"/> is <c>true</c>).
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        ///     The column number (1-based, unless <see cref="IsZeroBased"/> is <c>true</c>).
        /// </summary>
        public int ColumnNumber { get; }

        /// <summary>
        ///     Is the position 0-based (instead of 1-based)?
        /// </summary>
        public bool IsZeroBased { get; }

        /// <summary>
        ///     Is the position 1-based?
        /// </summary>
        public bool IsOneBased => !IsZeroBased;

        /// <summary>
        ///     Create a copy of the <see cref="Position"/> with the specified line number.
        /// </summary>
        /// <param name="lineNumber">
        ///     The new line number.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position WithLineNumber(int lineNumber) => new Position(lineNumber, ColumnNumber, IsZeroBased);

        /// <summary>
        ///     Create a copy of the <see cref="Position"/> with the specified column number.
        /// </summary>
        /// <param name="columnNumber">
        ///     The new column number.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position WithColumnNumber(int columnNumber) => new Position(LineNumber, columnNumber, IsZeroBased);

        /// <summary>
        ///     Create a copy of the <see cref="Position"/>, moving by the specified number of lines and / or columns.
        /// </summary>
        /// <param name="lineCount">
        ///     The number of lines (if any) to move by.
        /// </param>
        /// <param name="columnCount">
        ///     The number of columns (if any) to move by.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position Move(int lineCount = 0, int columnCount = 0)
        {
            return new Position(LineNumber + lineCount, ColumnNumber + columnCount, IsZeroBased);
        }

        /// <summary>
        ///     Create a new <see cref="Position"/> relative to the specified target position.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     The new position.
        /// </returns>
        public Position RelativeTo(Position position)
        {
            if (IsOneBased)
                position = position.ToOneBased();
            else if (IsZeroBased)
                position = position.ToZeroBased();

            Position origin = IsOneBased ? Origin : Zero;

            return Move(
                lineCount: origin.LineNumber - position.LineNumber,
                columnCount: origin.ColumnNumber - position.ColumnNumber
            );
        }

        /// <summary>
        ///     Create a new <see cref="Position"/> using the specified target position as the origin.
        /// </summary>
        /// <param name="position">
        ///     The target position.
        /// </param>
        /// <returns>
        ///     The new position.
        /// </returns>
        /// <remarks>
        ///     Moves the current position away from the origin by the target position's line and column counts.
        /// </remarks>
        public Position WithOrigin(Position position)
        {
            if (IsOneBased)
                position = position.ToOneBased();
            else if (IsZeroBased)
                position = position.ToZeroBased();

            Position origin = IsOneBased ? Origin : Zero;

            return Move(
                lineCount: position.LineNumber - origin.LineNumber,
                columnCount: position.ColumnNumber - origin.ColumnNumber
            );
        }

        /// <summary>
        ///     Create an <see cref="Range"/> that starts and ends at the <see cref="Position"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="Range"/>.
        /// </returns>
        public Range ToEmptyRange() => new Range(start: this, end: this);

        /// <summary>
        ///     Determine whether the position is equal to another object.
        /// </summary>
        /// <param name="other">
        ///     The other object.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the position and object are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            if (other is Position otherPosition)
                return Equals(otherPosition);

            return false;
        }

        /// <summary>
        ///     Get a hash code to represent the position.
        /// </summary>
        /// <returns>
        ///     The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 17;

            unchecked
            {
                hashCode += IsZeroBased ? LineNumber : LineNumber - 1;
                hashCode *= 37;

                hashCode += IsZeroBased ? ColumnNumber : ColumnNumber - 1;
                hashCode *= 37;
            }

            return hashCode;
        }

        /// <summary>
        ///     Determine whether the position is equal to another position.
        /// </summary>
        /// <param name="other">
        ///     The other position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the positions are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Position other)
        {
            if (IsOneBased)
                other = other.ToOneBased();
            else if (IsZeroBased)
                other = other.ToZeroBased();

            return other.LineNumber == LineNumber && other.ColumnNumber == ColumnNumber;
        }

        /// <summary>
        ///     Compare the position to another position.
        /// </summary>
        /// <param name="other">
        ///     The other position.
        /// </param>
        /// <returns>
        ///     0 if the positions are equal, greater than 0 if the other position is less than the current position, less than 0 if the other position is greater than the current position.
        /// </returns>
        public int CompareTo(Position other)
        {
            if (IsOneBased)
                other = other.ToOneBased();
            else if (IsZeroBased)
                other = other.ToZeroBased();

            int lineComparison = LineNumber.CompareTo(other.LineNumber);
            if (lineComparison != 0)
                return lineComparison;

            return ColumnNumber.CompareTo(other.ColumnNumber);
        }

        /// <summary>
        ///     Compare the position to a range.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     0 if the positions is within the range, greater than 0 if the position lies after than range, less than 0 position lies before the range.
        /// </returns>
        public int CompareTo(Range range)
        {
            int comparison = CompareTo(range.Start);
            if (comparison < 0)
                return comparison;

            // AF: This comparison is may now be suspect - remember that .Contains no longer includes the end position.

            comparison = CompareTo(range.End);
            if (comparison > 0)
                return comparison;

            return 0;
        }

        /// <summary>
        ///     Get a string representation of the position.
        /// </summary>
        /// <returns>
        ///     The string representation "LineNumber,ColumnNumber".
        /// </returns>
        public override string ToString() => string.Format("{0},{1}", LineNumber, ColumnNumber);

        /// <summary>
        ///     Convert the position to a one-based position.
        /// </summary>
        /// <returns>
        ///     The converted position (or the existing position if it's already one-based).
        /// </returns>
        public Position ToOneBased() => IsZeroBased ? new Position(LineNumber + 1, ColumnNumber + 1, false) : this;

        /// <summary>
        ///     Convert the position to a zero-based position.
        /// </summary>
        /// <returns>
        ///     The converted position (or the existing position if it's already zero-based).
        /// </returns>
        public Position ToZeroBased() => IsOneBased ? new Position(LineNumber - 1, ColumnNumber - 1, true) : this;

        /// <summary>
        ///     Create a new 0-based <see cref="Position"/>.
        /// </summary>
        /// <param name="lineNumber">
        ///     The line number (0-based).
        /// </param>
        /// <param name="columnNumber">
        ///     The column number (0-based).
        /// </param>
        public static Position FromZeroBased(int lineNumber, int columnNumber)
        {
            return new Position(lineNumber, columnNumber, isZeroBased: true);
        }

        /// <summary>
        ///     Create a new 0-based <see cref="Position"/>.
        /// </summary>
        /// <param name="lineNumber">
        ///     The line number (0-based).
        /// </param>
        /// <param name="columnNumber">
        ///     The column number (0-based).
        /// </param>
        public static Position FromZeroBased(long lineNumber, long columnNumber)
        {
            // Seriously, who has an XML document with more lines or columns than you can fit in an Int32?
            return FromZeroBased(
                (int)lineNumber,
                (int)columnNumber
            );
        }

        /// <summary>
        ///     Test 2 positions for equality.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the positions are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Position position1, Position position2)
        {
            return position1.Equals(position2);
        }

        /// <summary>
        ///     Test 2 positions for inequality.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the positions are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Position position1, Position position2)
        {
            return !position1.Equals(position2);
        }

        /// <summary>
        ///     Determine if a position is greater than another position.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position1"/> is greater than <paramref name="position2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(Position position1, Position position2)
        {
            return position1.CompareTo(position2) > 0;
        }

        /// <summary>
        ///     Determine if a position is greater than another position.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position1"/> is greater than <paramref name="position2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(Position position1, Position position2)
        {
            return position1.CompareTo(position2) >= 0;
        }

        /// <summary>
        ///     Determine if a position is less than another position.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position1"/> is greater than <paramref name="position2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(Position position1, Position position2)
        {
            return position1.CompareTo(position2) < 0;
        }

        /// <summary>
        ///     Determine if a position is less than another position.
        /// </summary>
        /// <param name="position1">
        ///     The first position.
        /// </param>
        /// <param name="position2">
        ///     The second position.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position1"/> is greater than <paramref name="position2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(Position position1, Position position2)
        {
            return position1.CompareTo(position2) <= 0;
        }

        /// <summary>
        ///     Determine if a position lies after a range.
        /// </summary>
        /// <param name="position">
        ///     The first range.
        /// </param>
        /// <param name="range">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position"/> is greater than <paramref name="range"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(Position position, Range range)
        {
            return position.CompareTo(range) > 0;
        }

        /// <summary>
        ///     Determine if a position lies after or within a range.
        /// </summary>
        /// <param name="position">
        ///     The first range.
        /// </param>
        /// <param name="range">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position"/> is greater than <paramref name="range"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(Position position, Range range)
        {
            return position.CompareTo(range) >= 0;
        }

        /// <summary>
        ///     Determine if a position lies before a range.
        /// </summary>
        /// <param name="position">
        ///     The first range.
        /// </param>
        /// <param name="range">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position"/> is greater than <paramref name="range"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(Position position, Range range)
        {
            return position.CompareTo(range) < 0;
        }

        /// <summary>
        ///     Determine if a position lies before or within a range.
        /// </summary>
        /// <param name="position">
        ///     The first range.
        /// </param>
        /// <param name="range">
        ///     The second range.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="position"/> is greater than <paramref name="range"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(Position position, Range range)
        {
            return position.CompareTo(range) <= 0;
        }
    }
}
