Imports System.Data.OleDb
Public Class DBConnection
    Private Shared classLocker As New Object()
    Private Shared istanza As OleDbConnection
    Private Sub New()
    End Sub
    Public Shared Function getConnection() As OleDbConnection
        If (istanza Is Nothing) Then
            SyncLock (classLocker)
                If (istanza Is Nothing) Then
                    istanza = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Password="""";User ID=Admin;Data Source=" & My.Application.Info.DirectoryPath & "\data\seriesmanager.mdb;")
                    istanza.Open()
                End If
                Return istanza
            End SyncLock
        End If
        Return istanza
    End Function
End Class


