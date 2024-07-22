Imports System.Drawing.Drawing2D
Imports System.Xml.Schema

Public Class Vision_System
    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private shapes As New List(Of Shape)()
    Private currentID As Integer = 1
    Private currentColor As Color = Color.Blue
    Private drawSquares As Boolean = False
    Dim openFileDialog As New OpenFileDialog()
    Private xposs As String
    Private yposs As String





    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        isDrawing = False
        drawSquares = False
        Guna2PictureBox1.Invalidate()
        Panel2.Show()
        Panel1.Hide()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        drawSquares = True ' Set flag to draw squares
        isDrawing = False ' Ensure we're not drawing circles
        Panel1.Show()
        Panel2.Hide()
    End Sub

    Private Sub Guna2PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseDown, Guna2PictureBox2.MouseDown
        If e.Button = MouseButtons.Left Then
            startPoint = e.Location
            endPoint = e.Location   ' Set endPoint initially to start point
            isDrawing = True
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseMove, Guna2PictureBox2.MouseMove
        If isDrawing AndAlso e.Button = MouseButtons.Left Then
            endPoint = e.Location
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Sub Guna2PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2PictureBox1.Paint, Guna2PictureBox2.Paint
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

    Private Sub Guna2PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseUp, Guna2PictureBox2.MouseUp

        If isDrawing Then
            If drawSquares Then
                ' Draw square as before
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim topLeft As New Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y))
                shapes.Add(New Square(topLeft, size, currentID, currentColor))
                AddShapeToDataGridView(currentID, "Square", topLeft.X, topLeft.Y, size.Width, size.Height)
            Else
                ' Draw circle with dynamic size
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                shapes.Add(New Circle(New Point(centerX, centerY), radius, currentID, currentColor))

                AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
            End If

            currentID += 1
            isDrawing = False
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Sub AddShapeToDataGridView(id As Integer, shape As String, x1 As Integer, y1 As Integer, width As Integer, Optional height As Single = 0, Optional centerX As Integer = 0, Optional centerY As Integer = 0)
        xposs = TextBox1.Text
        yposs = TextBox2.Text
        Dim centerPoint As String

        If shape = "Square" Then
            Dim x2 As Integer = x1 + width
            Dim y2 As Integer = y1 + height
            Dim centerXInt As Integer = CInt(x1 + width \ 2) ' Calculate center X as integer
            Dim centerYInt As Integer = CInt(y1 + height \ 2) ' Calculate center Y as integer
            centerPoint = $"({centerXInt}, {centerYInt})"
            Dim row As String() = {id.ToString(), shape, x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), centerPoint, xposs, yposs}
            Guna2DataGridView1.Rows.Add(row)
        ElseIf shape = "Circle" Then
            centerPoint = $"({centerX}, {centerY})"
            Dim row As String() = {id.ToString(), shape, x1.ToString(), y1.ToString(), width.ToString(), height.ToString(), centerPoint, xposs, yposs}
            Guna2DataGridView1.Rows.Add(row)
        Else
            Throw New ArgumentException("Invalid shape type")
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
    'https://www.blackbox.ai/share/09842a2d-32d5-4dcc-820a-9aaac33eb1ac
    Private Sub Guna2DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellContentClick
        ' Handle cell content click events here if needed
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Check if a row is selected
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Remove the corresponding shape from the list
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)
            Dim shapeToRemove As Shape = shapes.FirstOrDefault(Function(s) s.ID = selectedShapeID)
            If shapeToRemove IsNot Nothing Then
                shapes.Remove(shapeToRemove)
            End If

            ' Remove the row from the DataGridView
            Guna2DataGridView1.Rows.RemoveAt(rowIndex)

            ' Redraw PictureBox
            Guna2PictureBox1.Invalidate()
        Else
            MessageBox.Show("Please select a row to delete.")
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            currentColor = ColorDialog1.Color
        End If
    End Sub

    Private Sub Vision_System_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize your form or any required components here\
        Panel1.Hide()
        Panel2.Hide()
        Guna2PictureBox1.BackColor = Color.Transparent
        Guna2PictureBox1.Parent = Guna2PictureBox2
        Guna2PictureBox1.Location = New Point(0, 0)
        Guna2PictureBox1.Size = Guna2PictureBox2.Size
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        shapes.Clear()
        Guna2PictureBox1.Invalidate()
        Guna2DataGridView1.Rows.Clear()
        Guna2DataGridView1.Invalidate()
        currentID = 1
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png"
        openFileDialog.Title = "Select an Image to Upload"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Dim selectedFilePath As String = openFileDialog.FileName
            If IO.File.Exists(selectedFilePath) Then
                Dim image As Image = Image.FromFile(selectedFilePath)
                Guna2PictureBox1.Image = image
            Else
                MessageBox.Show("The file does not exist.")
            End If
        End If
    End Sub
    Private Sub UpdateDataGridViewWithTextBoxValues(rowIndex As Integer)
        ' Update the DataGridView to include values from TextBox1 and TextBox2
        Guna2DataGridView1.Rows(rowIndex).Cells(7).Value = TextBox1.Text
        Guna2DataGridView1.Rows(rowIndex).Cells(8).Value = TextBox2.Text
    End Sub
    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the top
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y - increaseAmount)
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the bottom
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Guna2Button6.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the right
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the left
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X - increaseAmount, square.TopLeft.Y)
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the top
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y + contractAmount)
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button9_Click(sender As Object, e As EventArgs) Handles Guna2Button9.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the bottom
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button10_Click(sender As Object, e As EventArgs) Handles Guna2Button10.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the right
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button11_Click(sender As Object, e As EventArgs) Handles Guna2Button11.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the left
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X + contractAmount, square.TopLeft.Y)
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button12_Click(sender As Object, e As EventArgs) Handles Guna2Button12.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)
            If circle IsNot Nothing Then
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X, circle.Center.Y - moveAmount)
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button15_Click(sender As Object, e As EventArgs) Handles Guna2Button15.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then

            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle downwards
                Dim moveAmount As Integer = 2
                circle.Center = New Point(circle.Center.X, circle.Center.Y + moveAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button13_Click(sender As Object, e As EventArgs) Handles Guna2Button13.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X - moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button14_Click(sender As Object, e As EventArgs) Handles Guna2Button14.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X + moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button16_Click(sender As Object, e As EventArgs) Handles Guna2Button16.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius += increaseAmount

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button17_Click(sender As Object, e As EventArgs) Handles Guna2Button17.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius -= increaseAmount

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Check if any row is selected in the DataGridView
        If Guna2DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a shape from the DataGridView.")
            Return
        End If

        ' Get the selected row index
        Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

        ' Get the corresponding shape ID
        Dim selectedShapeID As Integer = Convert.ToInt32(Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

        ' Find the shape with the corresponding ID
        Dim shapeToSave As Shape = shapes.FirstOrDefault(Function(s) s.ID = selectedShapeID)

        If shapeToSave Is Nothing Then
            MsgBox("Selected shape not found.")
            Return
        End If

        ' Determine the bounding box of the shape
        Dim boundingBox As Rectangle
        If TypeOf shapeToSave Is Square Then
            Dim square As Square = CType(shapeToSave, Square)
            boundingBox = New Rectangle(square.TopLeft, square.Size)
        ElseIf TypeOf shapeToSave Is Circle Then
            Dim circle As Circle = CType(shapeToSave, Circle)
            boundingBox = New Rectangle(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
        Else
            MsgBox("Unknown shape type.")
            Return
        End If

        ' Create a new bitmap with the size of the bounding box
        Dim bitmap As New Bitmap(boundingBox.Width, boundingBox.Height)

        ' Draw the relevant portion of the live camera PictureBox image onto the new bitmap
        Using g As Graphics = Graphics.FromImage(bitmap)
            ' Adjust the source rectangle to match the shape's bounding box within the PictureBox
            Dim sourceRect As New Rectangle(boundingBox.Location, boundingBox.Size)
            Dim destRect As New Rectangle(0, 0, boundingBox.Width, boundingBox.Height)

            ' Here we draw the image from Guna2PictureBox2, which contains the live camera feed
            g.DrawImage(Guna2PictureBox2.Image, destRect, sourceRect, GraphicsUnit.Pixel)
        End Using

        ' Save the new bitmap to the specified path
        Dim savePath As String = "A:\Project\Visual Path\image.png"
        bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)

        MsgBox("Image saved successfully to " & savePath)
    End Sub


    Private Sub Guna2PictureBox2_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox2.Click

    End Sub



    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub
End Class

