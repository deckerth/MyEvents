Namespace Global.MyEvents.Models

    Public Class Director

        Inherits DBObject
        Implements IEquatable(Of Director)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Director) As Boolean Implements IEquatable(Of Director).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Director
            Return DirectCast(MemberwiseClone(), Director)
        End Function

    End Class

End Namespace


