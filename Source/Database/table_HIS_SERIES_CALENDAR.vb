Imports System.Data.OleDb
Imports log4net
Public Class table_HIS_SERIES_CALENDAR
    Shared Sub Insert(ByVal Serie As String, ByVal season As Integer, ByVal episode As Integer, ByVal episodeairdate As Date, filename As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim cmd As OleDbCommand = New OleDbCommand("insert into HIS_SERIES_CALENDAR(seriename,serieseason,serieepisode,episodeairdate,filename) values(@seriename,@serieseason,@serieepisode,@episodeairdate,@filename)", DBConnection.getConnection)
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = Serie
        cmd.Parameters.Add("@serieseason", OleDbType.Integer).Value = season
        cmd.Parameters.Add("@serieepisode", OleDbType.Integer).Value = episode
        cmd.Parameters.Add("@episodeairdate", OleDbType.Date).Value = episodeairdate
        cmd.Parameters.Add("@filename", OleDbType.VarChar).Value = filename
        Try
            cmd.ExecuteNonQuery()
            log.Debug("Inserito nuovo episodio: " & Serie & " - Serie: " & season & " - Episodio: " & episode & " - Air date: " & episodeairdate)
        Catch e As OleDb.OleDbException
            log.Debug("Errore durante l'inserimento dell'episodio: " & Serie & " - Serie: " & season & " - Episodio: " & episode & " - Air date: " & episodeairdate)
            log.Debug("Il messaggio di errore è il seguente: " & e.Message)
        Finally
            cmd.Dispose()
        End Try
    End Sub

End Class
