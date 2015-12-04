using System;
using System.Data;

namespace Kentico.Glimpse.Database
{
    internal sealed class InformationEntryProvider : IEntryProvider
    {
        public bool Matches(DataRow row)
        {
            var isInformation = row["IsInformation"];
            var counter = row["Counter"];

            if (isInformation == DBNull.Value || counter == DBNull.Value)
            {
                return false;
            }

            return (bool)isInformation && (int)counter >= 0;
        }


        public object GetEntry(DataRow row)
        {
            return new InformationEntry
            {
                Title = GetTitle(row),
                Text = GetText(row)
            };
        }

        
        private string GetTitle(DataRow row)
        {
            var value = row["QueryName"];

            if (value == DBNull.Value)
            {
                return null;
            }

            return (string)value;
        }

        
        private string GetText(DataRow row)
        {
            return (string)row["QueryText"];
        }
    }
}
