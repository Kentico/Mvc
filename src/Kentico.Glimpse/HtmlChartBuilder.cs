using System;
using System.Globalization;
using System.Text;

namespace Kentico.Glimpse
{
    /// <summary>
    /// Builds a HTML progress chart that displays a value relative to a maximum value.
    /// The chart uses different colors depending on the percentage that the value represents.
    /// Percentage is constrained to an interval from 5 to 95.
    /// Percentage less than 25 is considered good (green color is used) while percentage greater than 75 is considered bad (red color is used).
    /// Percentage from 25 to 75 is considered neutral (orange color is used).
    /// When a threshold value is specified, any value that does not exceed this threshold is always considered good.
    /// </summary>
    internal sealed class HtmlChartBuilder
    {
        private long mValue;
        private long mMaxValue;
        private long mThresholdValue;

        // Background colors for the chart and the progress bar (good, neutral and bad)
        private static readonly string[] mBackgroundColors = { "bbffbb", "ffcc66", "ffbbbb" };
        private static readonly string[] mInnerBackgroundColors = { "88dd88", "ff9900", "ff3333" };


        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlChartBuilder"/> class.
        /// </summary>
        /// <param name="value">The value to display relative to the maximum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">The maximum value is negative or the value is negative or the value is greater than the maximum value.</exception>
        public HtmlChartBuilder(long value, long maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            if (value < 0 || value > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            mValue = value;
            mMaxValue = maxValue;
        }


        /// <summary>
        /// Sets the threshold value. A value that does not exceed this threshold is always considered good.
        /// </summary>
        /// <param name="thresholdValue">The threshold value.</param>
        /// <returns>This instance with the specified threshold value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The threshold value is negative.</exception>
        public HtmlChartBuilder WithThresholdValue(long thresholdValue)
        {
            if (thresholdValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(thresholdValue));
            }

            mThresholdValue = thresholdValue;

            return this;
        }

        
        /// <summary>
        /// Builds the chart HTML and returns it.
        /// </summary>
        /// <returns>An HTML fragment that represents a progress chart using the specified properties.</returns>
        public string Build()
        {
            if (mMaxValue == 0)
            {
                return String.Empty;
            }

            var percent = Math.Min(95, Math.Max(5, mValue * 100 / mMaxValue));

            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "<div style='height:10px;width:60px;overflow:hidden;background:#{0}' title='{1:P2}'>", GetBackgroundColor(mBackgroundColors, percent), (decimal)mValue / mMaxValue);
            builder.AppendFormat(CultureInfo.InvariantCulture, "<div style='float:left;height:10px;background:#{0};width:{1:D}%'>", GetBackgroundColor(mInnerBackgroundColors, percent), percent);
            builder.Append("</div></div>");

            return builder.ToString();
        }

        
        private string GetBackgroundColor(string[] backgroundColors, long percent)
        {
            if (percent < 25 || mValue <= mThresholdValue)
            {
                return backgroundColors[0];
            }

            if (percent > 75)
            {
                return backgroundColors[2];
            }

            return backgroundColors[1];
        }
    }
}
