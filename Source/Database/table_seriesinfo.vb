Imports System.Data.OleDb
Imports log4net
Public Class table_seriesinfo
    Public Sub InsertIntoTable(ByVal Serie As String, ByVal season As Integer, ByVal episode As Integer, ByVal episodeairdate As Date)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim cmd As OleDbCommand = New OleDbCommand("insert into seriesinfo(seriename,serieseason,serieepisode,episodeairdate,videoexist,subexist,subcreated) values(@seriename,@serieseason,@serieepisode,@episodeairdate,@videoexist,@subexist,@subcreated)", DBConnection.getConnection)
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = Serie
        cmd.Parameters.Add("@serieseason", OleDbType.Integer).Value = season
        cmd.Parameters.Add("@serieepisode", OleDbType.Integer).Value = episode
        cmd.Parameters.Add("@episodeairdate", OleDbType.Date).Value = episodeairdate
        cmd.Parameters.Add("@videoexist", OleDbType.VarChar).Value = "N"
        cmd.Parameters.Add("@subexist", OleDbType.VarChar).Value = "N"
        cmd.Parameters.Add("@subcreated", OleDbType.VarChar).Value = "N"
        Try
            cmd.ExecuteNonQuery()
            log.Debug("Inserito nuovo episodio: " & Serie & " - Serie: " & season & " - Episodio: " & episode & " - Air date: " & episodeairdate)
        Catch e As Exception
            log.Error("Errore durante l'inserimento dell'episodio: " & Serie & " - Serie: " & season & " - Episodio: " & episode & " - Air date: " & episodeairdate)
            log.Error("Il messaggio di errore è il seguente: " & e.Message)
        Finally
            cmd.Dispose()
        End Try
    End Sub

    Public Sub deletetable(ByVal series As String)
        Dim cmd As OleDbCommand = New OleDbCommand("delete * from seriesinfo where seriename = @seriename", DBConnection.getConnection)
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = series
        Try
            cmd.ExecuteNonQuery()
        Catch e As Exception
        End Try
        cmd.Dispose()
    End Sub
    Shared Sub deleteall()
        Dim cmd As OleDbCommand = New OleDbCommand("delete * from seriesinfo", DBConnection.getConnection)
        Try
            cmd.ExecuteNonQuery()
        Catch e As Exception
        End Try
        cmd.Dispose()
    End Sub

End Class
