Option Strict On

Namespace Converter

    Public Class FileError

        Public Const NOT_EXISTS As String = "The file doesn't exist"

        Private FilePath As String
        Private ErrorMessage As String

        Public Sub New(filePath As String, errorMessage As String)
            Me.FilePath = filePath
            Me.ErrorMessage = errorMessage
        End Sub

        Public Function GetFile() As String
            Return FilePath
        End Function

        Public Function GetError() As String
            Return ErrorMessage
        End Function

        Public Overrides Function ToString() As String
            Return $"Error: {ErrorMessage} on file {FilePath}"
        End Function

    End Class

End Namespace