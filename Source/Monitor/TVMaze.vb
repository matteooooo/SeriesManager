Imports System.Net
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports System.Data.OleDb
Imports log4net

Public Class TVMaze
    Sub searchForInformation()
        Dim wc As WebClient = New WebClient()
        Dim sql As String = "select * from ref_series_list where fromseason <> 100 and tvmazeid is not null"
        Dim cmd As OleDbCommand = New OleDbCommand(sql, DBConnection.getConnection)
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        If dr.HasRows Then
            While dr.Read
                Dim tempFileName As String = My.Application.Info.DirectoryPath & "\temp\" & dr("SerieName").ToString & ".txt"
                Dim filex As Boolean = File.Exists(tempFileName)
                Dim fileOld As Boolean
                If filex Then
                    If File.GetLastWriteTime(tempFileName).Date <= Date.Today.AddDays(-7) Then
                        fileOld = True
                    Else
                        fileOld = False
                    End If
                End If

                If filex = False Or fileOld = True Then
                    'wc.DownloadFile("http://api.tvmaze.com/shows/" & dr("tvmazeID").ToString & "/episodes", My.Application.Info.DirectoryPath & "\temp\" & dr("seriename").ToString & ".json")
                    Dim jsoncode As String = wc.DownloadString("http://api.tvmaze.com/shows/" & dr("tvmazeID").ToString & "/episodes")
                    Dim sr As System.IO.StreamWriter = New System.IO.StreamWriter(My.Application.Info.DirectoryPath & "\temp\" & dr("seriename").ToString & ".txt")
                    Try
                        Dim xReturn As List(Of TVMazeItem) = JsonConvert.DeserializeObject(Of List(Of TVMazeItem))(jsoncode)
                        Dim sMessage As String = ""
                        For Each TMitem As TVMazeItem In xReturn
                            Try
                                sr.Write(TMitem.airdate.ToString("dd/MM/yyyy"))
                                sr.Write(";")
                                sr.Write(TMitem.season)
                                sr.Write(";")
                                sr.Write(TMitem.number)
                                sr.Write(";")
                                sr.WriteLine(TMitem.name)
                                table_HIS_SERIES_CALENDAR.Insert(dr("seriename").ToString, TMitem.season, TMitem.number, TMitem.airdate, seriefileNameCreator(dr("seriename").ToString, TMitem.season, TMitem.number))
                            Catch ex As Exception
                            End Try
                        Next
                    Catch Exp As Exception
                    Finally
                        sr.Close()
                        sr.Dispose()
                    End Try
                End If
            End While
        End If
    End Sub
    Private Function seriefileNameCreator(SerieName As String, Season As Integer, Episode As Integer) As String
        Dim strSeason As String = Season.ToString
        Dim strEpisode As String = Episode.ToString
        If strSeason.ToString.Length = 1 Then
            strSeason = 0 & strSeason
        End If
        If strEpisode.ToString.Length = 1 Then
            strEpisode = 0 & strEpisode
        End If
        Return SerieName & " - " & strSeason & "x" & strEpisode & ".???"
    End Function

End Class

Public Class TVMazeItem
    Public Property id As String
    'Public Property url As String
    Public Property name As String
    Public Property season As Integer
    Public Property number As Integer
    Public Property airdate As Date
    'Public Property airtime As DateTime
    'Public Property airstamp As Date
    'Public Property runtime As Integer
    'Public Property summary As String
End Class



