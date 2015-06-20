﻿using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Linq;

namespace Bardock.Utils.UnitTest.AutoFixture.Xunit2.Attributes
{
    /// <summary>
    /// Provides a data source for a data theory, with the data coming from inline
    /// values combined with auto-generated data specimens generated by AutoFixture
    /// and <see cref="ICustomization"/> types to apply.
    /// </summary>
    public abstract class InlineAutoDataAndCustomizationsAttribute : InlineAutoDataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineAutoDataAndCustomizationsAttribute"/> class.
        /// </summary>
        /// <param name="autoDataAttribute">The <see cref="AutoDataAttribute"/> instance that provides auto-generated data specimens.</param>
        /// <param name="valuesAndCustomizationTypes">An array of inline values and customization types to be applied.</param>
        public InlineAutoDataAndCustomizationsAttribute(
            AutoDataAttribute autoDataAttribute,
            params object[] valuesAndCustomizationTypes)
            : base(
                autoDataAttribute,
                values: valuesAndCustomizationTypes.Where(x => !IsCustomizationType(x)).ToArray())
        {
            var customizations = valuesAndCustomizationTypes
                .Select(x => ToCustomizationTypeOrDefault(x))
                .Where(ct => ct != null)
                .Select(ct => (ICustomization)Activator.CreateInstance(ct, null));

            foreach (var c in customizations)
            {
                this.AutoDataAttribute.Fixture.Customize(c);
            }
        }

        /// <summary>
        /// Determines whether the specified object is a <see cref="ICustomization"/> type.
        /// </summary>
        /// <param name="target">The object to evaluate</param>
        private static bool IsCustomizationType(object target)
        {
            return ToCustomizationTypeOrDefault(target) != null;
        }

        /// <summary>
        /// Converts the specified object to a <see cref="ICustomization"/> type if possible, otherwise null.
        /// </summary>
        /// <param name="target">The object to convert</param>
        private static Type ToCustomizationTypeOrDefault(object target)
        {
            var type = target as Type;
            if (type != null && typeof(ICustomization).IsAssignableFrom(type))
            {
                return type;
            }
            return null;
        }
    }
}