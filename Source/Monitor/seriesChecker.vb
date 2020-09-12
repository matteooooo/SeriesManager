Imports System.Data.OleDb
Imports System.Data.SqlClient
Public Class seriesChecker
    Public Sub check()
        Dim cmd As OleDbCommand = New OleDbCommand("select * from seriesinfo", DBConnection.getConnection())
        Dim sqlda As OleDbDataAdapter = New OleDbDataAdapter(cmd)
        Dim ds As DataSet = New DataSet
        sqlda.Fill(ds, "seriesinfo")
        Dim cmdbuild As OleDbCommandBuilder = New OleDbCommandBuilder(sqlda)
        sqlda.UpdateCommand = cmdbuild.GetUpdateCommand()
        Dim td As DataTable = ds.Tables("seriesinfo")
        For i = 0 To td.Rows.Count - 1
            Dim dr As DataRow = td.Rows(i)
            If ExistFile(dr.Item("Seriename").ToString, Integer.Parse(dr.Item("SerieSeason").ToString), Integer.Parse(dr.Item("Serieepisode").ToString), "V") Then
                dr.Item("videoexist") = "S"
            End If
            If ExistFile(dr.Item("Seriename").ToString, Integer.Parse(dr.Item("SerieSeason").ToString), Integer.Parse(dr.Item("Serieepisode").ToString), "S") Then
                dr.Item("subexist") = "S"
            End If

            If IsSubtitleAvailable(dr.Item("Seriename").ToString, Integer.Parse(dr.Item("SerieSeason").ToString), Integer.Parse(dr.Item("Serieepisode").ToString)) Then
                dr.Item("subcreated") = "S"
            End If
        Next
        sqlda.Update(ds, "seriesinfo")
        cmd.Dispose()
    End Sub
    Private Function ExistFile(ByVal SerieName As String, ByVal SerieSeason As Integer, ByVal serieEpisode As Integer, ByVal filetype As String) As Boolean
        Dim retval As Boolean
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim Sql As String = "select * from files where seriename = @name and serieseason = @season and serieepisode = @episode and flagvs = @filetype"
        cmd = New OleDbCommand(Sql, DBConnection.getConnection())
        cmd.Parameters.Add("@name", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@season", OleDbType.Integer).Value = SerieSeason
        cmd.Parameters.Add("@episode", OleDbType.Integer).Value = serieEpisode
        cmd.Parameters.Add("@filetype", OleDbType.VarChar).Value = filetype
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            retval = True
        Else
            retval = False
        End If
        dr.Close()
        cmd.Dispose()
        dr.Dispose()
        Return retval
    End Function

    Private Function IsSubtitleAvailable(ByVal SerieName As String, ByVal SerieSeason As Integer, ByVal serieEpisode As Integer) As Boolean
        Dim retval As Boolean
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim Sql As String = "select * from subtitles where seriename = @name and stagione = @season and episodio = @episode"
        cmd = New OleDbCommand(Sql, DBConnection.getConnection())
        cmd.Parameters.Add("@name", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@season", OleDbType.Integer).Value = SerieSeason
        cmd.Parameters.Add("@episode", OleDbType.Integer).Value = serieEpisode
        dr = cmd.ExecuteReader

        If dr.HasRows Then
            retval = True
        Else
            retval = False
        End If
        dr.Close()
        cmd.Dispose()
        dr.Dispose()
        Return retval
    End Function


    Private Function ExistFile(ByVal FileName As String) As Boolean
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim Sql As String = "select * from files where name = @name"
        cmd = New OleDbCommand(Sql, DBConnection.getConnection())
        cmd.Parameters.Add("@name", OleDbType.VarChar).Value = FileName
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            Return True
        Else
            Return False
        End If
        dr.Close()
        dr.Dispose()
        cmd.Dispose()
    End Function
End Class
