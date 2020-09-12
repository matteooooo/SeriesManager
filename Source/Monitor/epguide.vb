Imports HtmlAgilityPack
Imports System.IO
Imports System.Data.OleDb
Imports log4net
Public Class epguide
    Dim c As List(Of serieinfo)
    Private Structure serieinfo
        Dim season As Integer
        Dim episode As Integer
        Dim airDate As Date
    End Structure
    Sub New(ByVal seriename As String, ByVal serielink As String)
        c = New List(Of serieinfo)
        GetEpisodesList(seriename, serielink)
        extractEpisodeList(seriename)
        InsertIntoTable(seriename)
    End Sub
    'Private Sub xGetEpisodesList(ByVal seriename As String, ByVal serielink As String)
    '    Dim log As ILog = LogManager.GetLogger("MonitorMain")
    '    Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\temp\" & seriename & ".list")
    '    Dim c As Collection = New Collection
    '    Dim hw As HtmlWeb = New HtmlWeb
    '    Dim hd As HtmlDocument = hw.Load(serielink)
    '    Dim nl As HtmlNodeCollection = hd.DocumentNode.SelectNodes("//pre")
    '    Try
    '        For Each nod In nl.Nodes
    '            sw.WriteLine(nod.InnerText())
    '        Next
    '    Catch ex As Exception
    '        log.Error("Errore durante il parsing degli episodi della serie: " & seriename)
    '        log.Error("Il messaggio di errore è il seguente: " & ex.Message)
    '    End Try
    '    sw.Close()
    '    sw.Dispose()
    'End Sub

    Private Sub GetEpisodesList(ByVal seriename As String, ByVal serielink As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\temp\" & seriename & ".list")
        Dim c As Collection = New Collection
        Dim hw As HtmlWeb = New HtmlWeb
        Dim hd As HtmlDocument = hw.Load(serielink)
        Try


            Dim nl As HtmlNodeCollection = hd.DocumentNode.SelectNodes("//div[@id='eplist']//table//tr/td[@class='epinfo left pad']")
            Dim nld As HtmlNodeCollection = hd.DocumentNode.SelectNodes("//div[@id='eplist']//table//tr/td[@class='epinfo right pad']")

            For i = 0 To nl.Count - 1
                sw.WriteLine(nl(i).InnerText.Replace("&nbsp;", "") & "|" & nld(i).InnerText)
            Next

        Catch ex As Exception
            log.Error("Errore durante il parsing degli episodi della serie: " & seriename)
            log.Error("Il messaggio di errore è il seguente: " & ex.Message)
        End Try

        sw.Close()
        sw.Dispose()
        'Try
        'sw.WriteLine(hd.DocumentNode.SelectSingleNode("//pre").InnerText)
        'Catch ex As Exception
        'log.Error("Errore durante il parsing degli episodi della serie: " & seriename)
        ' log.Error("Il messaggio di errore è il seguente: " & ex.Message)
        ' End Try
        'sw.Close()
        'sw.Dispose()
    End Sub

    Private Sub extractEpisodeList(ByVal seriename As String)
        Dim sr As StreamReader = New StreamReader(My.Application.Info.DirectoryPath & "\temp\" & seriename & ".list")
        Dim si As serieinfo
        Dim rowfields() As String
        Dim episodefields() As String
        Do While Not sr.EndOfStream
            Try
                Dim row As String = sr.ReadLine()
                rowfields = row.Split("|")
                episodefields = rowfields(0).Split("-")
                Dim episodeseason As Integer = Integer.Parse(episodefields(0).ToString)
                Dim episodenumber As Integer = Integer.Parse(episodefields(1).ToString)
                Dim episodeairdate As Date = date2date(rowfields(1))
                'If IsNumeric(row.Substring(0, 1)) Then
                'Dim strEpisodeCounter As String = row.Substring(0, 7).Replace(".", "").Trim
                '    Dim strepisodeNumber As String = row.Substring(7, 10).Trim
                '    Dim strairDate As String = row.Substring(27, 9).Trim
                '    Dim episodecounter As Integer
                '    If Integer.TryParse(strEpisodeCounter, episodecounter) Then
                '        Dim episodeseason As Integer = Integer.Parse(strepisodeNumber.Substring(0, strepisodeNumber.IndexOf("-")))
                '        Dim episodenumber As Integer = Integer.Parse(strepisodeNumber.Substring(strepisodeNumber.IndexOf("-") + 1, strepisodeNumber.Length - (strepisodeNumber.IndexOf("-") + 1)))
                si.season = episodeseason
                si.episode = episodenumber
                si.airDate = episodeairdate
                c.Add(si)
            Catch e As Exception
            End Try
        Loop
        sr.Close()
        sr.Dispose()
    End Sub
    Private Function date2date(ByVal dateIn As String) As Date
        Dim day As Integer = Integer.Parse(dateIn.Substring(0, 2))
        Dim year As Integer = Integer.Parse(dateIn.Substring(7, 2))
        If year > 70 Then
            year = year + 1900
        Else
            year = year + 2000
        End If
        Dim month As Integer = 0
        Select Case dateIn.Substring(3, 3).ToLower
            Case "jan"
                month = 1
            Case "feb"
                month = 2
            Case "mar"
                month = 3
            Case "apr"
                month = 4
            Case "may"
                month = 5
            Case "jun"
                month = 6
            Case "jul"
                month = 7
            Case "aug"
                month = 8
            Case "sep"
                month = 9
            Case "oct"
                month = 10
            Case "nov"
                month = 11
            Case "dec"
                month = 12
        End Select
        Return New Date(year, month, day)
    End Function

    Sub InsertIntoTable(ByVal SerieName As String)
        Dim tsH As table_HIS_SERIES_CALENDAR = New table_HIS_SERIES_CALENDAR
        tsH.deleteTable(SerieName)
        For i = 0 To c.Count - 1
            tsH.InsertIntoTable(SerieName, c.Item(i).season, c.Item(i).episode, c.Item(i).airDate, seriefileNameCreator(SerieName, c.Item(i).season, c.Item(i).episode))
        Next
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
