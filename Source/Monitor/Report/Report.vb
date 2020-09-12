Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Web
Public Class Report

    Function getMissingVideo() As Boolean
        Dim retval As Boolean = True

        Dim cmd As OleDbCommand = New OleDbCommand("select * from seriesinfo,series where seriesinfo.seriename = series.seriename and episodeairdate < @oggi and serieseason >= fromseason and videoexist = 'N' order by seriesinfo.seriename,serieseason,serieepisode", DBConnection.getConnection())
        Dim sb As StringBuilder = New StringBuilder
        cmd.Parameters.Add("@oggi", OleDbType.Date).Value = Date.Today()
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        sb.AppendLine("<html>")
        sb.AppendLine("<head>")
        sb.AppendLine("<meta http-equiv=""refresh"" content=""90"" >")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""seriesmonitor.css"">")
        sb.AppendLine("<title>SeriesMonitor " & Date.Now & "</title>")
        sb.AppendLine("</head>")
        sb.AppendLine("<body>")
        sb.AppendLine("<h2>Report generato il " & Date.Now & "</h2>")
        sb.AppendLine("<br/>")
        sb.AppendLine("<h1>Episodi mancanti</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<th>Serie</th>")
        sb.AppendLine("<th>Stagione</th>")
        sb.AppendLine("<th>Episodio</th>")
        'sb.AppendLine("<th>Fenopy</th>")
        sb.AppendLine("<th>Pirate Bay</th>")
        sb.AppendLine("<th>Kick Ass Torrent</th>")
        sb.AppendLine("</tr>")



        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("seriesinfo.seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td>")
                Dim sb0 As StringBuilder = New StringBuilder
                'sb0.Append("http://fenopy.se/?keyword=")
                'sb0.Append(dr("seriesinfo.seriename").ToString.Replace(" ", "+"))
                'sb0.Append("+")
                'sb0.Append("S")
                'sb0.Append(addzero(dr("serieSeason")))
                'sb0.Append("+")
                'sb0.Append("E")
                'sb0.Append(addzero(dr("serieEpisode")))
                'sb.Append("<a href=""" & sb0.ToString() & """>" & "Cerca su Fenopy" & "</a>")
                'sb.Append("</td><td>")
                sb0 = New StringBuilder
                sb0.Append("http://pirateproxy.net/search/")
                sb0.Append(dr("seriesinfo.seriename"))
                sb0.Append(" ")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Pirate Bay" & "</a>")
                sb.AppendLine("</td><td>")
                sb0 = New StringBuilder
                sb0.Append("https://kickass.to/usearch/")
                sb0.Append(dr("seriesinfo.seriename"))
                sb0.Append(" ")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Kick Ass Torrent" & "</a>")
                sb.AppendLine("</td></tr>")
            End While
        Else
            sb.AppendLine("<tr><td colspan=""6"">Nessuna puntata mancante</td></tr>")
            retval = False
        End If
        sb.AppendLine("</table>")
        dr.Close()


        Dim fs As DirectoryInfo = New DirectoryInfo("h:\serietv\@staging\")
        sb.AppendLine("<h1>Video in attesa di validazione</h1>")

        If fs.GetFiles.Count > 0 Then
            sb.AppendLine("<table>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th>Serie</th>")
            sb.AppendLine("<th>Stagione</th>")
            sb.AppendLine("<th>Episodio</th>")
            sb.AppendLine("<th>Nome file</th>")
            sb.AppendLine("</tr>")

            For Each fi In fs.GetFiles
                Select Case fi.Extension
                    Case ".avi", ".mpg", ".mp4", ".mkv"
                        sb.AppendLine("<tr>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(fi.Name.Substring(0, fi.Name.IndexOf("-") - 1))
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(LookupFx.ricercastagioneX(fi.Name).ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(LookupFx.ricercaEpisodioX(fi.Name).ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(fi.Name.ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("</tr>")
                End Select
            Next
            sb.AppendLine("</table>")
        End If
        'cmd = New OleDbCommand("select * from subtitles where isdownloaded = 'N' order by site,seriename", DBConnection.getConnection())
        Dim sbQuery As StringBuilder = New StringBuilder
        sbQuery.Append("select seriesinfo.seriename, seriesinfo.serieseason, seriesinfo.serieepisode, subtitles.site, subtitles.downloadlink ")
        sbQuery.Append("from seriesinfo,series,subtitles ")
        sbQuery.Append("where subcreated = 'S' ")
        sbQuery.Append("and subexist = 'N' ")
        sbQuery.Append("and seriesinfo.seriename = series.seriename ")
        sbQuery.Append("and seriesinfo.serieseason >= series.fromseason ")
        sbQuery.Append("and subtitles.seriename = seriesinfo.seriename ")
        sbQuery.Append("and subtitles.stagione = seriesinfo.serieseason ")
        sbQuery.Append("and subtitles.episodio = seriesinfo.serieepisode ")
        'sbQuery.Append("and seriesinfo.videoexist <> 'N' ")
        sbQuery.Append("and seriesinfo.episodeAirDate < @today ")
        sbQuery.Append("order by seriesinfo.seriename, seriesinfo.serieseason, seriesinfo.serieepisode ")

        cmd = New OleDbCommand(sbQuery.ToString, DBConnection.getConnection)
        cmd.Parameters.Add("@today", OleDbType.Date).Value = Date.Today

        dr = cmd.ExecuteReader
        If dr.HasRows Then
            sb.AppendLine("<h1>Sottotitoli mancanti</h1>")
            sb.AppendLine("<table>")
            sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Subber</th><th>Link</th></tr>")
            While dr.Read
                sb.Append("<tr>")
                sb.Append("<td>")
                sb.Append(dr("SerieName").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("SerieSeason").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("SerieEpisode").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Site").ToString)
                sb.Append("</td>")
                sb.Append("<td>")

                If dr("downloadlink").ToString <> "" Then
                    sb.Append("<a href='" & dr("downloadlink").ToString & "' target='_blank'>Link</a>")
                End If

                sb.Append("</td>")


                sb.AppendLine("<tr>")
            End While
            sb.AppendLine("</table><br/>")
        End If


        If fs.GetFiles("*.srt").Count > 0 Then
            sb.AppendLine("<br/>")
            sb.AppendLine("<h1>Sottotitoli in attesa di validazione</h1>")
            sb.AppendLine("<br/>")
            sb.AppendLine("<table>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th>Serie</th>")
            sb.AppendLine("<th>Stagione</th>")
            sb.AppendLine("<th>Episodio</th>")
            sb.AppendLine("<th>Nome file</th>")
            sb.AppendLine("</tr>")

            For Each fi In fs.GetFiles
                Select Case fi.Extension
                    Case ".srt"
                        sb.AppendLine("<tr>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(fi.Name.Substring(0, fi.Name.IndexOf("-") - 1))
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(LookupFx.ricercastagioneX(fi.Name).ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(LookupFx.ricercaEpisodioX(fi.Name).ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("<td>")
                        sb.AppendLine(fi.Name.ToString)
                        sb.AppendLine("</td>")
                        sb.AppendLine("</tr>")
                End Select
            Next
            sb.AppendLine("</table>")
        End If











        cmd = New OleDbCommand("select * from seriesinfo  where episodeairdate >= @data1 and episodeairdate <= @data2  order by episodeairdate asc", DBConnection.getConnection())
        cmd.Parameters.Add("@data1", OleDbType.Date).Value = Date.Today.AddDays(-1)
        cmd.Parameters.Add("@data2", OleDbType.Date).Value = Date.Today.AddDays(Double.Parse(My.Settings.REPORT_FILE_NUM_OF_DAYS))
        dr = cmd.ExecuteReader
        sb.AppendLine("<h1>Prossimi episodi</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Airdate</th></tr>")
        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td>")
                Dim app As String = Date.Parse(dr("EpisodeAirDate").ToString).ToString("dddd, dd MMMM yyyy")
                app = Replace(app, "ì", "&igrave;")
                sb.Append(app)
                sb.AppendLine("</td></tr>")
            End While

        End If
        sb.AppendLine("</table>")

        sb.Append("<br/>")
        sb.Append("<br/>")
        sb.Append("<br/>")
        sb.AppendLine("<h3><a href='file://g:\downloads\Vuze\completed'>Cartella file scaricati</a></h3>")
        sb.Append("<br/>")
        sb.AppendLine("</body></html>")
        Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\report\x" & My.Settings.REPORT_FILE_NAME)
        sw.Write(sb.ToString)
        sw.Close()
        sw.Dispose()
        cmd.Dispose()
        Return retval
    End Function




    Function addzero(ByVal value As Integer) As String
        Dim strvalue As String = value.ToString
        If strvalue.Length = 1 Then
            Return "0" & strvalue
        Else
            Return strvalue
        End If
    End Function



    Function getMissingInfo() As Boolean
        Dim retval As Boolean = True

        Dim cmd As OleDbCommand = New OleDbCommand("select his_series_calendar.seriename,his_series_calendar.serieseason, his_series_calendar.serieepisode from HIS_SERIES_CALENDAR,SERIES where episodeAirDate < @today and HIS_SERIES_CALENDAR.serieName = series.serieName and HIS_SERIES_CALENDAR.serieseason >= series.fromseason and filename not in (select replace(replace(replace(filename,'.avi','.???'),'.mkv','.???'),'.mp4','.???') from HIS_VIDEO_FILES)", DBConnection.getConnection())
        Dim sb As StringBuilder = New StringBuilder
        cmd.Parameters.Add("@today", OleDbType.Date).Value = Date.Today()
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        sb.AppendLine("<html>")
        sb.AppendLine("<head>")
        sb.AppendLine("<meta http-equiv=""refresh"" content=""90"" >")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""seriesmonitor.css"">")
        sb.AppendLine("<title>SeriesMonitor " & Date.Now & "</title>")
        sb.AppendLine("</head>")
        sb.AppendLine("<body>")
        sb.AppendLine("<h2>Report generato il " & Date.Now & "</h2>")
        sb.AppendLine("<br/>")
        sb.AppendLine("<h1>Episodi mancanti</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<th>Serie</th>")
        sb.AppendLine("<th>Stagione</th>")
        sb.AppendLine("<th>Episodio</th>")
        'sb.AppendLine("<th>Fenopy</th>")
        sb.AppendLine("<th>Pirate Bay</th>")
        sb.AppendLine("<th>Kick Ass Torrent</th>")
        sb.AppendLine("</tr>")

        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td>")
                Dim sb0 As StringBuilder = New StringBuilder
                sb0 = New StringBuilder
                sb0.Append("http://pirateproxy.net/search/")
                sb0.Append(dr("seriename"))
                sb0.Append(" ")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Pirate Bay" & "</a>")
                sb.AppendLine("</td><td>")
                sb0 = New StringBuilder
                sb0.Append("https://kickass.to/usearch/")
                sb0.Append(dr("seriename"))
                sb0.Append(" ")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Kick Ass Torrent" & "</a>")
                sb.AppendLine("</td></tr>")
            End While
        Else
            sb.AppendLine("<tr><td colspan=""6"">Nessuna puntata mancante</td></tr>")
            retval = False
        End If
        sb.AppendLine("</table>")
        dr.Close()

        cmd = New OleDbCommand("select * from HIS_VIDEO_FILES where folder = 'Staging'", DBConnection.getConnection)
        dr = cmd.ExecuteReader

        If dr.HasRows Then
            sb.AppendLine("<h1>Video in attesa di validazione</h1>")
            sb.AppendLine("<table>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th>Serie</th>")
            sb.AppendLine("<th>Stagione</th>")
            sb.AppendLine("<th>Episodio</th>")
            sb.AppendLine("<th>Nome file</th>")
            sb.AppendLine("</tr>")
            While dr.Read
                sb.AppendLine("<tr>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("serieName").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("SerieSeason").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("serieEpisode").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("filename").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("</tr>")
            End While
            sb.AppendLine("</table>")
        End If
        dr.Close()

        cmd = New OleDbCommand("select * from HIS_PUBLISHED_SUBTITLES where filename not in (select fileName from HIS_SRT_FILES)", DBConnection.getConnection)
        dr = cmd.ExecuteReader

        If dr.HasRows Then
            sb.AppendLine("<h1>Sottotitoli mancanti</h1>")
            sb.AppendLine("<table>")
            sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Subber</th><th>Link</th></tr>")
            While dr.Read
                sb.Append("<tr>")
                sb.Append("<td>")
                sb.Append(dr("SerieName").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Stagione").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Episodio").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Site").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                If dr("downloadlink").ToString <> "" Then
                    sb.Append("<a href='" & dr("downloadlink").ToString & "' target='_blank'>Link</a>")
                End If
                sb.Append("</td>")
                sb.AppendLine("<tr>")
            End While
            sb.AppendLine("</table><br/>")
        End If
        dr.Close()

        cmd = New OleDbCommand("select * from HIS_SRT_FILES where folder = 'Staging'", DBConnection.getConnection)
        dr = cmd.ExecuteReader

        If dr.HasRows Then
            sb.AppendLine("<h1>Sottotitoli in attesa di validazione</h1>")
            sb.AppendLine("<table>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th>Serie</th>")
            sb.AppendLine("<th>Stagione</th>")
            sb.AppendLine("<th>Episodio</th>")
            sb.AppendLine("<th>Nome file</th>")
            sb.AppendLine("</tr>")
            While dr.Read
                sb.AppendLine("<tr>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("serieName").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("SerieSeason").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("serieEpisode").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("<td>")
                sb.AppendLine(dr("filename").ToString)
                sb.AppendLine("</td>")
                sb.AppendLine("</tr>")
            End While
            sb.AppendLine("</table>")
        End If
        dr.Close()



        'If fs.GetFiles("*.srt").Count > 0 Then
        '    sb.AppendLine("<br/>")
        '    sb.AppendLine("<h1>Sottotitoli in attesa di validazione</h1>")
        '    sb.AppendLine("<br/>")
        '    sb.AppendLine("<table>")
        '    sb.AppendLine("<tr>")
        '    sb.AppendLine("<th>Serie</th>")
        '    sb.AppendLine("<th>Stagione</th>")
        '    sb.AppendLine("<th>Episodio</th>")
        '    sb.AppendLine("<th>Nome file</th>")
        '    sb.AppendLine("</tr>")

        '    For Each fi In fs.GetFiles
        '        Select Case fi.Extension
        '            Case ".srt"
        '                sb.AppendLine("<tr>")
        '                sb.AppendLine("<td>")
        '                sb.AppendLine(fi.Name.Substring(0, fi.Name.IndexOf("-") - 1))
        '                sb.AppendLine("</td>")
        '                sb.AppendLine("<td>")
        '                sb.AppendLine(LookupFx.ricercastagioneX(fi.Name).ToString)
        '                sb.AppendLine("</td>")
        '                sb.AppendLine("<td>")
        '                sb.AppendLine(LookupFx.ricercaEpisodioX(fi.Name).ToString)
        '                sb.AppendLine("</td>")
        '                sb.AppendLine("<td>")
        '                sb.AppendLine(fi.Name.ToString)
        '                sb.AppendLine("</td>")
        '                sb.AppendLine("</tr>")
        '        End Select
        '    Next
        '    sb.AppendLine("</table>")
        'End If


        cmd = New OleDbCommand("select * from HIS_SERIES_CALENDAR where episodeairdate >= @data1 and episodeairdate <= @data2  order by episodeairdate,seriename,serieSeason,serieEpisode asc", DBConnection.getConnection())
        cmd.Parameters.Add("@data1", OleDbType.Date).Value = Date.Today.AddDays(-1)
        cmd.Parameters.Add("@data2", OleDbType.Date).Value = Date.Today.AddDays(Double.Parse(My.Settings.REPORT_FILE_NUM_OF_DAYS))
        dr = cmd.ExecuteReader
        sb.AppendLine("<h1>Prossimi episodi</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Airdate</th></tr>")
        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td>")
                Dim app As String = Date.Parse(dr("EpisodeAirDate").ToString).ToString("dddd, dd MMMM yyyy")
                app = Replace(app, "ì", "&igrave;")
                sb.Append(app)
                sb.AppendLine("</td></tr>")
            End While

        End If
        sb.AppendLine("</table>")

        sb.Append("<br/>")
        sb.Append("<br/>")
        sb.Append("<br/>")
        sb.AppendLine("<h3><a href='file://g:\downloads\Vuze\completed'>Cartella file scaricati</a></h3>")
        sb.Append("<br/>")
        sb.AppendLine("</body></html>")
        Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\report\" & My.Settings.REPORT_FILE_NAME)
        sw.Write(sb.ToString)
        sw.Close()
        sw.Dispose()
        cmd.Dispose()
        Return retval
    End Function

End Class