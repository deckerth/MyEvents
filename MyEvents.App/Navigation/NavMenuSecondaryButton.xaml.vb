﻿Namespace Global.MyEvents.App.Navigation

    Public NotInheritable Class NavMenuSecondaryButton
        Inherits Button

        Public Sub New()

            InitializeComponent()

        End Sub


        Public Property Icon As String
            Get
                Return DirectCast(GetValue(IconProperty), String)
            End Get
            Set(value As String)
                SetValue(IconProperty, value)
            End Set
        End Property

        Public Shared ReadOnly Property IconProperty As DependencyProperty = DependencyProperty.Register(NameOf(Icon), GetType(System.Double), GetType(NavMenuSecondaryButton), Nothing)

        Public Property Label As String
            Get
                Return DirectCast(GetValue(LabelProperty), String)
            End Get
            Set(value As String)
                SetValue(LabelProperty, value)
            End Set
        End Property

        Public Shared ReadOnly Property LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(Double), GetType(NavMenuSecondaryButton), Nothing)

        Public Function FontIcon() As FontIcon
            Return ButtonIcon
        End Function
    End Class

End Namespace
