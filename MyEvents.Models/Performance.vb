﻿Namespace Global.MyEvents.Models

    Public Class Performance
        Inherits DBObject
        Implements IEquatable(Of Performance)

        Enum PerformanceType
            Concert
            Ballet
            Theater
            Opera
            Cinema
            OpenAir
            Planned
            DeleteValue
            KeepValue
            Undefined
        End Enum

        Public Sub New()

        End Sub

        Public Sub New(perfType As PerformanceType)
            Type = perfType
        End Sub

        Private Shared _newPerformanceId As Guid = Guid.NewGuid()

        Public Shared ReadOnly Property NewPerformanceId As Guid
            Get
                Return _newPerformanceId
            End Get
        End Property

        Private Shared _allPerformanceTypes As List(Of PerformanceType)

        Public Shared ReadOnly Property AllPerformanceTypes As List(Of PerformanceType)
            Get
                If _allPerformanceTypes Is Nothing Then
                    _allPerformanceTypes = New List(Of PerformanceType)
                    _allPerformanceTypes.Add(PerformanceType.Concert)
                    _allPerformanceTypes.Add(PerformanceType.Ballet)
                    _allPerformanceTypes.Add(PerformanceType.Opera)
                    _allPerformanceTypes.Add(PerformanceType.Theater)
                    _allPerformanceTypes.Add(PerformanceType.Cinema)
                    _allPerformanceTypes.Add(PerformanceType.OpenAir)
                    _allPerformanceTypes.Add(PerformanceType.Planned)
                End If
                Return _allPerformanceTypes
            End Get
        End Property

        Public Property Work As String = ""
        Public Property Composer As String = ""
        Public Property Performer As String = ""
        Public Property Director As String = ""
        Public Property Venue As String = ""
        Public Property Contributors As String = ""
        Public Property PerformanceDate As String = ""
        Public Property Type As PerformanceType = PerformanceType.Undefined
        Public Property Link As String = ""
        Public Property PerformanceCountry As String = ""


        Public Function IEquatable_Equals(other As Performance) As Boolean Implements IEquatable(Of Performance).Equals
            Return Type = other.Type AndAlso
            Work.Equals(other.Work) AndAlso
                Composer.Equals(other.Composer) AndAlso
                Performer.Equals(other.Performer) AndAlso
                Director.Equals(other.Director) AndAlso
                Venue.Equals(other.Venue) AndAlso
                Contributors.Equals(other.Contributors) AndAlso
                PerformanceDate.Equals(other.PerformanceDate) AndAlso
                Link.Equals(other.Link) AndAlso
                PerformanceCountry.Equals(other.PerformanceCountry)
        End Function

        Public Function Clone() As Performance
            Return DirectCast(MemberwiseClone(), Performance)
        End Function

        Public Function GetContributorsList() As List(Of String)
            Dim separators As Char()
            Dim result = New List(Of String)
            separators = New [Char]() {"/"c, ";"c}
            Dim list = Contributors.Split(separators)
            If list.Count > 0 Then
                For Each a In list
                    If a IsNot Nothing Then
                        result.Add(a.Trim())
                    End If
                Next
            End If
            Return result
        End Function

        Public Function GetSoloistsList() As List(Of String)
            Dim separators As Char()
            Dim result = New List(Of String)
            separators = New [Char]() {"/"c, ";"c}
            Dim list = Performer.Split(separators)
            If list.Count > 0 Then
                For Each a In list
                    If a IsNot Nothing Then
                        result.Add(a.Trim())
                    End If
                Next
            End If
            Return result
        End Function

        Private Function UpdateString(ByRef source As String, value As String) As Boolean
            Dim changed As Boolean = False

            If String.IsNullOrEmpty(source) AndAlso String.IsNullOrEmpty(value) Then
                Return False
            End If
            If (String.IsNullOrEmpty(source) AndAlso Not String.IsNullOrEmpty(value)) OrElse
               (Not String.IsNullOrEmpty(source) AndAlso String.IsNullOrEmpty(value)) OrElse
               (Not source.Equals(value)) Then
                source = value
                changed = True
            End If

            Return changed
        End Function

        Public Function UpdateFrom(anotherEvent As Performance) As Boolean
            Dim changed As Boolean

            If Type <> anotherEvent.Type Then
                Type = anotherEvent.Type
                changed = True
            End If

            changed = changed Or UpdateString(Work, anotherEvent.Work)
            changed = changed Or UpdateString(Composer, anotherEvent.Composer)
            changed = changed Or UpdateString(Performer, anotherEvent.Performer)
            changed = changed Or UpdateString(Director, anotherEvent.Director)
            changed = changed Or UpdateString(Venue, anotherEvent.Venue)
            changed = changed Or UpdateString(Contributors, anotherEvent.Contributors)
            changed = changed Or UpdateString(PerformanceDate, anotherEvent.PerformanceDate)
            changed = changed Or UpdateString(Link, anotherEvent.Link)
            changed = changed Or UpdateString(PerformanceCountry, anotherEvent.PerformanceCountry)


            Return changed
        End Function

    End Class

End Namespace

