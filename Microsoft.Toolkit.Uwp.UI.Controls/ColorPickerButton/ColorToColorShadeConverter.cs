﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Creates an accent color shade from a color value.
    /// Only +/- 3 shades from the given color are supported.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row", Justification = "Whitespace is used to align code in columns for readability.")]
    public class ColorToColorShadeConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            string language)
        {
            int shade;
            byte tolerance = 0x05;
            HsvColor hsvColor;
            Color rgbColor;

            // Get the current color in HSV
            if (value is Color valueColor)
            {
                rgbColor = valueColor;
                hsvColor = rgbColor.ToHsv();
            }
            else if (value is SolidColorBrush valueBrush)
            {
                rgbColor = valueBrush.Color;
                hsvColor = rgbColor.ToHsv();
            }
            else
            {
                throw new ArgumentException("Invalid color value provided, unable to convert to HsvColor");
            }

            // Get the value component delta
            try
            {
                shade = System.Convert.ToInt32(parameter?.ToString());
            }
            catch
            {
                throw new ArgumentException("Invalid parameter provided, unable to convert to double");
            }

            // Specially handle minimum (black) and maximum (white)
            if (rgbColor.R <= (0x00 + tolerance) &&
                rgbColor.G <= (0x00 + tolerance) &&
                rgbColor.B <= (0x00 + tolerance))
            {
                switch (shade)
                {
                    case 1:
                        return Color.FromArgb(rgbColor.A, 0x3F, 0x3F, 0x3F);
                    case 2:
                        return Color.FromArgb(rgbColor.A, 0x80, 0x80, 0x80);
                    case 3:
                        return Color.FromArgb(rgbColor.A, 0xBF, 0xBF, 0xBF);
                }

                return rgbColor;
            }
            else if (rgbColor.R >= (0xFF + tolerance) &&
                     rgbColor.G >= (0xFF + tolerance) &&
                     rgbColor.B >= (0xFF + tolerance))
            {
                switch (shade)
                {
                    case -1:
                        return Color.FromArgb(rgbColor.A, 0xBF, 0xBF, 0xBF);
                    case -2:
                        return Color.FromArgb(rgbColor.A, 0x80, 0x80, 0x80);
                    case -3:
                        return Color.FromArgb(rgbColor.A, 0x3F, 0x3F, 0x3F);
                }

                return rgbColor;
            }
            else
            {
                double colorHue        = hsvColor.H;
                double colorSaturation = hsvColor.S;
                double colorValue      = hsvColor.V;
                double colorAlpha      = hsvColor.A;

                // Use the HSV representation as it's more perceptual
                switch (shade)
                {
                    case -3:
                        {
                            colorHue        *= 1.0;
                            colorSaturation *= 1.10;
                            colorValue      *= 0.40;
                            break;
                        }

                    case -2:
                        {
                            colorHue        *= 1.0;
                            colorSaturation *= 1.05;
                            colorValue      *= 0.50;
                            break;
                        }

                    case -1:
                        {
                            colorHue        *= 1.0;
                            colorSaturation *= 1.0;
                            colorValue      *= 0.75;
                            break;
                        }

                    case 0:
                        {
                            // No change
                            break;
                        }

                    case 1:
                        {
                            colorHue        *= 1.00;
                            colorSaturation *= 1.00;
                            colorValue      *= 1.05;
                            break;
                        }

                    case 2:
                        {
                            colorHue        *= 1.00;
                            colorSaturation *= 0.75;
                            colorValue      *= 1.05;
                            break;
                        }

                    case 3:
                        {
                            colorHue        *= 1.00;
                            colorSaturation *= 0.65;
                            colorValue      *= 1.05;
                            break;
                        }
                }

                return Uwp.Helpers.ColorHelper.FromHsv(
                    Math.Clamp(colorHue,        0.0, 360.0),
                    Math.Clamp(colorSaturation, 0.0, 1.0),
                    Math.Clamp(colorValue,      0.0, 1.0),
                    Math.Clamp(colorAlpha,      0.0, 1.0));
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
