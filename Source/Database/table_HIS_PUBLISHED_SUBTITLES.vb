Imports System.Data.OleDb
Imports log4net
Public Class table_HIS_PUBLISHED_SUBTITLES
    Shared Sub InsertRaw(ByVal SerieName As String, ByVal stagione As Integer, ByVal episodio As Integer, ByVal SiteName As String, downloadlink As String, fileName As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim retval As Integer = 0
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into HIS_PUBLISHED_SUBTITLES(seriename,stagione,episodio,site,updatedate,isdownloaded,downloadlink,filename) values (@seriename,@stagione,@episodio,@sitename,@updatedate,'N',@downloadlink,@filename)", DBConnection.getConnection())
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@stagione", OleDbType.Integer).Value = stagione
        cmd.Parameters.Add("@episodio", OleDbType.Integer).Value = episodio
        cmd.Parameters.Add("@sitename", OleDbType.VarChar).Value = SiteName
        cmd.Parameters.Add("@updatedate", OleDbType.Date).Value = Date.Today
        cmd.Parameters.Add("@downloadlink", OleDbType.VarChar).Value = downloadlink
        cmd.Parameters.Add("@filename", OleDbType.VarChar).Value = fileName
        Try
            cmd.ExecuteNonQuery()
            log.Info("Inseriti nuovi sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName)
        Catch e As System.Data.OleDb.OleDbException
            Select Case e.ErrorCode
                Case -2147467259
                    log.Debug("Sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName & " già esistenti")
                Case Else
                    log.Error("Sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName & " non inseriti")
                    log.Error("Error message -> " & e.Message)
            End Select
        Catch ex As Exception
            log.Error("Sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName & " non inseriti")
            log.Error("Error message -> " & ex.Message)
        Finally
            cmd.Dispose()
        End Try
    End Sub
End Class
