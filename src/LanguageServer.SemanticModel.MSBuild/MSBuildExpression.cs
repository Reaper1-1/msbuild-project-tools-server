using Sprache;
using System;
using System.Linq;

namespace MSBuildProjectTools.LanguageServer.SemanticModel
{
    using MSBuildExpressions;
    using Utilities;

    /// <summary>
    ///     Helper methods for working with MSBuild expressions.
    /// /// </summary>
    public static class MSBuildExpression
    {
        /// <summary>
        ///     Parse an MSBuild expression.
        /// </summary>
        /// <param name="expression">
        ///     The expression to parse.
        /// </param>
        /// <returns>
        ///     An <see cref="ExpressionTree"/> representing the root of the expression tree.
        /// </returns>
        /// <exception cref="ParseException">
        ///     The expression could not be parsed.
        /// </exception>
        public static ExpressionTree Parse(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var parseResult = Parsers.Root.TryParse(expression);
            if (!parseResult.WasSuccessful)
            {
                string expectations = string.Empty;
                if (parseResult.Expectations.Any())
                {
                    expectations = string.Format(" (expected {0} at {1})",
                        string.Join(", ", parseResult.Expectations.Select(
                            expectation => string.Format("'{0}'", expectation)
                        )),
                        parseResult.Remainder
                    );
                }

                throw new ParseException(
                    string.Format("Failed to parse expression '{0}'{1}.", expression, expectations)
                );
            }

            return parseResult.Value.PostParse(
                new TextPositions(expression)
            );
        }

        /// <summary>
        ///     Try to parse an MSBuild expression.
        /// </summary>
        /// <param name="expression">
        ///     The expression to parse.
        /// </param>
        /// <param name="parsedExpression">
        ///     If successful, receives an <see cref="ExpressionTree"/> representing the root of the expression tree.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the expression was successfully parsed; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParse(string expression, out ExpressionTree parsedExpression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var parseResult = Parsers.Root.TryParse(expression);
            if (parseResult.WasSuccessful)
            {
                parsedExpression = parseResult.Value.PostParse(
                    new TextPositions(expression)
                );

                return true;
            }

            parsedExpression = null;

            return false;
        }

        /// <summary>
        ///     Parse a simple list.
        /// </summary>
        /// <param name="expression">
        ///     The MSBuild expression to parse.
        /// </param>
        /// <returns>
        ///     A <see cref="SimpleList"/> node representing the list and its items.
        /// </returns>
        public static SimpleList ParseSimpleList(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var parseResult = Parsers.SimpleLists.List.TryParse(expression);
            if (!parseResult.WasSuccessful)
            {
                string expectations = string.Empty;
                if (parseResult.Expectations.Any())
                {
                    expectations = string.Format(" (expected {0} at {1})",
                        string.Join(", ", parseResult.Expectations.Select(
                            expectation => string.Format("'{0}'", expectation)
                        )),
                        parseResult.Remainder
                    );
                }

                throw new ParseException(
                    string.Format("Failed to parse simple list{0}.", expectations)
                );
            }

            return parseResult.Value.PostParse(
                new TextPositions(expression)
            );
        }
    }
}
