Imports System.Threading
Imports System.Threading.Tasks

Public Class Form3
    Dim inc As Integer
    Dim cts As CancellationTokenSource

    Private Sub Guna2GradientTileButton1_MouseDown(sender As Object, e As MouseEventArgs) Handles Guna2GradientTileButton1.MouseDown
        cts = New CancellationTokenSource()
        Dim token As CancellationToken = cts.Token

        Task.Run(Sub()
                     While inc < 100
                         If token.IsCancellationRequested Then
                             inc = 0
                             Exit While
                         End If

                         inc += 1
                         Invoke(Sub() TextBox1.Text = inc.ToString())
                         Thread.Sleep(100) ' Adjust the delay as needed
                     End While
                 End Sub, token)
    End Sub

    Private Sub Guna2GradientTileButton1_MouseUp(sender As Object, e As MouseEventArgs) Handles Guna2GradientTileButton1.MouseUp
        If cts IsNot Nothing Then
            cts.Cancel()
        End If
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
