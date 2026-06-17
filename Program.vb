Option Strict On

Imports BulkConverter.Converter

Module Program

    Sub Main(args As String())

        ' let's check if the user provided at least a path
        If args.Count <= 0 Then
            Console.WriteLine("You must provide at least a path!")
            Return
        End If

        ' let's parse the arguments
        Dim config As New Config(args)

        ' let's create and start the converter
        Dim converter As New Converter.Converter(config.GetPaths(), config.GetCodec(), config.GetExtension(), config.GetProcessors())

        converter.Start()

        If config.IsDebug() Then
            For Each err As FileError In converter.GetErrors()
                Console.WriteLine(err)
            Next
        End If

    End Sub

End Module
