Imports System.Net
Imports System.Text
Imports System.IO
Imports Newtonsoft.Json

Module Module1

    Public Structure GetAdmin
        Private _UserName As String
        Public Property UserName As String
            Get
                Return _UserName
            End Get
            Set(ByVal value As String)
                _UserName = value
            End Set
        End Property

        Private _Password As String
        Public Property Password As String
            Get
                Return _Password
            End Get
            Set(ByVal value As String)
                _Password = value
            End Set
        End Property
    End Structure

    Public Structure PostData
        Private _JsonString As String
        Public Property JsonString As String
            Get
                Return _JsonString
            End Get
            Set(ByVal value As String)
                _JsonString = value
            End Set
        End Property

        Private _AuthenticationKey As String
        Public Property AuthenticationKey As String
            Get
                Return _AuthenticationKey
            End Get
            Set(ByVal value As String)
                _AuthenticationKey = value
            End Set
        End Property
    End Structure

    Public Structure GetHttpPostResponse
        Private _ResultStatus As String
        Public Property ResultStatus As String
            Get
                Return _ResultStatus
            End Get
            Set(ByVal value As String)
                _ResultStatus = value
            End Set
        End Property

        Private _Message As String
        Public Property Message As String
            Get
                Return _Message
            End Get
            Set(ByVal value As String)
                _Message = value
            End Set
        End Property
    End Structure

    Private strConf As String()
    Private strUserName As String
    Private strPassword As String
    Private strServerURL As String
    Private strJsonString As String
    Private strAuthenticationKey As String

    Sub Main()
        Try
            Dim sIniFile As String = System.Environment.CurrentDirectory & "\config.ini"
            Using sr As StreamReader = File.OpenText(sIniFile)
                Dim strLine As String = sr.ReadLine()
                Dim nextLine As String = strLine
                Do Until IsNothing(nextLine)
                    If IsNothing(nextLine) Then Exit Do
                    nextLine = sr.ReadLine()
                    strLine = strLine & "#" & nextLine
                Loop
                strConf = strLine.Split("#")
            End Using
            Dim stGetAdmin As GetAdmin = New GetAdmin()
            Dim strValue As String()
            strValue = strConf(1).Split("=")
            stGetAdmin.UserName = strValue(1)
            strValue = strConf(2).Split("=")
            stGetAdmin.Password = strValue(1)
            strValue = strConf(3).Split("=")
            strServerURL = strValue(1)
            strValue = strConf(4).Split("=")
            strJsonString = strValue(1)
            Dim strPostURL As String = strServerURL & "CheckAdminLoginAuthentication"
            Dim strPostData As String = JsonConvert.SerializeObject(stGetAdmin)
            Dim strRes As String = HttpPostResponse(strPostURL, strPostData)
            Dim objRes As GetHttpPostResponse = JsonConvert.DeserializeObject(strRes, GetType(GetHttpPostResponse))
            If objRes.ResultStatus.ToUpper <> "Failed".ToUpper Then
                strAuthenticationKey = objRes.Message.ToString()
                Dim stPostData As PostData = New PostData()
                stPostData.AuthenticationKey = strAuthenticationKey
                stPostData.JsonString = strJsonString
                strPostData = JsonConvert.SerializeObject(stPostData)
                strPostURL = strServerURL & "ReadPostedJobs"
                strRes = HttpPostResponse(strPostURL, strPostData)
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Console.ReadLine()
    End Sub

    Private Function HttpPostResponse(ByVal strPostURL As String, ByVal strPostData As String) As String
        Dim httpUrl As New System.Uri(strPostURL)
        Dim req As HttpWebRequest
        req = HttpWebRequest.Create(httpUrl)
        req.Timeout = 20000
        req.Method = "POST"
        req.ContentType = "application/x-www-form-urlencoded"
        Dim bytesData() As Byte = System.Text.Encoding.UTF8.GetBytes(strPostData)
        req.ContentLength = bytesData.Length
        Using postStream As Stream = req.GetRequestStream()
            postStream.Write(bytesData, 0, bytesData.Length)
        End Using
        Using res As HttpWebResponse = req.GetResponse()
            Console.WriteLine(res.StatusCode.ToString())
            Dim reader As StreamReader = New StreamReader(res.GetResponseStream, System.Text.Encoding.UTF8)
            Dim respResult As String = reader.ReadToEnd()
            Console.WriteLine(respResult)
            Return respResult
        End Using
        'Dim request As WebRequest = WebRequest.Create(strPostURL)
        'request.Method = "POST"
        'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(strPostData)
        'request.ContentType = "application/json"
        'request.ContentLength = byteArray.Length
        'Dim dataStream As Stream = request.GetRequestStream()
        'dataStream.Write(byteArray, 0, byteArray.Length)
        'dataStream.Close()
        'Dim response As WebResponse = request.GetResponse()
        'Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
        'dataStream = response.GetResponseStream()
        'Dim responseFromServer As String
        'Using reader As New StreamReader(dataStream)
        '    responseFromServer = reader.ReadToEnd()
        'End Using
        'dataStream.Close()
        'response.Close()
        'Console.WriteLine(responseFromServer)
        'Return responseFromServer
    End Function

End Module
