Option Strict On

Imports FFmpegConverter.Converter

Module Program
    Sub Main(args As String())

        ' let's check if the user provided at least a path
        If args.Count <= 0 Then
            Console.WriteLine("You must provide at least a path!")
            Return
        End If

        ' let's parse the arguments

        ' let's create and start the converter
        Dim converter As New Converter()

    End Sub
End Module
