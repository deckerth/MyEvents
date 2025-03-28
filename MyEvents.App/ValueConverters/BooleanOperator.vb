﻿Namespace Global.MyEvents.App.ValueConverters
    Public Class BooleanOperator

        Public Shared Function OpAnd(a As Boolean, b As Boolean) As Boolean
            Return a AndAlso b
        End Function

        Public Shared Function OpAndNot(a As Boolean, b As Boolean) As Boolean
            Return a AndAlso Not b
        End Function

        Public Shared Function OpOr(a As Boolean, b As Boolean) As Boolean
            Return a OrElse b
        End Function

        Public Shared Function OpNot(a As Boolean) As Boolean
            Return Not a
        End Function

        Public Shared Function OpNotNullAndNot(a As Object, b As Boolean) As Boolean
            Return a IsNot Nothing AndAlso Not b
        End Function

        Public Shared Function OpIsNotInitial(a As String) As Boolean
            Return Not String.IsNullOrEmpty(a)
        End Function

        Public Shared Function OpEqual1AndNotInitial(a As Integer, b As String) As Boolean
            Return a = 1 AndAlso Not String.IsNullOrEmpty(b)
        End Function

        Public Shared Function OpEqual1And(a As Integer, b As Boolean) As Boolean
            Return a = 1 AndAlso b
        End Function

        Public Shared Function OpNotAndGreater0(a As Object, b As Integer) As Boolean
            Return a IsNot Nothing AndAlso b > 0
        End Function

        Public Shared Function OpNotAndEqual1(a As Object, b As Integer) As Boolean
            Return a IsNot Nothing AndAlso b = 1
        End Function

    End Class

End Namespace