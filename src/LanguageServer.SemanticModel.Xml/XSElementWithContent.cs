using Microsoft.Language.Xml;

namespace MSBuildProjectTools.LanguageServer.SemanticModel
{
    /// <summary>
    ///     Represents an XML element with content.
    /// </summary>
    public class XSElementWithContent
        : XSElement
    {
        /// <summary>
        ///     The range, within the source text, spanned by the node.
        /// </summary>
        /// <param name="element">
        ///     The <see cref="XmlElementSyntax"/> represented by the <see cref="XSElementWithContent"/>.
        /// </param>
        /// <param name="range">
        ///     The <see cref="Range"/>, within the source text, spanned by the element and its content.
        /// </param>
        /// <param name="nameRange">
        ///     The range, within the source text, spanned by the element's name.
        /// </param>
        /// <param name="openingTagRange">
        ///     The <see cref="Range"/>, within the source text, spanned by the element's opening tag.
        /// </param>
        /// <param name="attributesRange">
        ///     The range, within the source text, spanned by the element's attributes.
        /// </param>
        /// <param name="contentRange">
        ///     The <see cref="Range"/>, within the source text, spanned by the element's content.
        /// </param>
        /// <param name="closingTagRange">
        ///     The <see cref="Range"/>, within the source text, spanned by the element's closing tag.
        /// </param>
        /// <param name="parent">
        ///     The <see cref="XSElementWithContent"/>'s parent element (if any).
        /// </param>
        public XSElementWithContent(XmlElementSyntax element, Range range, Range nameRange, Range openingTagRange, Range attributesRange, Range contentRange, Range closingTagRange, XSElement parent)
            : base(element, range, nameRange, attributesRange, parent)
        {
            OpeningTagRange = openingTagRange;
            ContentRange = contentRange;
            ClosingTagRange = closingTagRange;
        }

        /// <summary>
        ///     The <see cref="XmlElementSyntax"/> represented by the <see cref="XSElementWithContent"/>.
        /// </summary>
        public new XmlElementSyntax ElementNode => (XmlElementSyntax)SyntaxNode;

        /// <summary>
        ///     The <see cref="Range"/>, within the source text, spanned by the element's opening tag.
        /// </summary>
        public Range OpeningTagRange { get; }

        /// <summary>
        ///     The <see cref="Range"/>, within the source text, spanned by the element's content.
        /// </summary>
        public Range ContentRange { get; }

        /// <summary>
        ///     The <see cref="Range"/>, within the source text, spanned by the element's closing tag.
        /// </summary>
        public Range ClosingTagRange { get; }

        /// <summary>
        ///     The kind of XML node represented by the <see cref="XSNode"/>.
        /// </summary>
        public override XSNodeKind Kind => XSNodeKind.Element;

        /// <summary>
        ///     Does the <see cref="XSNode"/> represent valid XML?
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        ///     Does the <see cref="XSElement"/> have any content (besides attributes)?
        /// </summary>
        public override bool HasContent => Content.Count > 0;
    }
}
