using System;
using System.Collections;
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

        public PluginLib.IExternalControlData CreateEditorContent(string filePath)
        {
            DataGrid dg = new DataGrid();
            dg.LoadingRow += dg_LoadingRow;
            DataTable table = DataTableBuilder.GetDataTabletFromCSVFile(filePath);
            CSVControlData ret = new CSVControlData(filePath, dg, table);

            // This is hokey, set Tag to be our control data
            dg.Tag = ret;

            // Handle the "CTRL+S" for save
            dg.CommandBindings.Add(new System.Windows.Input.CommandBinding(
                System.Windows.Input.ApplicationCommands.Save, //CTRL+S
                (sender, e) => { //Exectued
                    DataTableBuilder.Write(table, table.TableName);
                    ret.IsDirty = false;
                },
                (sender, e) => { //CanExecute
                    e.CanExecute = ret.IsDirty;
                }));

            table.TableName = filePath;
            dg.ItemsSource = table.AsDataView();
            return ret;
        }

        // http://stackoverflow.com/questions/4661998/simple-way-to-display-row-numbers-on-wpf-datagrid
        void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }
    }
}
