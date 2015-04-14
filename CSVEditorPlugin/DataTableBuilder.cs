using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVEditorPlugin
{
    class DataTableBuilder
    {
        /// <summary>
        /// http://stackoverflow.com/questions/16606753/populating-a-dataset-from-a-csv-file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable GetDataTabletFromCSVFile(string filePath)
        {
            DataTable csvData = new DataTable();
            try {
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }

        /// <summary>
        /// http://www.codeproject.com/Tips/591034/Simplest-code-to-export-a-datatable-into-csv-forma
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="file"></param>
        public static void Write(DataTable dataTable, string file)
        {
            var lines = new List<string>();

            var valueLines = dataTable.AsEnumerable()
                   .Select(row => string.Join(",", row.ItemArray));            
                    lines.AddRange(valueLines );

            File.WriteAllLines("excel.csv",lines);

        }
    }
}
