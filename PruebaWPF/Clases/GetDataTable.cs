
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PruebaWPF.Clases
{
    class GetDataTable
    {

        public static DataTable GetDataT(DataGrid dataGrid)
        {

            DataTable data = new DataTable();
            DataColumn columna;

            //Se agregan todas las columnas del Grid al DataTable, no se usa foreach con DataColumn debido que se permite hacer dataGrid.columns  
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                columna = new DataColumn();
                columna.Caption = dataGrid.Columns[i].Header.ToString();
                columna.ColumnName = dataGrid.Columns[i].SortMemberPath;

                data.Columns.Add(columna);
            }

            foreach (var row in dataGrid.Items)
            {
                data.Rows.Add(row);
            }

            //Se eliminan las columas que no son visibles en el grid, no se eliminaron antes porque genera error al insertar las filas
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == Visibility.Hidden || dataGrid.Columns[i].Width == 0 || dataGrid.Columns[i].Header.Equals(""))
                {
                    data.Columns.Remove(dataGrid.Columns[i].SortMemberPath);
                }
            }

            return data;
        }
        public static DataTable GetDataGridRows(DataGrid dataGrid)
        {

            Visibility[] columnasOcultas = new Visibility[dataGrid.Columns.Count];

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == Visibility.Collapsed || dataGrid.Columns[i].Visibility == Visibility.Hidden)
                {
                    columnasOcultas[i] = dataGrid.Columns[i].Visibility;
                    dataGrid.Columns[i].Visibility = Visibility.Visible;
                }
            }
            DataGridSelectionMode selectionMode = dataGrid.SelectionMode;

            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);

            dataGrid.UnselectAllCells();

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (columnasOcultas[i] == Visibility.Collapsed || columnasOcultas[i] == Visibility.Hidden)
                {
                    dataGrid.Columns[i].Visibility = columnasOcultas[i];
                }
            }

            string result = (string)Clipboard.GetData(DataFormats.Text);
            Clipboard.Clear();
            string[] Lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string[] Columns, Fields;

            Columns = Lines[0].Split(new char[] { '\t' });
            int Cols = Columns.GetLength(0);
            DataTable dt = new DataTable();
            for (int i = 0; i < Cols; i++)
            {
                dt.Columns.Add(Columns[i] == "" ? ("Not_TextC01umn_@_" + i) : Columns[i], typeof(string));
            }

            DataRow Row;
            for (int r = 1; r < Lines.GetLength(0) - 1; r++)
            {
                Fields = Lines[r].Split(new char[] { '\t' });
                Row = dt.NewRow();

                for (int f = 0; f < dt.Columns.Count; f++)
                {
                    if (!Columns[f].Equals(""))
                    {
                        Row[f] = Fields[f];
                    }
                }

                dt.Rows.Add(Row);
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ColumnName.Contains("Not_TextC01umn_@_")) //Si la columna no tiene texto entonces no la agrego al Datatable
                {
                    dt.Columns.RemoveAt(i);
                    i--;
                }

            }
            //dataGrid.SelectionMode = selectionMode; //Devuelvo el modo de seleccióm.

            return dt;
        }
    }
}
