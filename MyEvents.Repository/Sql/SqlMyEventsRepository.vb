Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlMyEventsRepository
        Implements IMyEventsRepository

        Private ReadOnly _dbOptions As DbContextOptions(Of MyEventsContext)
        Private ReadOnly _context As MyEventsContext

        Public Sub New(dbOptionsBuilder As DbContextOptionsBuilder(Of MyEventsContext))
            _dbOptions = dbOptionsBuilder.Options
            Using db As New MyEventsContext(_dbOptions)
                db.Database.Migrate()
            End Using
            _context = New MyEventsContext(_dbOptions)
        End Sub

        Private _events As IEventRepository = Nothing
        Public ReadOnly Property Events As IEventRepository Implements IMyEventsRepository.Events
            Get
                If _events Is Nothing Then
                    _events = New SqlEventRepository(_context)
                End If
                Return _events
            End Get
        End Property

        Private _composers As IComposerRepository = Nothing
        Public ReadOnly Property Composers As IComposerRepository Implements IMyEventsRepository.Composers
            Get
                If _composers Is Nothing Then
                    _composers = New SqlComposerRepository(_context)
                End If
                Return _composers
            End Get
        End Property

        Private _directors As IDirectorRepository = Nothing
        Public ReadOnly Property Directors As IDirectorRepository Implements IMyEventsRepository.Directors
            Get
                If _directors Is Nothing Then
                    _directors = New SqlDirectorRepository(_context)
                End If
                Return _directors
            End Get
        End Property

        Private _performers As IPerformerRepository = Nothing
        Public ReadOnly Property Performers As IPerformerRepository Implements IMyEventsRepository.Performers
            Get
                If _performers Is Nothing Then
                    _performers = New SqlPerformerRepository(_context)
                End If
                Return _performers
            End Get
        End Property

        Private _soloists As ISoloistRepository = Nothing
        Public ReadOnly Property Soloists As ISoloistRepository Implements IMyEventsRepository.Soloists
            Get
                If _soloists Is Nothing Then
                    _soloists = New SqlSoloistRepository(_context)
                End If
                Return _soloists
            End Get
        End Property

        Private _venues As IVenueRepository = Nothing
        Public ReadOnly Property Venues As IVenueRepository Implements IMyEventsRepository.Venues
            Get
                If _venues Is Nothing Then
                    _venues = New SqlVenueRepository(_context)
                End If
                Return _venues
            End Get
        End Property


        Public ReadOnly Property ImportExportService As IImportExportService Implements IMyEventsRepository.ImportExportService
            Get
                Return New SqlImportExportService(Me)
            End Get
        End Property

        Public Sub StartMassUpdate() Implements IMyEventsRepository.StartMassUpdate
            _context.StartMassUpdate()
        End Sub

        Public Async Function EndMassUpdateAsync() As Task Implements IMyEventsRepository.EndMassUpdateAsync
            Await _context.EndMassUpdateModeAsync
        End Function
    End Class

End Namespace
