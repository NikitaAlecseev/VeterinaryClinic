using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VeterinaryClinic.Core.Datebase
{
    public class Command: DB
    {
        public DataTable MainTable;
        private List<ParametersSql> parametersSql = new List<ParametersSql>();
        public void LoadData(string _command)
        {
            openConnection();
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(_command, getConnection());
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                closeConnection();
                MainTable = dataTable;
            }
            catch (Exception ex)
            {
                closeConnection();
                MessageBox.Show(ex.Message, "Error #01", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SendCommand(string _command)
        {
            openConnection();
            try
            {
                SqlCommand Scommand = new SqlCommand(_command, getConnection());
                foreach (ParametersSql parS in parametersSql)
                {
                    Scommand.Parameters.Add(parS.Title, parS.TypeSql).Value = parS.Value;
                }
                Scommand.ExecuteNonQuery();
                closeConnection();
                parametersSql.Clear();
            }
            catch (Exception ex)
            {
                closeConnection();
                parametersSql.Clear();
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddParameter(string _title, SqlDbType _type, string _value)
        {
            ParametersSql parameterSql = new ParametersSql();
            parameterSql.Title = _title;
            parameterSql.TypeSql = _type;
            parameterSql.Value = _value;
            parametersSql.Add(parameterSql);
        }
        public class ParametersSql
        {
            public string Title;
            public SqlDbType TypeSql;
            public string Value;
        }
    }
}
