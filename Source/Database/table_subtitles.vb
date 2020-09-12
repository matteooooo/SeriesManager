Imports System.Data.OleDb
Imports log4net
Public Class table_subtitles
    Shared Sub DeleteAll()
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("delete * from files", DBConnection.getConnection())
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
    Shared Function InsertRaw(ByVal SerieName As String, ByVal stagione As Integer, ByVal episodio As Integer, ByVal SiteName As String) As Integer
        Dim log As ILog = LogManager.GetLogger("MonitorRSS")
        Dim retval As Integer = 0
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into subtitles(seriename,stagione,episodio,site,updatedate,isdownloaded) values (@seriename,@stagione,@episodio,@sitename,@updatedate,'N')", DBConnection.getConnection())
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@stagione", OleDbType.Integer).Value = stagione
        cmd.Parameters.Add("@episodio", OleDbType.Integer).Value = episodio
        cmd.Parameters.Add("@sitename", OleDbType.VarChar).Value = SiteName
        cmd.Parameters.Add("@updatedate", OleDbType.Date).Value = Date.Today
        Try
            cmd.ExecuteNonQuery()
            retval = 0
            log.Info("Inseriti nuovi sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName)
        Catch e As Exception
            retval = -1
        Finally
            cmd.Dispose()
        End Try
        Return retval
    End Function

    Shared Function InsertRaw(ByVal SerieName As String, ByVal stagione As Integer, ByVal episodio As Integer, ByVal SiteName As String, downloadlink As String) As Integer
        Dim log As ILog = LogManager.GetLogger("MonitorRSS")
        Dim retval As Integer = 0
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into subtitles(seriename,stagione,episodio,site,updatedate,isdownloaded,downloadlink) values (@seriename,@stagione,@episodio,@sitename,@updatedate,'N',@downloadlink)", DBConnection.getConnection())
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@stagione", OleDbType.Integer).Value = stagione
        cmd.Parameters.Add("@episodio", OleDbType.Integer).Value = episodio
        cmd.Parameters.Add("@sitename", OleDbType.VarChar).Value = SiteName
        cmd.Parameters.Add("@updatedate", OleDbType.Date).Value = Date.Today
        cmd.Parameters.Add("@downloadlink", OleDbType.VarChar).Value = downloadlink
        Try
            cmd.ExecuteNonQuery()
            retval = 0
            log.Info("Inseriti nuovi sottotitoli per la serie " & SerieName & " - Stagione: " & stagione & " - Episodio: " & episodio & " - Provider: " & SiteName)
        Catch e As Exception
            retval = -1
        Finally
            cmd.Dispose()
        End Try
        Return retval
    End Function

    Shared Function Insert(ByVal SerieName As String, ByVal stagione As Integer, ByVal episodio As Integer, ByVal SiteName As String, downloadlink As String) As Integer
        Dim retval As Boolean = False
        For i = episodio To 1 Step -1
            If i = episodio Then
                InsertRaw(SerieName, stagione, i, SiteName, downloadlink)
            Else
                InsertRaw(SerieName, stagione, i, SiteName, String.Empty)
            End If
        Next
        Return 0
    End Function

    Shared Sub CheckSubtitleFileExistence()
        Dim ht As Hashtable
        ht = New Hashtable
        Dim sql As String = "select name from files where flagvs = 'S'"
        Dim cmd As OleDbCommand = New OleDbCommand(sql, DBConnection.getConnection)
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        If dr.HasRows Then
            Do While dr.Read
                Try
                    ht.Add(dr("name").ToString, "")
                Catch ex As Exception
                End Try
            Loop
        End If

        dr.Close()
        dr.Dispose()

        cmd = New OleDbCommand("select * from subtitles where isdownloaded = 'N'", DBConnection.getConnection())
        Dim sqlda As OleDbDataAdapter = New OleDbDataAdapter(cmd)
        Dim ds As DataSet = New DataSet
        sqlda.Fill(ds, "subtitles")
        Dim cmdbuild As OleDbCommandBuilder = New OleDbCommandBuilder(sqlda)
        sqlda.UpdateCommand = cmdbuild.GetUpdateCommand()
        Dim td As DataTable = ds.Tables("subtitles")
        For i = 0 To td.Rows.Count - 1
            Dim drow As DataRow = td.Rows(i)
            Dim filename As String = drow.Item("seriename").ToString & " - " & addLeadZero(drow.Item("stagione").ToString) & "x" & addLeadZero(drow.Item("episodio").ToString) & ".srt"
            If ht.ContainsKey(filename) Then
                drow("isdownloaded") = "S"
            End If
        Next
        sqlda.Update(ds, "subtitles")

    End Sub

    Shared Function addLeadZero(ByVal val As String) As String
        Dim retval As String
        Select Case val.Length
            Case 1
                retval = "0" & val
            Case 2
                retval = val
            Case Else
                retval = "00"
        End Select
        Return retval
    End Function

End Class
