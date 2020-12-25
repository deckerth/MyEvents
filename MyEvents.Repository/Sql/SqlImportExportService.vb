Imports System.IO
Imports MyEvents.Models
Imports OfficeOpenXml

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlImportExportService
        Implements IImportExportService

        Private Const EventsWorkbook As String = "Events"
        Private Const ComposersWorkbook As String = "Composers"
        Private Const DirectorsWorkbook As String = "Directors"
        Private Const PerformersWorkbook As String = "Performers"
        Private Const SoloistsWorkbook As String = "Soloists"
        Private Const VenuesWorkbook As String = "Venues"
        Private Const CountriesWorkbook As String = "Countries"

        Private Repository As SqlMyEventsRepository

        Public Sub New(repo As SqlMyEventsRepository)
            Repository = repo
        End Sub

        Private Async Function ImportEventsAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task(Of UpdateCounters)

            Dim rows = worksheet.Dimension.Rows
            Dim books As New List(Of Performance)
            Dim counters As UpdateCounters
            For i = 2 To rows
                Dim errorFlag As Boolean = False
                Dim anEvent As New Performance
                Try
                    anEvent.Composer = worksheet.Cells(i, 2).Value.ToString()
                Catch ex As Exception
                    errorFlag = True
                End Try
                If Not errorFlag Then
                    Try
                        anEvent.Work = worksheet.Cells(i, 3).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.Director = worksheet.Cells(i, 4).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.Performer = worksheet.Cells(i, 5).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.Contributors = worksheet.Cells(i, 6).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.PerformanceDate = worksheet.Cells(i, 7).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.Venue = worksheet.Cells(i, 8).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Dim eventsType As String
                    Try
                        eventsType = worksheet.Cells(i, 9).Value.ToString()
                        Select Case eventsType
                            Case "Opera"
                                anEvent.Type = Performance.PerformanceType.Opera
                            Case "Ballet"
                                anEvent.Type = Performance.PerformanceType.Ballet
                            Case "Cinema"
                                anEvent.Type = Performance.PerformanceType.Cinema
                            Case "Concert"
                                anEvent.Type = Performance.PerformanceType.Concert
                            Case "OpenAir"
                                anEvent.Type = Performance.PerformanceType.OpenAir
                            Case "Theater"
                                anEvent.Type = Performance.PerformanceType.Theater
                        End Select
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.Link = worksheet.Cells(i, 10).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        anEvent.PerformanceCountry = worksheet.Cells(i, 11).Value.ToString()
                    Catch ex As Exception
                    End Try
                    books.Add(anEvent)
                End If
            Next

            Dim repo As SqlEventRepository = DirectCast(Repository.Events, SqlEventRepository)
            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                counters = Await repo.SetEvents(books)
            Else
                counters = Await repo.AddEvents(books)
            End If
            Return counters
        End Function

        Private Async Function ExportEventsAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(EventsWorkbook)

            'Add the headers

            worksheet.Cells(1, 2).Value = "Composer"
            worksheet.Cells(1, 3).Value = "Work"
            worksheet.Cells(1, 4).Value = "Director"
            worksheet.Cells(1, 5).Value = "Performer"
            worksheet.Cells(1, 6).Value = "Contributors"
            worksheet.Cells(1, 7).Value = "PerformanceDate"
            worksheet.Cells(1, 8).Value = "Venue"
            worksheet.Cells(1, 9).Value = "EventType"
            worksheet.Cells(1, 10).Value = "Link"
            worksheet.Cells(1, 11).Value = "PerformanceCountry"

            Dim events = Await Repository.Events.GetAsync()

            For i = 1 To events.Count
                Dim performance = events(i - 1)
                worksheet.Cells(i + 1, 2).Value = performance.Composer
                worksheet.Cells(i + 1, 3).Value = performance.Work
                worksheet.Cells(i + 1, 4).Value = performance.Director
                worksheet.Cells(i + 1, 5).Value = performance.Performer
                worksheet.Cells(i + 1, 6).Value = performance.Contributors
                worksheet.Cells(i + 1, 7).Value = performance.PerformanceDate
                worksheet.Cells(i + 1, 8).Value = performance.Venue

                Select Case performance.Type
                    Case Performance.PerformanceType.Opera
                        worksheet.Cells(i + 1, 9).Value = "Opera"
                    Case Performance.PerformanceType.Ballet
                        worksheet.Cells(i + 1, 9).Value = "Ballet"
                    Case Performance.PerformanceType.Cinema
                        worksheet.Cells(i + 1, 9).Value = "Cinema"
                    Case Performance.PerformanceType.Concert
                        worksheet.Cells(i + 1, 9).Value = "Concert"
                    Case Performance.PerformanceType.OpenAir
                        worksheet.Cells(i + 1, 9).Value = "OpenAir"
                    Case Performance.PerformanceType.Theater
                        worksheet.Cells(i + 1, 9).Value = "Theater"
                End Select

                worksheet.Cells(i + 1, 10).Value = performance.Link
                worksheet.Cells(i + 1, 11).Value = performance.PerformanceCountry

            Next

        End Function

        Private Async Function ImportComposersAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Composer)
            For i = 2 To rows
                Dim item As New Composer With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Composers, SqlComposerRepository).SetComposers(items)
            Else
                Await DirectCast(Repository.Composers, SqlComposerRepository).AddComposers(items)
            End If

        End Function

        Private Async Function ExportComposersAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(ComposersWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Composers.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportDirectorsAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Director)
            For i = 2 To rows
                Dim item As New Director With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Directors, SqlDirectorRepository).SetDirectors(items)
            Else
                Await DirectCast(Repository.Directors, SqlDirectorRepository).AddDirectors(items)
            End If

        End Function

        Private Async Function ExportDirectorsAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(DirectorsWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Directors.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportPerformersAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Performer)
            For i = 2 To rows
                Dim item As New Performer With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Performers, SqlPerformerRepository).SetPerformers(items)
            Else
                Await DirectCast(Repository.Performers, SqlPerformerRepository).AddPerformers(items)
            End If

        End Function

        Private Async Function ExportPerformersAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(PerformersWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Performers.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportSoloistsAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Soloist)
            For i = 2 To rows
                Dim item As New Soloist With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Soloists, SqlSoloistRepository).SetSoloists(items)
            Else
                Await DirectCast(Repository.Soloists, SqlSoloistRepository).AddSoloists(items)
            End If

        End Function

        Private Async Function ExportSoloistsAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(SoloistsWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Soloists.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportCountriesAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Country)
            For i = 2 To rows
                Dim item As New Country With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Countries, SqlCountryRepository).SetCountries(items)
            Else
                Await DirectCast(Repository.Countries, SqlCountryRepository).AddCountries(items)
            End If

        End Function

        Private Async Function ExportCountriesAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(CountriesWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Countries.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportVenuesAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Venue)
            For i = 2 To rows
                Dim item As New Venue With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                Try
                    item.Country = worksheet.Cells(i, 2).Value.ToString()
                Catch ex As Exception
                End Try
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceEvents Then
                Await DirectCast(Repository.Venues, SqlVenueRepository).SetVenues(items)
            Else
                Await DirectCast(Repository.Venues, SqlVenueRepository).AddVenues(items)
            End If

        End Function

        Private Async Function ExportVenuesAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(VenuesWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Venues.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
                worksheet.Cells(i + 1, 2).Value = item.Country
            Next

        End Function

        Public Async Function ImportAsync(InputStream As Stream, ImportOption As IImportExportService.ImportOptions) As Task(Of UpdateCounters) Implements IImportExportService.ImportAsync
            Dim counters As New UpdateCounters

            If InputStream IsNot Nothing Then
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial
                Using package = New ExcelPackage(InputStream)
                    counters = Await ImportEventsAsync(package.Workbook.Worksheets(EventsWorkbook), ImportOption)
                    Await ImportComposersAsync(package.Workbook.Worksheets(ComposersWorkbook), ImportOption)
                    Await ImportDirectorsAsync(package.Workbook.Worksheets(DirectorsWorkbook), ImportOption)
                    Await ImportPerformersAsync(package.Workbook.Worksheets(PerformersWorkbook), ImportOption)
                    Await ImportSoloistsAsync(package.Workbook.Worksheets(SoloistsWorkbook), ImportOption)
                    Await ImportVenuesAsync(package.Workbook.Worksheets(VenuesWorkbook), ImportOption)
                    Await ImportCountriesAsync(package.Workbook.Worksheets(CountriesWorkbook), ImportOption)
                End Using
            End If
            Return counters
        End Function

        Public Async Function ExportAsync(OutputStream As Stream) As Task Implements IImportExportService.ExportAsync
            If OutputStream IsNot Nothing Then
                Dim package As ExcelPackage
                Try
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial
                    package = New ExcelPackage(OutputStream)
                Catch ex As Exception
                End Try
                While package.Workbook.Worksheets.Count > 0
                    package.Workbook.Worksheets.Delete(0)
                End While
                Await ExportEventsAsync(package)
                Await ExportComposersAsync(package)
                Await ExportDirectorsAsync(package)
                Await ExportPerformersAsync(package)
                Await ExportSoloistsAsync(package)
                Await ExportVenuesAsync(package)
                Await ExportCountriesAsync(package)
                package.Save()
            End If
        End Function
    End Class

End Namespace
