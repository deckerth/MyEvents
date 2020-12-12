Imports System.IO

Namespace Global.MyEvents.Repository

    Public Interface IImportExportService
        Enum ImportOptions
            AddEvents
            ReplaceEvents
        End Enum

        Function ImportAsync(InputStream As Stream, ImportOption As ImportOptions) As Task(Of UpdateCounters)
        Function ExportAsync(OutputStream As Stream) As Task

    End Interface

End Namespace
