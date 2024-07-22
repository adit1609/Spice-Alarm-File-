Public Class Fiducial
    Private currentMousePosition As Point


    Private Sub Fiducial_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Add event handlers for MouseEnter, MouseLeave, and MouseMove
        AddHandler Guna2PictureBox1.MouseEnter, AddressOf Guna2PictureBox1_MouseEnter
        AddHandler Guna2PictureBox1.MouseLeave, AddressOf Guna2PictureBox1_MouseLeave
        AddHandler Guna2PictureBox1.MouseMove, AddressOf Guna2PictureBox1_MouseMove
        AddHandler Guna2PictureBox1.Paint, AddressOf Guna2PictureBox1_Paint
    End Sub

    Private Sub Guna2PictureBox1_MouseEnter(sender As Object, e As EventArgs)
        ' Force the PictureBox to repaint when the mouse enters
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Guna2PictureBox1_MouseLeave(sender As Object, e As EventArgs)
        ' Force the PictureBox to repaint when the mouse leaves
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Guna2PictureBox1_MouseMove(sender As Object, e As MouseEventArgs)
        ' Store the current mouse position and force the PictureBox to repaint
        currentMousePosition = e.Location
        Label1.Text = "X: " & currentMousePosition.X
        Label2.Text = "Y: " & currentMousePosition.Y
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Guna2PictureBox1_Paint(sender As Object, e As PaintEventArgs)
        If Guna2PictureBox1.ClientRectangle.Contains(currentMousePosition) Then
            'raw the crosshair
            Dim g As Graphics = e.Graphics
            Dim pen As New Pen(Color.Red, 1)

            'raw horizontal line
            g.DrawLine(pen, 0, currentMousePosition.Y, Guna2PictureBox1.Width, currentMousePosition.Y)
            'raw vertical line
            g.DrawLine(pen, currentMousePosition.X, 0, currentMousePosition.X, Guna2PictureBox1.Height)

            'raw fiducial mark (+ sign with square shape and circle)
            Dim fiducialPen As New Pen(Color.Black, 3)
            Dim fiducialBrush As New SolidBrush(Color.White)

            'raw square shape
            Dim squareRect As New Rectangle(currentMousePosition.X - 60, currentMousePosition.Y - 60, 120, 120)
            g.FillRectangle(fiducialBrush, squareRect)
            g.DrawRectangle(fiducialPen, squareRect)

            'raw circle
            Dim circleRect As New Rectangle(currentMousePosition.X - 20, currentMousePosition.Y - 20, 40, 40)
            g.FillEllipse(fiducialBrush, circleRect)
            g.DrawEllipse(fiducialPen, circleRect)
        End If
    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click
        ' Handle the click event if needed
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label1_Click_1(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class