
Imports System.Data.OleDb
Imports log4net
Public Class table_HIS_VIDEO_FILES
    Const TABLE_NAME = "HIS_VIDEO_FILES"

    Shared Sub Delete()
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("delete * from " & TABLE_NAME, DBConnection.getConnection())
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
    Shared Sub Insert(ByVal FullPath As String, ByVal name As String, ByVal SerieName As String, ByVal SerieSeason As Integer, ByVal SerieEpisode As Integer, folder As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into " & TABLE_NAME & " (filePath,FileName,seriename,serieseason,serieepisode,folder) values (@fn,@n,@seriename,@serieseason,@serieepisode,@folder)", DBConnection.getConnection())
        cmd.Parameters.Add("@fn", OleDbType.VarChar).Value = FullPath
        cmd.Parameters.Add("@n", OleDbType.VarChar).Value = name
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@serieseason", OleDbType.Integer).Value = SerieSeason
        cmd.Parameters.Add("@serieepisode", OleDbType.Integer).Value = SerieEpisode
        cmd.Parameters.Add("@folder", OleDbType.VarChar).Value = folder
        Try
            cmd.ExecuteNonQuery()
            log.Debug("Informazioni per il file " & FullPath & " inserite correttamente")
        Catch ex As Exception
            log.Error("Informazioni per il file " & FullPath & " NON inserite correttamente")
            log.Error("L'errore è il seguente: " & ex.Message)
        End Try
        cmd.Dispose()
    End Sub

    Shared Sub addFakeFiles()
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into " & TABLE_NAME & " (FilePath,FileName,SerieName,SerieSeason,SerieEpisode,folder) select filepath,filename,serieName,serieSeason,SerieEpisode,'Standard' from HIS_ADDED_FILES where filename not like '%.srt%'", DBConnection.getConnection())

        Try
            cmd.ExecuteNonQuery()
            log.Debug("File video da aggiungere inseriti correttamente")
        Catch ex As Exception
            log.Debug("File video da aggiungere NON inseriti correttamente")
            log.Error("L'errore è il seguente: " & ex.Message)
        End Try
        cmd.Dispose()
    End Sub
End Class