Imports System.Data.OleDb
Imports log4net
Public Class table_files
    Shared Sub Delete()
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("delete * from files", DBConnection.getConnection())
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
    Shared Sub Insert(ByVal FullPath As String, ByVal name As String, ByVal flagvs As String, ByVal SerieName As String, ByVal SerieSeason As Integer, ByVal SerieEpisode As Integer)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim cmd As OleDbCommand
        log.Debug("Fullpath -> " & FullPath)
        cmd = New OleDbCommand("insert into files(fullpath,name,flagvs,seriename,serieseason,serieepisode) values (@fn,@n,@flagvs,@seriename,@serieseason,@serieepisode)", DBConnection.getConnection())
        cmd.Parameters.Add("@fn", OleDbType.VarChar).Value = FullPath
        cmd.Parameters.Add("@n", OleDbType.VarChar).Value = name
        cmd.Parameters.Add("@flagvs", OleDbType.VarChar).Value = flagvs
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@serieseason", OleDbType.Integer).Value = SerieSeason
        cmd.Parameters.Add("@serieepisode", OleDbType.Integer).Value = SerieEpisode
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Shared Sub addfakefile()
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim cmd As OleDbCommand
        'log.Debug("Fullpath -> " & FullPath)
        cmd = New OleDbCommand("insert into files select * from files_added", DBConnection.getConnection())
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
End Class
