
Imports System.Data.OleDb
Imports log4net
Public Class table_HIS_SRT_TEMPORARY_FILES
    Const TABLE_NAME = "HIS_SRT_TEMPORARY_FILES"

    Shared Sub Delete()
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("delete * from " & TABLE_NAME, DBConnection.getConnection())
        cmd.Parameters.Add("@tableName", OleDbType.VarChar).Value = TABLE_NAME
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
    Shared Sub Insert(ByVal name As String, ByVal filetargetname As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into " & TABLE_NAME & " (FileName,fileTargetName) values (@FileName,@fileTargetName)", DBConnection.getConnection())
        cmd.Parameters.Add("@filename", OleDbType.VarChar).Value = name
        cmd.Parameters.Add("@filetargetname", OleDbType.VarChar).Value = fileTargetName
        Try
            cmd.ExecuteNonQuery()
            log.Debug("Informazioni per il file " & name & " inserite correttamente")
        Catch ex As Exception
            log.Error("Informazioni per il file " & name & " NON inserite correttamente")
            log.Error("L'errore è il seguente: " & ex.Message)
        End Try
        cmd.Dispose()
    End Sub
End Class