using System;
using System.Collections.Generic;
using System.Globalization;

using CMS.DataEngine;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;

namespace Kentico.Glimpse.Database
{
    /// <summary>
    /// Represents a Glimpse tab that displays Kentico debug information related to SQL queries.
    /// </summary>
    public sealed class KenticoDatabaseTab : TabBase, ITabLayout, ILayoutControl
    {
        private const string SECTION_KEY_STATISTICS = "Statistics";
        private const string SECTION_KEY_QUERIES = "Queries";


        private static readonly object mLayout = CreateLayout();


        /// <summary>
        /// Gets the layout of debug information.
        /// </summary>
        /// <returns></returns>
        public object GetLayout()
        {
            return mLayout;
        }


        /// <summary>
        /// Gets the value that indicates whether the top level keys of debug information will be treated as section names.
        /// </summary>
        public bool KeysHeadings
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Gets the display name of this tab.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Kentico SQL";
            }
        }


        /// <summary>
        /// Returns debug information that will be sent to the Glimpse client.
        /// </summary>
        /// <param name="context">The tab context.</param>
        /// <returns>An object with debug information that the Glimpse client can display.</returns>
        public override object GetData(ITabContext context)
        {
            var connectionStringRegistry = new ConnectionStringRegistry();
            var entryFactory = new EntryFactory(connectionStringRegistry);
            var requestMetadataProvider = new RequestMetadataProvider(entryFactory);
            var requestMetadata = requestMetadataProvider.GetRequestMetadata(SqlDebug.CurrentRequestLog);

            var items = new List<object[]>
            {
                new object[] {"Ordinal", "Information", "Text", "Result", "Received", "Sent", "Duration", "Stack trace"}
            };

            var nextOrdinal = 1;
            foreach (var entry in requestMetadata.Entries)
            {
                CommandEntry commandEntry = entry as CommandEntry;
                if (commandEntry != null)
                {
                    items.Add(new object[] {
                        String.Format(CultureInfo.InvariantCulture, "{0:N0}", nextOrdinal++),
                        GetInformation(commandEntry),
                        GetText(commandEntry),
                        GetResult(commandEntry),
                        String.Format(CultureInfo.InvariantCulture, "!{0:N0} B {1}!", commandEntry.BytesReceived, GetBytesReceivedChart(requestMetadata, commandEntry)),
                        String.Format(CultureInfo.InvariantCulture, "!{0:N0} B {1}!", commandEntry.BytesSent, GetBytesSentChart(requestMetadata, commandEntry)),
                        String.Format(CultureInfo.InvariantCulture, "!{0:N0} ms {1}!", commandEntry.Duration.TotalMilliseconds, GetDurationChart(requestMetadata, commandEntry)),
                        commandEntry.StackTrace,
                        commandEntry.IsDuplicate ? "warn" : String.Empty
                    });
                    continue;
                }

                ConnectionEntry connectionEntry = entry as ConnectionEntry;
                if (connectionEntry != null)
                {
                    items.Add(new object[] {
                        String.Empty,
                        GetInformation(connectionEntry),
                        String.Empty,
                        null,
                        null,
                        null,
                        null,
                        connectionEntry.StackTrace,
                        "quiet"
                    });
                    continue;
                }

                InformationEntry informationEntry = entry as InformationEntry;
                if (informationEntry != null)
                {
                    items.Add(new object[] {
                        String.Empty,
                        GetInformation(informationEntry),
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        "info"
                    });
                    continue;
                }
            }

            var statictics = new object[] {
                new object[] { "Name", "Value" },
                new object[] { "Queries", String.Format(CultureInfo.InvariantCulture, "{0:N0}", requestMetadata.Statistics.TotalCommands) },
                new object[] { "Duplicate queries", String.Format(CultureInfo.InvariantCulture, "{0:N0}", requestMetadata.Statistics.TotalDuplicateCommands) },
                new object[] { "Duration", String.Format(CultureInfo.InvariantCulture, "{0:N0} ms", requestMetadata.Statistics.TotalDuration.TotalMilliseconds) },
                new object[] { "Received", String.Format(CultureInfo.InvariantCulture, "{0:N0} B", requestMetadata.Statistics.TotalBytesReceived) },
                new object[] { "Sent", String.Format(CultureInfo.InvariantCulture, "{0:N0} B", requestMetadata.Statistics.TotalBytesSent) }
            };

            return new Dictionary<string, object>
            {
                { SECTION_KEY_STATISTICS, statictics },
                { SECTION_KEY_QUERIES, items }
            };
        }


        private static object CreateLayout()
        {
            var result = TabLayout.Create();

            result.Cell(SECTION_KEY_STATISTICS, TabLayout.Create().Row(r =>
            {
                r.Cell(0).WithTitle("Name").WidthInPixels(150).AlignRight();
                r.Cell(1).WithTitle("Value");
            }));

            result.Cell(SECTION_KEY_QUERIES, TabLayout.Create().Row(r =>
            {
                r.Cell(0).WidthInPixels(50).AlignRight();                   // Ordinal
                r.Cell(1).WidthInPixels(150).DisablePreview();              // Information
                r.Cell(2).AsCode(CodeType.Sql).DisablePreview();            // Text
                r.Cell(3).WidthInPixels(80);                                // Result
                r.Cell(4).WidthInPixels(60).AlignRight().DisablePreview();  // Received
                r.Cell(5).WidthInPixels(60).AlignRight().DisablePreview();  // Sent
                r.Cell(6).WidthInPixels(60).AlignRight().DisablePreview();  // Duration
                r.Cell(7).WidthInPixels(150);                               // Stack trace
            }));

            return result.Build();
        }


        private string GetInformation(CommandEntry entry)
        {
            if (entry.IsDuplicate)
            {
                return "Duplicate query";
            }

            return String.Empty;
        }


        private string GetInformation(ConnectionEntry entry)
        {
            if (entry.CustomConnectionStringName != null)
            {
                return String.Format("[{0}] {1}", entry.CustomConnectionStringName, entry.Text);
            }

            return String.Format("{0}", entry.Text);
        }


        private string GetInformation(InformationEntry entry)
        {
            if (entry.Title != null)
            {
                if (entry.Text != null)
                {
                    return String.Format("{0}: {1}", entry.Title, entry.Text);
                }

                return entry.Title;
            }

            return entry.Text;
        }


        private string GetText(CommandEntry entry)
        {
            if (entry.Name != null)
            {
                if (entry.CustomConnectionStringName != null)
                {
                    return String.Format("-- [{0}] {1}\r\n{2}", entry.CustomConnectionStringName, entry.Name, entry.Text);
                }

                return String.Format("-- {0}\r\n{1}", entry.Name, entry.Text);
            }

            return entry.Text;
        }


        private string GetResult(CommandEntry entry)
        {
            if (String.IsNullOrEmpty(entry.Result))
            {
                return String.Empty;
            }

            var index = entry.Result.IndexOf(']');

            return entry.Result.Substring(0, index + 1) + ')';
        }


        private string GetBytesReceivedChart(RequestMetadata metadata, CommandEntry entry)
        {
            return new HtmlChartBuilder(entry.BytesReceived, metadata.Statistics.TotalBytesReceived).WithThresholdValue(512).Build();
        }


        private string GetBytesSentChart(RequestMetadata metadata, CommandEntry entry)
        {
            return new HtmlChartBuilder(entry.BytesSent, metadata.Statistics.TotalBytesSent).WithThresholdValue(512).Build();
        }

        
        private string GetDurationChart(RequestMetadata metadata, CommandEntry entry)
        {
            return new HtmlChartBuilder(entry.Duration.Ticks, metadata.Statistics.TotalDuration.Ticks).WithThresholdValue(5 * TimeSpan.TicksPerMillisecond).Build();
        }
    }
}
