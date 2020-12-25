
Namespace Global.MyEvents.Repository

    Public Interface IMyEventsRepository

        ReadOnly Property Events As IEventRepository
        ReadOnly Property Composers As IComposerRepository
        ReadOnly Property Directors As IDirectorRepository
        ReadOnly Property Performers As IPerformerRepository
        ReadOnly Property Soloists As ISoloistRepository
        ReadOnly Property Venues As IVenueRepository
        ReadOnly Property Countries As ICountryRepository
        ReadOnly Property ImportExportService As IImportExportService

        Sub StartMassUpdate()
        Function EndMassUpdateAsync() As Task

    End Interface

End Namespace
