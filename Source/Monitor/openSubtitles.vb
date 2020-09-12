Imports System.Net
Imports System.Xml
Imports System.Data.OleDb
Imports log4net
Public Class openSubtitles
    Sub searchForNewSubtitles()
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim sql As String = "select * from REF_SERIES_LIST where idMovie is not null"
        Dim cmd As OleDbCommand = New OleDbCommand(sql, DBConnection.getConnection)
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        If dr.HasRows Then
            Do While dr.Read
                log.Debug("Cerco i sottotitoli per la serie " & dr("SerieName").ToString)

                Try
                    getSubtitlesInfo(dr("SerieName").ToString, dr("idMovie").ToString)
                Catch ex As Exception
                    log.Error("Errore durante la ricerca dei sottotitoli per la serie " & dr("SerieName").ToString)
                    log.Error("Error message -> " & ex.Message)
                End Try

            Loop
        End If
    End Sub
    Sub getSubtitlesInfo(SerieName As String, serieCode As String)
        Dim url As String = "https://www.opensubtitles.org/en/ssearch/sublanguageid-ita/idmovie-" & serieCode & "/XML"
        Dim serieSeason As String = String.Empty
        Dim serieEpisode As String = String.Empty
        Dim downloadlink As String = String.Empty
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)"
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        ' Check if the response is OK (status code 200)
        If response.StatusCode = System.Net.HttpStatusCode.OK Then
            ' Parse the contents from the response to a stream object
            Dim stream As System.IO.Stream = response.GetResponseStream()
            ' Create a reader for the stream object
            Dim reader As New System.IO.StreamReader(stream)
            ' Read from the stream object using the reader, put the contents in a string
            Dim contents As String = reader.ReadToEnd()
            ' Create a new, empty XML document
            Dim document As New System.Xml.XmlDocument()
            ' Load the contents into the XML document
            document.LoadXml(contents)
            Dim nl As XmlNodeList = document.SelectNodes("//subtitle")
            For i = 0 To nl.Count - 1
                Dim xmldoc As XmlDocument = New XmlDocument
                xmldoc.LoadXml("<s>" & nl(i).InnerXml & "</s>")
                Try
                    serieSeason = xmldoc.SelectSingleNode("//SeriesSeason").InnerText
                Catch ex As Exception
                    serieSeason = "0"
                End Try

                Try
                    serieEpisode = xmldoc.SelectSingleNode("//SeriesEpisode").InnerText
                Catch ex As Exception
                    serieEpisode = "0"
                End Try

                Try
                    downloadlink = xmldoc.SelectSingleNode("//SeriesDownloadsCnt").Attributes.GetNamedItem("LinkDownload").InnerText
                Catch ex As Exception
                    downloadlink = ""
                End Try

                If downloadlink <> "" And serieSeason <> "0" Then
                    table_HIS_PUBLISHED_SUBTITLES.InsertRaw(SerieName, Integer.Parse(serieSeason), Integer.Parse(serieEpisode), "OPENSUBTITLES", "https://www.opensubtitles.org" & downloadlink, Common.seriefileNameCreator(SerieName, Integer.Parse(serieSeason), Integer.Parse(serieEpisode)))
                End If

            Next
        End If
    End Sub



End Class
