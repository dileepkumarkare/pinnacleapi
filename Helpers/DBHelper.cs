using System.Data.SqlClient;
using System.Data;
using Serilog;

namespace Pinnacle.Helpers
{
    public class DBHelper
    {
        private SqlTransaction sqlTran;
        private string _connectionstring;
        public DBHelper(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        /// <summary>
        /// To Execute Queries (Insert, Update or Delete) 
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns>No. of affected records</returns>
        public int ExecuteNonQuery(String query)
        {
            SqlConnection cnn = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand(query, cnn);
            //if (query.StartsWith("INSERT") | query.ToLower().StartsWith("insert") | query.StartsWith("UPDATE") | query.StartsWith("update") | query.StartsWith("DELETE") | query.StartsWith("delete"))
            if (query.ToLower().StartsWith("insert") | query.ToLower().StartsWith("update") | query.ToLower().StartsWith("delete"))
            cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;

            int retval;
            try
            {
                cnn.Open();

                cmd.CommandTimeout = 600;
                retval = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retval = 0;
                
            }
            finally
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
            }
            return retval;
        }

        /// <summary>
        /// To Execute Stored Procedure (Insert, Update or Delete) 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteNonQuerySP(String query, SqlParameter[] parameters, bool flag)
        {

            SqlConnection cnn = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand(query, cnn);

            if (query.ToLower().StartsWith("insert") | query.ToLower().StartsWith("update") | query.ToLower().StartsWith("delete"))
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;

            int i;
            for (i = 0; i < parameters.Length; i++)
                cmd.Parameters.Add(parameters[i]);

            object retval = null;
            try
            {
                cnn.Open();
                cmd.CommandTimeout = 600;
                retval = cmd.ExecuteNonQuery();

                //output parameter
                if (flag == true)
                    retval = cmd.Parameters[parameters.Length - 1].Value;
            }
            catch (Exception ex)
            {
                retval = null;
                
            }
            finally
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
            }
            return retval;
        }

        /// <summary>
        /// To Execute Stored Procedure With Transaction Commit Or RollBack (Insert, Update or Delete) 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters">0 for Single Transaction without BeginTran and CommitTran</param>
        ///<param name="parameters">For Multiple Transaction with BeginTran and CommitTran</param>
        ///<param name="parameters">1 First Transaction</param>
        ///<param name="parameters">2 Middle Transaction</param>
        ///<param name="parameters">3 Last Transaction</param>
        /// <returns></returns>
        public object ExecuteNonQuerySPTrans(String query, SqlParameter[] parameters, bool flag, int num)
        {
            SqlConnection cnn = new SqlConnection(_connectionstring);
            cnn.Open();
            SqlCommand cmd = new SqlCommand(query, cnn);

            if (num == 1)
                //sqlTran = cnn.BeginTransaction();
                sqlTran = cnn.BeginTransaction(IsolationLevel.ReadCommitted);

            //if (num == 1 | num == 2 | num == 3)
            //    cmd.Transaction = sqlTran;

            if (query.StartsWith("INSERT") | query.StartsWith("insert") | query.StartsWith("UPDATE") | query.StartsWith("update") | query.StartsWith("DELETE") | query.StartsWith("delete"))
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;

            int i;
            for (i = 0; i < parameters.Length; i++)
                cmd.Parameters.Add(parameters[i]);

            object retval;
            try
            {
                //cnn.Open();
                retval = cmd.ExecuteNonQuery();

                if (num == 3)
                    sqlTran.Commit();
                //output parameter
                if (flag == true)
                    retval = cmd.Parameters[parameters.Length - 1].Value;
            }
            catch (Exception ex)
            {
                retval = null;
                sqlTran.Rollback();

               
            }
            finally
            {
                if (num == 0 | num == 3)
                    cnn.Close();
                //if (cnn.State == ConnectionState.Open) cnn.Close();
            }
            return retval;

        }

        /// <summary>
        /// Executes a Scalar query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>No. of affected records</returns>
        public Object ExecuteScalar(String query)
        {
            SqlConnection cnn = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand(query, cnn);
            if (query.StartsWith("SELECT") | query.StartsWith("select"))
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;

            Object retval;
            try
            {
                cnn.Open();
                cmd.CommandTimeout = 600;
                retval = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retval = null;
                
            }
            finally
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
            }
            return retval;
        }

        /// <summary>
        /// Executes a Scalar Query using Stored Procedure
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Object ExecuteScalarSP(String query, SqlParameter[] parameters)
        {
            SqlConnection cnn = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand(query, cnn);
            if (query.StartsWith("SELECT") | query.StartsWith("select"))
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;

            int i;
            for (i = 0; i < parameters.Length; i++)
                cmd.Parameters.Add(parameters[i]);

            Object retval;
            try
            {
                cnn.Open();
                cmd.CommandTimeout = 600;
                retval = cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                retval = null;
            }
            finally
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
            }
            return retval;
        }
        /// <summary>
        /// Executes a SqlDataReader
        /// </summary>
        /// <param name="query"></param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(String query)
        {
            SqlDataReader dr = null;
            try
            {
                SqlConnection cnn = new SqlConnection(_connectionstring);
                SqlCommand cmd = new SqlCommand(query, cnn);
                if (query.StartsWith("SELECT") | query.StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                cnn.Open();
                //user has to close the SqlDataReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                
            }
            return dr;
        }

        /// <summary>
        /// Executes a stored procedure and returns a SqlDataReader
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReaderSP(String query, SqlParameter[] parameters)
        {
            SqlDataReader dr = null;
            try
            {
                SqlConnection cnn = new SqlConnection(_connectionstring);
                SqlCommand cmd = new SqlCommand(query, cnn);
                if (query.StartsWith("SELECT") | query.StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                int i;
                for (i = 0; i < parameters.Length; i++)
                    cmd.Parameters.Add(parameters[i]);

                cnn.Open();
                cmd.CommandTimeout = 600;
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                dr = null;
            }
            return dr;
        }

        /// <summary>
        /// Executes a DataSet
        /// </summary>
        /// <param name="query"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(String query)
        {
            DataSet ds = null;
            try
            {
                SqlConnection cnn = new SqlConnection(_connectionstring);
                SqlCommand cmd = new SqlCommand(query, cnn);
                if (query.StartsWith("SELECT") | query.StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex)
            {
            }
            return ds;
        }

        /// <summary>
        /// Executes a Dataset using Stored Procedure
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSetSP(String query, SqlParameter[] parameters)
        {
            DataSet ds = null;
            try
            {

                SqlConnection cnn = new SqlConnection(_connectionstring);
                SqlCommand cmd = new SqlCommand(query, cnn);
                if (query.StartsWith("SELECT") | query.StartsWith("select") | query.StartsWith("INSERT") | query.StartsWith("insert") | query.StartsWith("UPDATE") | query.StartsWith("update") | query.StartsWith("DELETE") | query.StartsWith("delete"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 2000;
                int i;
                for (i = 0; i < parameters.Length; i++)
                    cmd.Parameters.Add(parameters[i]);
                cmd.CommandTimeout = 600;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);

            }
            catch (Exception ex)
            {
                ds = null;
                Log.Information(ex.ToString());
               
            }
            return ds;
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }


        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>

        private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }


    }
}
