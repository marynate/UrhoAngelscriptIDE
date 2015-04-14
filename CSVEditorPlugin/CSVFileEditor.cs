using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSVEditorPlugin
{
    public class CSVFileEditor : PluginLib.IFileEditor
    {
        public bool CanEditFile(string filePath, string fileExtension)
        {
            if (fileExtension.Contains("csv"))
                return true;
            return false;
        }

        public object CreateEditorContent(string filePath, out object userData)
        {
            DataGrid dg = new DataGrid();
            dg.LoadingRow += dg_LoadingRow;
            DataTable table = DataTableBuilder.GetDataTabletFromCSVFile(filePath);
            table.TableName = filePath;
            dg.Tag = table;
            dg.ItemsSource = ((DataTable)dg.Tag).AsDataView();
            table.RowChanged += table_RowChanged;
            table.RowDeleted += table_RowDeleted;
            table.TableNewRow += table_TableNewRow;
            userData = null;
            return dg;
        }

        // http://stackoverflow.com/questions/4661998/simple-way-to-display-row-numbers-on-wpf-datagrid
        void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        void table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataTable table = sender as DataTable;
            if (table != null)
                DataTableBuilder.Write(table, table.TableName);
        }

        void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            DataTable table = sender as DataTable;
            if (table != null)
                DataTableBuilder.Write(table, table.TableName);
        }

        void table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            DataTable table = sender as DataTable;
            if (table != null)
                DataTableBuilder.Write(table, table.TableName);
        }

        public void SaveContent(object contentGiven, object userData, string filePath)
        {
            DataTable table = ((DataGrid)contentGiven).DataContext as DataTable;
            DataTableBuilder.Write(table, filePath);
        }
    }
}
