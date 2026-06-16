Option Strict On

Namespace Converter

    Public Class Config

        Private ProcessorCount As Integer
        Private OutputCodec As String
        Private OutputExtension As String
        Private Paths As List(Of String)
        Private DebugMode As Boolean

        Public Sub New(params As String())
            ProcessorCount = Environment.ProcessorCount
            Paths = New List(Of String)(params.Length)
            OutputCodec = Nothing
            OutputExtension = Nothing
            DebugMode = False

            ParseArgs(params)

            If OutputCodec Is Nothing Or OutputExtension Is Nothing Then
                ' if nothing is provided let's use 192kbps Opus
                OutputCodec = "libopus"
                OutputExtension = "opus"
            End If
        End Sub

        Private Sub ParseArgs(args As String())
            Dim numberOfPaths As Integer = 0

            For i As Integer = 0 To args.Length - 1
                Dim arg As String = args(i)

                If arg(0) = "-" Then
                    ' if the argument starts with the dash it's a config param, let's understand
                    ' which it is
                    Select Case arg(1)
                        Case "p"c
                            i += 1
                            Dim providedProcessorCount As Integer = Convert.ToInt32(args(i))

                            If providedProcessorCount > 0 Then
                                ProcessorCount = providedProcessorCount
                            Else
                                Console.WriteLine($"The provided processor count {args(i)} Is Not valid, so the system {ProcessorCount} available ones will be used!")
                            End If

                        Case "c"c
                            i += 1
                            OutputCodec = args(i)

                        Case "e"c
                            i += 1
                            OutputExtension = args(i)

                        Case "d"c
                            DebugMode = True

                    End Select
                Else
                    ' there's no dash so it's a path, let's add it to the paths
                    Paths.Add(arg)
                    numberOfPaths += 1
                End If

            Next

            ' let's check for massive capacity errors that must be corrected (if the
            ' threshold is low it's not worth the reallocation)
            If Paths.Capacity >= 2 * numberOfPaths Then
                Paths.TrimExcess()
            End If
        End Sub

        Public Function GetPaths() As ICollection(Of String)
            Return Paths
        End Function

        Public Function GetProcessors() As Integer
            Return ProcessorCount
        End Function

        Public Function GetCodec() As String
            Return OutputCodec
        End Function

        Public Function GetExtension() As String
            Return OutputExtension
        End Function

        Public Function IsDebug() As Boolean
            Return DebugMode
        End Function

    End Class

End Namespace
