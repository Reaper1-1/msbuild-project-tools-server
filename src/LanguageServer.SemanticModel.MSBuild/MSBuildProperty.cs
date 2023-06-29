using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;

namespace MSBuildProjectTools.LanguageServer.SemanticModel
{
    /// <summary>
    ///     A property in an MSBuild project.
    /// </summary>
    public sealed class MSBuildProperty
        : MSBuildObject<ProjectProperty>
    {
        /// <summary>
        ///     Create a new <see cref="MSBuildProperty"/>.
        /// </summary>
        /// <param name="property">
        ///     The underlying MSBuild <see cref="ProjectProperty"/>.
        /// </param>
        /// <param name="declaringXml">
        ///     The <see cref="ProjectPropertyElement"/> that results in the underlying MSBuild <see cref="ProjectProperty"/>'s current value.
        /// </param>
        /// <param name="propertyElement">
        ///     An <see cref="XSElement"/> representing the property's XML element.
        /// </param>
        public MSBuildProperty(ProjectProperty property, ProjectPropertyElement declaringXml, XSElement propertyElement)
            : base(property, propertyElement)
        {
            if (declaringXml == null)
                throw new ArgumentNullException(nameof(declaringXml));

            DeclaringXml = declaringXml;
        }

        /// <summary>
        ///     The property name.
        /// </summary>
        public override string Name => Property.Name;

        /// <summary>
        ///     The kind of MSBuild object represented by the <see cref="MSBuildProperty"/>.
        /// </summary>
        public override MSBuildObjectKind Kind => MSBuildObjectKind.Property;

        /// <summary>
        ///     The full path of the file where the property is declared.
        /// </summary>
        public override string SourceFile => Property.Xml.Location.File;

        /// <summary>
        ///     The property's declaring element.
        /// </summary>
        public XSElement Element => (XSElement)Xml;

        /// <summary>
        ///     The property's evaluated value.
        /// </summary>
        public string Value => Property.EvaluatedValue;

        /// <summary>
        ///     The property's raw (unevaluated) value.
        /// </summary>
        public string RawValue => Property.UnevaluatedValue;

        /// <summary>
        ///     The underlying MSBuild <see cref="ProjectProperty"/>.
        /// </summary>
        public ProjectProperty Property => UnderlyingObject;

        /// <summary>
        ///     The <see cref="ProjectPropertyElement"/> that results in the underlying MSBuild <see cref="ProjectProperty"/>'s current value.
        /// </summary>
        public ProjectPropertyElement DeclaringXml { get; }

        /// <summary>
        ///     Has the property value been overridden elsewhere?
        /// </summary>
        public bool IsOverridden => Property.Xml != DeclaringXml;
    }
}
