Imports Guna.UI2.WinForms
Imports AForge.Video
Imports AForge.Video.DirectShow

Public Class Form8
    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private shapes As New List(Of Shape)()
    Private currentID As Integer = 1
    Private currentColor As Color = Color.Blue
    Private drawSquares As Boolean = False
    Private videoSource As VideoCaptureDevice

    Private Sub Form8_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim videoDevices As New FilterInfoCollection(FilterCategory.VideoInputDevice)
        If videoDevices.Count = 0 Then
            MessageBox.Show("No video devices found")
            Return
        End If

        videoSource = New VideoCaptureDevice(videoDevices(0).MonikerString)
        AddHandler videoSource.NewFrame, AddressOf VideoSource_NewFrame
        videoSource.Start()
    End Sub

    Private Sub VideoSource_NewFrame(sender As Object, eventArgs As NewFrameEventArgs)
        Dim bitmap As Bitmap = DirectCast(eventArgs.Frame.Clone(), Bitmap)
        Guna2PictureBox1.Image = bitmap
    End Sub

    Private Sub Form8_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If videoSource IsNot Nothing AndAlso videoSource.IsRunning Then
            videoSource.SignalToStop()
            videoSource.WaitForStop()
        End If
    End Sub

    Private Sub Guna2PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2PictureBox1.Paint
        Dim g As Graphics = e.Graphics
        Dim font As New Font("Arial", 12)
        Dim brush As New SolidBrush(Color.Black)

        ' Draw all stored shapes
        For Each shape In shapes
            Dim pen As New Pen(shape.Color, 2)
            If TypeOf shape Is Square Then
                Dim square As Square = DirectCast(shape, Square)
                g.DrawRectangle(pen, square.TopLeft.X, square.TopLeft.Y, square.Size.Width, square.Size.Height)
                g.DrawString(square.ID.ToString(), font, brush, square.TopLeft.X, square.TopLeft.Y - 20)
            ElseIf TypeOf shape Is Circle Then
                Dim circle As Circle = DirectCast(shape, Circle)
                g.DrawEllipse(pen, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
                g.DrawString(circle.ID.ToString(), font, brush, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius - 20)
            End If
        Next

        ' Draw the current shape if drawing is in progress
        If isDrawing Then
            Dim pen As New Pen(currentColor, 2)
            If drawSquares Then
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim rect As New Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), size.Width, size.Height)
                g.DrawRectangle(pen, rect)
                g.DrawString(currentID.ToString(), font, brush, rect.X, rect.Y - 20)
            Else ' Draw circle
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2)
                g.DrawString(currentID.ToString(), font, brush, centerX - radius, centerY - radius - 20)
            End If
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseUp
        If isDrawing Then
            If drawSquares Then
                ' Draw square as before
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim topLeft As New Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y))
                shapes.Add(New Square(topLeft, size, currentID, currentColor))
                'AddShapeToDataGridView(currentID, "Square", topLeft.X, topLeft.Y, size.Width, size.Height)
            Else
                ' Draw circle with dynamic size
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                shapes.Add(New Circle(New Point(centerX, centerY), radius, currentID, currentColor))
                ' AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
            End If

            currentID += 1
            isDrawing = False
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseMove
        If isDrawing AndAlso e.Button = MouseButtons.Left Then
            endPoint = e.Location
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseDown
        If e.Button = MouseButtons.Left Then
            startPoint = e.Location
            endPoint = e.Location   ' Set endPoint initially to start point
            isDrawing = True
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Class Shape
        Public Property ID As Integer
        Public Property Color As Color

        Public Sub New(id As Integer, color As Color)
            Me.ID = id
            Me.Color = color
        End Sub
    End Class

    Private Class Square
        Inherits Shape

        Public Property TopLeft As Point
        Public Property Size As Size

        Public Sub New(topLeft As Point, size As Size, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.TopLeft = topLeft
            Me.Size = size
        End Sub
    End Class

    Private Class Circle
        Inherits Shape

        Public Property Center As Point
        Public Property Radius As Integer

        Public Sub New(center As Point, radius As Integer, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.Center = center
            Me.Radius = radius
        End Sub
    End Class

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        isDrawing = False
        drawSquares = False
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Guna2PictureBox1_MouseEnter(sender As Object, e As EventArgs) Handles Guna2PictureBox1.MouseEnter
        If videoSource IsNot Nothing AndAlso videoSource.IsRunning Then
            videoSource.SignalToStop()
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles Guna2PictureBox1.MouseLeave
        If videoSource IsNot Nothing AndAlso Not videoSource.IsRunning Then
            videoSource.Start()
        End If
    End Sub
End Class
