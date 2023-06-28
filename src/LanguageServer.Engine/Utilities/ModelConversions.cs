using LspModels = OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace MSBuildProjectTools.LanguageServer.Utilities
{
    /// <summary>
    ///     Extension methods for converting models between native and third-party representations.
    /// </summary>
    public static class ModelConversions
    {
        /// <summary>
        ///     Convert the <see cref="Position"/> to its Language Server Protocol equivalent.
        /// </summary>
        /// <param name="position">
        ///     The <see cref="Position"/> to convert.
        /// </param>
        /// <returns>
        ///     The equivalent <see cref="LspModels.Position"/>.
        /// </returns>
        public static LspModels.Position ToLsp(this Position position)
        {
            position = position.ToZeroBased(); // LSP is zero-based.

            return new LspModels.Position(
                position.LineNumber,
                position.ColumnNumber
            );
        }

        /// <summary>
        ///     Convert the Language Server Protocol <see cref="LspModels.Position"/> to its native equivalent.
        /// </summary>
        /// <param name="position">
        ///     The <see cref="LspModels.Position"/> to convert.
        /// </param>
        /// <returns>
        ///     The equivalent <see cref="Position"/>.
        /// </returns>
        public static Position ToNative(this LspModels.Position position)
        {
            // LSP is zero-based.
            return Position.FromZeroBased(
                position.Line,
                position.Character
            ).ToOneBased();
        }

        /// <summary>
        ///     Convert the <see cref="Range"/> to its Language Server Protocol equivalent.
        /// </summary>
        /// <param name="range">
        ///     The <see cref="Range"/> to convert.
        /// </param>
        /// <returns>
        ///     The equivalent <see cref="LspModels.Range"/>.
        /// </returns>
        public static LspModels.Range ToLsp(this Range range)
        {
            return new LspModels.Range(
                range.Start.ToLsp(),
                range.End.ToLsp()
            );
        }

        /// <summary>
        ///     Convert the Language Server Protocol <see cref="LspModels.Range"/> to its native equivalent.
        /// </summary>
        /// <param name="range">
        ///     The <see cref="LspModels.Range"/> to convert.
        /// </param>
        /// <returns>
        ///     The equivalent <see cref="Range"/>.
        /// </returns>
        public static Range ToNative(this LspModels.Range range)
        {
            return new Range(
                range.Start.ToNative(),
                range.End.ToNative()
            );
        }
    }
}
