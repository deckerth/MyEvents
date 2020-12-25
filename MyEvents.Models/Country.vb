Namespace Global.MyEvents.Models

    Public Class Country
        Inherits DBObject
        Implements IEquatable(Of Country)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Country) As Boolean Implements IEquatable(Of Country).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Country
            Return DirectCast(MemberwiseClone(), Country)
        End Function

    End Class

End Namespace

