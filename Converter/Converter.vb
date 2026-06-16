Option Strict On

Imports System.Collections.Concurrent
Imports System.IO
Imports System.Threading

Namespace Converter

    Public Class Converter

        Private InputFiles As ConcurrentQueue(Of String)
        Private CompletedFiles As Integer
        Private ProcessorCount As Integer
        Private ErrorList As ConcurrentBag(Of FileError)
        Private OutputCodec As String
        Private Extension As String

        Public Sub New(files As ICollection(Of String), outputCodec As String, extension As String, processorCount As Integer)
            CompletedFiles = 0
            ErrorList = New ConcurrentBag(Of FileError)()

            Me.Extension = extension
            Me.OutputCodec = outputCodec
            Me.ProcessorCount = processorCount

            AllocateQueue(files)
        End Sub

        Public Sub Start()
            Console.WriteLine($"Starting converting {InputFiles.Count} files using {ProcessorCount} processors...")

            ' let's convert the files in parallel
            Parallel.For(0, ProcessorCount, AddressOf ConvertFiles)

            Console.WriteLine($"Conversion finished! {CompletedFiles} completed files and {ErrorList.Count} errors.")
        End Sub

        Private Sub ConvertFiles(processorId As Integer)
            ' let's extract an element from the queue
            Dim currentFile As String = Nothing

            ' let's grab a file from the queue (if there are still)
            While InputFiles.TryDequeue(currentFile)

                ' let's extract the file name that we'll later use for the output
                Dim fileName As String = Path.GetFileNameWithoutExtension(currentFile)

                ' let's define the process options, start the process and save the error output
                Dim processOptions As New ProcessStartInfo With {
                    .FileName = "ffmpeg",
                    .Arguments = $"-i ""{currentFile}"" -vn -c:a {OutputCodec} ""{fileName}.{Extension}""",
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardError = True
                }

                Dim process As Process = Process.Start(processOptions)
                Dim errorOutput As String = process.StandardError.ReadToEnd()
                process.WaitForExit()

                ' if the process ended gracefully everything's fine, otherwise we'll save the error in the list
                If process.ExitCode <> 0 Then
                    Console.WriteLine($"[{processorId + 1}/{ProcessorCount}]: Conversion of {fileName} failed!")
                    ErrorList.Add(New FileError(currentFile, errorOutput))
                Else
                    Console.WriteLine($"[{processorId + 1}/{ProcessorCount}]: Conversion of {fileName} completed!")
                End If

                ' let's update in a thread safe way the completed files counter
                Interlocked.Increment(CompletedFiles)

            End While
        End Sub

        Private Sub AllocateQueue(files As ICollection(Of String))
            ' for each file let's check if it exists or not, in case of directory let's add
            ' each of its file recursively

            InputFiles = New ConcurrentQueue(Of String)

            For Each filePath In files

                If File.Exists(filePath) Then
                    ' the path is a file, let's add it to the queue
                    InputFiles.Enqueue(filePath)

                ElseIf Directory.Exists(filePath) Then
                    ' it's a directory so let's fully explore it
                    ExploreDirectory(filePath)

                Else
                    ' the provided path doesn't exist so let's add it as an error
                    ErrorList.Add(New FileError(filePath, FileError.NOT_EXISTS))

                End If

            Next
        End Sub

        Private Sub ExploreDirectory(directoryPath As String)

            ' let's add the files
            For Each file In Directory.GetFiles(directoryPath)
                InputFiles.Enqueue(file)
            Next

            Dim parallelOptions As New ParallelOptions With {
                .MaxDegreeOfParallelism = Environment.ProcessorCount
            }

            ' let's call the procedure on the directories recursively in parallel
            Parallel.ForEach(Directory.GetDirectories(directoryPath), parallelOptions, AddressOf ExploreDirectory)

        End Sub

        Public Function GetErrors() As List(Of FileError)
            Return New List(Of FileError)(ErrorList)
        End Function

    End Class

End Namespace
