Imports log4net
Public Class seriesList
    Private Shared classLocker As New Object()
    Private Shared istanza As List(Of structures.Serie)

    Private Sub New()
    End Sub

    Public Shared Function getList() As List(Of structures.Serie)
        'If (istanza Is Nothing) Then
        SyncLock (classLocker)
            If (istanza Is Nothing) Then
                Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
                Dim cmd As OleDb.OleDbCommand = New OleDb.OleDbCommand("select * from REF_series_LIST order by seriename", DBConnection.getConnection)
                Dim dr As OleDb.OleDbDataReader = cmd.ExecuteReader
                log.Debug("Inizio caricamento attributi delle serie")
                istanza = New List(Of structures.Serie)
                Dim seriePath As String = String.Empty
                If dr.HasRows Then
                    While dr.Read
                        Dim s As structures.Serie
                        s.serieName = dr("seriename").ToString
                        s.serieRegExp = dr("regexp").ToString
                        Select Case dr("directory").ToString.ToUpper
                            Case "STAGING"
                                seriePath = My.Settings.TARGET_DIR & "\@Staging"
                            Case "STANDARD"
                                seriePath = My.Settings.TARGET_DIR & "\" & dr("seriename").ToString
                        End Select
                        s.seriePath = seriePath
                        istanza.Add(s)
                    End While
                End If
                log.Debug("Caricamento caricamento attributi delle serie completato")
            End If
            Return istanza
        End SyncLock
        'End If
        Return istanza
    End Function
End Class
'Imports System.Data.OleDb
'Public Class DBConnection
'    Private Shared classLocker As New Object()
'    Private Shared istanza As OleDbConnection
'    Private Sub New()
'    End Sub
'    Public Shared Function getConnection() As OleDbConnection
'        If (istanza Is Nothing) Then
'            SyncLock (classLocker)
'                If (istanza Is Nothing) Then
'                    istanza = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Password="""";User ID=Admin;Data Source=" & My.Application.Info.DirectoryPath & "\data\seriesmanager.mdb;")
'                    istanza.Open()
'                End If
'                Return istanza
'            End SyncLock
'        End If
'        Return istanza
'    End Function
'End Class