Imports System.Data.OleDb
Public Class table_missingsubtitles
    Shared Sub DeleteAll()
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("delete * from missingsubtitles", DBConnection.getConnection())
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub
    Shared Function Insert(ByVal SerieName As String, ByVal episodecode As Integer) As Integer
        Dim retval As Integer = 0
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand("insert into missingsubtitles(seriename,stagione,episodio) values (@seriename,@stagione,@episodio)", DBConnection.getConnection())
        cmd.Parameters.Add("@seriename", OleDbType.VarChar).Value = SerieName
        cmd.Parameters.Add("@stagione", OleDbType.Integer).Value = Math.Truncate(episodecode / 100)
        cmd.Parameters.Add("@episodio", OleDbType.Integer).Value = episodecode Mod 100
        Try
            cmd.ExecuteNonQuery()
            retval = 0
        Catch e As Exception
            retval = -1
        Finally
            cmd.Dispose()
        End Try
        Return retval
    End Function
End Class
